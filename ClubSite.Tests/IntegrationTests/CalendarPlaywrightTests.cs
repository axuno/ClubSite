// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ClubSite.Models;
using ClubSite.Pages;
using ClubSite.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Moq;
using NodaTime;
using NUnit.Framework;
using Piranha;
using Piranha.AspNetCore.Helpers;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Services;
using PiranhaMedia = Piranha.Models.Media;
using NodaClock = NodaTime.IClock;

#nullable enable

namespace ClubSite.Tests.IntegrationTests;

/// <summary>
/// Playwright integration tests for <see cref="CalendarPage"/> covering filtering, display mode enum ("Alle Termine"),
/// external link attributes (<c>rel="noopener noreferrer"</c>), internal events without links, and live Piranha media updates.
/// </summary>
[TestFixture]
[Category("Integration")]
public class CalendarPlaywrightTests : PageTest
{
    private WebApplication? _app;
    private string _baseUrl = string.Empty;
    private Guid _pageId;
    private Guid _bvvMediaId;
    private Guid _clubMediaId;
    private TestCalendarStorage _storage = null!;
    private PiranhaMedia _bvvMedia = null!;
    private PiranhaMedia _clubMedia = null!;

    private sealed class FixedClock(Instant instant) : NodaClock
    {
        public Instant GetCurrentInstant() => instant;
    }

    private sealed class TestCalendarStorage : IStorage
    {
        private readonly Dictionary<Guid, byte[]> _files = new();

        public void SetFile(Guid id, string content) => _files[id] = Encoding.UTF8.GetBytes(content);

        public Task<IStorageSession> OpenAsync() => Task.FromResult<IStorageSession>(new Session(this));

        public string GetPublicUrl(PiranhaMedia media, string filename) => $"/uploads/{media.Id}/{filename}";

        public string GetResourceName(PiranhaMedia media, string filename) => $"{media.Id}/{filename}";

        private sealed class Session(TestCalendarStorage owner) : IStorageSession
        {
            public Task<bool> GetAsync(PiranhaMedia media, string filename, Stream stream)
            {
                if (!owner._files.TryGetValue(media.Id, out var bytes))
                {
                    return Task.FromResult(false);
                }

                stream.Write(bytes, 0, bytes.Length);
                return Task.FromResult(true);
            }

            public Task<string> PutAsync(PiranhaMedia media, string filename, string contentType, Stream stream) =>
                throw new NotSupportedException();

            public Task<string> PutAsync(PiranhaMedia media, string filename, string contentType, byte[] bytes) =>
                throw new NotSupportedException();

            public Task<bool> DeleteAsync(PiranhaMedia media, string filename) =>
                throw new NotSupportedException();

            public void Dispose()
            {
            }
        }
    }

