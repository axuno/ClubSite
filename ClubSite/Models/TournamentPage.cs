// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace ClubSite.Models
{
    /// <summary>
    /// Custom <see cref="TournamentPage"/> definition.
    /// </summary>
    [PageType(Title = "Turnier VC Neusäß", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
    [ContentTypeRoute(Title = "Default",
        Route = "/TournamentPage")] // Route is the base name of the Page name (TournamentPage.cshtml)
    public class TournamentPage : Page<TournamentPage>
    {
        [Region(Title = "Turnierbeschreibung", Description = "Festlegung der Eckdaten des Turniers")]
        public TournamentDefinition TournamentDefinition { get; set; } = new();
    }
}