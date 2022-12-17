// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ClubSite.Pages;

public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;
    private readonly ILogger _notFoundLogger;
    
    public ErrorModel(ILogger<ErrorModel> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _notFoundLogger = loggerFactory.CreateLogger(nameof(ClubSite) + ".NotFound");
    }

    [BindProperty] public string? OrigPath { get; set; }
    [BindProperty] public Exception? Exception { get; set; }
    [BindProperty] public string? SentStatusCode { get; set; }
    [BindProperty] public string? SentStatusText { get; set; }
    [BindProperty] public string? Description { get; set; }

    public void OnGet(string? id = null)
    {
        // status code is already set!
        if (Response.StatusCode.ToString() != id)
        {
            _logger.LogWarning("StatusCode: {StatusCode} != id: {id}, Path: {OrigPath}", Response.StatusCode, id, OrigPath);
        }

        if (string.IsNullOrEmpty(id) && Response.StatusCode != (int) System.Net.HttpStatusCode.OK)
            id = Response.StatusCode.ToString();

        // The StatusCodePagesMiddleware stores a request-feature with
        // the original path on the HttpContext, that can be accessed from the Features property.
        // Note: IExceptionHandlerFeature does not contain the path
        var exceptionFeature = HttpContext.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error != null)
        {
            OrigPath = exceptionFeature.Path;
            Exception = exceptionFeature.Error;
            _logger.LogCritical(Exception, "Path: {OrigPath}", OrigPath);
        }
        else
        {
            OrigPath = HttpContext.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>()?.OriginalPath;

            if (Response.StatusCode == 404)
                _notFoundLogger.LogInformation("{NotFound}", new {Status = Response.StatusCode, Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1", Path = OrigPath});
            else
                _logger.LogWarning("StatusCode: {StatusCode}, Path: {OrigPath}", Response.StatusCode, OrigPath);
        }

        SentStatusCode = Response.StatusCode.ToString();
        SentStatusText = Resources.StatusCodes.ResourceManager.GetString("E" + id) ?? "Fehler";
        Description = Resources.StatusDescriptions.ResourceManager.GetString("E" + id) ??
                      "Ein Fehler ist aufgetreten";
    }
}