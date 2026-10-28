// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models;

/// <summary>
/// PIRANHA Page Type for the calendar events page.
/// Displays events from .ics files using iCal.net.
/// </summary>
[PageType(Title = "Terminkalender", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = "/CalendarPage")]
public class CalendarPage : Page<CalendarPage>
{
}
