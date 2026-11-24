// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace ClubSite.Services;

/// <summary>
/// Service interface for loading, parsing and filtering Piranha-managed .ics calendar documents using iCal.Net.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Loads the provided Piranha media documents and returns events within the specified Berlin wall-clock date range,
    /// evaluating RRULEs and RECURRENCE-ID overrides for recurring events.
    /// </summary>
    Task<CalendarLoadResult> LoadEventsAsync(
        IEnumerable<Media> calendarDocuments,
        DateTime? rangeStart,
        DateTime? rangeEnd);
}

/// <summary>
/// Aggregated result returned when loading one or more Piranha-managed .ics calendar documents.
/// </summary>
public class CalendarLoadResult
{
    public IReadOnlyList<CalendarEventItem> Events { get; init; } = [];
    public IReadOnlyList<string> AvailableTeams { get; init; } = [];
    public IReadOnlyList<string> AvailableCategories { get; init; } = [];
    public DateTime? LastUpdated { get; init; }
}

/// <summary>
/// Represents a single calendar event ready for display.
/// </summary>
public class CalendarEventItem
{
    public string Title { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsAllDay { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Team { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Url { get; init; }
    public bool IsHomeGame { get; init; }
}
