using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Blog.Services;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IEmailSender, MailkitEmailSender>();
            services.AddIdentity<AppUser, AppRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 8;
            }).AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<ApplicationDbContext>();
            services.ConfigureApplicationCookie(opt =>
            {
                opt.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Identity/Account/AccessDenied");
            });
            services.AddAuthentication(o =>
            {
                // This forces challenge results to be handled by Google OpenID Handler, so there's no
                // need to add an AccountController that emits challenges for Login.
                o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // This forces forbid results to be handled by Google OpenID Handler, which checks if
                // extra scopes are required and does automatic incremental auth.
                o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // Default scheme that will handle everything else.
                // Once a user is authenticated, the OAuth2 token info is stored in cookies.
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogleOpenIdConnect(options =>
            {
                options.ClientId = "";
                options.ClientSecret = "";
            })
            .AddFacebook(options =>
            {
                options.AppId = "";
                options.AppSecret = "";
            });
            
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!dbContext.Role.Any())
                {
                    roleManager.CreateAsync(new AppRole { Id = Guid.NewGuid().ToString(), Name = "Admin" });
                    roleManager.CreateAsync(new AppRole { Id = Guid.NewGuid().ToString(), Name = "Reader"});
                    Console.WriteLine("Roles Successfully Created");
                }
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
