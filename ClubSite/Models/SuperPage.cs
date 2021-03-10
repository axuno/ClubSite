// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

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
    /// Test page using all field types
    /// </summary>
    [PageType(Title = "Super Page", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
    [ContentTypeRoute(Title = "Default",
        Route = "/superpage")] // Route is the base name of the Page name (SuperPage.cshtml)
    public class SuperPage : Page<SuperPage>
    {
        /// <summary>
        /// Gets/sets the available links.
        /// </summary>
        [Region(ListTitle = "ButtonText", ListPlaceholder = "New Link", Icon = "fas fa-quote-right")]
        public IList<Href> Links { get; set; } = new List<Href>();

        [Region(Title = "All fields", Display = RegionDisplayMode.Setting)]
        [RegionDescription(
            "Vestibulum id ligula porta felis euismod <strong>semper</strong>. Curabitur blandit tempus porttitor.")]
        public AllFields? AllFields { get; set; }
    }

    /// <summary>
    /// Simple link region.
    /// </summary>
    public class Href
    {
        [Field(Title = "Button Text", Options = FieldOption.HalfWidth)]
        public StringField? ButtonText { get; set; }

        [Field(Title = "Button Link", Options = FieldOption.HalfWidth)]
        public PageField? ButtonLink { get; set; }
    }

    /// <summary>
    /// Test Field with all field types.
    /// </summary>
    public class AllFields
    {
        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public AudioField? Audio { get; set; }

        [Field(Placeholder = "Etiam porta sem malesuada magna mollis euismod.")]
        public CheckBoxField? CheckBox { get; set; }
        
        [Field(Title = "Color Field Title")]
        public ColorField? ColorField { get; set; }

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
        [FieldDescription(
            "Duis mollis, est non <strong>commodo luctus</strong>, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.")]
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