// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClubSite.Models;

public enum ThemeColor
{
    // Note: When using the member name as the Description,
    //       the Display attribute could be omitted. This is for localization.

    [Display(Description = "Standard")]
    Default,
    [Display(Description = nameof(Light))]
    Light,
    [Display(Description = nameof(Dark))]
    Dark,
    [Display(Description = nameof(Primary))]
    Primary,
    [Display(Description = nameof(Secondary))]
    Secondary,
    [Display(Description = nameof(Success))]
    Success,
    [Display(Description = nameof(Danger))]
    Danger,
    [Display(Description = nameof(Warning))]
    Warning,
    [Display(Description = nameof(Info))]
    Info
}