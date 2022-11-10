using Shinra.Clients.Models;
using Shinra.Services.Models;
using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class BFAPointer : ExpansionPointerBase, IExpansionPointer
    {
        public BFAPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Tol Dagor", 1 },
                { "The MOTHERLODE!!", 1 },
                { "Temple of Sethraliss", 1 },
                { "Waycrest Manor", 1 },
                { "Atal'Dazar", 1 },
                { "The Underrot", 1 },
                { "Freehold", 1 },
                { "Shrine of the Storm", 1 }
            };
            return pointableInstances;
        }
    }
}