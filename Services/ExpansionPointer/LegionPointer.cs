using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class LegionPointer : ExpansionPointerBase, IExpansionPointer
    {
        public LegionPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Halls of Valor", 1 },
                { "Maw of Souls", 1 },
                { "Neltharion's Lair", 1 },
                { "Vault of the Wardens", 1 },
                { "Violet Hold", 1 },
                { "Eye of Azshara", 1 },
                { "Darkheart Thicket", 1 },
                { "Black Rook Hold", 1 }
            };
            return pointableInstances;
        }
    }
}
