using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class WarlordsPointer : ExpansionPointerBase, IExpansionPointer
    {
        public WarlordsPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Auchindoun", 1 },
                { "Bloodmaul Slag Mines", 1 },
                { "The Everbloom", 1 },
                { "Grimrail Depot", 1 },
                { "Iron Docks", 1 },
                { "Shadowmoon Burial Grounds", 1 },
                { "Skyreach", 1 },
                { "Upper Blackrock Spire", 1 }
            };
            return pointableInstances;
        }
    }
}