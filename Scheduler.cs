using Hangfire;
using Shinra.Clients;
using System;

namespace Shinra
{
    public class Scheduler
    {
        public Scheduler() {}
        public static void Configure() 
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