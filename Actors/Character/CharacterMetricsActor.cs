using Akka.Actor;
using Shinra.Messages.Character;
using System;

namespace Shinra.Actors.Character
{
    public class CharacterMetricsActor : ReceiveActor
    {
        private long CharactersProcessed;
        private TimeSpan TotalProfileFetchTime;
        private TimeSpan TotalStatisticsFetchTime;
        private TimeSpan TotalMythicPlusFetchTime;
        private TimeSpan TotalCharacterSaveTime;
        private long TotalExceptions;
        private CharacterMetric LastMetricProcessed;

        public CharacterMetricsActor()
        {
            Receive<CharacterMetric>(message => ParseMetricMessage(message));
            Receive<CharacterMetricsRequest>(message => SendMetricsResponse());
        }

        private void ParseMetricMessage(CharacterMetric message)
        {
            if (!string.IsNullOrEmpty(message.LastError))
            {
                TotalExceptions++;
            }
            else
            {
                CharactersProcessed++;
                if (message.ProfileFetchTime.HasValue)
                {
                    TotalProfileFetchTime += message.ProfileFetchTime.Value;
                }
                TotalStatisticsFetchTime += message.StatisticsFetchTime;
                if (message.MythicPlusFetchTime.HasValue)
                {
                    TotalMythicPlusFetchTime += message.MythicPlusFetchTime.Value;
                }
                TotalCharacterSaveTime += message.CharacterSaveTime;
            }
            LastMetricProcessed = message;
        }

        private void SendMetricsResponse()
        {
            Sender.Tell(new CharacterMetricsResponse(CharactersProcessed, TotalProfileFetchTime, TotalStatisticsFetchTime, 
                TotalMythicPlusFetchTime, TotalCharacterSaveTime, TotalExceptions, LastMetricProcessed));
        }
    }
}
