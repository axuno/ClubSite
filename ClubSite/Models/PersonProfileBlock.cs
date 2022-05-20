// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;
using Piranha.Models;

namespace ClubSite.Models
{
    [BlockType(Name = "Person Profile", Category = "Content", Icon = "fas fa-user")]
    public class PersonProfileBlock : Block
    {
        [Field(Title = "Funktionsbezeichnung")] 
        [StringFieldSettings(MaxLength = 30)]
        public StringField Title { get; set; } = new();

        [Field(Title = "Name, Kontaktdaten", Options = FieldOption.HalfWidth)]
        public HtmlField? Contact { get; set; }

        [Field(Title = "Foto", Options = FieldOption.HalfWidth)]
        public ImageField? Photo { get; set; }
    }
}