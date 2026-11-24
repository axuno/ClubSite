// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClubSite.Models;
using ClubSite.Pages;
using ClubSite.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using NUnit.Framework;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Services;

namespace ClubSite.Tests;

[TestFixture]
public class CalendarServiceTests
{
    private sealed class InMemoryStorage : IStorage
    {
        private readonly Dictionary<Guid, byte[]> _contentByMediaId = new();
        public int ReadCount { get; private set; }

        public void SetContent(Guid mediaId, string icsContent)
        {
            _contentByMediaId[mediaId] = Encoding.UTF8.GetBytes(icsContent);
        }

        public void Remove(Guid mediaId) => _contentByMediaId.Remove(mediaId);

        public Task<IStorageSession> OpenAsync() => Task.FromResult<IStorageSession>(new Session(this));

        public string GetPublicUrl(Media media, string filename) => $"/uploads/{media.Id}/{filename}";

        public string GetResourceName(Media media, string filename) => $"{media.Id}/{filename}";

        private sealed class Session(InMemoryStorage owner) : IStorageSession
        {
            public Task<bool> GetAsync(Media media, string filename, Stream stream)
            {
                owner.ReadCount++;
                if (!owner._contentByMediaId.TryGetValue(media.Id, out var bytes))
                {
                    return Task.FromResult(false);
                }

                stream.Write(bytes, 0, bytes.Length);
                return Task.FromResult(true);
            }

            public Task<string> PutAsync(Media media, string filename, string contentType, Stream stream) =>
                throw new NotSupportedException();

            public Task<string> PutAsync(Media media, string filename, string contentType, byte[] bytes) =>
                throw new NotSupportedException();

            public Task<bool> DeleteAsync(Media media, string filename) =>
                throw new NotSupportedException();

            public void Dispose()
            {
            }
        }
    }

    private sealed class FixedClock(Instant instant) : IClock
    {
        public Instant GetCurrentInstant() => instant;
    }

    private static Media CreateMedia(Guid id, string filename, DateTime lastModified, string content, InMemoryStorage storage, MediaType type = MediaType.Document)
    {
        storage.SetContent(id, content);
        return new Media
        {
            Id = id,
            Filename = filename,
            Type = type,
            Size = Encoding.UTF8.GetByteCount(content),
            LastModified = lastModified
        };
    }

