using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;

namespace ClubSite.Pages;

[BindProperties]
public class CalendarPageModel : SinglePage<Models.CalendarPage>
{
    private readonly ICalendarService _calendarService;

    public CalendarPageModel(ICalendarService calendarService, Piranha.IApi api, IModelLoader loader)
        : base(api, loader)
    {
        _calendarService = calendarService;
    }

    /// <summary>
    /// Filter: team (e.g. Damen, Herren, Jugend). Empty = all teams.
    /// </summary>
    public string TeamFilter { get; set; } = string.Empty;

    /// <summary>
    /// Filter: category (e.g. Training, Spieltag). Empty = all categories.
    /// </summary>
    public string CategoryFilter { get; set; } = string.Empty;

    /// <summary>
    /// Display mode: "upcoming" (default) or "past".
    /// </summary>
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
    /// Whether calendar files exist.
    /// </summary>
    public bool HasIcsFiles => _calendarService.HasCalendarFiles();

    public override Task<IActionResult> OnGet(Guid id, bool draft = false)
    {
        base.OnGet(id, draft);

        LoadFilterFromQueryString();
        LoadEvents();

        return Task.FromResult<IActionResult>(Page());
    }

    private void LoadFilterFromQueryString()
    {
        TeamFilter = Request.Query[nameof(TeamFilter)].FirstOrDefault() ?? string.Empty;
        CategoryFilter = Request.Query[nameof(CategoryFilter)].FirstOrDefault() ?? string.Empty;
        Mode = Request.Query[nameof(Mode)].FirstOrDefault() ?? "upcoming";
    }

    private void LoadEvents()
    {
        var now = DateTime.Now;

        var (rangeStart, rangeEnd) = Mode == "past"
            ? (now.AddMonths(-6), now)
            : (now.AddMonths(-1), now.AddMonths(6));

        var allEvents = _calendarService.GetEvents(rangeStart, rangeEnd);

        AvailableTeams = _calendarService.GetAvailableTeams();
        AvailableCategories = _calendarService.GetAvailableCategories();

        var filtered = allEvents.AsEnumerable();

        if (!string.IsNullOrEmpty(TeamFilter))
            filtered = filtered.Where(e => e.Team == TeamFilter);

        if (!string.IsNullOrEmpty(CategoryFilter))
            filtered = filtered.Where(e => e.Category == CategoryFilter);

        if (Mode == "past")
            filtered = filtered.Where(e => e.StartDate < now);
        else
            filtered = filtered.Where(e => e.StartDate >= now);

        Events = filtered.OrderBy(e => e.StartDate).ToList();
    }
}