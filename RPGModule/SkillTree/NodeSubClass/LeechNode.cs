using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class LeechNode : Node
    {
        LeechType leechType;
        public LeechType GetLeechType
        {
            get
            {
                return leechType;
            }
        }
        public LeechNode(LeechType _leechType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            leechType = _leechType;
        }

        public float GetLeech()
        {
            return value * level;
        }
    }
}
