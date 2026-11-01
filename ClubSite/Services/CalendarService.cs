using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
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
            // ICal.Net 6 fails on non-URI ORGANIZER values (e.g. "ORGANIZER:Volleyball")
            var cleaned = System.Text.RegularExpressions.Regex.Replace(
                icsContent, @"^ORGANIZER:.*$", "ORGANIZER:mailto:club@volleyballclub.de",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            var calendars = CalendarSerializer.DeserializeCollection<Calendar>(cleaned);
            var items = new List<CalendarEventItem>();
            foreach (var cal in calendars)
            {
                if (cal != null)
                    items.AddRange(ParseCalendar(cal, rangeStart, rangeEnd));
            }
            return items.OrderBy(e => e.StartDate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse calendar file {FilePath}", calendarFile);
        }

        return [];
    }

    public List<string> GetAvailableTeams() => GetDistinctFieldValues(e => e.Team);
    public List<string> GetAvailableCategories() => GetDistinctFieldValues(e => e.Category);

    public bool HasCalendarFiles()
    {
        _hasCalendarFiles ??= FindCalendarFile() != null;
        return _hasCalendarFiles.Value;
    }

    private string? FindCalendarFile()
    {
        var uploadsRoot = Path.Combine(_webRoot, UploadsDir);
        if (!Directory.Exists(uploadsRoot))
            return null;

        var files = Directory.GetFiles(uploadsRoot, CalendarFileName, SearchOption.AllDirectories);
        if (files.Length == 0) return null;
        if (files.Length <= 1) return files[0];

        var newest = files.OrderByDescending(File.GetLastWriteTime).First();
        _logger.LogInformation("Multiple calendar files found, using newest: {FilePath}", newest);
        return newest;
    }

    private List<CalendarEventItem> ParseCalendar(Calendar calendar, DateTime rangeStart, DateTime rangeEnd)
    {
        var items = new List<CalendarEventItem>();

        foreach (var ev in calendar.Events)
        {
            if (ev.DtStart == null) continue;

            var (team, category) = ParseCategoriesFromEvent(ev);
            var start = ToLocal(ev.DtStart);
            var end = ev.DtEnd != null ? ToLocal(ev.DtEnd) : start;
            var duration = end - start;

            if (IsWeeklyRecurrence(ev))
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

    private static bool IsWeeklyRecurrence(CalendarEvent ev)
    {
        return ev.RecurrenceRule != null && ev.RecurrenceRule.Frequency == FrequencyType.Weekly;
    }

    private static List<CalendarEventItem> ExpandWeeklyRecurrence(
        CalendarEvent ev, DateTime start, TimeSpan duration,
        DateTime rangeStart, DateTime rangeEnd,
        string team, string category)
    {
        var items = new List<CalendarEventItem>();
        var rule = ev.RecurrenceRule!;

        var until = rule.Until != null
            ? (rule.Until.ToDateTimeUnspecified() < rangeEnd ? rule.Until.ToDateTimeUnspecified() : rangeEnd)
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

        return (ev.Categories[0], ev.Categories.Count > 1 ? ev.Categories[1] : string.Empty);
    }

    private static DateTime ToLocal(CalDateTime cal)
    {
        var dt = cal.ToDateTimeUnspecified();
        if (cal.IsUtc)
            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
        return dt;
    }

    private List<string> GetDistinctFieldValues(Func<CalendarEventItem, string> selector)
    {
        var values = new HashSet<string>();

        var calendarFile = FindCalendarFile();
        if (calendarFile == null) return [];

        try
        {
            var icsContent = File.ReadAllText(calendarFile);
            var cleaned = System.Text.RegularExpressions.Regex.Replace(
                icsContent, @"^ORGANIZER:.*$", "ORGANIZER:mailto:club@volleyballclub.de",
                System.Text.RegularExpressions.RegexOptions.Multiline);
            var calendars = CalendarSerializer.DeserializeCollection<Calendar>(cleaned);

            foreach (var cal in calendars)
            {
                if (cal == null) continue;
                foreach (var ev in cal.Events)
            {
                var (team, category) = ParseCategoriesFromEvent(ev);
                if (!string.IsNullOrEmpty(team)) values.Add(team);
                if (!string.IsNullOrEmpty(category)) values.Add(category);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract distinct field values from calendar file");
        }

        return [.. values.OrderBy(v => v)];
    }
}