// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.Net;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace ClubSite.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
public class BasicIntegrationTests
{
    private static readonly WebApplicationFactory<Program> Factory = new();
    private static readonly HttpClient Client = Factory.CreateClient();

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Client.Dispose();
        Factory.Dispose();
    }

    private static IEnumerable<string> GetSitemapUrls()
    {
        var response = Client.GetAsync("/sitemap.xml").GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var xml = XDocument.Parse(content);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var locElements = xml.Descendants(ns + "loc").ToList();
        // Fallback in case the sitemap has no namespace declared
        if (locElements.Count == 0)
        {
            locElements = xml.Descendants("loc").ToList();
        }

        foreach (var loc in locElements)
        {
            var absoluteUrl = loc.Value.Trim();
            if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri)) 
                continue;
            
            var path = uri.PathAndQuery;
            yield return string.IsNullOrEmpty(path) ? "/" : path;
        }
    }

    [TestCaseSource(nameof(GetSitemapUrls))]
    public async Task GetEndpoints_ReturnSuccessAndCorrectContentType(string url)
    {
        var response = await Client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Be("text/html; charset=utf-8");
    }
}
