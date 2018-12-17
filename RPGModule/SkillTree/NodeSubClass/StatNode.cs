using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class StatNode : Node
    {
        Stat StatType;
        bool flat;
        public bool GetFlat
        {
            get
            {
                return flat;
            }
        }
        public Stat GetStatType
        {
            get
            {
                return StatType;
            }
        }
        public StatNode(Stat _statType, bool _flat, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            StatType = _statType;
            flat = _flat;
        }

        public float GetDamage()
        {
            return value * level;
        }
    }
}
