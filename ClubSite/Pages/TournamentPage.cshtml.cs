// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
            await GetRegistrations();

            if (HttpContext.Request.Query.ContainsKey("csv"))
            {
                var stream = new MemoryStream();
                CreateCsv(stream);
                return File(
                    stream, 
                    "text/csv", 
                    "Anmeldungen.csv");
            }
            
            return result;
        }

        public IList<TournamentRegistration> AllRegistrations { get; set; } = new List<TournamentRegistration>();

        private async Task GetRegistrations()
        {
            AllRegistrations = await _clubDbContext.TournamentRegistration?.Where(tr =>
                tr.TournamentDate.Date ==
                (Data.TournamentDefinition.DateFrom != null && Data.TournamentDefinition.DateFrom.Value.HasValue
                    ? Data.TournamentDefinition.DateFrom.Value.Value.Date
                    : new DateTime())).ToListAsync()! ?? new List<TournamentRegistration>();
        }

        private void CreateCsv(Stream stream)
        {
            var sb = new StringBuilder();
            sb.Append(
                "Id\tTurnierdatum\tTeamname\tClubname\tAnrede\tVorname\tNachname\tTelefon\tE-Mail\tNachricht\tNachrücker\tAngemeldet\tAbgemeldet\r\n");
            foreach (var reg in AllRegistrations)
            {
                sb.AppendFormat("\"{0:N}\"\t", reg.RegistrationId);
                sb.AppendFormat("\"{0}\"\t", reg.TournamentDate);
                sb.AppendFormat("\"{0}\"\t", reg.TeamName);
                sb.AppendFormat("\"{0}\"\t", reg.ClubName);
                sb.AppendFormat("\"{0}\"\t", reg.Gender == "f" ? "Frau" : "Herr");
                sb.AppendFormat("\"{0}\"\t", reg.FirstName);
                sb.AppendFormat("\"{0}\"\t", reg.LastName);
                sb.AppendFormat("\"{0}\"\t", reg.Fone);
                sb.AppendFormat("\"{0}\"\t", reg.Email);
                sb.AppendFormat("\"{0}\"\t", reg.Message?.Replace("\n", " * ").Replace("\r", ""));
                sb.AppendFormat("\"{0}\"\t", reg.IsStandByRegistration);
                sb.AppendFormat("\"{0}\"\t", reg.RegisteredOn);
                sb.AppendFormat("\"{0}\"\r\n", reg.RegCanceledOn);
            }

            var writer = new StreamWriter(stream,Encoding.UTF8);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
            writer.Flush();
            stream.Position = 0;
        }
    }
}