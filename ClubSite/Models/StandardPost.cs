// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models;

[PostType(Title = "Standard post")]
public class StandardPost : Post<StandardPost>
{
}