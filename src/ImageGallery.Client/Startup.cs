﻿using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Disable JWT claim name transformation mapping.
            // This will keep the original claim from the token.
            // e.g. sub => http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                 .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            // create an HttpClient used for accessing the Web API service
            services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44366/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            // create an HttpClient used for accessing the IDP (IdentityServer)
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44318/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            // 
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // Add to cookie once the identity token is validated and transferred to user identity claim.
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => // register open ID connect handler.
            {
                // Configure open ID connect middleware.
                // Default settings can be found at: https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.OpenIdConnect/OpenIdConnectOptions.cs
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://localhost:44318"; // Our IdentityServer in Marvin.IDP project. Use this URL to read the discovery document to discovery endpoints.
                options.ClientId = "imagegalleryclient"; // client application ID which is presented to IdenityServer during token request.
                options.ResponseType = "code"; // use Authorization Code flow for code grant flow
                // options.UsePkce = false;
                // options.CallbackPath = new PathString("..."); // if not specified, use default redirect url that was registered in Marvin.IDP project.
                options.Scope.Add("openid"); // request user identity scope
                options.Scope.Add("profile"); // request user profile scope which can access user profiles such as given name and family name
                options.Scope.Add("address"); // request address scope
                options.Scope.Add("roles"); // request for roles scope
                // options.ClaimActions.Remove("nbf"); // remove notbefore (nbf) claim filters so that notbefore claim is not filtered by the middleware.
                options.ClaimActions.DeleteClaim("address");
                options.ClaimActions.DeleteClaim("sid"); // remove (filter out) sid claim. the API naming is a bit confusing though.
                options.ClaimActions.DeleteClaim("idp");
                options.ClaimActions.DeleteClaim("s_hash");
                options.ClaimActions.DeleteClaim("auth_time");
                options.ClaimActions.MapUniqueJsonKey("role", "role"); // add role to claim transformation mapping. otherwise, the User claims created by the middleware won't include role claim.
                options.SaveTokens = true; // middleware saves the received tokens so it can be used afterwards
                options.ClientSecret = "secret"; // Client secret which is presented to IdenityServer during token request.
                options.GetClaimsFromUserInfoEndpoint = true; // Call UserInfo endpoint on IdentityServer to get user profile
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Tell the framework where to read user role.
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for
                // production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
