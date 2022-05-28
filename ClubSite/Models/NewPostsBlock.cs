// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha;
using Piranha.Data;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models
{
    [BlockType(Name = "Latest Posts", Category = "References", Icon = "fas fa-link")]
    public class NewPostsBlock : Piranha.Extend.Block
    {
        private const int NumOfDays = 30;
        private const int MaxPosts = 2;
        
        [Field(Title = "Überschrift")]
        public StringField Title { get; set; } = new() { Value = string.Empty };

        [Field(Title = "Tage seit letztem Beitrag", Description = "In diesem Zeitraum werden Beiträge als \"aktuell\" behandelt.")]
        public NumberField NumOfDaysSincePublication { get; set; } = new() { Value = NumOfDays };

        [Field(Title = "Max. Anzahl der angezeigten Beiträge", Description = "Empfohlen: 2. Werte kleiner 1 unterdrücken die Ausgabe")]
        public NumberField NumOfPostsToDisplay { get; set; } = new() { Value = MaxPosts };

        public List<PostInfo> Posts { get; private set; } = new();

        public PageInfo ArchivePage { get; set; } = new();
        public async Task Init(IDb db, IApi api)
        {
            var blogId = db.Set<Page>().First(p => p.PageTypeId == nameof(StandardArchive)).Id;
            // PageInfo will exclude all regions and blocks from the model, which we don't need here
            ArchivePage = await api.Pages.GetByIdAsync<PageInfo>(blogId);

            if(NumOfPostsToDisplay < 1) return;
            var postArchive = await api.Archives.GetByIdAsync<PostInfo>(blogId);
            
            // Returns only posts that have IsPublished true, while api.Posts.GetAllAsync gets ALL
            var posts = postArchive.Posts.Take(NumOfPostsToDisplay.Value ?? 0).ToList();
            if (posts.Any()
                && posts.First().Published >= DateTime.Now.Date.AddDays(-1 * NumOfDaysSincePublication.Value ?? 0))
            {
                Posts = posts;
            }
        }
    }
}