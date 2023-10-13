using System;
using System.Collections.Generic;

namespace Shinra.Services.ExpansionPointer
{
    public class BurningCrusadePointer : ExpansionPointerBase, IExpansionPointer
    {
        public BurningCrusadePointer() : base(BuildPointableInstances(), false) { }
        private static Dictionary<string, double> BuildPointableInstances()
        {
            var pointableInstances = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "Hellfire Ramparts", 1 },
                { "The Blood Furnace", 1 },
                { "Slave Pens", 1 },
                { "The Underbog", 1 },
                { "Mana-Tombs", 1 },
                { "Auchenai Crypts", 1 },
                { "Old Hillsbrad Foothills", 1 },
                { "Sethekk Halls", 1 },
                { "The Steamvault", 1 },
                { "Shadow Labyrinth", 1 },
                { "The Shattered Halls", 1 },
                { "The Black Morass", 1 },
                { "The Botanica", 1 },
                { "The Mechanar", 1 },
                { "The Arcatraz", 1 },
                { "Magisters' Terrace", 1 },
            };
            return pointableInstances;
        }
    }
}