using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Shinra.Actors;
using System;

namespace Shinra.Config
{
    public static class WebHosting
    {
        internal static void Configure(this WebApplication app)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireDashboard();
            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHangfireDashboard();

            Scheduler.Configure();
            ActorService.Configure(app.Services);
        }
    }
}
