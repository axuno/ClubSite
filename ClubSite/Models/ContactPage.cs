﻿// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models;

/// <summary>
/// Custom <see cref="TournamentPage"/> definition.
/// </summary>
[PageType(Title = "Contact", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
[ContentTypeRoute(Title = "Default",
    Route = "/ContactPage")] // Route is the base name of the Page name (ContactPage.cshtml)
public class ContactPage : Page<ContactPage>
{
}