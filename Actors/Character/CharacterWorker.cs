using Akka.Actor;
using Akka.Event;
using Shinra.Clients;
using Shinra.Messages;
using Shinra.Messages.Character;
using Shinra.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Shinra.Actors.Character
{
    public class CharacterWorker : ReceiveActor
    {
        private UpdateCharacterMessage _characterMessage;
        private readonly IActorRef _supervisor;
        private readonly IActorRef _metricActor;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly BlizzardParserService _service;
        private readonly IBlizzardClient _client;
        private readonly IBlizzardDataAccess _db;
        private TimeSpan? _profileFetchTime;
        private TimeSpan _statisticsFetchTime;
        private TimeSpan? _mythicPlusFetchTime;
        private TimeSpan _characterSaveTime;
        private Stopwatch _stopWatch;
        private Exception? _exceptionRaised;

        public CharacterWorker(IActorRef supervisorRef, BlizzardParserService service, IBlizzardClient client, IBlizzardDataAccess db, IActorRef metricActor)
        {
            _supervisor = supervisorRef;
            _metricActor = metricActor;
            _service = service;
            _client = client;
            _db = db;
            BecomeReady();
        }
        protected override void PreStart() 
        {
            _stopWatch = new Stopwatch();
        }
        private void Ready()
        {
            Receive<UpdateCharacterMessage>(message => ProcessUpdateCharacterMessage(message));
            ReceiveAny(message => _logger.Warning("Unhandled Message while ready {@message}", message));
        }
        private void BecomeReady()
        {
            if (_characterMessage != null)
            {
                _metricActor.Tell(new CharacterMetric($"{_characterMessage.Region}-{_characterMessage.Realm}-{_characterMessage.CharacterName}", _profileFetchTime, _statisticsFetchTime, _mythicPlusFetchTime, _characterSaveTime, _exceptionRaised?.Message));
            }
            _profileFetchTime = null;
            _mythicPlusFetchTime = null;
            _exceptionRaised = null;
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
            Receive<FailureMessage>(message =>
            {
                _exceptionRaised = message.Exception;
                BecomeReady();
            });
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
                _stopWatch.Restart();
                _client.GetCharacterProfile(_characterMessage.Region, _characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                    success: (successMessage) => {
                        _stopWatch.Stop();
                        _profileFetchTime = _stopWatch.Elapsed;
                        return new GetCharacterProfile(successMessage);
                    },
                    failure: (ex) => new FailureMessage(ex));
            }
        }

        void ProcessGetCharacterProfileMessage(GetCharacterProfile message)
        {
            _stopWatch.Restart();
            _client.GetCharacterStatistics(_characterMessage.Region, _characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                success: (successMessage) => {
                    _stopWatch.Stop();
                    _statisticsFetchTime = _stopWatch.Elapsed;
                    return new GetCharacterStatistics(successMessage, message.Profile); 
                },
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessGetCharacterStatisticsMessage(GetCharacterStatistics message)
        {
            if (_characterMessage.Level == 70)
            {
                _stopWatch.Restart();
                _client.GetMythicPlusSeasonDetails(_characterMessage.Region, _characterMessage.Realm, _characterMessage.CharacterName).PipeTo(Self,
                    success: (successMessage) => {
                        _stopWatch.Stop();
                        _mythicPlusFetchTime = _stopWatch.Elapsed;
                        return new GetMythicPlusScore(successMessage, message.Statistics, message.Profile); 
                    },
                    failure: (ex) => new FailureMessage(ex));
            }
            else
            {
                ProcessCharacterPoints(message);
            }
        }

        void ProcessCharacterPoints(GetCharacterStatistics message)
        {
            var pointContainer = _service.ParseCharacter(_characterMessage.Region, message.Statistics, message.Profile);
            _stopWatch.Restart();
            _db.SaveCharacterPoints(pointContainer).PipeTo(Self,
                success: (successMessage) => {
                    _stopWatch.Stop();
                    _characterSaveTime = _stopWatch.Elapsed;
                    return new CharacterUpdated(successMessage); 
                },
                failure: (ex) => new FailureMessage(ex));
        }

        void ProcessCharacterPointsWithMythic(GetMythicPlusScore message)
        {
            var pointContainer = _service.ParseCharacter(_characterMessage.Region, message.Statistics, message.Profile, message.MythicScore);
            _stopWatch.Restart();
            _db.SaveCharacterPoints(pointContainer).PipeTo(Self,
                success: (successMessage) => {
                    _stopWatch.Stop();
                    _characterSaveTime = _stopWatch.Elapsed;
                    return new CharacterUpdated(successMessage);
                },
                failure: (ex) => new FailureMessage(ex));
        }
    }
}