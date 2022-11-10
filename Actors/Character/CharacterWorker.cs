using Akka.Actor;
using Akka.Event;
using Shinra.Clients;
using Shinra.Messages;
using Shinra.Messages.Character;
using Shinra.Services;
using System;
using System.Threading.Tasks;

namespace Shinra.Actors.Character
{
    public class CharacterWorker : ReceiveActor
    {
        private UpdateCharacterMessage _characterMessage;
        private readonly IActorRef _supervisor;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly BlizzardParserService _service;
        public CharacterWorker(IActorRef supervisorRef, BlizzardParserService service)
        {
            _supervisor = supervisorRef;
            _service = service;
            BecomeReady();
        }
        protected override void PreStart() { }
        private void Ready()
        {
            Receive<UpdateCharacterMessage>(message => ProcessNotifyCandidateMessage(message));
            ReceiveAny(message => _logger.Warning("Unhandled Message while ready {@message}", message));
        }
        private void BecomeReady()
        {
            Become(Ready);
            _supervisor.Tell(new WorkerAvailable());
        }
        protected override void PreRestart(Exception reason, object message)
        {
            UpdateCharacterMessage characterMessage = message as UpdateCharacterMessage;
            _logger.Error(reason, "Error processing character @CharacterID-@Realm", characterMessage.CharacterName, characterMessage.Realm);
            base.PreRestart(reason, message);
        }

        private void Busy()
        {
            Receive<CharacterUpdated>(message => {
                CacheService.Set(message.Statistics.character.name, message.Statistics, 90);
                BecomeReady();
            });
            Receive<FailureMessage>(message => BecomeReady());
            ReceiveAny(message => _logger.Warning("Unhandled Message while busy {@message}", message));
        }

        void ProcessNotifyCandidateMessage(UpdateCharacterMessage message)
        {
            Become(Busy);
            _characterMessage = message;
            _service.ParseCharacter(message.Realm, message.CharacterName).PipeTo(Self, 
                success: (successMessage) => new CharacterUpdated(successMessage),
                failure: (ex) => new FailureMessage(ex));
        }
    }
}