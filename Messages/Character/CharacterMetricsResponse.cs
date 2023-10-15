using System;

namespace Shinra.Messages.Character
{
    public class CharacterMetricsResponse
    {
        public long CharactersProcessed;
        public TimeSpan TotalProfileFetchTime;
        public TimeSpan TotalStatisticsFetchTime;
        public TimeSpan TotalMythicPlusFetchTime;
        public TimeSpan TotalCharacterSaveTime;
        public long TotalExceptions;
        public CharacterMetric LastMetricProcessed;
        public CharacterMetricsResponse(long charactersProcessed, TimeSpan totalProfileFetchTime, TimeSpan totalStatisticsFetchTime, TimeSpan totalMythicPlusFetchTime, 
            TimeSpan totalCharacterSaveTime, long totalExceptions, CharacterMetric lastMetricProcessed) 
        { 
            CharactersProcessed = charactersProcessed;
            TotalProfileFetchTime = totalProfileFetchTime;
            TotalStatisticsFetchTime = totalStatisticsFetchTime;
            TotalMythicPlusFetchTime = totalMythicPlusFetchTime;
            TotalCharacterSaveTime = totalCharacterSaveTime;
            TotalExceptions = totalExceptions;
            LastMetricProcessed = lastMetricProcessed;
        }
    }
}
