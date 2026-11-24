// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubSite.Services;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace ClubSite.Pages;

/// <summary>
/// Page model for Piranha <see cref="Models.CalendarPage"/> displaying events from Piranha-managed .ics assets.
/// </summary>
public class CalendarPageModel : SinglePage<Models.CalendarPage>
{
    private static readonly DateTimeZone BerlinZone = DateTimeZoneProviders.Tzdb["Europe/Berlin"];
    private readonly ICalendarService _calendarService;
    private readonly IClock _clock;

    public CalendarPageModel(
        ICalendarService calendarService,
        IApi api,
        IModelLoader loader,
        IClock? clock = null)
        : base(api, loader)
    {
        _calendarService = calendarService;
        _clock = clock ?? SystemClock.Instance;
    }

    /// <summary>
    /// Filter: team (e.g. Damen, Herren, Jugend). Empty = all teams.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string TeamFilter { get; set; } = string.Empty;

    /// <summary>
    /// Filter: category (e.g. Training, Spieltag). Empty = all categories.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string CategoryFilter { get; set; } = string.Empty;

    /// <summary>
    /// Display mode: "upcoming" (default) or "past".
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string Mode { get; set; } = "upcoming";

    /// <summary>
    /// Filtered events for the current view.
    /// </summary>
    public List<CalendarEventItem> Events { get; set; } = [];

    /// <summary>
    /// Available teams for the filter dropdown.
    /// </summary>
    public List<string> AvailableTeams { get; set; } = [];

    /// <summary>
    /// Available categories for the filter dropdown.
    /// </summary>
    public List<string> AvailableCategories { get; set; } = [];

    /// <summary>
    /// Whether at least one valid Piranha-managed .ics calendar document is configured on this page.
    /// </summary>
    public bool HasIcsFiles { get; private set; }

    /// <summary>
    /// Most recent modification timestamp across all configured calendar documents (Europe/Berlin wall time).
    /// </summary>
    public DateTime? LastUpdated { get; private set; }

    public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
    {
        var result = await base.OnGet(id, draft);
        if (result is not Microsoft.AspNetCore.Mvc.RazorPages.PageResult || Data == null)
        {
            return result;
        }

        LoadFilterFromQueryString();
        await LoadEventsAsync();

        return result;
    }

    private void LoadFilterFromQueryString()
    {
        if (Request?.Query == null)
        {
            return;
        }

        if (Request.Query.TryGetValue(nameof(TeamFilter), out var team))
        {
            TeamFilter = team.FirstOrDefault() ?? string.Empty;
        }

        if (Request.Query.TryGetValue(nameof(CategoryFilter), out var category))
        {
            CategoryFilter = category.FirstOrDefault() ?? string.Empty;
        }

        if (Request.Query.TryGetValue(nameof(Mode), out var mode))
        {
            Mode = mode.FirstOrDefault() ?? "upcoming";
        }
    }

    private async Task LoadEventsAsync()
    {
        var mediaDocuments = await ResolveCalendarMediaAsync();
        HasIcsFiles = mediaDocuments.Count > 0;
        if (!HasIcsFiles)
        {
            Events = [];
            AvailableTeams = [];
            AvailableCategories = [];
            LastUpdated = null;
            return;
        }

        // Calendar display and filtering always use Berlin wall-clock time,
        // independently of the server's configured time zone.
        var now = _clock.GetCurrentInstant().InZone(BerlinZone).LocalDateTime.ToDateTimeUnspecified();

        var (rangeStart, rangeEnd) = Mode == "past"
            ? (now.AddMonths(-6), now)
            : (now.AddMonths(-1), now.AddMonths(6));

        var loadResult = await _calendarService.LoadEventsAsync(mediaDocuments, rangeStart, rangeEnd);

        AvailableTeams = [.. loadResult.AvailableTeams];
        AvailableCategories = [.. loadResult.AvailableCategories];
        LastUpdated = loadResult.LastUpdated;

        var filtered = loadResult.Events.AsEnumerable();

        if (!string.IsNullOrEmpty(TeamFilter))
        {
            filtered = filtered.Where(e => string.Equals(e.Team, TeamFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrEmpty(CategoryFilter))
        {
            filtered = filtered.Where(e => string.Equals(e.Category, CategoryFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        if (Mode == "past")
        {
            filtered = filtered.Where(e => e.EndDate < now);
        }
        else
        {
            filtered = filtered.Where(e => e.EndDate >= now);
        }

        Events = filtered.OrderBy(e => e.StartDate).ToList();
    }

    private async Task<List<Media>> ResolveCalendarMediaAsync()
    {
        var documents = new List<Media>();
        var seenIds = new HashSet<Guid>();

        if (Data?.CalendarFiles == null)
        {
            return documents;
        }

        foreach (var entry in Data.CalendarFiles)
        {
            var documentField = entry?.Document;
            if (documentField?.Id is not Guid mediaId || mediaId == Guid.Empty || !seenIds.Add(mediaId))
            {
                continue;
            }

            // Prefer fetching through Piranha's cached IMediaService so replacing the media
            // asset in the Manager invalidates Piranha's cache and yields the updated LastModified.
            var media = await _api.Media.GetByIdAsync(mediaId) ?? documentField.Media;
            if (media != null && !string.IsNullOrWhiteSpace(media.Filename))
            {
                documents.Add(media);
            }
        }

        return documents;
    }
}
