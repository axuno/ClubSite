// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models
{
    [BlockGroupType(Name = "Person Profile", Category = "Content", Icon = "fas fa-images",
        Display = BlockDisplayMode.Horizontal)]
    [BlockItemType(Type = typeof(ImageBlock))]
    [BlockItemType(Type = typeof(HtmlBlock))]
    public class PersonProfileBlock : BlockGroup
    {
        public StringField Title { get; set; } = new StringField();
    }
}