using Shinra.Clients.Models;
using Shinra.Services.Models;
using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class ExpansionPointerFactory
    {
        private static Dictionary<string, IExpansionPointer> _expansions = new Dictionary<string, IExpansionPointer>(StringComparer.OrdinalIgnoreCase)
        {
            { "Legion", new LegionPointer() },
            { "Battle for Azeroth", new BFAPointer() },
            { "Classic", new ClassicPointer() }
        };
        public ExpansionPointerFactory() { }
        public IExpansionPointer GetInstance(string expansion)
        {
            return _expansions.ContainsKey(expansion) ? _expansions[expansion] : _expansions["classic"];
        }
    }
}
