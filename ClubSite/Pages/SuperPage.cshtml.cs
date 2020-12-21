//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using ClubSite.Models;

namespace ClubSite.Pages
{
    public class SuperPageModel : SinglePage<SuperPage>
    {
        private readonly IDb _db;

        public SuperPageModel(IApi api, IModelLoader loader, IDb db) : base(api, loader)
        {
            _db = db;
        }

        public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            var result = await base.OnGet(id, draft);
            
 
 
            return result;
        }
    }
}
