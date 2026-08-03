// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite
//

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha;
using Piranha.Manager;

namespace ClubSite.Areas.Manager.Pages;

/// <summary>
/// Manager page for configuring the floating shortcut tab.
/// Params are stored as Piranha Params and read at render time
/// by <c>Pages/_FloatingRegisterTab.cshtml</c>.
/// </summary>
[Authorize(Policy = Permission.Admin)]
public class ShortcutTabModel : PageModel
{
    private readonly IApi _api;
    private readonly IAuthorizationService _auth;

    public ShortcutTabModel(IApi api, IAuthorizationService auth)
    {
        _api = api;
        _auth = auth;

        Enabled = false;
        Link = string.Empty;
        Text = string.Empty;
        Color = "#dc3545";
        HideMobile = false;
    }

    [BindProperty]
    [Display(Name = "Shortcut tab enabled")]
    public bool Enabled { get; set; }

    [BindProperty]
    [Display(Name = "Link target")]
    public string Link { get; set; }

    [BindProperty]
    [Display(Name = "Button text")]
    public string Text { get; set; }

    [BindProperty]
    [Display(Name = "Background color")]
    public string Color { get; set; }

    [BindProperty]
    [Display(Name = "Hide on mobile")]
    public bool HideMobile { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (!(await _auth.AuthorizeAsync(User, Permission.Admin)).Succeeded)
            return Forbid();

        var enabledParam = await _api.Params.GetByKeyAsync("shortcut-tab-enabled");
        var linkParam = await _api.Params.GetByKeyAsync("shortcut-tab-link");
        var textParam = await _api.Params.GetByKeyAsync("shortcut-tab-text");
        var colorParam = await _api.Params.GetByKeyAsync("shortcut-tab-color");
        var hideMobileParam = await _api.Params.GetByKeyAsync("shortcut-tab-hide-mobile");

        Enabled = enabledParam?.Value?.Trim() == "true";
        Link = linkParam?.Value ?? "";
        Text = textParam?.Value ?? "";
        Color = colorParam?.Value ?? "#dc3545";
        HideMobile = hideMobileParam?.Value?.Trim() == "true";

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!(await _auth.AuthorizeAsync(User, Permission.Admin)).Succeeded)
            return Forbid();

        if (!ModelState.IsValid)
            return Page();

        await SaveOrDeleteParam("shortcut-tab-enabled", Enabled ? "true" : "false");
        await SaveOrDeleteParam("shortcut-tab-link", Link);
        await SaveOrDeleteParam("shortcut-tab-text", Text);
        await SaveOrDeleteParam("shortcut-tab-color", Color);
        await SaveOrDeleteParam("shortcut-tab-hide-mobile", HideMobile ? "true" : "false");

        ViewData["Message"] = "The shortcut tab settings have been saved.";
        ViewData["MessageCss"] = "success";

        return Page();
    }

    private async Task SaveOrDeleteParam(string key, string value)
    {
        var existing = await _api.Params.GetByKeyAsync(key);

        if (string.IsNullOrEmpty(value))
        {
            if (existing != null)
            {
                await _api.Params.DeleteAsync(existing);
            }
            return;
        }

        if (existing != null)
        {
            existing.Value = value;
            await _api.Params.SaveAsync(existing);
        }
        else
        {
            await _api.Params.SaveAsync(new Piranha.Data.Param
            {
                Key = key,
                Value = value
            });
        }
    }
}