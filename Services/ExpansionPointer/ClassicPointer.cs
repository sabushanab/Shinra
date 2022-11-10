using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class ClassicPointer : ExpansionPointerBase, IExpansionPointer
    {
        public ClassicPointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            return pointableInstances;
        }
    }
}
