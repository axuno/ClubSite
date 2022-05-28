// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace ClubSite.Models;

public class TournamentDefinition
{
    [Field(Title = "Primäres Bild", Placeholder = "Bild einsetzen")]
    public ImageField TopImage { get; set; } = new();
        
    [Field(Title = "Turniername", Placeholder = "z.B. 28. Neusäßer Mixedturnier")]
    public StringField Name { get; set; } = new();

    [Field(Title = "Datum von", Placeholder = "Datum")]
    public DateField DateFrom { get; set; } = new();

    [Field(Title = "Datum bis", Placeholder = "Datum")]
    public DateField DateTo { get; set; } = new();

    [Field(Title = "Uhrzeit von/bis", Placeholder = "Uhrzeit")]
    public StringField TimeFromTo { get; set; } = new();

    [Field(Title = "Beginn der Anmeldefrist", Placeholder = "Datum")]
    public DateField RegistrationStart { get; set; } = new();

    [Field(Title = "Anmeldeschluss", Placeholder = "Datum")]
    public DateField RegistrationDeadline { get; set; } = new();

    [Field(Title = "Anzahl Teams", Placeholder = "Anzahl")]
    public NumberField NumberOfTeams { get; set; } = new();

    [Field(Title = "Team-Zusammensetzung", Placeholder = "z.B. Anzahl Damen/Herren")]
    public StringField TeamComposition { get; set; } = new();

    [Field(Title = "Startgeld", Placeholder = "Startgeld")]
    public StringField EntryFee { get; set; } = new();

    [Field(Title = "Bankverbindung")]
    public TextField BankDetails { get; set; } = new();

    [Field(Title = "Adresse", Placeholder = "Hallenadresse")]
    public StringField Address { get; set; } = new();

    [Field(Title = "Infos", Placeholder = "Ablauf, Regeln, Infos")]
    public HtmlField Infos { get; set; } = new();

    public bool IsOver(DateTime dateToTest)
    {
        return DateFrom.Value == null 
               || DateTo.Value == null 
               || (DateFrom.Value.HasValue &&
                   DateTo.Value.HasValue &&
                   DateOnly.FromDateTime(DateTo.Value.Value) <=
                   DateOnly.FromDateTime(dateToTest));
    }

    public bool IsRegistrationAllowed(DateTime dateToTest)
    {
        return DateFrom.Value != null 
               && DateTo.Value != null
               && RegistrationStart.Value.HasValue
               && RegistrationDeadline.Value.HasValue
               && dateToTest.Date >= RegistrationStart.Value.Value.Date
               && dateToTest.Date <= RegistrationDeadline.Value.Value.Date;
    }

    public bool IsRegistrationPeriodSet()
    {
        return RegistrationStart.Value.HasValue && RegistrationDeadline.Value.HasValue;
    }
}