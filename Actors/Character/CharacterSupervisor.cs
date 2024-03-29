using Akka.Actor;
using Shinra.Clients;
using Shinra.Messages;
using Shinra.Messages.Character;
using Shinra.Services;
using System.Collections.Generic;
namespace Shinra.Actors.Character
{
    public class CharacterSupervisor : ReceiveActor
    {
        private IActorRef _metricActor;
        readonly Queue<UpdateCharacterMessage> _queue1 = new Queue<UpdateCharacterMessage>();
        private readonly Queue<IActorRef> _availableWorkers;
        private readonly BlizzardParserService _service;
        private readonly IBlizzardClient _client;
        private readonly IBlizzardDataAccess _db;
        private int _numberOfWorkers = 2;
        protected override void PreStart()
        {
            _metricActor = Context.ActorOf(Props.Create<CharacterMetricsActor>());
            for (var i = 0; i < _numberOfWorkers; i++)
            {
                Context.ActorOf(Props.Create<CharacterWorker>(Self, _service, _client, _db, _metricActor));
            }
        }
        public CharacterSupervisor(BlizzardParserService service, IBlizzardClient client, IBlizzardDataAccess db)
        {
            _availableWorkers = new Queue<IActorRef>();
            _service = service;
            _client = client;
            _db = db;
            Receive<WorkerAvailable>(message => MakeWorkerAvailable(message));
            Receive<UpdateCharacterMessage>(message => UpdateCharacterData(message));
            Receive<GetQueueLength>(message => GetQueueLength());
            Receive<GetAvailableWorkers>(message => GetAvailableWorkers());
            Receive<CharacterMetricsRequest>(message => _metricActor.Forward(message));
        }
        void UpdateCharacterData(UpdateCharacterMessage message)
        {
            if (_availableWorkers.Count > 0)
            {
                _availableWorkers.Dequeue().Tell(message);
            }
            else
            {
                _queue1.Enqueue(message);
            }
        }
        private void MakeWorkerAvailable(WorkerAvailable message)
        {
            var nextmessage = GetNextCharacter();
            if (nextmessage != null)
            {
                Sender.Tell(nextmessage);
            }
            else
            {
                _availableWorkers.Enqueue(Sender);
            }
        }
        private UpdateCharacterMessage GetNextCharacter()
        {
            if (_queue1.Count > 0)
            {
                return _queue1.Dequeue();
            }
            return null;
        }
        public bool IsWorkAvailable
        {
            get { return _queue1.Count > 0; }
        }
        void GetQueueLength()
        {
            Sender.Tell($"Queue Length: {_queue1.Count}  Workers Available {_availableWorkers.Count}");
        }
        void GetAvailableWorkers()
        {
            Sender.Tell(_availableWorkers.Count);
        }
    }
}