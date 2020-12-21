//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ClubSite.Library;
using ClubSite.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ClubSite.Pages
{
    [BindProperties]
    public class ContactModel : PageModel
    {
        private readonly Services.IMailService _mailService;
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(Services.IMailService mailService, ILogger<ContactModel> logger)
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
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [DataType(DataType.Text)]
        [Display(Name = "Anrede")]
        [RegularExpression("[mfu]", ErrorMessage = "Ungültiger Schlüssel für 'Anrede'")]
        public string Gender { get; set; }

        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [DataType(DataType.Text)]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [DataType(DataType.Text)]
        [Display(Name = "Familienname")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        [Display(Name = "Betreff")]
        [DataType(DataType.Text)]
        public string Subject { get; set; }

        [Display(Name = "Nachricht")] public string Message { get; set; }

        [Display(Name = "Ergebnis der Rechenaufgabe im Bild")]
        [Required(AllowEmptyStrings = false,
            ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
            ErrorMessageResourceType = typeof(DataAnnotationResource))]
        public string Captcha { get; set; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellation)
        {
            if (Captcha != HttpContext.Session.GetString(CaptchaSvgGenerator.CaptchaSessionKeyName))
            {
                ModelState.AddModelError(nameof(Captcha), "Ergebnis der Rechenaufgabe ist nicht korrekt");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                HttpContext.Session.Remove(CaptchaSvgGenerator.CaptchaSessionKeyName);
                await _mailService.SendContactFormEmailAsync($"{FirstName.Trim()} {LastName.Trim()}", Email, Subject, GetFormMailMessage(), cancellation);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Sending mail to '{0}' failed.", Email);
            }
            
            return RedirectToPage("ContactConfirmation");
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
}
