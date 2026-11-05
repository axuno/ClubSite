using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using NodaTime;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ClubSite.Services;

/// <summary>
/// Service to load, parse and filter .ics calendar files using iCal.Net.
/// </summary>
public class CalendarService : ICalendarService
{
    private string _calendarFileName = "events.ics";
    private string _uploadsDir = "uploads";
    private const int MaxEvents = 300;

    private readonly string _webRoot;
    private readonly ILogger<CalendarService> _logger;
    private Calendar? _cachedCalendar;

    public CalendarService(IWebHostEnvironment env, ILogger<CalendarService> logger,
        IConfiguration configuration)
    {
        _webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _logger = logger;

        // Allow override from AppSettings.json > Calendar.FileName / Calendar.UploadsDir
        try
        {
            var fileName = configuration["Calendar:FileName"];
            if (!string.IsNullOrEmpty(fileName)) _calendarFileName = fileName;
            var uploadsDir = configuration["Calendar:UploadsDir"];
            if (!string.IsNullOrEmpty(uploadsDir)) _uploadsDir = uploadsDir;
        }
        catch { }
    }

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
            // ICal.Net 6 Location property returns only venue name, not full address
            // Use raw property value for the complete address
            string location = ev.Location?.ToString() ?? string.Empty;

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
        return LoadCalendar() != null;
    }

    private Calendar? LoadCalendar()
    {
        if (_cachedCalendar != null) return _cachedCalendar;

        var calendarFile = FindCalendarFile();
        if (calendarFile == null) return null;

        try
        {
            var icsContent = File.ReadAllText(calendarFile);
            var calendars = CalendarSerializer.DeserializeCollection<Calendar>(icsContent);
            _cachedCalendar = calendars?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse calendar file {FilePath}", calendarFile);
        }
        return _cachedCalendar;
    }

    private string? FindCalendarFile()
    {
        var uploadsRoot = Path.Combine(_webRoot, _uploadsDir);
        if (!Directory.Exists(uploadsRoot)) return null;

        var files = Directory.GetFiles(uploadsRoot, _calendarFileName, SearchOption.AllDirectories);
        if (files.Length == 0) return null;
        if (files.Length <= 1) return files[0];
        return files.OrderByDescending(File.GetLastWriteTime).First();
    }




    private static CalendarEventItem MakeItem(CalendarEvent ev, DateTime start, DateTime end,
        string team, string category, string location) => new CalendarEventItem
    {
        Title = ev.Summary?.ToString() ?? "(no title)",
        StartDate = start,
        EndDate = end,
        Location = location,
        Description = ev.Description?.ToString() ?? string.Empty,
        Team = team,
        Category = category,
        IsHomeGame = IsHomeGame(category),
        Url = ev.Url?.ToString()
    };

    private static (string team, string category) ParseCategories(CalendarEvent ev)
    {
        if (ev.Categories is not { Count: > 0 })
            return (string.Empty, string.Empty);
        return (ev.Categories[0], ev.Categories.Count > 1 ? ev.Categories[1] : string.Empty);
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
