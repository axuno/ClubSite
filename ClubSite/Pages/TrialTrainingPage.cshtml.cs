// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ClubSite.Library;
using ClubSite.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;

namespace ClubSite.Pages;

[BindProperties]
[ValidateAntiForgeryToken]
public class TrialTrainingPageModel : SinglePage<Models.TrialTrainingPage>
{
    private readonly Services.IMailService _mailService;
    private readonly ILogger<TrialTrainingPageModel> _logger;

    public TrialTrainingPageModel(Services.IMailService mailService, IApi api, IModelLoader loader, ILogger<TrialTrainingPageModel> logger) : base(api, loader)
    {
        _mailService = mailService;
        _logger = logger;
    }

    [Display(Name = "E-Mail")]
    [EmailAddress(ErrorMessageResourceName = nameof(DataAnnotationResource.EmailAddressInvalid),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    public string Email { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    [Display(Name = "Anrede")]
    [RegularExpression("[mfu]", ErrorMessage = "Ungültiger Schlüssel für 'Anrede'")]
    public string Gender { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    [Display(Name = "Vorname")]
    public string FirstName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    [Display(Name = "Familienname")]
    public string LastName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Telefon")]
    public string? PhoneNumber { get; set; }

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Range(1, 120)]
    [Display(Name = "Alter")]
    public int Age { get; set; }

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "Spielerfahrung")]
    [RegularExpression("^(beginner|some-experience|hobby|advanced|club-player)$",
        ErrorMessage = "Ungültiger Wert für Spielerfahrung")]
    public string Experience { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "Team-Präferenz")]
    [RegularExpression("^(none|mixed|women|men|youth)$",
        ErrorMessage = "Ungültiger Wert für Team-Präferenz")]
    public string TeamPreference { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "Ziel")]
    [RegularExpression("^(other|active|train-only|explore)$",
        ErrorMessage = "Ungültiger Wert für Ziel")]
    public string Goal { get; set; } = string.Empty;

    [Display(Name = "Anmerkungen")]
    [DataType(DataType.MultilineText)]
    public string? Comment { get; set; }

    [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    public string Captcha { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync(Guid id, bool draft, CancellationToken cancellation)
    {
        try
        {
            Data = await _loader.GetPageAsync<Models.TrialTrainingPage>(id, HttpContext.User, draft);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }

        if (Captcha != HttpContext.Session.GetString(CaptchaSvgGenerator.CaptchaSessionKeyName) && !string.IsNullOrEmpty(Captcha))
            ModelState.AddModelError(nameof(Captcha), "Ergebnis der Rechenaufgabe ist nicht korrekt");
        if (!EmailValidator.IsValid(Email))
            ModelState.AddModelError($"{nameof(Email)}", $"'{nameof(Email)}' enthält keine gültige E-Mail Adresse");

        if (!ModelState.IsValid) return Page();

        try
        {
            HttpContext.Session.Remove(CaptchaSvgGenerator.CaptchaSessionKeyName);
            await _mailService.SendTrialTrainingEmailAsync(
                $"{FirstName.Trim()} {LastName.Trim()}",
                Email,
                GetFormMailMessage(),
                cancellation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Sending trial training email for '{0}' failed.", Email);
        }

        return Redirect("/probetraining:anfrage-erhalten");
    }

    public List<SelectListItem> GenderOptions() => [
        new() { Value = string.Empty, Text = "Bitte auswählen" },
        new() { Value = "f", Text = "Frau" },
        new() { Value = "m", Text = "Herr" },
        new() { Value = "u", Text = "Keine Angabe" }
    ];


    public List<SelectListItem> ExperienceOptions() => [
        new() { Value = string.Empty, Text = "Bitte auswählen" },
        new() { Value = "beginner", Text = "Anfänger" },
        new() { Value = "some-experience", Text = "geringe Erfahrung" },
        new() { Value = "hobby", Text = "Hobbyniveau" },
        new() { Value = "advanced", Text = "fortgeschritten" },
        new() { Value = "club-player", Text = "Vereinsspieler" }
    ];

    public List<SelectListItem> TeamOptions() => [
        new() { Value = string.Empty, Text = "Bitte auswählen" },
        new() { Value = "none", Text = "keine Präferenz" },
        new() { Value = "mixed", Text = "Mixed" },
        new() { Value = "women", Text = "Damen" },
        new() { Value = "men", Text = "Herren" },
        new() { Value = "youth", Text = "Jugendliche" }
    ];

    public List<SelectListItem> GoalOptions() => [
        new() { Value = string.Empty, Text = "Bitte auswählen" },
        new() { Value = "other", Text = "Sonstiges" },
        new() { Value = "active", Text = "Aktiv spielen in einer Mannschaft" },
        new() { Value = "train-only", Text = "Nur trainieren" },
        new() { Value = "explore", Text = "Erstmal kennenlernen" }
    ];

    /// <summary>Looks up a label from shared options; returns "nicht angegeben" on mismatch.</summary>
    public static string Label(List<SelectListItem> options, string value)
    {
        var found = options.Find(o => o.Value == value);
        return found is null ? "[nicht angegeben]" : found.Text!;
    }

    private string GetFormMailMessage()
    {
        var genderLabel = Label(GenderOptions(), Gender);
        genderLabel = genderLabel == "Keine Angabe" ? $"[{genderLabel}]" : genderLabel;
        var experienceLabel = Label(ExperienceOptions(), Experience);
        var teamLabel = Label(TeamOptions(), TeamPreference);
        var goalLabel = Label(GoalOptions(), Goal);

        return
            $"""
             Anfrage für ein Probetraining

             {genderLabel} {FirstName} {LastName}
             Telefon: {(string.IsNullOrWhiteSpace(PhoneNumber) ? "-" : PhoneNumber)}
             E-Mail:  {Email}

             Alter: {Age}
             Spielerfahrung: {experienceLabel}
             Team-Präferenz: {teamLabel}
             Ziel: {goalLabel}

             Anmerkungen:
             ----------------------------------------
             {(string.IsNullOrWhiteSpace(Comment) ? "(keine)" : Comment)}


             ----------------------------------------
             Browser: {(Request.Headers.ContainsKey(HeaderNames.UserAgent) ? Request.Headers[HeaderNames.UserAgent].ToString() : "unbekannt")}
             IP-Adresse: {HttpContext.Connection.RemoteIpAddress}

             """;
    }
}
