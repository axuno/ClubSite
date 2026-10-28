// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;

namespace ClubSite.Services;

/// <summary>
/// Service interface for loading, parsing and filtering .ics calendar files using iCal.net.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Loads all calendar files and returns events within the specified date range,
    /// evaluating RRULE for recurring events.
    /// </summary>
    List<CalendarEventItem> GetEvents(DateTime rangeStart, DateTime rangeEnd);

    /// <summary>
    /// Returns distinct team values from all .ics files (for filter dropdowns).
    /// </summary>
    List<string> GetAvailableTeams();

    /// <summary>
    /// Returns distinct category values from all .ics files (for filter dropdowns).
    /// </summary>
    List<string> GetAvailableCategories();

    /// <summary>
    /// Checks whether at least one calendar file exists.
    /// </summary>
    bool HasCalendarFiles();
}

/// <summary>
/// Represents a single calendar event ready for display.
/// </summary>
public class CalendarEventItem
{
    public string Title { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsAllDay => StartDate.TimeOfDay == TimeSpan.Zero && EndDate.TimeOfDay == TimeSpan.Zero;
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Team { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
}