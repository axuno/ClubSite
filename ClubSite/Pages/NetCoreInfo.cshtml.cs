using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClubSite.Pages;

public class NetCoreInfoModel : PageModel
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IAuthorizationService _authorization;
        
    public NetCoreInfoModel(IWebHostEnvironment hostingEnvironment, IAuthorizationService authorization)
    {
        _hostingEnvironment = hostingEnvironment;
        _authorization = authorization;
    }
        
    public async Task<IActionResult> OnGetAsync()
    {
        if (!(await _authorization.AuthorizeAsync(User, Piranha.Manager.Permission.Pages)).Succeeded)
        {
            return Forbid();
        }
            
        var p = new Process {StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                Arguments = "--info",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };

        p.Start();
            
        var sdkFolder = @"c:\Program Files\dotnet\sdk\";
        var stdout = $"Web root path: {_hostingEnvironment.WebRootPath}\n\n";
        stdout += $"{await p.StandardOutput.ReadToEndAsync()}\nFolder: {sdkFolder}\n";
        await p.WaitForExitAsync();

        var f = Directory.GetDirectories(sdkFolder);
        f.ToList().ForEach(e => stdout += e + '\n');

        return Content(stdout);
    }
}