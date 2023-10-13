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
            Receive<GetCharacterProfile>(message => ProcessGetCharacterProfileMessage(message));
            Receive<GetCharacterStatistics>(message => ProcessGetCharacterStatisticsMessage(message));
            Receive<GetMythicPlusScore>(message => ProcessCharacterPointsWithMythic(message));
            Receive<CharacterUpdated>(message => BecomeReady());
            Receive<FailureMessage>(message => BecomeReady());
            ReceiveAny(message => _logger.Warning("Unhandled Message while busy {@message}", message));
        }

        void ProcessUpdateCharacterMessage(UpdateCharacterMessage message)
        {
            Become(Busy);
            _characterMessage = message;
            if (message.Level == 70)
            {
                Self.Tell(new GetCharacterProfile(new Clients.Models.CharacterProfile()
                {
                    character_class = new Clients.Models.CharacterClass() { name = message.CharacterClass },
                    name = message.CharacterName,
                    level = message.Level.Value
                }));
            }
            else
            {
                _client.GetCharacterProfile(_characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                    success: (successMessage) => new GetCharacterProfile(successMessage),
                    failure: (ex) => new FailureMessage(ex));
            }
        }

        void ProcessGetCharacterProfileMessage(GetCharacterProfile message)
        {
            _client.GetCharacterStatistics(_characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                success: (successMessage) => new GetCharacterStatistics(successMessage, message.Profile),
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessGetCharacterStatisticsMessage(GetCharacterStatistics message)
        {
            if (_characterMessage.Level == 70)
            {
                _client.GetMythicPlusSeasonDetails(_characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                    success: (successMessage) => new GetMythicPlusScore(successMessage, message.Statistics, message.Profile),
                    failure: (ex) => new FailureMessage(ex));
            }
            else
            {
                ProcessCharacterPoints(message);
            }
        }

        void ProcessCharacterPoints(GetCharacterStatistics message)
        {
            var pointContainer = _service.ParseCharacter(message.Statistics, message.Profile);
            _db.SaveCharacterPoints(pointContainer).PipeTo(Self,
                success: (successMessage) => new CharacterUpdated(successMessage),
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessCharacterPointsWithMythic(GetMythicPlusScore message)
        {
            var pointContainer = _service.ParseCharacter(message.Statistics, message.Profile, message.MythicScore);
            _db.SaveCharacterPoints(pointContainer).PipeTo(Self,
                success: (successMessage) => new CharacterUpdated(successMessage),
                failure: (ex) => new FailureMessage(ex));
        }
    }
}