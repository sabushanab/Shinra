using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class CataclysmPointer : ExpansionPointerBase, IExpansionPointer
    {
        public CataclysmPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Blackrock Caverns", 1 },
                { "Grim Batol", 1 },
                { "Halls of Origination", 1 },
                { "Lost City of the Tol'vir", 1 },
                { "The Stonecore", 1 },
                { "The Vortex Pinnacle", 1 },
                { "Throne of the Tides", 1 }
            };
            return pointableInstances;
        }
    }
}