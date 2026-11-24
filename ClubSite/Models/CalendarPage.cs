// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.Collections.Generic;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models;

/// <summary>
/// PIRANHA Page Type for the calendar events page.
/// Displays events from one or more Piranha-managed .ics document assets using iCal.Net.
/// </summary>
[PageType(Title = "Terminkalender", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = "/CalendarPage")]
public class CalendarPage : Page<CalendarPage>
{
    /// <summary>
    /// Gets or sets the list of .ics calendar files managed through the Piranha Media Manager.
    /// </summary>
    [Region(
        Title = "Kalenderdateien (.ics)",
        ListTitle = "Title",
        ListPlaceholder = "Kalenderdatei (.ics)",
        Icon = "fas fa-calendar-alt",
        Description = "Eine oder mehrere .ics-Kalenderdateien aus der Piranha-Mediathek auswählen.")]
    public IList<CalendarDocumentRegion> CalendarFiles { get; set; } = new List<CalendarDocumentRegion>();
}

/// <summary>
/// Region item referencing a single .ics document managed by Piranha.
/// </summary>
public class CalendarDocumentRegion
{
    /// <summary>
    /// Optional label shown in the Piranha Manager list view (e.g. "BVV Spieltage" or "Vereinstermine").
    /// </summary>
    [Field(Title = "Bezeichnung", Options = FieldOption.HalfWidth, Placeholder = "z. B. Spieltage oder Vereinstermine")]
    public StringField Title { get; set; } = new();

    /// <summary>
    /// Document asset pointing to the uploaded .ics file.
    /// </summary>
    [Field(Title = ".ics-Datei", Options = FieldOption.HalfWidth)]
    public DocumentField Document { get; set; } = new();
}
