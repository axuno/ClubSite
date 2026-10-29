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
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;

namespace ClubSite.Pages;

/// <summary>Value/label pair for shared dropdown options.</summary>
public class Option
{
    public string V { get; set; } = string.Empty;
    public string L { get; set; } = string.Empty;
}

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

    // ────────────────────────────────────────────────────────
    // Shared option definitions (single source of truth)
    // Used by both the view (dropdowns) and GetFormMailMessage()
    // ────────────────────────────────────────────────────────

    public List<Option> GenderOptions() => new()
    {
        new Option { V = string.Empty, L = "keine Angabe" },
        new Option { V = "f", L = "Frau" },
        new Option { V = "m", L = "Herr" }
    };

    public List<Option> ExperienceOptions() => new()
    {
        new Option { V = string.Empty, L = "Bitte auswählen" },
        new Option { V = "beginner", L = "Anfänger" },
        new Option { V = "some-experience", L = "geringe Erfahrung" },
        new Option { V = "hobby", L = "Hobbyniveau" },
        new Option { V = "advanced", L = "fortgeschritten" },
        new Option { V = "club-player", L = "Vereinsspieler" }
    };

    public List<Option> TeamOptions() => new()
    {
        new Option { V = string.Empty, L = "Bitte auswählen" },
        new Option { V = "none", L = "keine Präferenz" },
        new Option { V = "mixed", L = "Mixed" },
        new Option { V = "women", L = "Damen" },
        new Option { V = "men", L = "Herren" },
        new Option { V = "youth", L = "Jugendliche" }
    };

    public List<Option> GoalOptions() => new()
    {
        new Option { V = string.Empty, L = "Bitte auswählen" },
        new Option { V = "other", L = "Sonstiges" },
        new Option { V = "active", L = "Aktiv spielen in einer Mannschaft" },
        new Option { V = "train-only", L = "Nur trainieren" },
        new Option { V = "explore", L = "Erstmal kennenlernen" }
    };

    /// <summary>Looks up a label from shared options; returns "nicht angegeben" on mismatch.</summary>
    public static string Label(List<Option> options, string value)
    {
        var found = options.Find(o => o.V == value);
        return found is null ? "nicht angegeben" : found.L;
    }

    private string GetFormMailMessage()
    {
        var genderLabel = Label(GenderOptions(), Gender);
        var experienceLabel = Label(ExperienceOptions(), Experience);
        var teamLabel = Label(TeamOptions(), TeamPreference);
        var goalLabel = Label(GoalOptions(), Goal);

        return
            $@"Anfrage für ein Probetraining

{(genderLabel)} {FirstName} {LastName}
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
";
    }
}
