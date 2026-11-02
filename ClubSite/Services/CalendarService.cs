using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Ical.Net.DataTypes;
using NodaTime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ClubSite.Services;

/// <summary>
/// Service to load, parse and filter .ics calendar files using iCal.Net.
/// </summary>
public class CalendarService : ICalendarService
{
    private const string CalendarFileName = "events.ics";
    private const string UploadsDir = "uploads";
    private const int MaxEvents = 300;

    private readonly string _webRoot;
    private readonly ILogger<CalendarService> _logger;
    private bool? _hasCalendarFiles;

    public CalendarService(IWebHostEnvironment env, ILogger<CalendarService> logger)
    {
        _webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _logger = logger;
    }

    /// <summary>
    /// Returns calendar events within the given date range.
    /// Uses iCal.Net's built-in GetOccurrences for RRULE expansion.
    /// </summary>
    public List<CalendarEventItem> GetEvents(DateTime rangeStart, DateTime rangeEnd)
    {
        var calendar = LoadCalendar();
        if (calendar == null) return [];

        var items = new List<CalendarEventItem>();
        int count = 0;
        foreach (var ev in calendar.Events)
        {
            if (ev.DtStart == null || count >= MaxEvents) continue;

            var (team, category) = ParseCategories(ev);
            var location = ev.Location?.ToString() ?? string.Empty;

            // Convert range to NodaTime ZonedDateTime for ICal.Net 6
            var zone = DateTimeZoneProviders.Tzdb["Europe/Berlin"];
            var startInst = Instant.FromUtc(rangeStart.Year, rangeStart.Month, rangeStart.Day, rangeStart.Hour, rangeStart.Minute, rangeStart.Second);
            var startZoned = startInst.InZone(zone);

            // ICal.Net's GetOccurrences handles RRULE expansion natively
            foreach (var occ in ev.GetOccurrences(startZoned))
            {
                if (count >= MaxEvents) break;

                var occStart = CalDateTime.FromZonedDateTime(occ.Start).ToDateTimeUnspecified();
                var occEnd = CalDateTime.FromZonedDateTime(occ.End).ToDateTimeUnspecified();

                if (occStart > rangeEnd) break;
                if (occEnd < rangeStart) continue;

                items.Add(MakeItem(ev, occStart, occEnd, team, category, location));
                count++;
            }
        }
        return [.. items.OrderBy(e => e.StartDate)];
    }

    public List<string> GetAvailableTeams() =>
        CollectDistinct(e => e.Team);

    public List<string> GetAvailableCategories() =>
        CollectDistinct(e => e.Category);

    public bool HasCalendarFiles()
    {
        _hasCalendarFiles ??= FindCalendarFile() != null;
        return _hasCalendarFiles.Value;
    }

    private Calendar? LoadCalendar()
    {
        var calendarFile = FindCalendarFile();
        if (calendarFile == null) return null;

        try
        {
            var icsContent = File.ReadAllText(calendarFile);
            var calendars = CalendarSerializer.DeserializeCollection<Calendar>(icsContent);
            return calendars?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse calendar file {FilePath}", calendarFile);
        }
        return null;
    }

    private string? FindCalendarFile()
    {
        var uploadsRoot = Path.Combine(_webRoot, UploadsDir);
        if (!Directory.Exists(uploadsRoot)) return null;

        var files = Directory.GetFiles(uploadsRoot, CalendarFileName, SearchOption.AllDirectories);
        if (files.Length == 0) return null;
        if (files.Length <= 1) return files[0];
        return files.OrderByDescending(File.GetLastWriteTime).First();
    }

    private static bool IsHomeGame(string category)
    {
        return !string.IsNullOrEmpty(category)
            && category.ToLowerInvariant().Contains("heimspieltag");
    }

    private List<string> CollectDistinct(Func<CalendarEventItem, string> selector)
    {
        var values = new HashSet<string>();
        foreach (var item in GetEvents(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(2)))
        {
            var val = selector(item);
            if (!string.IsNullOrEmpty(val)) values.Add(val);
        }
        return [.. values.OrderBy(v => v)];
    }
}
