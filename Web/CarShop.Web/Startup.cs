﻿namespace CarShop.Web
{
    using System.Linq;
    using System.Reflection;

    using CarShop.Data;
    using CarShop.Data.Common;
    using CarShop.Data.Common.Models;
    using CarShop.Data.Common.Repositories;
    using CarShop.Data.Models;
    using CarShop.Data.Models.Vehicles;
    using CarShop.Data.Repositories;
    using CarShop.Data.Seeding;
    using CarShop.Services.BusAds;
    using CarShop.Services.Buses;
    using CarShop.Services.CarAds;
    using CarShop.Services.Cars;
    using CarShop.Services.Data;
    using CarShop.Services.Images;
    using CarShop.Services.Mapping;
    using CarShop.Services.Messaging;
    using CarShop.Services.MotorcycleAds;
    using CarShop.Services.Motorcycles;
    using CarShop.Services.SaveImagesService;
    using CarShop.Services.TruckAds;
    using CarShop.Services.Trucks;
    using CarShop.Services.Users;
    using CarShop.Services.Vehicles;
    using CarShop.Web.ViewModels;
    using CarShop.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Framework services
            // TODO: Add pooling when this bug is fixed: https://github.com/aspnet/EntityFrameworkCore/issues/9741
            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"));
                    options.UseLazyLoadingProxies();
                });

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddAuthentication();
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = this.configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = this.configuration["Authentication:Facebook:AppSecret"];
            //})
            //.AddGoogle(options =>
            //{
            //    IConfigurationSection googleAuthNSection =
            //        this.configuration.GetSection("Authentication:Google");
            //
            //    options.ClientId = googleAuthNSection["ClientId"];
            //   options.ClientSecret = googleAuthNSection["ClientSecret"];
            //});

            services.AddControllersWithViews();

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                    options.ConsentCookie.Name = ".AspNetCore.ConsentCookie";
                });

            services.AddSingleton(this.configuration);

            // Identity stores
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISmsSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<ITrucksService, TrucksService>();
            services.AddTransient<IMotorcyclesService, MotorcyclesService>();
            services.AddTransient<IBusesService, BusesService>();
            services.AddTransient<ICarsService, CarsService>();
            services.AddTransient<IVehiclesService, VehiclesService>();
            services.AddTransient<ICarAdsService, CarAdsService>();
            services.AddTransient<IBusAdsService, BusAdsService>();
            services.AddTransient<ITruckAdsService, TruckAdsService>();
            services.AddTransient<IMotorcycleAdsService, MotorcycleAdsService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<ISaveImagesService, SaveImagesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(CreateCarViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRouting();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();

            //app.UseResponseCompression();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
