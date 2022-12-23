// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClubSite.Models;

public enum ThemeColor
{
    [Display(Description = "Standard")]
    Default,
    [Display(Description = "Hell")]
    Light,
    [Display(Description = "Dunkel")]
    Dark,
    [Display(Description = "Primär")]
    Primary,
    [Display(Description = "Sekundär")]
    Secondary,
    [Display(Description = "Erfolg")]
    Success,
    [Display(Description = "Gefahr")]
    Danger,
    [Display(Description = "Warnung")]
    Warning,
    [Display(Description = "Info")]
    Info
}