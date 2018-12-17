using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class PerkNode : Node
    {
        Perk perk;
        public Perk GetPerk
        {
            get
            {
                return perk;
            }
        }
        public PerkNode(Perk _perk, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            perk = _perk;
        }
    }
}
