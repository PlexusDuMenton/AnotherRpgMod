using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;
namespace AnotherRpgMod.RPGModule
{
    class DamageNode : Node
    {
        DamageType damageType;
        bool flat;
        public bool GetFlat
        {
            get
            {
                return flat;
            }
        }
        public DamageType GetDamageType
        {
            get
            {
                return damageType;
            }
        }
        public DamageNode(DamageType _damageType, bool _flat, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            damageType = _damageType;
            flat = _flat;
        }

        public float GetDamage()
        {
            return value * level;
        }
    }
}
