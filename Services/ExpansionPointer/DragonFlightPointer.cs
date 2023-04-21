using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class DragonFlightPointer : ExpansionPointerBase, IExpansionPointer
    {
        public DragonFlightPointer() : base(BuildPointableInstances(), true) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Algeth'ar Academy", 1 },
                { "Brackenhide Hollow", 1 },
                { "Halls of Infusion", 1 },
                { "Neltharus", 1 },
                { "Ruby Life Pools", 1 },
                { "The Azure Vault", 1 },
                { "The Nokhud Offensive", 1 },
                { "Uldaman: Legacy of Tyr", 1 }
            };
            return pointableInstances;
        }
    }
}