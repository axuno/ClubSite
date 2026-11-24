// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Evaluation;
using Ical.Net.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NodaTime;
using Piranha;
using Piranha.Models;

namespace ClubSite.Services;

/// <summary>
/// Loads, caches, parses and filters Piranha-managed .ics calendar files using iCal.Net.
/// Parsed calendars are cached by Piranha media identity and last-modified version so uploads
/// and replacements made through the Piranha Manager become visible immediately.
/// </summary>
public class CalendarService : ICalendarService
{
    private const int MaxEvents = 300;
    private const int MaxOccurrencesPerEvent = 2000;
    private const int MaxCalendarFilesPerPage = 20;
    private const long MaxCalendarFileBytes = 5 * 1024 * 1024;
    private const int MaxUnmatchedIncrementsLimit = 1000;
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromHours(1);
    private static readonly DateTimeZone BerlinZone = DateTimeZoneProviders.Tzdb["Europe/Berlin"];

    private readonly IStorage _storage;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CalendarService> _logger;
    private readonly IClock _clock;

    public CalendarService(
        IStorage storage,
        IMemoryCache memoryCache,
        ILogger<CalendarService> logger,
        IClock? clock = null)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clock = clock ?? SystemClock.Instance;
    }

    public async Task<CalendarLoadResult> LoadEventsAsync(
        IEnumerable<Media> calendarDocuments,
        DateTime? rangeStart,
        DateTime? rangeEnd)
    {
        ArgumentNullException.ThrowIfNull(calendarDocuments);

        var inputList = calendarDocuments as ICollection<Media> ?? calendarDocuments.ToList();
        var distinctMedia = inputList
            .Where(IsValidCalendarMedia)
            .GroupBy(media => media.Id)
            .Select(group => group.First())
            .Take(MaxCalendarFilesPerPage)
            .ToList();

        if (inputList.Count > MaxCalendarFilesPerPage)
        {
            _logger.LogWarning("Calendar page references more than {MaxFiles} documents; extra entries are ignored.",
                MaxCalendarFilesPerPage);
        }

        if (distinctMedia.Count == 0)
        {
            return new CalendarLoadResult();
        }

        var loadedCalendars = new List<Calendar>();
        DateTime? lastUpdated = null;

        foreach (var media in distinctMedia)
        {
            if (media.LastModified != default)
            {
                var mediaModified = ToBerlinWallTime(media.LastModified);
                if (!lastUpdated.HasValue || mediaModified > lastUpdated.Value)
                {
                    lastUpdated = mediaModified;
                }
            }

            var calendars = await GetOrLoadCalendarsAsync(media).ConfigureAwait(false);
            if (calendars.Count > 0)
            {
                loadedCalendars.AddRange(calendars);
            }
        }

        if (loadedCalendars.Count == 0)
        {
            return new CalendarLoadResult { LastUpdated = lastUpdated };
        }

        var nowBerlin = _clock.GetCurrentInstant().InZone(BerlinZone).LocalDateTime.ToDateTimeUnspecified();
        var effectiveStart = rangeStart ?? nowBerlin.AddYears(-2);
        var effectiveEnd = rangeEnd ?? nowBerlin.AddYears(2);
        if (effectiveEnd < effectiveStart)
        {
            return new CalendarLoadResult { LastUpdated = lastUpdated };
        }

        var startZoned = ResolveBerlinWallTime(effectiveStart);
        var endZoned = ResolveBerlinWallTime(effectiveEnd);
        var startInstant = startZoned.ToInstant();
        var endInstant = endZoned.ToInstant();

        var events = ExpandEvents(loadedCalendars, startZoned, startInstant, endInstant);

        var filterWindowStart = ResolveBerlinWallTime(nowBerlin.AddYears(-2));
        var filterWindowEnd = ResolveBerlinWallTime(nowBerlin.AddYears(2));
        var filterEvents = ExpandEvents(
            loadedCalendars,
            filterWindowStart,
            filterWindowStart.ToInstant(),
            filterWindowEnd.ToInstant());

        var teams = filterEvents
            .Select(e => e.Team)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var categories = filterEvents
            .Select(e => e.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(c => c, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new CalendarLoadResult
        {
            Events = events,
            AvailableTeams = teams,
            AvailableCategories = categories,
            LastUpdated = lastUpdated
        };
    }

    private async Task<IReadOnlyList<Calendar>> GetOrLoadCalendarsAsync(Media media)
    {
        var cacheKey = BuildCacheKey(media);
        if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<Calendar>? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            if (media.Size > MaxCalendarFileBytes)
            {
                _logger.LogWarning("Skipping Piranha calendar media {MediaId} ({Filename}) because size {Size} exceeds limit {Limit}.",
                    media.Id, media.Filename, media.Size, MaxCalendarFileBytes);
                return [];
            }

            using var session = await _storage.OpenAsync().ConfigureAwait(false);
            using var memoryStream = new MemoryStream();
            var found = await session.GetAsync(media, media.Filename, memoryStream).ConfigureAwait(false);
            if (!found)
            {
                _logger.LogWarning("Piranha media file {MediaId} ({Filename}) was not found in storage.",
                    media.Id, media.Filename);
                return [];
            }

            if (memoryStream.Length > MaxCalendarFileBytes)
            {
                _logger.LogWarning("Skipping Piranha calendar media {MediaId} ({Filename}) because stream size {Size} exceeds limit {Limit}.",
                    media.Id, media.Filename, memoryStream.Length, MaxCalendarFileBytes);
                return [];
            }

            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var icsContent = await reader.ReadToEndAsync().ConfigureAwait(false);

            var calendars = CalendarSerializer.DeserializeCollection<Calendar>(icsContent)?.ToList() ?? [];
            _memoryCache.Set(cacheKey, (IReadOnlyList<Calendar>)calendars, new MemoryCacheEntryOptions
            {
                SlidingExpiration = CacheSlidingExpiration
            });
            return calendars;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read or parse Piranha calendar media {MediaId} ({Filename}).",
                media.Id, media.Filename);
            return [];
        }
    }

    private bool IsValidCalendarMedia(Media? media)
    {
        if (media == null || media.Id == Guid.Empty || string.IsNullOrWhiteSpace(media.Filename))
        {
            return false;
        }

        if (media.Type != MediaType.Document)
        {
            _logger.LogWarning("Skipping non-document Piranha media {MediaId} ({Filename}, Type={MediaType}).",
                media.Id, media.Filename, media.Type);
            return false;
        }

        // Guard against traversal segments in filenames before invoking IStorageSession.GetAsync
        if (media.Filename.Contains('/') || media.Filename.Contains('\\')
            || !string.Equals(Path.GetFileName(media.Filename), media.Filename, StringComparison.Ordinal)
            || media.Filename.Contains("..", StringComparison.Ordinal))
        {
            _logger.LogWarning("Skipping Piranha calendar media {MediaId} with unsafe filename {Filename}.",
                media.Id, media.Filename);
            return false;
        }

        if (!string.Equals(Path.GetExtension(media.Filename), ".ics", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Skipping Piranha media {MediaId} ({Filename}) because it is not an .ics file.",
                media.Id, media.Filename);
            return false;
        }

        return true;
    }

    private static string BuildCacheKey(Media media) =>
        $"ClubSite:Calendar:{media.Id:N}:{media.LastModified.Ticks}:{media.Size}:{media.Filename}";

    private List<CalendarEventItem> ExpandEvents(
        IReadOnlyList<Calendar> calendars,
        ZonedDateTime rangeStartZoned,
        Instant rangeStartInstant,
        Instant rangeEndInstant)
    {
        var evaluationOptions = new EvaluationOptions
        {
            MaxUnmatchedIncrementsLimit = MaxUnmatchedIncrementsLimit
        };

        var expanded = new List<(Instant SortInstant, CalendarEventItem Item)>();

        foreach (var calendar in calendars)
        {
            try
            {
                var calendarCount = 0;
                // Use Calendar.GetOccurrences so UID + RECURRENCE-ID overrides are properly reconciled
                // and occurrences are streamed in chronological order.
                foreach (var occ in calendar.GetOccurrences<CalendarEvent>(rangeStartZoned, evaluationOptions))
                {
                    if (occ.Source is not CalendarEvent ev || ev.DtStart == null)
                    {
                        continue;
                    }

                    var occStartBerlin = occ.Start.WithZone(BerlinZone);
                    var occEndBerlin = occ.End.WithZone(BerlinZone);
                    var occStartInstant = occStartBerlin.ToInstant();
                    var occEndInstant = occEndBerlin.ToInstant();

                    if (occStartInstant > rangeEndInstant)
                    {
                        break;
                    }

                    if (occEndInstant < rangeStartInstant)
                    {
                        continue;
                    }

                    if (++calendarCount > MaxOccurrencesPerEvent)
                    {
                        _logger.LogWarning("Calendar occurrence expansion exceeded per-calendar limit {Limit}.",
                            MaxOccurrencesPerEvent);
                        break;
                    }

                    var (team, category) = ParseCategories(ev);
                    var location = ev.Location?.ToString() ?? string.Empty;

                    expanded.Add((occStartInstant, MakeItem(
                        ev,
                        occStartBerlin.LocalDateTime.ToDateTimeUnspecified(),
                        occEndBerlin.LocalDateTime.ToDateTimeUnspecified(),
                        team,
                        category,
                        location)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to evaluate recurring occurrences for a calendar.");
            }
        }

        return expanded
            .OrderBy(entry => entry.SortInstant)
            .ThenBy(entry => entry.Item.Title, StringComparer.CurrentCultureIgnoreCase)
            .Take(MaxEvents)
            .Select(entry => entry.Item)
            .ToList();
    }

    private static ZonedDateTime ResolveBerlinWallTime(DateTime wallTime)
    {
        var local = LocalDateTime.FromDateTime(wallTime);
        return BerlinZone.AtLeniently(local);
    }

    private static DateTime ToBerlinWallTime(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => Instant.FromDateTimeUtc(value).InZone(BerlinZone).LocalDateTime.ToDateTimeUnspecified(),
            DateTimeKind.Local => Instant.FromDateTimeUtc(value.ToUniversalTime()).InZone(BerlinZone).LocalDateTime.ToDateTimeUnspecified(),
            _ => value
        };
    }

    private static CalendarEventItem MakeItem(
        CalendarEvent ev,
        DateTime start,
        DateTime end,
        string team,
        string category,
        string location) => new()
    {
        Title = ev.Summary?.ToString() ?? "(ohne Titel)",
        StartDate = start,
        EndDate = end,
        IsAllDay = ev.IsAllDay,
        Location = location,
        Description = ev.Description?.ToString() ?? string.Empty,
        Team = team,
        Category = category,
        IsHomeGame = IsHomeGame(category),
        Url = SanitizeExternalUrl(ev.Url?.ToString())
    };

    private static string? SanitizeExternalUrl(string? rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(rawUrl.Trim(), UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps
            ? uri.AbsoluteUri
            : null;
    }

    private static (string team, string category) ParseCategories(CalendarEvent ev)
    {
        if (ev.Categories is not { Count: > 0 })
        {
            return (string.Empty, string.Empty);
        }

        return (ev.Categories[0], ev.Categories.Count > 1 ? ev.Categories[1] : string.Empty);
    }

    private static bool IsHomeGame(string category)
    {
        return !string.IsNullOrEmpty(category)
            && category.Contains("heimspieltag", StringComparison.OrdinalIgnoreCase);
    }
}
