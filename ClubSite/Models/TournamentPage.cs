// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models
{
    /// <summary>
    /// Custom <see cref="TournamentPage"/> definition.
    /// </summary>
    [PageType(Title = "Turnier VC Neusäß", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
    [PageTypeRoute(Title = "Default",
        Route = "/TournamentPage")] // Route is the base name of the Page name (TournamentPage.cshtml)
    public class TournamentPage : Page<TournamentPage>
    {
        [Region(Title = "Turnierbeschreibung")]
        [RegionDescription("Festlegung der Eckdaten des Turniers")]
        public TournamentDefinition TournamentDefinition { get; set; } = new TournamentDefinition();
    }

    public class TournamentDefinition
    {
        [Field(Title = "Primäres Bild", Placeholder = "Bild einsetzen")]
        public ImageField TopImage { get; set; } = new ImageField();
        
        [Field(Title = "Turniername", Placeholder = "z.B. 28. Neusäßer Mixedturnier")]
        public StringField Name { get; set; } = new StringField();

        [Field(Title = "Datum von", Placeholder = "Datum")]
        public DateField DateFrom { get; set; } = new DateField();

        [Field(Title = "Datum bis", Placeholder = "Datum")]
        public DateField DateTo { get; set; } = new DateField();

        [Field(Title = "Uhrzeit von/bis", Placeholder = "Uhrzeit")]
        public StringField TimeFromTo { get; set; } = new StringField();

        [Field(Title = "Beginn der Anmeldefrist", Placeholder = "Datum")]
        public DateField RegistrationStart { get; set; } = new DateField();

        [Field(Title = "Anmeldeschluss", Placeholder = "Datum")]
        public DateField RegistrationDeadline { get; set; } = new DateField();

        [Field(Title = "Anzahl Teams", Placeholder = "Anzahl")]
        public NumberField NumberOfTeams { get; set; } = new NumberField();

        [Field(Title = "Team-Zusammensetzung", Placeholder = "z.B. Anzahl Damen/Herren")]
        public StringField TeamComposition { get; set; } = new StringField();

        [Field(Title = "Startgeld", Placeholder = "Startgeld + ggf. Kaution")]
        public StringField EntryFee { get; set; } = new StringField();

        [Field(Title = "Adresse", Placeholder = "Hallenadresse")]
        public StringField Address { get; set; } = new StringField();

        [Field(Title = "Infos", Placeholder = "Ablauf, Regeln, Infos")]
        public HtmlField Infos { get; set; } = new HtmlField();
    }
}