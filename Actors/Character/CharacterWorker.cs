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
        private readonly IBlizzardClient _client;
        private readonly IBlizzardDataAccess _db;
        public CharacterWorker(IActorRef supervisorRef, BlizzardParserService service, IBlizzardClient client, IBlizzardDataAccess db)
        {
            _supervisor = supervisorRef;
            _service = service;
            _client = client;
            _db = db;
            BecomeReady();
        }
        protected override void PreStart() { }
        private void Ready()
        {
            Receive<UpdateCharacterMessage>(message => ProcessUpdateCharacterMessage(message));
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
            Receive<GetCharacterStatistics>(message => ProcessGetCharacterStatisticsMessage(message));
            Receive<GetCharacterProfile>(message => ProcessGetCharacterProfileMessage(message));
            Receive<CharacterUpdated>(message => BecomeReady());
            Receive<FailureMessage>(message => BecomeReady());
            ReceiveAny(message => _logger.Warning("Unhandled Message while busy {@message}", message));
        }

        void ProcessUpdateCharacterMessage(UpdateCharacterMessage message)
        {
            Become(Busy);
            _characterMessage = message;
            _client.GetCharacterStatistics(message.Realm, message.CharacterName).PipeTo(Self, 
                success: (successMessage) => new GetCharacterStatistics(successMessage),
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessGetCharacterStatisticsMessage(GetCharacterStatistics message)
        {
            _client.GetCharacterProfile(_characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                success: (successMessage) => new GetCharacterProfile(message.Statistics, successMessage),
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessGetCharacterProfileMessage(GetCharacterProfile message)
        {
            var pointContainer = _service.ParseCharacter(message.Statistics, message.Profile);
            _db.SaveCharacterPoints(pointContainer).PipeTo(Self,
                success: (successMessage) => new CharacterUpdated(successMessage),
                failure: (ex) => new FailureMessage(ex));
        }
    }
}