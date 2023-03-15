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
public class ContactPageModel : SinglePage<Models.ContactPage>
{
    private readonly Services.IMailService _mailService;
    private readonly ILogger<ContactPageModel> _logger;

    public ContactPageModel(Services.IMailService mailService, IApi api, IModelLoader loader, ILogger<ContactPageModel> logger) : base(api, loader)
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
    [Display(Name = "Betreff")]
    [DataType(DataType.Text)]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "Nachricht")] public string? Message { get; set; }

    [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    public string Captcha { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync(Guid id, bool draft, CancellationToken cancellation)
    {
        try
        {
            // The base model loads Data only during 'OnGetAsync'
            Data = await _loader.GetPageAsync<Models.ContactPage>(id, HttpContext.User, draft);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
            
        if (Captcha != HttpContext.Session.GetString(CaptchaSvgGenerator.CaptchaSessionKeyName) && !string.IsNullOrEmpty(Captcha))
            ModelState.AddModelError(nameof(Captcha), "Ergebnis der Rechenaufgabe ist nicht korrekt");
        if(!EmailValidator.IsValid(Email))
            ModelState.AddModelError($"{nameof(Email)}", $"'{nameof(Email)}' enthält keine gültige E-Mail Adresse");

        if (!ModelState.IsValid) return Page();

        try
        {
            HttpContext.Session.Remove(CaptchaSvgGenerator.CaptchaSessionKeyName);
            await _mailService.SendContactFormEmailAsync($"{FirstName.Trim()} {LastName.Trim()}", Email, Subject,
                GetFormMailMessage(), cancellation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Sending mail to '{0}' failed.", Email);
        }

        return Redirect("/kontakt:nachricht-erhalten");
    }

    private string GetFormMailMessage()
    {
        return
            $@"{(Gender == "f" ? "Frau" : "Herr")}
{FirstName} {LastName}
Telefon: {(string.IsNullOrWhiteSpace(PhoneNumber) ? "-" : PhoneNumber)}
E-Mail:  {Email}

Nachricht:
----------------------------------------
{(string.IsNullOrWhiteSpace(Message) ? "(keine)" : Message)}


----------------------------------------
Browser: {(Request.Headers.ContainsKey(HeaderNames.UserAgent) ? Request.Headers[HeaderNames.UserAgent].ToString() : "unbekannt")}
IP-Adresse: {HttpContext.Connection.RemoteIpAddress}
";
    }
}