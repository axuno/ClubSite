// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;


namespace ClubSite;

public class Program
{
    /// <summary>
    /// The name of the configuration folder, which is relative to HostingEnvironment.ContentRootPath.
    /// Constant is also used in components where IWebHostEnvironment is injected
    /// </summary>
    public const string ConfigurationFolder = "Configuration";

    public static async Task Main(string[] args)
    {
        // NLog: setup the logger first to catch all errors
        var currentDir = Directory.GetCurrentDirectory();

        var logger = LogManager.Setup()
            .LoadConfigurationFromFile($"{currentDir}{Path.DirectorySeparatorChar}{ConfigurationFolder}{Path.DirectorySeparatorChar}NLog.Internal.config")
            .GetCurrentClassLogger();

        // Allows for <target name="file" xsi:type="File" fileName = "${var:logDirectory}logfile.log"... >
        LogManager.Configuration.Variables["logDirectory"] = $"{currentDir}{Path.DirectorySeparatorChar}";

        try
        {
            logger.Trace($"Configuration of {nameof(Microsoft.AspNetCore.WebHost)} starting.");
            logger.Info($"This app runs as {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} process.\n\n");
                
            var builder = SetupBuilder(args);

            var loggingConfig = builder.Configuration.GetSection("Logging");
            builder.Logging.ClearProviders();
            // Enable NLog as logging provider for Microsoft.Extension.Logging
            builder.Logging.AddNLog(loggingConfig);
            LogManager.Setup()
                .LoadConfigurationFromFile(Path.Combine(builder.Environment.ContentRootPath, ConfigurationFolder,
                    $"NLog.{builder.Environment.EnvironmentName}.config"));
            
            WebAppStartup.ConfigureServices(builder.Environment, builder.Configuration, builder.Services);

            var app = builder.Build();
            WebAppStartup.Configure(app, app.Services.GetRequiredService<ILoggerFactory>());
            
            await app.RunAsync();
        }
        catch (Exception e)
        {
            logger.Fatal(e, $"Application stopped after Exception. {e.Message}");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }

    public static WebApplicationBuilder SetupBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.GetName().Name, // don't use Assembly.Fullname
            WebRootPath = "wwwroot"
            // Note: ContentRootPath is detected by the framework.
            // If set explicitly as WebApplicationOptions, WebApplicationFactory in unit tests does not override it.
            // Set WebRootPath as folder relative to ContentRootPath.
        });
    
        var absoluteConfigurationPath = Path.Combine(builder.Environment.ContentRootPath,
            ConfigurationFolder);

        builder.Configuration.SetBasePath(absoluteConfigurationPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true, reloadOnChange: true)
            .AddJsonFile(@"credentials.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"credentials.{builder.Environment.EnvironmentName}.json",
                optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        if (builder.Environment.IsDevelopment())
        {
            var secretsFolder = Path.Combine(builder.Environment.ContentRootPath, ConfigurationFolder, @$"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Secrets");
            if (!Directory.Exists(secretsFolder)) throw new DirectoryNotFoundException("Secrets folder not found");
            builder.Configuration.AddJsonFile(Path.Combine(secretsFolder, @"credentials.json"), false);
            builder.Configuration.AddJsonFile(Path.Combine(secretsFolder, $"credentials.{builder.Environment.EnvironmentName}.json"), false);
        }

        // Use static web assets from League (and other referenced projects or packages)
        builder.WebHost.UseStaticWebAssets();

        return builder;
    }
}