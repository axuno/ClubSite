// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClubSite.Data;
using ClubSite.Data.Poco;
using ClubSite.Library;
using ClubSite.Models;
using ClubSite.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Piranha;

namespace ClubSite.Pages
{
    /// <summary>
    /// The class is the model for a tournament registration.
    /// </summary>
    /// <remarks>
    /// For model binding, the properties of a model MUST NOT BE pre-initialized.
    /// Pre-initialized fields makes these fields "required" and cause unexpected model errors.
    /// </remarks>
    [AutoValidateAntiforgeryToken]
    [Bind(nameof(TournamentDate), nameof(Captcha), nameof(Registration))]
    public class TournamentRegistrationModel : PageModel
    {
        private readonly ClubDbContext _clubDbContext;
        private readonly IApi _api;
        private readonly Services.IMailService _mailService;
        private readonly ILogger<TournamentRegistrationModel> _logger;
        
        public TournamentRegistrationModel(ClubDbContext context, IApi api, Services.IMailService mailService, ILogger<TournamentRegistrationModel> logger)
        {
            _clubDbContext = context;
            _api = api;
            _mailService = mailService;
            _logger = logger;
            TournamentPage = new TournamentPage();
            Registration = new TournamentRegistration();
        }

        [BindProperty]
        public TournamentRegistration Registration { get; set; }

        [BindProperty]
        [Display(Name = "Lösung")]
        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        public string? Captcha { get; set; }
        
        [BindProperty] // hidden field
        public long TournamentDate { get; set; }
        
        [BindNever]
        public TournamentPage TournamentPage { get; set; }
        
        [BindNever]
        public bool MaxRegistrationsReached { get; set; }
        
        [BindNever]
        public IList<TournamentRegistration> AllRegistrations { get; set; } = new List<TournamentRegistration>();

        public async Task OnGetAsync(long dateTicks, Guid registrationId)
        {
            try
            {
                await SetupModel(dateTicks, registrationId);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error setting up the model for date {0}", new DateTime(dateTicks), e);
            }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Model must always be set up
                await SetupModel(TournamentDate, Registration.RegistrationId);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error setting up the model for date {0}", new DateTime(TournamentDate), e);
            }
            
            // Handle data annotation model binding errors
            if (!ModelState.IsValid) return Page();

            ModelState.Clear();
            
            await TryUpdateModelAsync(new {Registration});
            if (Captcha != HttpContext.Session.GetString(CaptchaSvgGenerator.CaptchaSessionKeyName) &&
                !string.IsNullOrEmpty(Captcha))
                ModelState.AddModelError(nameof(Captcha), "Ergebnis der Rechenaufgabe war nicht korrekt");
            if (AllRegistrations.Any(ar =>
                ar.TeamName != null 
                && ar.Id != Registration.Id 
                && ar.TeamName.Replace(" ", string.Empty).Equals(
                    Registration.TeamName?.Replace(" ", string.Empty),
                    StringComparison.CurrentCultureIgnoreCase)))
                ModelState.AddModelError(nameof(Registration) + "." + nameof(Registration.TeamName),
                    "Teamname ist bereits vergeben");
            if (!ModelState.IsValid) return Page();

            try
            {
                var isNewRegistration = Registration.RegistrationId == new Guid() && Registration.Id == 0;
                var now = DateTime.Now;
                if (isNewRegistration)
                {
                    // New record
                    Registration.RegistrationId = Guid.NewGuid();
                    Registration.CreatedOn = Registration.ModifiedOn = Registration.RegisteredOn = now;
                    Registration.TournamentDate = new DateTime(TournamentDate);
                    Registration.IsStandByRegistration = MaxRegistrationsReached;
                    await _clubDbContext.TournamentRegistration!.AddAsync(Registration, cancellationToken);
                }
                else
                {
                    Registration.ModifiedOn = now;
                }
                
                await _clubDbContext.SaveChangesAsync(cancellationToken);
                await _clubDbContext.DisposeAsync();

                if (isNewRegistration)
                {
                    await _mailService.SendTournamentRegistrationEmailAsync(Registration.GetCompleteName(),
                        Registration.Email!,
                        $"Anmeldung zum Turnier am {TournamentPage.TournamentDefinition.DateFrom.Value!.Value.ToShortDateString()}",
                        GetConfirmationMessage(), cancellationToken);
                }
                
                // Todo: Add notification for successful saving on TournamentPage
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error saving the tournament registration", e);
                // Todo: Add notification for failure of saving on TournamentPage
            }
            
            // Redirect to the TournamentPage
            return Redirect(TournamentPage?.Permalink ?? "/");            
        }

        private async Task SetupModel(long date, Guid? registrationId)
        {
            // Create date from ticks
            var tournamentDate = new DateTime(date).Date;
            
            // Get the TournamentPage containing the definition for the given date
            TournamentPage = (await _api.Pages.GetAllAsync<TournamentPage>()).FirstOrDefault(p =>
                                  p.TournamentDefinition.DateFrom.Value.HasValue &&
                                  p.TournamentDefinition.DateFrom.Value.Value.Date.Equals(tournamentDate)) ??
                              throw new Exception($"{nameof(Models.TournamentPage)} for date '{tournamentDate}' not found");
            
            var definition = TournamentPage.TournamentDefinition;

            if (definition == null || definition.DateFrom == null || !definition.DateFrom.Value.HasValue || !definition.NumberOfTeams.Value.HasValue)
            {
                throw new Exception($"Tournament definition has no '{nameof(definition.DateFrom)}' defined. {nameof(TournamentRegistrationModel)} cannot be created.");
            }

            AllRegistrations = (await _clubDbContext.TournamentRegistration?
                .Where(tr => tr.TournamentDate.Date == definition.DateFrom.Value.Value.Date).ToListAsync()! ?? new List<TournamentRegistration>());

            MaxRegistrationsReached =
                AllRegistrations.Count(tr => !tr.RegCanceledOn.HasValue) >= definition.NumberOfTeams.Value.Value;

            TournamentDate = definition.DateFrom.Value.Value.Ticks;
            
            if (registrationId != null)
            {
                Registration =
                    AllRegistrations.FirstOrDefault(r => r.RegistrationId.Equals(registrationId)) ??
                    new TournamentRegistration();
            }
        }
        
        private string GetConfirmationMessage()
        {
            var definition = TournamentPage!.TournamentDefinition;
            return
                $@"Eingangsbestätigung der Turnieranmeldung {(Registration.IsStandByRegistration ? "\n(als möglicher \"Nachrücker\")" : "")}
{definition.Name.Value}
{new string('=', definition.Name.Value.Length)}

Datum:       {definition.DateFrom.Value!.Value.ToLongDateString()}
Adresse:     {definition.Address.Value}
Startgebühr: {definition.EntryFee.Value}

Angemeldet von:
Name:    {(Registration.Gender == "f" ? "Frau" : "Herrn")} {Registration.GetCompleteName()}
Telefon: {Registration.Fone}
E-Mail:  {Registration.Email}

Nachricht:
----------------------------------------
{(string.IsNullOrWhiteSpace(Registration.Message) ? "(keine)" : "Message")}
----------------------------------------
{(Registration.IsStandByRegistration ? "\n" : "\nBankverbindung für die Startgebühr:\n" + definition.BankDetails.Value + "\n" )}
Vielen Dank!

Volleyballclub Neusäß e.V.


Browser: {(Request.Headers.ContainsKey(HeaderNames.UserAgent) ? Request.Headers[HeaderNames.UserAgent].ToString() : "unbekannt")}
IP-Adresse: {HttpContext.Connection.RemoteIpAddress}
";
        }
    }
}