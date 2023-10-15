using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shinra.Clients;
using Shinra.Services;

namespace Shinra.Config
{
    public static class Dependencies
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddHttpClient<IBlizzardClient, BlizzardClient>();
            services.AddSingleton<BlizzardParserService>();
            services.AddSingleton<IBlizzardDataAccess, MongoDataAccess>();
            services.AddHangfire(c => c.UseMemoryStorage());
            services.AddHangfireServer();
        }
    }
}
