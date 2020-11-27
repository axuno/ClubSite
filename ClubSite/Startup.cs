using System;
using System.IO;
using ClubSite.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.Data.EF.SQLServer;
using Piranha.Manager.Editor;

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
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // required for cookies and session cookies (will throw CryptographicException without)
            services.AddDataProtection()
                .SetApplicationName("ClubSite")
                .SetDefaultKeyLifetime(System.TimeSpan.FromDays(360))
                .PersistKeysToFileSystem(
                    new DirectoryInfo(Path.Combine(WebHostEnvironment.ContentRootPath, "DataProtectionKeys")))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
            
            services.AddMemoryCache(); // Adds a default in-memory cache implementation

            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = WebHostEnvironment.IsDevelopment();

                options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                options.UseImageSharp();
                options.UseManager(); // https://localhost:44306/manager/pages  user: admin, pw: password
                options.UseTinyMCE();
                options.UseMemoryCache();
                options.UseEF<SQLServerDb>(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));
                options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));
            });
            services.AddPiranhaApplication();

            services.AddDbContext<Data.ClubDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VolleyballClub")));

            // MUST be before AddMvc!
            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".sid";
                options.Cookie.IsEssential = true;
            });
            
            services.AddRazorPages().AddPiranhaManagerOptions();

            services.AddMvc()
                .AddSessionStateTempDataProvider()
                .AddMvcOptions(options =>
                {
                })
                .AddControllersAsServices();

            services.Configure<ConfigurationPoco.MailSettings>(
                Configuration.GetSection(nameof(ConfigurationPoco.MailSettings)) ?? throw new ArgumentNullException($"Configuration section '{nameof(ConfigurationPoco.MailSettings)}' not found."));
            
            services.AddTransient<Services.IMailService, Services.MailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            var cultureInfo = new System.Globalization.CultureInfo("de-DE");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            EditorConfig.FromFile("editorconfig.json");

            // Keep before .UsePiranha()
            app.UseSession();

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });
            // For static files using a content type provider:
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            // Make sure .webmanifest files don't cause a 404
            provider.Mappings[".webmanifest"] = "application/manifest+json";
        }
    }
}
