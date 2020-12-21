//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models
{
    [PostType(Title = "Standard post")]
    public class StandardPost  : Post<StandardPost>
    {
    }
}