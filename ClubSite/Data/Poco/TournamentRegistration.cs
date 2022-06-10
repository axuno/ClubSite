// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClubSite.Resources;

#nullable enable

namespace ClubSite.Data.Poco;

public class TournamentRegistration
{
    [Key] public long Id { get; set; }
    [Column(TypeName = "identifier")] public Guid RegistrationId { get; set; }
    [Column(TypeName = "datetime")] public DateTime TournamentDate { get; set; }
    public int? Rank { get; set; }

    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    [Display(Name = "Teamname")]
    public string? TeamName { get; set; }

    [Display(Name = "Vereinsname")]
    public string? ClubName { get; set; }

    [RegularExpression("[mfu]", ErrorMessage = "Ungültiger Schlüssel für 'Anrede'")]
    [Display(Name = "Anrede")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    public string? Gender { get; set; }

    [Display(Name = "Vorname")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    public string? FirstName { get; set; }

    [Display(Name = "Familienname")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }

    [Display(Name = "Telefon")]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [DataType(DataType.Text)]
    public string? Fone { get; set; }

    [Display(Name = "E-Mail")]
    [EmailAddress(ErrorMessageResourceName = nameof(DataAnnotationResource.EmailAddressInvalid),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    [Required(AllowEmptyStrings = false,
        ErrorMessageResourceName = nameof(DataAnnotationResource.PropertyValueRequired),
        ErrorMessageResourceType = typeof(DataAnnotationResource))]
    public string? Email { get; set; }

    [Display(Name = "Nachricht/Hinweis")]
    public string? Message { get; set; }

    [Display(Name = "Kennzeichen \"Nachrücker\"")]
    public bool IsStandByRegistration { get; set; }
        
    [Display(Name = "Datum Absage durch Team")]
    [Column(TypeName = "datetime")] public DateTime? RegCanceledOn { get; set; }
        
    [Column(TypeName = "datetime")] public DateTime? RegisteredOn { get; set; }
    [Column(TypeName = "datetime")] public DateTime? CreatedOn { get; set; }
    [Column(TypeName = "datetime")] public DateTime? ModifiedOn { get; set; }

    public string GetTeamNameWithClub()
    {
        return string.Join(' ', (TeamName ?? string.Empty).Trim(),
            !string.IsNullOrWhiteSpace(ClubName) ? "(" + ClubName.Trim() + ")" : string.Empty);
    }

    public string GetCompleteName()
    {
        return string.Join(' ', (FirstName ?? string.Empty).Trim(), (LastName ?? string.Empty).Trim());
    }
}