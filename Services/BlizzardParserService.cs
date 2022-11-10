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
        public async Task<PointContainer> ParseCharacter(string realm, string characterName)
        {
            var statistics = await _client.GetCharacterStatistics(realm, characterName);
            var profile = await _client.GetCharacterProfile(realm, characterName);
            var pointContainer = new PointContainer(realm, characterName, profile.level, profile.character_class.name);
            pointContainer.TotalPoints += profile.level;
            foreach(var category in statistics.categories)
            {
                switch(category.name)
                {
                    case string name when name.Equals("Dungeons & Raids", StringComparison.OrdinalIgnoreCase):
                        pointContainer = ParseExpansions(category, pointContainer);
                        break;
                }
            }

            return pointContainer;
        }

        public PointContainer ParseCharacter(CharacterStatistics statistics)
        {
            return new PointContainer("realmTest", "nameTest", 0, "classTest");
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
    }
}
