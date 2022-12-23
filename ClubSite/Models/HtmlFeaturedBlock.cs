// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace ClubSite.Models;

[BlockType(Name = "Content-Feat.", Category = "Content", Icon = "fas fa-paragraph", Width = EditorWidth.Centered)]
public class HtmlFeaturedBlock : HtmlBlock
{
    [Field(Title = "Background Color", Description = "Select website theme color")]
    public SelectField<ThemeColor> BackgroundColor { get; set; } = new(); // titles for select items taken from enum Display attribute
}