    [OneTimeSetUp]
    public async Task StartTestHost()
    {
        _pageId = Guid.NewGuid();
        _bvvMediaId = Guid.NewGuid();
        _clubMediaId = Guid.NewGuid();
        _storage = new TestCalendarStorage();

        var bvvIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:bvv-past
            DTSTART;TZID=Europe/Berlin:20260505T140000
            DTEND;TZID=Europe/Berlin:20260505T160000
            SUMMARY:Damen 1 Auswärtsspiel
            LOCATION:Sporthalle Augsburg
            CATEGORIES:Damen 1,Spieltag
            URL:https://volleyball.bayern/spiel/101
            END:VEVENT
            BEGIN:VEVENT
            UID:bvv-future
            DTSTART;TZID=Europe/Berlin:20260620T150000
            DTEND;TZID=Europe/Berlin:20260620T173000
            SUMMARY:Damen 1 Heimspieltag
            LOCATION:Halle Neusäß
            CATEGORIES:Damen 1,Heimspieltag
            URL:https://volleyball.bayern/spiel/202
            END:VEVENT
            END:VCALENDAR
            """;

        var clubIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:club-internal
            DTSTART;TZID=Europe/Berlin:20260625T183000
            DTEND;TZID=Europe/Berlin:20260625T203000
            SUMMARY:Mitgliederversammlung
            LOCATION:Vereinsheim
            CATEGORIES:Verein,Vereinstermin
            END:VEVENT
            END:VCALENDAR
            """;

        _bvvMedia = new PiranhaMedia
        {
            Id = _bvvMediaId,
            Filename = "bvv.ics",
            Type = MediaType.Document,
            Size = Encoding.UTF8.GetByteCount(bvvIcs),
            LastModified = new DateTime(2026, 6, 1, 10, 0, 0)
        };
        _clubMedia = new PiranhaMedia
        {
            Id = _clubMediaId,
            Filename = "verein.ics",
            Type = MediaType.Document,
            Size = Encoding.UTF8.GetByteCount(clubIcs),
            LastModified = new DateTime(2026, 6, 2, 12, 30, 0)
        };
        _storage.SetFile(_bvvMediaId, bvvIcs);
        _storage.SetFile(_clubMediaId, clubIcs);

        var port = GetFreePort();
        _baseUrl = $"http://127.0.0.1:{port}";

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = Path.Combine(FindSolutionDirectory(), "ClubSite"),
            WebRootPath = "wwwroot",
            EnvironmentName = "Development"
        });
        builder.WebHost.UseUrls(_baseUrl);
        builder.Logging.ClearProviders();

        var pageData = new CalendarPage
        {
            Id = _pageId,
            Title = "Termine",
            CalendarFiles =
            [
                new CalendarDocumentRegion
                {
                    Title = new StringField { Value = "BVV" },
                    Document = new DocumentField { Id = _bvvMediaId, Media = _bvvMedia }
                },
                new CalendarDocumentRegion
                {
                    Title = new StringField { Value = "Verein" },
                    Document = new DocumentField { Id = _clubMediaId, Media = _clubMedia }
                }
            ]
        };

        var mediaService = new Mock<IMediaService>();
        mediaService.Setup(m => m.GetByIdAsync(_bvvMediaId)).ReturnsAsync(() => _bvvMedia);
        mediaService.Setup(m => m.GetByIdAsync(_clubMediaId)).ReturnsAsync(() => _clubMedia);

        var api = new Mock<IApi>();
        api.SetupGet(a => a.Media).Returns(mediaService.Object);

        var loader = new Mock<IModelLoader>();
        loader.Setup(l => l.GetPageAsync<CalendarPage>(_pageId, It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<bool>()))
            .ReturnsAsync(pageData);

        var siteHelper = new Mock<ISiteHelper>();
        siteHelper.SetupGet(s => s.Sitemap).Returns(new Sitemap());

        var webApp = new Mock<IApplicationService>();
        webApp.SetupGet(w => w.Site).Returns(siteHelper.Object);
        webApp.SetupGet(w => w.PageId).Returns(_pageId);

        builder.Services.AddMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IStorage>(_storage);
        builder.Services.AddSingleton<NodaClock>(new FixedClock(Instant.FromUtc(2026, 6, 10, 10, 0)));
        builder.Services.AddScoped<ICalendarService, CalendarService>();
        builder.Services.AddScoped(_ => api.Object);
        builder.Services.AddScoped(_ => loader.Object);
        builder.Services.AddScoped(_ => webApp.Object);
        builder.Services.AddRazorPages()
            .AddApplicationPart(typeof(CalendarPageModel).Assembly);

        _app = builder.Build();
        // Mirror Piranha's page routing middleware: rewrite /termine to /CalendarPage while preserving filter query parameters.
        _app.Use(async (context, next) =>
        {
            if (context.Request.Path.Equals("/termine", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Path = "/CalendarPage";
                var existing = context.Request.QueryString.HasValue
                    ? context.Request.QueryString.Value!.TrimStart('?')
                    : string.Empty;
                var idParam = $"id={_pageId}";
                context.Request.QueryString = new QueryString(
                    string.IsNullOrEmpty(existing) ? $"?{idParam}" : $"?{idParam}&{existing}");
            }

            await next();
        });
        _app.UseRouting();
        _app.MapRazorPages();

        await _app.StartAsync();
    }

    [OneTimeTearDown]
    public async Task StopTestHost()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    [Test]
    public async Task CalendarPage_RendersUpcomingFiltersModesLinksAndUpdatesLive()
    {
        var url = $"{_baseUrl}/termine";
        var response = await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        response.Should().NotBeNull();
        response!.Status.Should().Be((int)HttpStatusCode.OK);

        // Heading & footer show real title and last-updated timestamp instead of hardcoded Version: 2.0
        var pageText = await Page.TextContentAsync("body");
        pageText.Should().Contain("Termine");
        pageText.Should().Contain("Letzte Aktualisierung: 02.06.2026 12:30 Uhr");
        pageText.Should().NotContain("Version: 2.0");

        // Upcoming mode shows future BVV game (with safe external link) and internal event (without link)
        var bvvLink = Page.Locator("a", new PageLocatorOptions { HasTextString = "Damen 1 Heimspieltag" });
        await bvvLink.WaitForAsync();
        (await bvvLink.GetAttributeAsync("rel")).Should().Be("noopener noreferrer");
        (await bvvLink.GetAttributeAsync("target")).Should().Be("_blank");

        (await Page.Locator("td", new PageLocatorOptions { HasTextString = "Mitgliederversammlung" }).CountAsync()).Should().BeGreaterThan(0);
        (await Page.Locator("a", new PageLocatorOptions { HasTextString = "Mitgliederversammlung" }).CountAsync()).Should().Be(0);
        (await Page.Locator("td", new PageLocatorOptions { HasTextString = "Damen 1 Auswärtsspiel" }).CountAsync()).Should().Be(0);

        // Select "Alle Termine" (enum Mode=All) -> includes both past and upcoming events
        await Page.Locator("#Mode").SelectOptionAsync(nameof(CalendarDisplayMode.All));
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        var allRowsText = await Page.Locator("tbody").TextContentAsync();
        allRowsText.Should().Contain("Damen 1 Auswärtsspiel");
        allRowsText.Should().Contain("Damen 1 Heimspieltag");
        allRowsText.Should().Contain("Mitgliederversammlung");

        // Filter by Team = Verein -> only internal club event remains
        await Page.Locator("#TeamFilter").SelectOptionAsync("Verein");
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        var filteredRowsText = await Page.Locator("tbody").TextContentAsync();
        filteredRowsText.Should().Contain("Mitgliederversammlung");
        filteredRowsText.Should().NotContain("Damen 1 Heimspieltag");

        // Simulate Piranha Manager replacing club.ics -> reload reflects updated title immediately without restart
        var updatedClubIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:club-internal
            DTSTART;TZID=Europe/Berlin:20260625T183000
            DTEND;TZID=Europe/Berlin:20260625T203000
            SUMMARY:Sommerfest 2026
            LOCATION:Vereinsheim
            CATEGORIES:Verein,Vereinstermin
            END:VEVENT
            END:VCALENDAR
            """;
        _storage.SetFile(_clubMediaId, updatedClubIcs);
        _clubMedia = new PiranhaMedia
        {
            Id = _clubMediaId,
            Filename = "verein.ics",
            Type = MediaType.Document,
            Size = Encoding.UTF8.GetByteCount(updatedClubIcs),
            LastModified = new DateTime(2026, 6, 4, 9, 15, 0)
        };

        await Page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        var updatedText = await Page.TextContentAsync("body");
        updatedText.Should().Contain("Sommerfest 2026");
        updatedText.Should().NotContain("Mitgliederversammlung");
        updatedText.Should().Contain("Letzte Aktualisierung: 04.06.2026 09:15 Uhr");
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string FindSolutionDirectory()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "ClubSite.slnx")))
            {
                return current.FullName;
            }
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate ClubSite.slnx.");
    }
}
