using Hangfire;
using Shinra.Clients;
using Shinra.Services;
using System;

namespace Shinra
{
    public class Scheduler
    {
        public Scheduler() {}
        public static void Configure() 
        {
            RecurringJob.RemoveIfExists(nameof(IBlizzardDataAccess.UpdateAllCharacterPoints));
            RecurringJob.AddOrUpdate(nameof(IBlizzardDataAccess.UpdateAllCharacterPoints), (IBlizzardDataAccess client) => client.UpdateAllCharacterPoints(),
            "*/20 * * * *");
        }
    }
}