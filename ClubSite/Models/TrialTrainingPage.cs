// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models;

/// <summary>
/// Custom page type for the trial training ("Probetraining") registration form.
/// </summary>
[PageType(Title = "Trial Training", UsePrimaryImage = false, UseExcerpt = false, UseBlocks = false)]
[ContentTypeRoute(Title = "Default", Route = "/TrialTrainingPage")]
public class TrialTrainingPage : Page<TrialTrainingPage>
{
}