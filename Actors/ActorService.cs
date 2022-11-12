using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Shinra.Actors.Character;
using Shinra.Clients;
using Shinra.Services;
using System;

namespace Shinra.Actors
{
    public static class ActorService
    {
        private static ActorSystem actorSystem;
        public static IActorRef CharacterSupervisor;
        public static void Configure(IServiceProvider provider)
        {
            actorSystem = ActorSystem.Create("ActorSystem");
            using (var scope = provider.CreateScope())
            {
                CharacterSupervisor = actorSystem.ActorOf(Props.Create<CharacterSupervisor>(
                    scope.ServiceProvider.GetRequiredService<BlizzardParserService>(),
                    scope.ServiceProvider.GetRequiredService<IBlizzardClient>(),
                    scope.ServiceProvider.GetRequiredService<IBlizzardDataAccess>()
                ), "character-supervisor");
            }
        }
    }
}
