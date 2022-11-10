using Shinra.Clients.Models;
using Shinra.Services.Models;
using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class LegionPointer : ExpansionPointerBase, IExpansionPointer
    {
        public LegionPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            return pointableInstances;
        }
    }
}
