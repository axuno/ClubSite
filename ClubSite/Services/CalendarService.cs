// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ClubSite.Services;

/// <summary>
/// Service to load, parse and filter .ics calendar files using iCal.net.
/// </summary>
public class CalendarService : ICalendarService
{
    private readonly string _webRoot;
    private readonly ILogger<CalendarService> _logger;

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
        var items = new List<CalendarEventItem>();

        var calendarFiles = FindCalendarFiles();
        if (calendarFiles.Count == 0)
        {
            _logger.LogWarning("No calendar files found in uploads directory");
            return items;
        }

        foreach (var filePath in calendarFiles)
        {
            try
            {
                var icsContent = File.ReadAllText(filePath);
                var calendar = Calendar.Load(icsContent);
                if (calendar != null)
                    items.AddRange(ParseCalendar(calendar, rangeStart, rangeEnd));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse calendar file {FilePath}", filePath);
            }
        }

        return items.OrderBy(e => e.StartDate).ToList();
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
    /// Checks whether at least one calendar file exists.
    /// </summary>
    public bool HasCalendarFiles()
    {
        return FindCalendarFiles().Count > 0;
    }

    /// <summary>
    /// Finds the calendar file (events.ics) under wwwroot/uploads/.
    /// If multiple exist, returns only the newest one.
    /// </summary>
    private List<string> FindCalendarFiles()
    {
        var uploadsRoot = Path.Combine(_webRoot, "uploads");
        if (!Directory.Exists(uploadsRoot))
            return new List<string>();

        var files = Directory.GetFiles(uploadsRoot, "events.ics", SearchOption.AllDirectories).ToList();

        if (files.Count > 1)
        {
            var newest = files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
            _logger.LogInformation("Multiple calendar files found, using newest: {FilePath}", newest);
            return new List<string> { newest };
        }

        return files;
    }

    private List<CalendarEventItem> ParseCalendar(Calendar calendar, DateTime rangeStart, DateTime rangeEnd)
    {
        var items = new List<CalendarEventItem>();

        foreach (var component in calendar.Events)
        {
            if (component is not CalendarEvent ev) continue;
            if (ev.DtStart == null) continue;

            var (team, category) = ParseCategoriesFromEvent(ev);
            var start = ToLocal(ev.DtStart);
            var end = ev.DtEnd != null ? ToLocal(ev.DtEnd) : start;
            var duration = end - start;

            if (ev.RecurrenceRule != null &&
                ev.RecurrenceRule.Frequency == FrequencyType.Weekly)
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

        var dayNumbers = new List<int>();
        if (rule.ByDay is { Count: > 0 })
        {
            dayNumbers.AddRange(rule.ByDay.Select(wd => (int)wd.DayOfWeek));
        }
        else
        {
            dayNumbers.Add((int)start.DayOfWeek);
        }

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
        var team = string.Empty;
        var category = string.Empty;

        if (ev.Categories is not { Count: > 0 })
            return (team, category);

        team = ev.Categories[0];
        if (ev.Categories.Count > 1)
            category = ev.Categories[1];

        return (team, category);
    }

    private DateTime ToLocal(CalDateTime cal)
    {
        if (cal == null) return DateTime.Now;
        if (cal.IsUtc)
            return TimeZoneInfo.ConvertTimeFromUtc(cal.Value, TimeZoneInfo.Local);
        return cal.Value;
    }

    private List<string> GetDistinctFieldValues(Func<CalendarEventItem, string> selector)
    {
        var values = new HashSet<string>();
        var rangeEnd = DateTime.Now.AddYears(2);
        var rangeStart = DateTime.Now.AddYears(-1);

        foreach (var item in GetEvents(rangeStart, rangeEnd))
        {
            var val = selector(item);
            if (!string.IsNullOrEmpty(val))
                values.Add(val);
        }

        return values.OrderBy(v => v).ToList();
    }
}