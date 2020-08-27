using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Shinra.Actors.Character;
using Shinra.Clients;
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
            CharacterSupervisor = actorSystem.ActorOf(Props.Create<CharacterSupervisor>(provider.GetRequiredService<IXIVAPIClient>()), "character-supervisor");
        }
    }
}
