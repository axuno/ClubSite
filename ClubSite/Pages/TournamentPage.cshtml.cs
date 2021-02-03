// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubSite.Data.Poco;
using ClubSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;

namespace ClubSite.Pages
{
    public class TournamentPageModel : SinglePage<TournamentPage>
    {
        private readonly IDb _db;
        private readonly Data.ClubDbContext _clubDbContext;

        public TournamentPageModel(IApi api, IModelLoader loader, IDb db, Data.ClubDbContext clubDbContext) : base(api,
            loader)
        {
            _db = db;
            _clubDbContext = clubDbContext;
        }

        public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            var result = await base.OnGet(id, draft);
            AllRegistrations = await _clubDbContext.TournamentRegistration?.Where(tr =>
                tr.TournamentDate.Date ==
                (Data.TournamentDefinition.DateFrom != null && Data.TournamentDefinition.DateFrom.Value.HasValue
                    ? Data.TournamentDefinition.DateFrom.Value.Value.Date
                    : new DateTime())).ToListAsync()! ?? new List<TournamentRegistration>();
            return result;
        }

        public IList<TournamentRegistration> AllRegistrations { get; set; } = new List<TournamentRegistration>();
    }
}