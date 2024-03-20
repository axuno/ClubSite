// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.Data.EF.SQLServer;
using System;
using Microsoft.AspNetCore.Http;
using ClubSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using Piranha.AttributeBuilder;
using Piranha.Manager.Editor;
using Piranha;

namespace ClubSite;

/// <summary>
/// The startup class to set up and configure the ClubSite.
/// </summary>
public static class WebAppStartup
{
    /// <summary>
    /// The method gets called by <see cref="Program"/> at startup, BEFORE building the app is completed.
    /// </summary>
    public static void ConfigureServices(IWebHostEnvironment context, IConfigurationManager configuration, IServiceCollection services)
    {
        // required for cookies and session cookies (will throw CryptographicException without)
        services.AddDataProtection()
            .SetApplicationName(context.ApplicationName)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(360))
            .PersistKeysToFileSystem(
                new DirectoryInfo(Path.Combine(context.ContentRootPath, "DataProtectionKeys")))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration() {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

        services.AddMemoryCache(); // Adds a default in-memory cache implementation

        // Custom ClubSite db context
        services.AddDbContext<Data.ClubDbContext>((sp, options) =>
            options.UseSqlServer(configuration.GetConnectionString("VolleyballClub")));

        // Piranha service setup
        services.AddPiranha(svcBuilder =>
        {
            svcBuilder.AddRazorRuntimeCompilation = context.IsDevelopment();

            svcBuilder.UseCms();
            svcBuilder.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
            svcBuilder.UseImageSharp();
            svcBuilder.UseManager(); // https://localhost:44307/manager/ initial user: admin, pw: password
            svcBuilder.UseTinyMCE();
            svcBuilder.UseMemoryCache();
            svcBuilder.UseEF<SQLServerDb>(db =>
                db.UseSqlServer(configuration.GetConnectionString("VolleyballClub")));
            svcBuilder.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                db.UseSqlServer(configuration.GetConnectionString("VolleyballClub")));
        });
            
        // MUST be before AddMvc!
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = ".sid";
            options.Cookie.IsEssential = true;
        });

        services.AddRazorPages().AddPiranhaManagerOptions();

        services.AddMvc()
            .AddSessionStateTempDataProvider()
            .AddMvcOptions(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => string.Format(Resources.ModelBindingMessageResource.ValueMustNotBeNull));
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, val) => string.Format(Resources.ModelBindingMessageResource.AttemptedValueIsInvalid, x, val));
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => string.Format(Resources.ModelBindingMessageResource.ValueIsInvalid, x));
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => string.Format(Resources.ModelBindingMessageResource.ValueMustBeANumber, x));
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => Resources.ModelBindingMessageResource.MissingKeyOrValue);
            })
            .AddControllersAsServices();

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
        });

        services.Configure<ConfigurationPoco.MailSettings>(
            configuration.GetSection(nameof(ConfigurationPoco.MailSettings)) ??
            throw new InvalidOperationException(
                $"Configuration section '{nameof(ConfigurationPoco.MailSettings)}' not found."));

        services.AddTransient<Services.IMailService, Services.MailService>();

        // We use EPPlus in a noncommercial context according to the Polyform Noncommercial license:
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    }

    /// <summary>
    /// The method gets called by <see cref="Program"/> at startup, AFTER building the app is completed.
    /// </summary>
    public static void Configure(WebApplication app, ILoggerFactory loggerFactory)
    {
        app.UseHttpsRedirection();

        // Note: Piranha sets culture and language
        // from table Piranha_Sites field Culture and the UI language
        // from table Piranha_Sites field LanguageId (stored in table Piranha_Languages)
        // This overrides the following settings:
        var cultureInfo = CultureInfo.GetCultureInfo("de-DE");
        CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture =
            CultureInfo.CurrentUICulture = cultureInfo;

        // Make sure we can connect to the database
        using (var connection =
               new Microsoft.Data.SqlClient.SqlConnection(app.Configuration.GetConnectionString("VolleyballClub")))
            try
            {
                connection.Open();
            }
            catch
            {
                loggerFactory.CreateLogger(nameof(WebAppStartup)).LogCritical("Failed to connect: {ConnectionString}", connection.ConnectionString);
            }
            finally
            {
                connection.Close();
            }

        var env = app.Environment;

        #region * Setup error handling *

        // Error handling must be one of the very first things to configure
        if (env.IsProduction())
        {
            // The StatusCodePagesMiddleware should be one of the earliest 
            // middleware in the pipeline, as it can only modify the response 
            // of middleware that comes after it in the pipeline
            app.UseStatusCodePagesWithReExecute($"/Error/{{0}}");
            app.UseExceptionHandler("/Error/500");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
        }

        #endregion

        // Keep before .UsePiranha()
        app.UseSession();

        #region *** Rewrite domains (even those without SSL certificate) to https://www.volleyballclub.de ***

        app.UseRewriter(new RewriteOptions()
            .AddRedirectToWwwPermanent()
            .AddRedirectToHttpsPermanent()
        );

        #endregion

        #region *** Configure PiranhaCMS ***

        // Configure Tiny MCE
        EditorConfig.FromFile($@"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{Program.ConfigurationFolder}{Path.DirectorySeparatorChar}editorconfig.json");

        // Middleware setup
        app.UsePiranha(options =>
        {
            // Initialize Piranha
            App.Init(options.Api);
            // Allow SVG files to be uploaded in manager
            App.MediaTypes.Images.Add(".svg", "image/svg+xml", false);
            options.UseManager();
            options.UseTinyMCE();
            options.UseIdentity();
            
            // Build all content types in this assembly
            _ = new ContentTypeBuilder(options.Api)
                .AddAssembly(typeof(Program).Assembly)
                .Build()
                .DeleteOrphans();

            // To build specific types:
            // new ContentTypeBuilder(options.Api).AddType(typeof(...)

            /*
             * Here you can configure the different permissions
             * that you want to use for securing content in the
             * application.
            options.UseSecurity(o =>
            {
                o.UsePermission("WebUser", "Web User");
            });
            */

            /*
             * Here you can specify the login url for the front end
             * application. This does not affect the login url of
             * the manager interface.
                options.LoginUrl = "login";
             */
        });

        // Register custom blocks
        App.Blocks.Register<PersonProfileBlock>();
        App.Blocks.Register<NewPostsBlock>();
        App.Blocks.Register<HtmlFeaturedBlock>();

        #endregion

        // For static files using a content type provider:
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        // Make sure .webmanifest files don't cause a 404
        provider.Mappings[".webmanifest"] = "application/manifest+json";
        app.UseStaticFiles(new StaticFileOptions {
            ContentTypeProvider = provider,
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(5)
                };
            }
        });
    }
}
