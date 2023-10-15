using Shinra.Clients;
using Shinra.Clients.Models;
using Shinra.Services.ExpansionPointer;
using Shinra.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class BlizzardParserService
    {
        private IBlizzardClient _client;
        private static ExpansionPointerFactory _expansionFactory = new ExpansionPointerFactory();
        public BlizzardParserService(IBlizzardClient client)
        {
            _client = client;
        }

        public async Task<PointContainer> ParseCharacter(string region, string realm, string characterName)
        {
            var statistics = await _client.GetCharacterStatistics(region, realm, characterName);
            if (statistics.character == null) 
            { 
                return new PointContainer(region, realm, characterName, 0, ""); 
            }
            var profile = await _client.GetCharacterProfile(region, realm, characterName);
            CharacterMythicPlusSeasonDetails mythicPlusDetails = null;
            if (profile.level == 70)
            {
                mythicPlusDetails = await _client.GetMythicPlusSeasonDetails(region, realm, characterName);
            }
            return ParseContainer(region, statistics, profile, mythicPlusDetails);
        }

        public PointContainer ParseCharacter(string region, CharacterStatistics statistics, CharacterProfile profile, CharacterMythicPlusSeasonDetails mythicPlusDetails = null)
        {
            return ParseContainer(region, statistics, profile, mythicPlusDetails);
        }

        public PointContainer ParseContainer(string region, CharacterStatistics statistics, CharacterProfile profile, CharacterMythicPlusSeasonDetails mythicPlusDetails = null)
        {
            var pointContainer = new PointContainer(region, statistics.character.realm.name, statistics.character.name, profile.level, profile.character_class.name);
            pointContainer.TotalPoints += profile.level;
            foreach (var category in statistics.categories)
            {
                switch (category.name)
                {
                    case string name when name.Equals("Dungeons & Raids", StringComparison.OrdinalIgnoreCase):
                        pointContainer = ParseExpansions(category, pointContainer);
                        break;
                    case string name when name.Equals("Deaths", StringComparison.OrdinalIgnoreCase):
                        pointContainer = ParseDeaths(category, pointContainer);
                        break;
                }
            }
            pointContainer.MythicPlusScore = mythicPlusDetails?.mythic_rating?.rating ?? 0;
            pointContainer.TotalPoints += mythicPlusDetails?.mythic_rating?.rating ?? 0;

            return pointContainer;
        }

        public PointContainer ParseExpansions(Category category, PointContainer container)
        {
            var pointCategories = new List<PointCategory>();
            foreach(var subCategory in category.sub_categories)
            {
                IExpansionPointer pointer = _expansionFactory.GetInstance(subCategory.name);
                var pointCategory = pointer.GetPointsForExpansion(subCategory);
                if (pointCategory.TotalPoints > 0)
                {
                    pointCategories.Add(pointCategory);
                    container.TotalPoints += pointCategory.TotalPoints;
                }
            }
            container.Categories.AddRange(pointCategories);
            return container;
        }

        public PointContainer ParseDeaths(Category category, PointContainer container)
        {
            foreach(var statistic in category.statistics)
            {
                switch(statistic.name)
                {
                    case string name when name.Equals("Total deaths", StringComparison.OrdinalIgnoreCase):
                        container.HasDied = statistic.quantity > 0;
                        return container;
                }
            }

            return container;
        }
    }
}
