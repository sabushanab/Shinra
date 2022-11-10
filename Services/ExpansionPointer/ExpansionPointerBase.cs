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
                var instanceName = instanceRegex.Match(instance.name).Groups?[2]?.Value;
                if (instanceName != null)
                {
                    var subCategory = new PointSubCategory(instanceName);
                    if (_pointableInstances.ContainsKey(instanceName))
                    {
                        var pointAmount = _pointableInstances[instanceName];
                        subCategory.Points = pointAmount;
                        pointCategory.TotalPoints += pointAmount;
                        pointCategory.SubCategories.Add(subCategory);
                    }
                }
            }
            return pointCategory;
        }
    }
}
