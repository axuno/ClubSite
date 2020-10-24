﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models
{
    [BlockGroupType(Name = "Person Profile Block", Category = "Media", Icon = "fas fa-images",Display = BlockDisplayMode.Vertical)]
    [BlockItemType(Type = typeof(ImageBlock))]
    [BlockItemType(Type = typeof(Piranha.Extend.Blocks.HtmlBlock))]
    public class PersonProfileBlock : BlockGroup
    {
        public StringField Title { get; set; }
    }
}
