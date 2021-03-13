// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace ClubSite
{
    public class Program
    {
        /// <summary>
        /// The name of the configuration folder, which is relative to HostingEnvironment.ContentRootPath.
        /// Constant is also used in components where IWebHostEnvironment is injected
        /// </summary>
        public const string ConfigurationFolder = "Configuration";

        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var currentDir = Directory.GetCurrentDirectory();
            var logger = NLogBuilder
                .ConfigureNLog($@"{currentDir}{Path.DirectorySeparatorChar}{ConfigurationFolder}{Path.DirectorySeparatorChar}NLog.Internal.config")
                .GetCurrentClassLogger();

            // Allows for <target name="file" xsi:type="File" fileName = "${var:logDirectory}logfile.log"... >
            NLog.LogManager.Configuration.Variables["logDirectory"] = currentDir + Path.DirectorySeparatorChar;

            try
            {
                logger.Trace($"Configuration of {nameof(WebHost)} starting.");
                // http://zuga.net/articles/cs-how-to-determine-if-a-program-process-or-file-is-32-bit-or-64-bit/
                logger.Info($"This app runs as {(System.Environment.Is64BitProcess ? "64-bit" : "32-bit")} process.\n\n");
                
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Fatal(e, $"Application stopped after Exception. {e.Message}");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var configPath = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath,
                        ConfigurationFolder);
                    config.SetBasePath(configPath)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                            true)
                        .AddJsonFile(@"credentials.json", false, true)
                        .AddJsonFile($"credentials.{hostingContext.HostingEnvironment.EnvironmentName}.json", false,
                            true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);

                    var secretsFolder = Path.Combine(configPath, @$"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Secrets");
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        if (!Directory.Exists(secretsFolder))
                            throw new DirectoryNotFoundException("Secrets folder not found");
                        config.AddJsonFile(Path.Combine(secretsFolder, @"credentials.json"), false);
                        config.AddJsonFile(
                            Path.Combine(secretsFolder,
                                $"credentials.{hostingContext.HostingEnvironment.EnvironmentName}.json"), false);
                    }

                    NLogBuilder.ConfigureNLog(Path.Combine(configPath,
                        $"NLog.{hostingContext.HostingEnvironment.EnvironmentName}.config"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    // Note: This logging configuration overrides any call to SetMinimumLevel!
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .UseNLog(); // NLog: Setup NLog for dependency injection;
        }
    }
}