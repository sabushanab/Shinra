﻿using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class GetCharacterStatistics
    {
        public CharacterProfile Profile { get; set; }
        public CharacterStatistics Statistics { get; private set; }
        public GetCharacterStatistics(CharacterStatistics statistics, CharacterProfile profile)
        {
            Statistics = statistics;
            Profile = profile;
        }
    }
}
