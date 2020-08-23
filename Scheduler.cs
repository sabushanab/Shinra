using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Shinra.Clients;

namespace Shinra
{
    public class Scheduler
    {
        public Scheduler() {}
        public static void Configure(IServiceProvider provider) 
        {
            RecurringJob.RemoveIfExists(nameof(XIVAPIClient.GetFreeCompanyMembers));
            RecurringJob.AddOrUpdate(nameof(XIVAPIClient.GetFreeCompanyMembers), (IXIVAPIClient client) => client.GetFreeCompanyMembers(),
            "*/10 * * * *");
            RecurringJob.RemoveIfExists(nameof(XIVAPIClient.GetEachFreeCompanyMember));
            RecurringJob.AddOrUpdate(nameof(XIVAPIClient.GetEachFreeCompanyMember), (IXIVAPIClient client) => client.GetEachFreeCompanyMember(),
            "*/30 * * * *");
        }
    }
}