using System;

namespace Shinra.Messages.Character
{
    public class CharacterMetric
    {
        public string _id { get; private set; }
        public TimeSpan? ProfileFetchTime;
        public TimeSpan StatisticsFetchTime;
        public TimeSpan? MythicPlusFetchTime;
        public TimeSpan CharacterSaveTime;
        public string LastError;

        public CharacterMetric(string id, TimeSpan? profileFetchTime, TimeSpan statisticsFetchTime, TimeSpan? mythicPlusFetchTime, TimeSpan characterSaveTime, string lastError)
        {
            _id = id;
            ProfileFetchTime = profileFetchTime;
            StatisticsFetchTime = statisticsFetchTime;
            MythicPlusFetchTime = mythicPlusFetchTime;
            CharacterSaveTime = characterSaveTime;
            LastError = lastError;
        }
    }
}
