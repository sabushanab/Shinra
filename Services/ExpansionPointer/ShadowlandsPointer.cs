using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class ShadowlandsPointer : ExpansionPointerBase, IExpansionPointer
    {
        public ShadowlandsPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "The Necrotic Wake", 1 },
                { "Plaguefall", 1 },
                { "Mists of Tirna Scithe", 1 },
                { "Halls of Atonement", 1 },
                { "Theater of Pain", 1 },
                { "De Other Side", 1 },
                { "Spires of Ascension", 1 },
                { "Sanguine Depths", 1 }
            };
            return pointableInstances;
        }
    }
}