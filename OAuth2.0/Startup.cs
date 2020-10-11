using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace OAuth2_0_Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = "ClientCookie";
                config.DefaultSignInScheme = "ClientCookie";
                config.DefaultChallengeScheme = "OAuth20";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OAuth20", config =>
                {
                    config.CallbackPath = "/oauth2/callback";

                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    
                    config.AuthorizationEndpoint = "https://localhost:44324/oauth2/authorize";
                    config.TokenEndpoint = "https://localhost:44324/oauth2/token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context => {
                            var token = context.AccessToken;

                            var payload = token.Split(".")[1];
                            var bytes = Convert.FromBase64String(payload + "=");
                            var jsonPayload = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
