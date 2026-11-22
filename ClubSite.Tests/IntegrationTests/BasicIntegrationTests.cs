// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MimeKit;
using NUnit.Framework;

namespace ClubSite.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
public class PlaywrightBasicIntegrationTests : PageTest
{
    private const string BaseUrl = "http://127.0.0.1:50789";
    private readonly HttpClient _client = new();
    private Process _appProcess;
    private IReadOnlyList<string> _sitemapPaths = [];
    private readonly string _mailDirectory = Path.Combine(Path.GetTempPath(), "ClubSite.Playwright.Tests", Guid.NewGuid().ToString("N"));

    [OneTimeSetUp]
    public async Task StartApplicationAndLoadSitemap()
    {
        var solutionDirectory = FindSolutionDirectory();
        var projectPath = Path.Combine(solutionDirectory, "ClubSite", "ClubSite.csproj");
        Directory.CreateDirectory(_mailDirectory);

        foreach (var file in Directory.GetFiles(_mailDirectory, "*.eml"))
            File.Delete(file);

        _appProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-build --urls {BaseUrl}",
            WorkingDirectory = solutionDirectory,
            UseShellExecute = false,
            Environment =
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["MailSettings__Sender__File__Path"] = _mailDirectory
            }
        }) ?? throw new InvalidOperationException("Could not start the ClubSite application.");

        await WaitUntilReadyAsync();

        var response = await _client.GetAsync(GetAbsoluteUrl("/sitemap.xml"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var xml = XDocument.Parse(content);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var locElements = xml.Descendants(ns + "loc").ToList();
        if (locElements.Count == 0)
        {
            locElements = xml.Descendants("loc").ToList();
        }

        _sitemapPaths = locElements
            .Select(loc => Uri.TryCreate(loc.Value.Trim(), UriKind.Absolute, out var uri)
                ? uri.PathAndQuery
                : string.Empty)
            .Select(path => string.IsNullOrEmpty(path) ? "/" : path)
            .Where(path => !string.IsNullOrEmpty(path))
            .Distinct()
            .ToList();
    }

    [OneTimeTearDown]
    public void StopApplication()
    {
        _client.Dispose();

        if (Directory.Exists(_mailDirectory))
        {
            Directory.Delete(_mailDirectory, recursive: true);
        }

        if (_appProcess is not null && !_appProcess.HasExited)
        {
            _appProcess.Kill(entireProcessTree: true);
        }

        _appProcess?.Dispose();
    }

    [Test]
    public async Task GetSitemapEndpoints_ReturnSuccessAndCorrectContentType()
    {
        foreach (var path in _sitemapPaths)
        {
            var response = await Page.GotoAsync(GetAbsoluteUrl(path).ToString(),
                new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

            response.Should().NotBeNull($"{path} should return a page response");
            response!.Status.Should().Be((int)HttpStatusCode.OK, $"{path} should return HTTP 200");
            response.Headers["content-type"].Should().StartWith("text/html", $"{path} should return HTML");
        }
    }

    [Test]
    public async Task SubmitContactForm_WithValidData()
    {
        ClearMailDirectory();

        var contactPath = _sitemapPaths.FirstOrDefault(path =>
            path.Contains("kontakt", StringComparison.OrdinalIgnoreCase)) ?? "/kontakt";

        var response = await Page.GotoAsync(GetAbsoluteUrl(contactPath).ToString(),
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        response.Should().NotBeNull();
        response!.Status.Should().Be((int)HttpStatusCode.OK);

        var captchaImage = Page.Locator("#CaptchaImage");
        await captchaImage.WaitForAsync();

        var captchaSource = await captchaImage.GetAttributeAsync("src");
        captchaSource.Should().NotBeNullOrEmpty();

        var captchaUrl = new Uri(new Uri(BaseUrl), captchaSource!).ToString();
        var captchaSvg = await Page.EvaluateAsync<string>(
            "async src => { const response = await fetch(src); if (!response.ok) throw new Error(response.status); return await response.text(); }",
            captchaUrl);
        var captchaDocument = XDocument.Parse(captchaSvg);
        var captchaText = captchaDocument.Descendants()
            .First(element => element.Name.LocalName == "text")
            .Value;

        var captchaResult = CalculateCaptchaResult(captchaText);

        await Page.Locator("#Email").FillAsync("playwright@example.com");
        await Page.Locator("#Gender").SelectOptionAsync("m");
        await Page.Locator("#FirstName").FillAsync("Playwright");
        await Page.Locator("#LastName").FillAsync("Test");
        await Page.Locator("#PhoneNumber").FillAsync("0123456789");
        await Page.Locator("#Subject").FillAsync("Playwright Contact Form Test");
        await Page.Locator("#Message").FillAsync("Automated test of the contact form.");
        await Page.Locator("#Captcha").FillAsync(captchaResult.ToString());

        var navigationTask = Page.WaitForURLAsync("**/kontakt:nachricht-erhalten",
            new PageWaitForURLOptions { Timeout = 30_000 });

        await Page.Locator("button[type='submit']").ClickAsync();
        await navigationTask;

        Page.Url.Should().Contain("/kontakt:nachricht-erhalten");

        var mailFile = await WaitForMailAsync(TimeSpan.FromSeconds(10));
        var mail = await MimeMessage.LoadAsync(mailFile);

        mail.Subject.Should().Be("Playwright Contact Form Test");
        mail.From.Mailboxes.Should().ContainSingle(mailbox =>
            mailbox.Address == "noreply@volleyballclub.de");
        mail.ReplyTo.Mailboxes.Should().ContainSingle(mailbox =>
            mailbox.Address == "playwright@example.com");
        mail.TextBody.Should().Contain("Automated test of the contact form.");
        mail.TextBody.Should().Contain("Playwright Test");
        mail.TextBody.Should().Contain("E-Mail:  playwright@example.com");
    }

    [Test]
    public async Task SubmitTrialTrainingForm_WithValidData()
    {
        ClearMailDirectory();

        var trialTrainingPath = _sitemapPaths.FirstOrDefault(path =>
            path.Equals("/training/ausprobieren", StringComparison.OrdinalIgnoreCase));

        trialTrainingPath.Should().Be("/training/ausprobieren",
            "the sitemap should contain the trial-training form page. Available paths: {0}",
            string.Join(", ", _sitemapPaths));

        var trialTrainingUrl = GetAbsoluteUrl(trialTrainingPath!).ToString();
        var response = await Page.GotoAsync(trialTrainingUrl,
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        response.Should().NotBeNull();
        response!.Status.Should().Be((int)HttpStatusCode.OK,
            "trial-training page should exist at {0}", trialTrainingUrl);

        var captchaImage = Page.Locator("#CaptchaImage");
        await captchaImage.WaitForAsync();

        var captchaSource = await captchaImage.GetAttributeAsync("src");
        captchaSource.Should().NotBeNullOrEmpty();

        var captchaUrl = new Uri(new Uri(BaseUrl), captchaSource!).ToString();
        var captchaSvg = await Page.EvaluateAsync<string>(
            "async src => { const response = await fetch(src); if (!response.ok) throw new Error(response.status); return await response.text(); }",
            captchaUrl);
        var captchaDocument = XDocument.Parse(captchaSvg);
        var captchaText = captchaDocument.Descendants()
            .First(element => element.Name.LocalName == "text")
            .Value;

        var captchaResult = CalculateCaptchaResult(captchaText);

        await Page.Locator("#Gender").SelectOptionAsync("m");
        await Page.Locator("#FirstName").FillAsync("Playwright");
        await Page.Locator("#LastName").FillAsync("Trial");
        await Page.Locator("#Email").FillAsync("playwright-trial@example.com");
        await Page.Locator("#PhoneNumber").FillAsync("0123456789");
        await Page.Locator("#Age").FillAsync("25");
        await Page.Locator("#Experience").SelectOptionAsync("hobby");
        await Page.Locator("#TeamPreference").SelectOptionAsync("mixed");
        await Page.Locator("#Goal").SelectOptionAsync("active");
        await Page.Locator("#Comment").FillAsync("Automated Playwright trial-training request.");
        await Page.Locator("#Captcha").FillAsync(captchaResult.ToString());

        var navigationTask = Page.WaitForURLAsync("**/probetraining:anfrage-erhalten",
            new PageWaitForURLOptions { Timeout = 30_000 });

        await Page.Locator("button[type='submit']").ClickAsync();
        await navigationTask;

        Page.Url.Should().Contain("/probetraining:anfrage-erhalten");

        var mailFile = await WaitForMailAsync(TimeSpan.FromSeconds(10));
        var mail = await MimeMessage.LoadAsync(mailFile);

        mail.Subject.Should().Be("Anfrage: Probetraining");
        mail.From.Mailboxes.Should().ContainSingle(mailbox =>
            mailbox.Address == "noreply@volleyballclub.de");
        mail.ReplyTo.Mailboxes.Should().ContainSingle(mailbox =>
            mailbox.Address == "playwright-trial@example.com");
        mail.TextBody.Should().Contain("Automated Playwright trial-training request.");
        mail.TextBody.Should().Contain("Alter: 25");
        mail.TextBody.Should().Contain("E-Mail:  playwright-trial@example.com");
    }

    #region *** Helper Methods

    private async Task WaitUntilReadyAsync()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        while (!timeout.IsCancellationRequested)
        {
            if (_appProcess?.HasExited == true)
            {
                throw new InvalidOperationException(
                    $"The ClubSite application stopped before startup completed (exit code {_appProcess.ExitCode}).");
            }

            try
            {
                using var response = await _client.GetAsync(GetAbsoluteUrl("/sitemap.xml"), timeout.Token);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException) when (!timeout.IsCancellationRequested)
            {
            }

            await Task.Delay(500, timeout.Token);
        }

        throw new TimeoutException("The ClubSite application did not become ready within 60 seconds.");
    }

    private static Uri GetAbsoluteUrl(string url) => new(new Uri(BaseUrl), url);

    private static string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory is not null)
        {
            if (directory.GetFiles("ClubSite.slnx").Length > 0)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find the ClubSite.slnx solution directory.");
    }

    private void ClearMailDirectory()
    {
        if (!Directory.Exists(_mailDirectory))
            Directory.CreateDirectory(_mailDirectory);

        foreach (var file in Directory.GetFiles(_mailDirectory, "*.eml"))
            File.Delete(file);
    }

    private async Task<string> WaitForMailAsync(TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < deadline)
        {
            var mailFile = Directory.Exists(_mailDirectory)
                ? Directory.GetFiles(_mailDirectory, "*.eml")
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault()
                : null;

            if (mailFile is not null)
                return mailFile;

            await Task.Delay(100);
        }

        throw new TimeoutException(
            $"The contact-form mail was not created in {_mailDirectory} within {timeout.TotalSeconds:0} seconds.");
    }

    private static int CalculateCaptchaResult(string expression)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            expression.Trim(),
            @"^(\d+)\s*([+*\-/–])\s*(\d+)$");

        match.Success.Should().BeTrue($"captcha expression '{expression}' should be supported");

        var firstNumber = int.Parse(match.Groups[1].Value);
        var operatorCharacter = match.Groups[2].Value;
        var secondNumber = int.Parse(match.Groups[3].Value);

        return operatorCharacter switch
        {
            "+" => firstNumber + secondNumber,
            "-" or "–" => firstNumber - secondNumber,
            "*" => firstNumber * secondNumber,
            "/" => firstNumber / secondNumber,
            _ => throw new InvalidOperationException($"Unsupported captcha operator: {operatorCharacter}")
        };
    }

    #endregion
}

