// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubSite.Data.Poco;
using ClubSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "Anmeldungen.xlsx");
            }
            
            return result;
        }

        public IList<TournamentRegistration> AllRegistrations { get; set; } = new List<TournamentRegistration>();

        private async Task GetRegistrations()
        {
            if (_clubDbContext.TournamentRegistration is null) return; // take the default empty list

            AllRegistrations = await _clubDbContext.TournamentRegistration.Where(tr =>
                tr.TournamentDate.Date ==
                (Data.TournamentDefinition.DateFrom.Value.HasValue
                    ? Data.TournamentDefinition.DateFrom.Value.Value.Date
                    : new DateTime())).ToListAsync();
        }

        private void CreateCsv(Stream stream)
        {
            using var p = new ExcelPackage();

            // A workbook must have at least on cell, so lets add one:
            var ws = p.Workbook.Worksheets.Add("Anmeldungen");
            // To set values in the spreadsheet we use the Cells indexer:
            ws.Cells["A1"].Value = "Nr.";
            ws.Cells["B1"].Value = "Id";
            ws.Cells["C1"].Value = "Turnierdatum";
            ws.Cells["D1"].Value = "Teamname";
            ws.Cells["E1"].Value = "Clubname";
            ws.Cells["F1"].Value = "Anrede";
            ws.Cells["G1"].Value = "Vorname";
            ws.Cells["H1"].Value = "Nachname";
            ws.Cells["I1"].Value = "Telefon";
            ws.Cells["J1"].Value = "E-Mail";
            ws.Cells["K1"].Value = "Nachricht";
            ws.Cells["L1"].Value = "Nachrücker";
            ws.Cells["M1"].Value = "Angemeldet";
            ws.Cells["N1"].Value = "Abgemeldet";

            var line = 1;
            foreach (var reg in AllRegistrations)
            {
                line++;
                ws.Cells[$"B{line}"].Value = reg.RegistrationId;
                ws.Cells[$"C{line}"].Value = reg.TournamentDate;
                ws.Cells[$"C{line}"].Style.Numberformat.Format = "dd.MM.yyyy";
                ws.Cells[$"D{line}"].Value = reg.TeamName;
                ws.Cells[$"E{line}"].Value = reg.ClubName;
                ws.Cells[$"F{line}"].Value = reg.Gender == "f" ? "Frau" : "Herr";
                ws.Cells[$"G{line}"].Value = reg.FirstName;
                ws.Cells[$"H{line}"].Value = reg.LastName;
                ws.Cells[$"I{line}"].Value = reg.Fone;
                ws.Cells[$"I{line}"].Style.Numberformat.Format = "@"; // @ means "text"
                ws.Cells[$"J{line}"].Value = reg.Email;
                ws.Cells[$"K{line}"].Value = reg.Message;
                ws.Cells[$"K{line}"].Style.WrapText = true;
                ws.Cells[$"L{line}"].Value = reg.IsStandByRegistration;
                ws.Cells[$"M{line}"].Value = reg.RegisteredOn;
                ws.Cells[$"M{line}"].Style.Numberformat.Format = "dd.MM.yyyy";
                ws.Cells[$"N{line}"].Value = reg.RegCanceledOn;
                ws.Cells[$"N{line}"].Style.Numberformat.Format = "dd.MM.yyyy";
            }
            
            ws.Cells["A1:N1"].Style.Font.Bold = true;
            ws.Cells["A1:N1"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            ws.Cells[$"A2:A{line}"].FillNumber(1, 1);
            ws.Cells[$"A1:N{line}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            ws.Cells.AutoFitColumns();
            ws.Columns[11].Width = 40; // column width for optional message

            p.SaveAs(stream);
            stream.Flush();
            stream.Position = 0;
        }
    }
}