using Shinra.Clients.Models;
using Shinra.Services.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shinra.Services.ExpansionPointer
{
    public class ExpansionPointerBase
    {
        private readonly Dictionary<string, double> _pointableInstances;
        private readonly bool _currentExpansion;
        private readonly List<string> _ignoreDifficulties = new List<string>() { "Heroic", "Mythic" };
        private readonly Regex instanceRegex = new Regex(@"\({1}(normal|heroic|mythic)?\s?(.+)\){1}", RegexOptions.IgnoreCase);
        public ExpansionPointerBase(Dictionary<string, double> availableInstances, bool currentExpansion)
        {
            _pointableInstances = availableInstances;
            _currentExpansion = currentExpansion;
        }
        public PointCategory GetPointsForExpansion(Category expansion)
        {
            var pointCategory = new PointCategory(expansion.name);
            foreach (var instance in expansion.statistics)
            {
                var groups = instanceRegex.Match(instance.name).Groups;
                var difficulty = groups[1].Value ?? "";
                var instanceName = groups[2].Value;
                var fullInstanceName = !string.IsNullOrEmpty(difficulty) ? $"{difficulty} {instanceName}" : $"{instanceName}";
                if (!_currentExpansion && _ignoreDifficulties.Contains(difficulty))
                {
                    continue;
                }
                else if (instanceName != null)
                {
                    if (_pointableInstances.ContainsKey(instanceName))
                    {
                        var pointAmount = _pointableInstances[instanceName];
                        if (!pointCategory.SubCategories.ContainsKey(instanceName))
                        {
                            pointCategory.SubCategories.Add(fullInstanceName, pointAmount);
                            pointCategory.TotalPoints += pointAmount;
                        }
                    }
                }
            }
            return pointCategory;
        }
    }
}
