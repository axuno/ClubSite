// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;
using ClubSite.Models;

namespace ClubSite.Pages;

public class SuperPageModel : SinglePage<SuperPage>
{
    private readonly IDb _db;

    public SuperPageModel(IApi api, IModelLoader loader, IDb db) : base(api, loader)
    {
        _db = db;
    }

    async public override Task<IActionResult> OnGet(Guid id, bool draft = false)
    {
        var result = await base.OnGet(id, draft);


        return result;
    }
}