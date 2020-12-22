//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models
{
    /// <summary>
    /// Basic page with main content in markdown.
    /// </summary>
    [PageType(Title = "Turnier VC Neusäß", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
    [PageTypeRoute(Title = "Default", Route = "/TournamentPage")] // Route is the base name of the Page name (TournamentPage.cshtml)
    public class TournamentPage : Page<TournamentPage>
    {
        [Region(Title = "Turnierbeschreibung")]
        [RegionDescription("Festlegung der Eckdaten des Turniers")]
        public TournamentDefinition TournamentDefinition { get; set; } = new TournamentDefinition();

        /// <summary>
        /// Gets/sets the available links.
        /// </summary>
        //[Region(ListTitle = "ButtonText", ListPlaceholder = "New Link", Icon = "fas fa-quote-right")]
        //public IList<Href2> Links { get; set; } = new List<Href2>();

        //[Region(Title = "All fields")]
        //[RegionDescription("Vestibulum id ligula porta felis euismod <strong>semper</strong>. Curabitur blandit tempus porttitor.")]
        //public AllFields2 AllFields { get; set; }
    }

    public class TournamentDefinition
    {
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


    /// <summary>
    /// Simple link region.
    /// </summary>
    public class Href2
    {
        [Field(Title = "Button Text", Options = FieldOption.HalfWidth)]
        public StringField ButtonText { get; set; } = new StringField();

        [Field(Title = "Button Link", Options = FieldOption.HalfWidth)]
        public PageField ButtonLink { get; set; } = new PageField();
    }

    /// <summary>
    /// Test Field with all field types.
    /// </summary>
    public class AllFields2
    {
        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public AudioField? Audio { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public CheckBoxField? CheckBox { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public DateField? Date { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public HtmlField? Html { get; set; }

        [Field(Options = FieldOption.HalfWidth, Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public DocumentField? Document { get; set; }

        [Field(Options = FieldOption.HalfWidth, Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public ImageField? Image { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public MediaField? Media { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        [FieldDescription("Duis mollis, est non <strong>commodo luctus</strong>, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.")]
        public VideoField? Video { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public MarkdownField? Markdown { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public NumberField? Number { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public PageField? Page { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public PostField? Post { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public StringField? String { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public TextField? Text { get; set; }
    }
}
