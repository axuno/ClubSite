// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://https://github.com/axuno/ClubSite

using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models
{
    [PageType(Title = "Standard archive", IsArchive = true)]
    public class StandardArchive : Page<StandardArchive>
    {
    }
}