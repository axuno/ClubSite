// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite
//

// This file is a copy of the Piranha Manager login page.
// https://raw.githubusercontent.com/PiranhaCMS/piranha.core/refs/heads/master/core/Piranha.Manager.LocalAuth/Areas/Manager/Pages/Login.cshtml.cs
// but using a different name for the page model class to avoid conflicts with the Piranha Manager login page.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Piranha.Manager;
using Piranha.Manager.LocalAuth;

namespace ClubSite.Areas.Manager.Pages;

/// <summary>
/// View model for the login page.
/// The difference to the Piranha Manager login page is that this
/// model implements logging
/// </summary>
public class LoginModel : PageModel
{
    private readonly ISecurity _service;
    private readonly ManagerLocalizer _localizer;
    private readonly ILogger<LoginModel> _logger;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="service">The current security service</param>
    /// <param name="localizer">The manager localizer</param>
    /// <param name="logger">The login logger</param>
    public LoginModel(ISecurity service, ManagerLocalizer localizer, ILogger<LoginModel> logger)
    {
        _service = service;
        _localizer = localizer;
        _logger = logger;
        ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Gets/sets the model for binding form data.
    /// </summary>
    /// <value></value>
    [BindProperty]
    public required InputModel Input { get; set; }

    /// <summary>
    /// Gets/sets the optional return url after successful
    /// authorization.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets/sets the possible error message to be returned
    /// after failed authorization.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Model for form data.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets/sets the user name.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public string? Username { get; set; }

        /// <summary>
        /// Gets/sets the password.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Gets the login page.
    /// </summary>
    /// <param name="returnUrl">The optional return url</param>
    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl;
    }

    /// <summary>
    /// Handles authorization after a post.
    /// </summary>
    /// <param name="returnUrl">The optional return url</param>
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await _service.SignOut(HttpContext);

        if (!ModelState.IsValid || (await _service.SignIn(HttpContext, Input.Username, Input.Password)) !=
            LoginResult.Succeeded)
        {
            _logger.LogWarning("Login failed for user {@Username}.", Input.Username);
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, _localizer.General["Username and/or password are incorrect."].Value);
            return Page();
        }

        _logger.LogInformation("Login successful for user {@Username}.", Input.Username);

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect($"~/manager/login/auth?returnUrl={returnUrl}");
        }

        return LocalRedirect("~/manager/login/auth");
    }
}
