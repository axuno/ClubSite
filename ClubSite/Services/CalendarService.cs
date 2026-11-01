using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
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

    public List<CalendarEventItem> GetEvents(DateTime rangeStart, DateTime rangeEnd)
    {
        var calendar = LoadCalendar();
        if (calendar == null) return [];

        var items = new List<CalendarEventItem>();
        int count = 0;
        foreach (var ev in calendar.Events)
        {
            if (ev.DtStart == null) continue;
            if (count++ >= MaxEvents) break;

            var start = ToLocalDate(ev.DtStart);
            var end = ev.DtEnd != null ? ToLocalDate(ev.DtEnd) : start;

            if (end < rangeStart || start > rangeEnd) continue;

            var (team, category) = ParseCategories(ev);
            var location = ev.Location?.ToString() ?? string.Empty;

            if (IsRecurring(ev))
            {
                ExpandRecurrence(ev, start, end, rangeStart, rangeEnd, team, category, location, items);
            }
            else
            {
                items.Add(MakeItem(ev, start, end, team, category, location));
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

        var newest = files.OrderByDescending(File.GetLastWriteTime).First();
        return newest;
    }

    private static CalendarEventItem MakeItem(CalendarEvent ev, DateTime start, DateTime end, string team, string category, string location)
    {
        return new CalendarEventItem
        {
            Title = ev.Summary ?? "(no title)",
            StartDate = start,
            EndDate = end,
            Location = location,
            Description = ev.Description ?? string.Empty,
            Team = team,
            Category = category,
            IsHomeGame = IsHomeVenue(location)
        };
    }

    private static bool IsRecurring(CalendarEvent ev) =>
        ev.RecurrenceRule != null && ev.RecurrenceRule.Frequency == FrequencyType.Weekly;

    private static void ExpandRecurrence(CalendarEvent ev, DateTime start, TimeSpan duration,
        DateTime rangeStart, DateTime rangeEnd, string team, string category, string location,
        List<CalendarEventItem> items)
    {
        var rule = ev.RecurrenceRule!;
        var until = rule.Until?.ToDateTimeUnspecified() ?? rangeEnd;
        if (until > rangeEnd) until = rangeEnd;

        var maxOccurrences = rule.Count > 0 ? rule.Count : 200;
        var interval = rule.Interval > 0 ? rule.Interval : 1;

        var dayNumbers = rule.ByDay switch
        {
            { Count: > 0 } => [.. rule.ByDay.Select(wd => (int)wd.DayOfWeek)],
            _ => [(int)start.DayOfWeek]
        };

        var current = start > rangeStart ? start : rangeStart;
        var occurrenceCount = 0;

        while (current <= until && occurrenceCount < maxOccurrences)
        {
            foreach (var targetDay in dayNumbers)
            {
                var diff = targetDay - (int)current.DayOfWeek;
                if (diff < 0) diff += 7;
                var occurrenceDate = current.AddDays(diff);
                if (occurrenceDate < start || occurrenceDate > until) continue;

                items.Add(new CalendarEventItem
                {
                    Title = ev.Summary ?? "(no title)",
                    StartDate = occurrenceDate + start.TimeOfDay,
                    EndDate = occurrenceDate + start.TimeOfDay + duration,
                    Location = location,
                    Description = ev.Description ?? string.Empty,
                    Team = team,
                    Category = category,
                    IsHomeGame = IsHomeVenue(location)
                });

                if (++occurrenceCount >= maxOccurrences) break;
            }
            if (occurrenceCount >= maxOccurrences) break;
            current = current.AddDays(7 * interval);
        }
    }

    private static (string team, string category) ParseCategories(CalendarEvent ev)
    {
        if (ev.Categories is not { Count: > 0 })
            return (string.Empty, string.Empty);
        return (ev.Categories[0], ev.Categories.Count > 1 ? ev.Categories[1] : string.Empty);
    }

    private static DateTime ToLocalDate(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc)
            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
        return dt;
    }

    private static bool IsHomeVenue(string location)
    {
        if (string.IsNullOrWhiteSpace(location)) return false;
        var loc = location.ToLowerInvariant();
        return loc.Contains("neusäß") || loc.Contains("neusaess") || loc.Contains("86356");
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
