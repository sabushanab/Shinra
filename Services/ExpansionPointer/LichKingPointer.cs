using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class LichKingPointer : ExpansionPointerBase, IExpansionPointer
    {
        public LichKingPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Ahn'kahet: The Old Kingdom", 1 },
                { "Azjol-Nerub", 1 },
                { "Drak'Tharon Keep", 1 },
                { "Gundrak", 1 },
                { "Halls of Lightning", 1 },
                { "Halls of Reflection", 1 },
                { "Halls of Stone", 1 },
                { "Pit of Saron", 1 },
                { "The Culling of Stratholme", 1 },
                { "The Forge of Souls", 1 },
                { "The Nexus", 1 },
                { "The Oculus", 1 },
                { "The Violet Hold", 1 },
                { "Trial of the Champion", 1 },
                { "Utgarde Keep", 1 },
                { "Utgarde Pinnacle", 1 },
            };
            return pointableInstances;
        }
    }
}