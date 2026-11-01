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
/// Service to load, parse and filter .ics calendar files using iCal.net.
/// </summary>
public class CalendarService : ICalendarService
{
    private const string CalendarFileName = "events.ics";
    private const string UploadsDir = "uploads";

    private readonly string _webRoot;
    private readonly ILogger<CalendarService> _logger;
    private bool? _hasCalendarFiles;

    public CalendarService(IWebHostEnvironment env, ILogger<CalendarService> logger)
    {
        _webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _logger = logger;
    }

    /// <summary>
    /// Loads the calendar file from the uploads directory and returns events
    /// within the specified date range (evaluating RRULE for recurring events).
    /// </summary>
    public List<CalendarEventItem> GetEvents(DateTime rangeStart, DateTime rangeEnd)
    {
        var calendarFile = FindCalendarFile();
        if (calendarFile == null)
        {
            _logger.LogWarning("No calendar files found in uploads directory");
            return [];
        }

        try
        {
            var icsContent = File.ReadAllText(calendarFile);
            var calendar = CalendarSerializer.Deserialize(icsContent);
            if (calendar != null)
                return ParseCalendar(calendar, rangeStart, rangeEnd)
                    .OrderBy(e => e.StartDate)
                    .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse calendar file {FilePath}", calendarFile);
        }

        return [];
    }

    /// <summary>
    /// Returns distinct team values from all .ics files (for filter dropdowns).
    /// </summary>
    public List<string> GetAvailableTeams()
    {
        return GetDistinctFieldValues(e => e.Team);
    }

    /// <summary>
    /// Returns distinct category values from all .ics files (for filter dropdowns).
    /// </summary>
    public List<string> GetAvailableCategories()
    {
        return GetDistinctFieldValues(e => e.Category);
    }

    /// <summary>
    /// Checks whether at least one calendar file exists (cached).
    /// </summary>
    public bool HasCalendarFiles()
    {
        _hasCalendarFiles ??= FindCalendarFile() != null;
        return _hasCalendarFiles.Value;
    }

    /// <summary>
    /// Finds the newest events.ics file under wwwroot/uploads/.
    /// Returns null if none found.
    /// </summary>
    private string? FindCalendarFile()
    {
        var uploadsRoot = Path.Combine(_webRoot, UploadsDir);
        if (!Directory.Exists(uploadsRoot))
            return null;

        var files = Directory.GetFiles(uploadsRoot, CalendarFileName, SearchOption.AllDirectories);

        if (files.Length == 0)
            return null;

        if (files.Length <= 1)
            return files[0];

        var newest = files.OrderByDescending(File.GetLastWriteTime).First();
        _logger.LogInformation("Multiple calendar files found, using newest: {FilePath}", newest);
        return newest;
    }

    private List<CalendarEventItem> ParseCalendar(Calendar calendar, DateTime rangeStart, DateTime rangeEnd)
    {
        var items = new List<CalendarEventItem>();

        foreach (var component in calendar.Events)
        {
            if (component is not CalendarEvent ev || ev.DtStart == null) continue;

            var (team, category) = ParseCategoriesFromEvent(ev);
            var start = ToLocal(ev.DtStart);
            var end = ev.DtEnd != null ? ToLocal(ev.DtEnd) : start;
            var duration = end - start;

            if (ev.RecurrenceRule?.Frequency == FrequencyType.Weekly)
            {
                items.AddRange(
                    ExpandWeeklyRecurrence(ev, start, duration, rangeStart, rangeEnd, team, category));
            }
            else
            {
                items.Add(new CalendarEventItem
                {
                    Title = ev.Summary ?? "(no title)",
                    StartDate = start,
                    EndDate = end,
                    Location = ev.Location ?? string.Empty,
                    Description = ev.Description ?? string.Empty,
                    Team = team,
                    Category = category
                });
            }
        }

        return items;
    }

    /// <summary>
    /// Expands weekly recurring events (RRULE FREQ=WEEKLY) into individual CalendarEventItems.
    /// </summary>
    private static List<CalendarEventItem> ExpandWeeklyRecurrence(
        CalendarEvent ev, DateTime start, TimeSpan duration,
        DateTime rangeStart, DateTime rangeEnd,
        string team, string category)
    {
        var items = new List<CalendarEventItem>();
        var rule = ev.RecurrenceRule!;

        var until = rule.Until != null
            ? (rule.Until.Value < rangeEnd ? rule.Until.Value : rangeEnd)
            : rangeEnd;

        var maxOccurrences = rule.Count > 0 ? rule.Count : 200;
        var interval = rule.Interval > 0 ? rule.Interval : 1;

        var dayNumbers = rule.ByDay switch
        {
            { Count: > 0 } => rule.ByDay.Select(wd => (int)wd.DayOfWeek).ToList(),
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

                if (occurrenceDate < start) continue;
                if (occurrenceDate > until) break;

                items.Add(new CalendarEventItem
                {
                    Title = ev.Summary ?? "(no title)",
                    StartDate = occurrenceDate + start.TimeOfDay,
                    EndDate = occurrenceDate + start.TimeOfDay + duration,
                    Location = ev.Location ?? string.Empty,
                    Description = ev.Description ?? string.Empty,
                    Team = team,
                    Category = category
                });

                if (++occurrenceCount >= maxOccurrences) break;
            }

            if (occurrenceCount >= maxOccurrences) break;
            current = current.AddDays(7 * interval);
        }

        return items;
    }

    private static (string team, string category) ParseCategoriesFromEvent(CalendarEvent ev)
    {
        if (ev.Categories is not { Count: > 0 })
            return (string.Empty, string.Empty);

        var team = ev.Categories[0];
        var category = ev.Categories.Count > 1 ? ev.Categories[1] : string.Empty;
        return (team, category);
    }


    private DateTime ToLocal(CalDateTime cal)
    {
        if (cal.IsUtc)
            return TimeZoneInfo.ConvertTimeFromUtc(cal, TimeZoneInfo.Local);
        return cal;
    }

    private List<string> GetDistinctFieldValues(Func<CalendarEventItem, string> selector)
    {
        var values = new HashSet<string>();
        var rangeStart = DateTime.Now.AddYears(-1);
        var rangeEnd = DateTime.Now.AddYears(2);

        var calendarFile = FindCalendarFile();
        if (calendarFile == null) return [];

        try
        {
            var icsContent = File.ReadAllText(calendarFile);
            var calendar = CalendarSerializer.Deserialize(icsContent);
            if (calendar == null) return [];

            foreach (var component in calendar.Events)
            {
                if (component is not CalendarEvent ev) continue;
                var (team, category) = ParseCategoriesFromEvent(ev);
                if (!string.IsNullOrEmpty(team)) values.Add(team);
                if (!string.IsNullOrEmpty(category)) values.Add(category);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract distinct field values from calendar file");
        }

        return values.OrderBy(v => v).ToList();
    }
}