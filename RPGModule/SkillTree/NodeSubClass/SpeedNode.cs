using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class SpeedNode : Node
    {
        DamageType damageType;
        public DamageType GetDamageType
        {
            get
            {
                return damageType;
            }
        }
        public SpeedNode(DamageType _damageType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1, bool _ascended = false) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel,_ascended)
        {
            damageType = _damageType;
        }

        public float GetSpeed()
        {
            return value * level;
        }
    }
}
