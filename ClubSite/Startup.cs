// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.IO;
using ClubSite.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.Data.EF.SQLServer;
using Piranha.Manager.Editor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ClubSite
{
    public class Startup
    {
        /// <summary>
        /// Gets the application configuration properties of this application.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the information about the web hosting environment of this application.
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }
        
        /// <summary>
        /// Gets the logger for class <see cref="Startup"/>.
        /// </summary>
        public ILogger<Startup> Logger { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog();                
            });

            Logger = loggerFactory.CreateLogger<Startup>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // required for cookies and session cookies (will throw CryptographicException without)
            services.AddDataProtection()
                .SetApplicationName("ClubSite")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(360))
                .PersistKeysToFileSystem(
                    new DirectoryInfo(Path.Combine(WebHostEnvironment.ContentRootPath, "DataProtectionKeys")))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration() {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

            // Make sure we can connect to the database
            using (var connection =
                new Microsoft.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("VolleyballClub")))
                try
                {
                    connection.Open();
                }
                finally
                {
                    connection.Close();
                }

            services.AddMemoryCache(); // Adds a default in-memory cache implementation

            // Custom ClubSite db context
            services.AddDbContext<Data.ClubDbContext>((sp, options) =>
                options.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));

            // Piranha service setup
            services.AddPiranha(svcBuilder =>
            {
                svcBuilder.AddRazorRuntimeCompilation = WebHostEnvironment.IsDevelopment();

                svcBuilder.UseCms();
                svcBuilder.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                svcBuilder.UseImageSharp();
                svcBuilder.UseManager(); // https://localhost:44306/manager/ initial user: admin, pw: password
                svcBuilder.UseTinyMCE();
                svcBuilder.UseMemoryCache();
                svcBuilder.UseEF<SQLServerDb>(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));
                svcBuilder.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));
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
                Configuration.GetSection(nameof(ConfigurationPoco.MailSettings)) ??
                throw new InvalidOperationException(
                    $"Configuration section '{nameof(ConfigurationPoco.MailSettings)}' not found."));

            services.AddTransient<Services.IMailService, Services.MailService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <param name="env">The <see cref="IWebHostEnvironment"/></param>
        /// <param name="api">The PiranhaCms <see cref="IApi"/></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            _ = env.EnvironmentName;
            var cultureInfo = new System.Globalization.CultureInfo("de-DE");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                System.Globalization.CultureInfo.CurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture =
                System.Globalization.CultureInfo.CurrentCulture = cultureInfo;

            if (false)
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseStatusCodePagesWithReExecute($"/Error/{{0}}");
                app.UseExceptionHandler($"/Error/500");
                // instruct the browsers to always access the site via HTTPS
                app.UseHsts();
            }

            // Initialize Piranha
            App.Init(api);

            // Build all content types in this assembly
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Register custom blocks
            App.Blocks.Register<PersonProfileBlock>();

            /* To build specific types:
             new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.BlogArchive))
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage))
                .Build()
                .DeleteOrphans();
            new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.BlogPost))
                .Build()
                .DeleteOrphans();
            new Piranha.AttributeBuilder.SiteTypeBuilder(api)
                .AddType(typeof(Models.StandardSite))
                .Build()
                .DeleteOrphans();
             */

            // Configure Tiny MCE
            EditorConfig.FromFile($@"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{Program.ConfigurationFolder}{Path.DirectorySeparatorChar}editorconfig.json");

            // Keep before .UsePiranha()
            app.UseSession();

            #region *** Rewrite domains (even those without SSL certificate) to https://www.volleyballclub.de ***

            app.UseRewriter(new RewriteOptions()
                .AddRedirectToWwwPermanent()
                .AddRedirectToHttpsPermanent()
            );

            #endregion

            // Middleware setup
            app.UsePiranha(options =>
            {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });

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
}