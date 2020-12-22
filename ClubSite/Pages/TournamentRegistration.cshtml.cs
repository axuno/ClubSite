// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClubSite.Data;
using ClubSite.Data.Poco;

namespace ClubSite.Pages
{
    public class TournamentRegistrationModel : PageModel
    {
        private readonly ClubDbContext _clubDbContext;

        public TournamentRegistrationModel(ClubDbContext context)
        {
            _clubDbContext = context;
        }

        public IList<TournamentRegistration> TournamentRegistration { get; set; } = new List<TournamentRegistration>();

        async public Task OnGetAsync()
        {
            TournamentRegistration = await _clubDbContext.TournamentRegistration
                .Where(tr => tr.TournamentDate.Year == 2020).ToListAsync();
        }
    }
}