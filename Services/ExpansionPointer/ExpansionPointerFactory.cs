using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class ExpansionPointerFactory
    {
        private static Dictionary<string, IExpansionPointer> _expansions = new Dictionary<string, IExpansionPointer>(StringComparer.OrdinalIgnoreCase)
        {
            { "Classic", new ClassicPointer() },
            { "The Burning Crusade", new BurningCrusadePointer() },
            { "Wrath of the Lich King", new LichKingPointer() },
            { "Cataclysm", new CataclysmPointer() },
            { "Mists of Pandaria", new MistsPointer() },
            { "Warlords of Draenor", new WarlordsPointer() },
            { "Legion", new LegionPointer() },
            { "Battle for Azeroth", new BFAPointer() },
            { "Shadowlands", new ShadowlandsPointer() }
        };
        public ExpansionPointerFactory() { }
        public IExpansionPointer GetInstance(string expansion)
        {
            return _expansions.ContainsKey(expansion) ? _expansions[expansion] : _expansions["classic"];
        }
    }
}
