//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClubSite.Controllers
{
    /// <summary>
    /// Captcha Controller
    /// </summary>
    [Route("captcha")]
    public class Captcha : Controller
	{
		public async Task<IActionResult> Index()
        {
            return await GetSvgContent();
        }

        private Task<ContentResult> GetSvgContent()
        {
            using var ci = new ClubSite.Library.CaptchaSvgGenerator(null, 151, 51, Color.FromArgb(0x30, 0x40, 0x5E),
                Color.FromArgb(0xFF,0xFF,0xFF), Color.FromArgb(0x30, 0x40, 0x5E));

            var result = ci.SetTextWithMathCalc(5).ToString(); // GenerateRandomString(5)
            HttpContext.Session.SetString(CaptchaSessionKeyName, result);

            // Change the response headers to output an un-cached image.
            HttpContext.Response.Clear();
            HttpContext.Response.Headers.Add("Expires", DateTime.UtcNow.Date.AddDays(-1).ToString("R"));
            HttpContext.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache");

            HttpContext.Response.ContentType = "image/svg+xml";
            return Task.FromResult(Content(ci.Image));
        }

	    private string CaptchaSessionKeyName => ClubSite.Library.CaptchaSvgGenerator.CaptchaSessionKeyName;
	}
}