    [TestCase("20260115", "20260115T091500Z", "20260115T094500Z")] // Winter (CET, UTC+1)
    [TestCase("20260715", "20260715T081500Z", "20260715T084500Z")] // Summer (CEST, UTC+2)
    [TestCase("20260329", "20260329T081500Z", "20260329T084500Z")] // Spring-forward day
    [TestCase("20261025", "20261025T091500Z", "20261025T094500Z")] // Fall-back day
    public async Task LoadEventsAsync_InterpretsRangeInEuropeBerlinAcrossDst(string date, string startUtc, string endUtc)
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var media = CreateMedia(
            Guid.NewGuid(),
            "events.ics",
            new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc),
            "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nBEGIN:VEVENT\r\nUID:dst-test\r\n" +
            $"DTSTART:{startUtc}\r\nDTEND:{endUtc}\r\nSUMMARY:Spieltag\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n",
            storage);

        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 6, 1, 10, 0)));

        var startDate = DateTime.ParseExact(date + " 10:00", "yyyyMMdd HH:mm", null);
        var result = await service.LoadEventsAsync([media], startDate, startDate.AddHours(1));

        result.Events.Should().ContainSingle();
        result.Events[0].StartDate.Should().Be(startDate.AddMinutes(15));
        result.Events[0].EndDate.Should().Be(startDate.AddMinutes(45));
    }

    [Test]
    public async Task LoadEventsAsync_WeeklyRruleAcrossSpringForwardKeepsBerlinWallClockTime()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:training-weekly
            DTSTART;TZID=Europe/Berlin:20260322T190000
            DTEND;TZID=Europe/Berlin:20260322T203000
            RRULE:FREQ=WEEKLY;COUNT=3
            SUMMARY:Damen Training
            CATEGORIES:Damen 1,Training
            END:VEVENT
            END:VCALENDAR
            """;
        var media = CreateMedia(Guid.NewGuid(), "training.ics", new DateTime(2026, 3, 1, 9, 0, 0), ics, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 3, 20, 12, 0)));

        var result = await service.LoadEventsAsync([media], new DateTime(2026, 3, 20, 0, 0, 0), new DateTime(2026, 4, 10, 23, 59, 0));

        result.Events.Should().HaveCount(3);
        result.Events.Select(e => e.StartDate).Should().Equal(
            new DateTime(2026, 3, 22, 19, 0, 0),
            new DateTime(2026, 3, 29, 19, 0, 0),
            new DateTime(2026, 4, 5, 19, 0, 0));
    }

    [Test]
    public async Task LoadEventsAsync_HandlesSkippedSpringForwardGapWithoutThrowing()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:gap-event
            DTSTART:20260329T011500Z
            DTEND:20260329T014500Z
            SUMMARY:Nach Umstellung
            END:VEVENT
            END:VCALENDAR
            """;
        var media = CreateMedia(Guid.NewGuid(), "gap.ics", new DateTime(2026, 3, 1, 9, 0, 0), ics, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 3, 29, 0, 0)));

        // 02:30 Berlin does not exist on 2026-03-29; leniently maps to 03:30+02:00, which falls inside 03:15-03:45+02:00.
        var result = await service.LoadEventsAsync([media], new DateTime(2026, 3, 29, 2, 30, 0), new DateTime(2026, 3, 29, 4, 0, 0));

        result.Events.Should().ContainSingle();
        result.Events[0].StartDate.Should().Be(new DateTime(2026, 3, 29, 3, 15, 0));
    }

    [Test]
    public async Task LoadEventsAsync_ReconcilesRecurrenceIdOverrideWithoutDuplicatingOriginal()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var ics = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:series-1
            DTSTART;TZID=Europe/Berlin:20260501T180000
            DTEND;TZID=Europe/Berlin:20260501T200000
            RRULE:FREQ=WEEKLY;COUNT=2
            SUMMARY:Reguläres Training
            END:VEVENT
            BEGIN:VEVENT
            UID:series-1
            RECURRENCE-ID;TZID=Europe/Berlin:20260508T180000
            DTSTART;TZID=Europe/Berlin:20260508T193000
            DTEND;TZID=Europe/Berlin:20260508T210000
            SUMMARY:Verlegtes Training
            END:VEVENT
            END:VCALENDAR
            """;
        var media = CreateMedia(Guid.NewGuid(), "series.ics", new DateTime(2026, 5, 1, 8, 0, 0), ics, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 5, 1, 8, 0)));

        var result = await service.LoadEventsAsync([media], new DateTime(2026, 5, 1, 0, 0, 0), new DateTime(2026, 5, 15, 23, 59, 0));

        result.Events.Should().HaveCount(2);
        result.Events[0].Title.Should().Be("Reguläres Training");
        result.Events[1].Title.Should().Be("Verlegtes Training");
        result.Events[1].StartDate.Should().Be(new DateTime(2026, 5, 8, 19, 30, 0));
    }

    [Test]
    public async Task LoadEventsAsync_MergesMultiplePiranhaDocumentsAndTracksLatestModified()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var bvvIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:bvv-1
            DTSTART;TZID=Europe/Berlin:20260610T140000
            DTEND;TZID=Europe/Berlin:20260610T160000
            SUMMARY:BVV Spieltag Damen 1
            CATEGORIES:Damen 1,Heimspieltag
            URL:https://volleyball.bayern/spiel/123
            END:VEVENT
            END:VCALENDAR
            """;
        var clubIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:club-1
            DTSTART;TZID=Europe/Berlin:20260605T180000
            DTEND;TZID=Europe/Berlin:20260605T200000
            SUMMARY:Jahreshauptversammlung
            CATEGORIES:Verein,Vereinstermin
            END:VEVENT
            END:VCALENDAR
            """;

        var media1 = CreateMedia(Guid.NewGuid(), "bvv.ics", new DateTime(2026, 6, 1, 10, 0, 0), bvvIcs, storage);
        var media2 = CreateMedia(Guid.NewGuid(), "verein.ics", new DateTime(2026, 6, 3, 15, 30, 0), clubIcs, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 6, 1, 12, 0)));

        var result = await service.LoadEventsAsync([media1, media2], new DateTime(2026, 6, 1, 0, 0, 0), new DateTime(2026, 7, 1, 0, 0, 0));

        result.Events.Should().HaveCount(2);
        result.Events[0].Title.Should().Be("Jahreshauptversammlung");
        result.Events[0].Url.Should().BeNull();
        result.Events[1].Title.Should().Be("BVV Spieltag Damen 1");
        result.Events[1].Url.Should().Be("https://volleyball.bayern/spiel/123");
        result.AvailableTeams.Should().Equal("Damen 1", "Verein");
        result.AvailableCategories.Should().Equal("Heimspieltag", "Vereinstermin");
        result.LastUpdated.Should().Be(new DateTime(2026, 6, 3, 15, 30, 0));
    }

    [Test]
    public async Task LoadEventsAsync_InvalidatesCacheAutomaticallyWhenPiranhaMediaIsUpdated()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var mediaId = Guid.NewGuid();
        var v1 = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:live-1
            DTSTART;TZID=Europe/Berlin:20260612T180000
            DTEND;TZID=Europe/Berlin:20260612T190000
            SUMMARY:Termin Alt
            END:VEVENT
            END:VCALENDAR
            """;
        var v2 = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:live-1
            DTSTART;TZID=Europe/Berlin:20260612T180000
            DTEND;TZID=Europe/Berlin:20260612T190000
            SUMMARY:Termin Neu
            END:VEVENT
            END:VCALENDAR
            """;

        var mediaV1 = CreateMedia(mediaId, "events.ics", new DateTime(2026, 6, 1, 10, 0, 0), v1, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 6, 1, 12, 0)));

        var first = await service.LoadEventsAsync([mediaV1], new DateTime(2026, 6, 1, 0, 0, 0), new DateTime(2026, 7, 1, 0, 0, 0));
        var secondCached = await service.LoadEventsAsync([mediaV1], new DateTime(2026, 6, 1, 0, 0, 0), new DateTime(2026, 7, 1, 0, 0, 0));

        first.Events[0].Title.Should().Be("Termin Alt");
        secondCached.Events[0].Title.Should().Be("Termin Alt");
        storage.ReadCount.Should().Be(1, "unchanged media version should be served from memory cache");

        // Simulate Piranha Manager replacing the file in-place (same Media.Id, newer LastModified)
        var mediaV2 = CreateMedia(mediaId, "events.ics", new DateTime(2026, 6, 2, 11, 0, 0), v2, storage);
        var afterUpdate = await service.LoadEventsAsync([mediaV2], new DateTime(2026, 6, 1, 0, 0, 0), new DateTime(2026, 7, 1, 0, 0, 0));

        afterUpdate.Events[0].Title.Should().Be("Termin Neu");
        storage.ReadCount.Should().Be(2, "updated media metadata must trigger reload without app restart");
    }

    [Test]
    public async Task LoadEventsAsync_RejectsUnsafeFilenamesNonIcsAndJavascriptUrls()
    {
        var storage = new InMemoryStorage();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var validIcs = """
            BEGIN:VCALENDAR
            VERSION:2.0
            BEGIN:VEVENT
            UID:xss-check
            DTSTART;TZID=Europe/Berlin:20260615T180000
            DTEND;TZID=Europe/Berlin:20260615T190000
            SUMMARY:Sicherer Termin
            URL:javascript:alert(1)
            END:VEVENT
            END:VCALENDAR
            """;

        var traversal = CreateMedia(Guid.NewGuid(), "../secret.ics", DateTime.UtcNow, validIcs, storage);
        var pdf = CreateMedia(Guid.NewGuid(), "brochure.pdf", DateTime.UtcNow, validIcs, storage);
        var valid = CreateMedia(Guid.NewGuid(), "verein.ics", DateTime.UtcNow, validIcs, storage);
        var service = new CalendarService(storage, cache, NullLogger<CalendarService>.Instance,
            new FixedClock(Instant.FromUtc(2026, 6, 1, 12, 0)));

        var result = await service.LoadEventsAsync([traversal, pdf, valid], new DateTime(2026, 6, 1, 0, 0, 0), new DateTime(2026, 7, 1, 0, 0, 0));

        result.Events.Should().ContainSingle();
        result.Events[0].Url.Should().BeNull("non-http/https schemes must be rejected");
        storage.ReadCount.Should().Be(1, "traversal and non-.ics assets must never be read from storage");
    }

    [Test]
    public async Task CalendarPageModel_OnGet_AwaitsPiranhaPageAndLoadsMediaDocuments()
    {
        var pageId = Guid.NewGuid();
        var mediaId = Guid.NewGuid();
        var media = new Media { Id = mediaId, Filename = "spieltage.ics", Type = MediaType.Document, LastModified = new DateTime(2026, 6, 2, 14, 0, 0) };
        var pageData = new CalendarPage
        {
            Id = pageId,
            Title = "Termine",
            CalendarFiles =
            [
                new CalendarDocumentRegion
                {
                    Title = new StringField { Value = "BVV" },
                    Document = new DocumentField { Id = mediaId, Media = media }
                }
            ]
        };

        var loader = new Mock<IModelLoader>();
        loader.Setup(l => l.GetPageAsync<CalendarPage>(pageId, It.IsAny<System.Security.Claims.ClaimsPrincipal>(), false))
            .ReturnsAsync(pageData);

        var mediaService = new Mock<IMediaService>();
        mediaService.Setup(m => m.GetByIdAsync(mediaId)).ReturnsAsync(media);

        var api = new Mock<IApi>();
        api.SetupGet(a => a.Media).Returns(mediaService.Object);

        var calendarService = new Mock<ICalendarService>();
        calendarService.Setup(s => s.LoadEventsAsync(
                It.Is<IEnumerable<Media>>(docs => docs.Single().Id == mediaId),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>()))
            .ReturnsAsync(new CalendarLoadResult
            {
                Events =
                [
                    new CalendarEventItem
                    {
                        Title = "Heimspiel",
                        StartDate = new DateTime(2026, 6, 20, 14, 0, 0),
                        EndDate = new DateTime(2026, 6, 20, 16, 0, 0),
                        Team = "Damen 1",
                        Category = "Heimspieltag"
                    }
                ],
                AvailableTeams = ["Damen 1"],
                AvailableCategories = ["Heimspieltag"],
                LastUpdated = media.LastModified
            });

        var model = new CalendarPageModel(calendarService.Object, api.Object, loader.Object,
            new FixedClock(Instant.FromUtc(2026, 6, 10, 10, 0)))
        {
            PageContext = new PageContext { HttpContext = new DefaultHttpContext() }
        };

        var actionResult = await model.OnGet(pageId);

        actionResult.Should().BeOfType<PageResult>();
        model.HasIcsFiles.Should().BeTrue();
        model.Events.Should().ContainSingle(e => e.Title == "Heimspiel");
        model.LastUpdated.Should().Be(media.LastModified);
    }
}
