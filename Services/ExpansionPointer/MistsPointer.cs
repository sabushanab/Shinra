using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class MistsPointer : ExpansionPointerBase, IExpansionPointer
    {
        public MistsPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Temple of the Jade Serpent", 1 },
                { "Stormstout Brewery", 1 },
                { "Shado-Pan Monastery", 1 },
                { "Mogu'shan Palace", 1 },
                { "Gate of the Setting Sun", 1 },
                { "Siege of Niuzao Temple", 1 }
            };
            return pointableInstances;
        }
    }
}