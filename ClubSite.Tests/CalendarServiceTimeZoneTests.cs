// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms of the MIT license.

using ClubSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace ClubSite.Tests;

[TestFixture]
public class CalendarServiceTimeZoneTests
{
    [TestCase("20260115", "20260115T091500Z", "20260115T094500Z")]
    [TestCase("20260715", "20260715T081500Z", "20260715T084500Z")]
    [TestCase("20260329", "20260329T081500Z", "20260329T084500Z")]
    [TestCase("20261025", "20261025T091500Z", "20261025T094500Z")]
    public void BerlinWindowIncludesUtcEventAfterBoundary(string date, string start, string end)
    {
        var root = Path.Combine(Path.GetTempPath(), "calendar-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "uploads"));
        try
        {
            File.WriteAllText(Path.Combine(root, "uploads", "events.ics"),
                "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nBEGIN:VEVENT\r\nUID:test\r\n" +
                $"DTSTART:{start}\r\nDTEND:{end}\r\nSUMMARY:Test\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n");
            var env = new Mock<IWebHostEnvironment>();
            env.SetupGet(x => x.WebRootPath).Returns(root);
            var service = new CalendarService(env.Object, NullLogger<CalendarService>.Instance,
                new ConfigurationBuilder().Build());
            var startDate = DateTime.ParseExact(date + " 10:00", "yyyyMMdd HH:mm", null);
            var events = service.GetEvents(startDate, startDate.AddHours(1));
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0].StartDate, Is.EqualTo(startDate.AddMinutes(15)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
