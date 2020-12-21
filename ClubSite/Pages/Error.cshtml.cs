//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClubSite.Pages
{
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string OrigPath { get; set; }
        [BindProperty]
        public Exception Exception { get; set; }
        [BindProperty]
        public string SentStatusCode { get; set; }
        [BindProperty]
        public string SentStatusText { get; set; }
        [BindProperty]
        public string Description { get; set; }

        public void OnGet(string id = null)
        {
            id ??= string.Empty;
            id = id.Trim();

            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("de");
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("de");
            
            // The StatusCodePagesMiddleware stores a request-feature with
            // the original path on the HttpContext, that can be accessed from the Features property.
            // Note: IExceptionHandlerFeature does not contain the path
            var exceptionFeature = HttpContext.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (exceptionFeature?.Error != null)
            {
                OrigPath = exceptionFeature.Path;
                Exception = exceptionFeature.Error;
                _logger.LogCritical(Exception, "Path: {0}", OrigPath);
            }
            else
            {
                OrigPath = HttpContext.Features
                    .Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>()?.OriginalPath;
                _logger.LogInformation("Path: {0}, StatusCode: {1}", OrigPath, id);
            }

            SentStatusCode = id;
            SentStatusText = Resources.StatusCodes.ResourceManager.GetString("E"+id) ?? "Fehler";
            Description = Resources.StatusDescriptions.ResourceManager.GetString("E"+id) ?? "Ein Fehler ist aufgetreten";
        }
    }
}
