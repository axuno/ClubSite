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
        private readonly ClubSite.Data.ClubDbContext _clubDbContext;

        public TournamentRegistrationModel(ClubSite.Data.ClubDbContext context)
        {
            _clubDbContext = context;
        }

        public IList<TournamentRegistration> TournamentRegistration { get;set; }

        public async Task OnGetAsync()
        {
            TournamentRegistration = await _clubDbContext.TournamentRegistration.Where(tr => tr.TournamentDate.Year == 2020).ToListAsync();
        }
    }
}
