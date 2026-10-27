// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ClubSite.Library;
using ClubSite.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [RegularExpression("[mf]", ErrorMessage = "Ungültiger Schlüssel für 'Anrede'")]
    public string Gender { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    [Display(Name = "Geschlecht")]
    [RegularExpression("[mwd]", ErrorMessage = "Ungültiger Schlüssel für 'Geschlecht'")]
    public string GenderInfo { get; set; } = string.Empty;

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
    public string Experience { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "Team-Präferenz")]
    public string TeamPreference { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Display(Name = "Ziel")]
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

    private string GetFormMailMessage()
    {
        var genderLabel = Gender switch
        {
            "f" => "Frau",
            "m" => "Herr",
            _ => "nicht angegeben"
        };

        var genderInfoLabel = GenderInfo switch
        {
            "m" => "männlich",
            "w" => "weiblich",
            "d" => "divers",
            _ => "nicht angegeben"
        };

        var experienceLabel = Experience switch
        {
            "beginner" => "Anfänger",
            "some-experience" => "geringe Erfahrung",
            "hobby" => "Hobbyniveau",
            "advanced" => "fortgeschritten",
            "club-player" => "Vereinsspieler",
            _ => Experience
        };

        var teamLabel = TeamPreference switch
        {
            "mixed" => "Mixed",
            "women" => "Damen",
            "men" => "Herren",
            "youth" => "Jugendliche",
            "none" => "keine Präferenz",
            _ => TeamPreference
        };

        var goalLabel = Goal switch
        {
            "active" => "Aktiv spielen mit Mannschaft",
            "train-only" => "Nur trainieren",
            "explore" => "Erstmal kennenlernen",
            "other" => "Sonstiges",
            _ => Goal
        };

        return
            $@"Anfrage für ein Probetraining

{(genderLabel)} {FirstName} {LastName}
Telefon: {(string.IsNullOrWhiteSpace(PhoneNumber) ? "-" : PhoneNumber)}
E-Mail:  {Email}

Geschlecht: {genderInfoLabel}
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
";
    }
}
