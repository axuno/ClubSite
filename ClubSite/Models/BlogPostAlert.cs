// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Linq;
using System.Threading.Tasks;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.Data;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;
using Block = Piranha.Extend.Block;

namespace ClubSite.Models
{
    [BlockType(Name = "Blog Post Alert", Category = "Content", Icon = "fas fa-rss")]
    public class BlogPostAlertBlock : Block
    {
        [Field(Title = "Kurzbeschreibung", Description = "Erscheint immer, wenn der Alert gezeigt wird.")] 
        [StringFieldSettings(MaxLength = 75)]
        public StringField Description { get; set; } = new();

        [Field(Title = "Anzeigedauer (Tage)", Description = "Anzeigedauer nach Veröffentlichung des letzten Blog-Beitrags (1-14 Tage)")]
        public NumberField? MaxDaysSincePublication { get; set; } = new();

        public Post? Post { get; private set; }

        public Task Init(IDb db, IApi api)
        {
            var days = MaxDaysSincePublication?.Value ?? 0;
            var latestPostsFromAllArchives = db.Posts
                .Where(p => p.Published >= DateTime.Now.AddDays(days * -1))
                .OrderByDescending(p => p.Published)
                .Take(1)
                .Select(p => p)
                .FirstOrDefault();
            Post = latestPostsFromAllArchives;
            return Task.CompletedTask;
        }
    }
}