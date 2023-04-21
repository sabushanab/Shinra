using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class ClassicPointer : ExpansionPointerBase, IExpansionPointer
    {
        public ClassicPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Ragefire Chasm", 1 },
                { "Wailing Caverns", 1 },
                { "Deadmines", 1 },
                { "Shadowfang Keep", 1 },
                { "Stormwind Stockade", 1 },
                { "Blackfathom Deeps", 1 },
                { "Gnomeregan", 1 },
                { "Razorfen Kraul", 1 },
                { "Scarlet Monastery", 1 },
                { "Scarlet Halls", 1 },
                { "Razorfen Downs", 1 },
                { "Uldaman", 1 },
                { "Zul'Farrak", 1 },
                { "Lower Blackrock Spire", 1 },
                { "Stratholme - Service Entrance", 1 },
                { "Sunken Temple", 1 },
                { "Scholomance", 1 },
                { "Stratholme - Main Gate", 1 },
                { "BlackRock Depths - Detention Center", 1 },
                { "BlackRock Depths - Upper City", 1 },
                { "Dire Maul - Capital Gardens", 1 },
                { "Dire Maul - Gordok Commons", 1 },
                { "Dire Maul - Warpwood Quarter", 1 },
                { "Maraudon", 1 },
                { "Maraudon - Earth Song Falls", 1 },
                { "Maraudon - Foulspore Cavern", 1 },
                { "Maraudon - The Wicked Quarter", 1 }
            };
            return pointableInstances;
        }
    }
}