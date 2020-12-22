//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubSite.Data.Poco;
using ClubSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;

namespace ClubSite.Pages
{
    public class TournamentPageModel : SinglePage<TournamentPage>
    {
        private readonly IDb _db;
        private readonly ClubSite.Data.ClubDbContext _clubDbContext;

        public TournamentPageModel(IApi api, IModelLoader loader, IDb db, ClubSite.Data.ClubDbContext clubDbContext) : base(api, loader)
        {
            _db = db;
            _clubDbContext = clubDbContext;
        }

        public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            var result = await base.OnGet(id, draft);
            Registrations = await _clubDbContext.TournamentRegistration.Where(tr =>
                !tr.RegCanceledOn.HasValue &&
                tr.TournamentDate.Date ==
                (Data.TournamentDefinition.DateFrom != null && Data.TournamentDefinition.DateFrom.Value.HasValue
                    ? Data.TournamentDefinition.DateFrom.Value.Value.Date
                    : new DateTime())).ToListAsync();
            return result;
        }

        public IList<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    }
}
