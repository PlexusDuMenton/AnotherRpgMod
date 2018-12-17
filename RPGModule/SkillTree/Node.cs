using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;


namespace AnotherRpgMod.RPGModule
{
    class Node
    {
        protected NodeType Type;


        protected int level = 0;
        protected NodeParent Parent;

        protected int maxLevel = 1;
        protected int pointsPerLevel = 1;
        protected float value = 0;
        protected int levelRequirement = 0;
        protected bool unlocked = false;

        public NodeType GetNodeType
        {
            get
            {
                return Type;
            }
        }
        public NodeParent GetParent { get { return Parent; } }
        public void SetParrent(NodeParent Parent)
        {
            this.Parent = Parent;
        }
        public int GetLevel
        {
            get
            {
                return level;
            }
        }
        public float GetValue
        {
            get
            {
                return value;
            }
        }
        public int GetMaxLevel
        {
            get
            {
                return maxLevel;
            }
        }
        public int GetCostPerLevel
        {
            get
            {
                return pointsPerLevel;
            }
        }
        public int GetLevelRequirement
        {
            get
            {
                return levelRequirement;
            }
        }
        public bool GetUnlock
        {
            get
            {
                return unlocked;
            }
        }
        public bool GetActivate
        {
            get
            {
                return activate;
            }
        }
        public bool GetEnable
        {
            get
            {
                return enable;
            }
        }



        protected bool activate = false;

        protected bool enable = false;

        public virtual void ToggleEnable()
        {
            enable = !enable;
        }

        public Node(NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1)
        {

            Type = _type;
            value = _value;
            maxLevel = _maxLevel;
            pointsPerLevel = _pointsPerLevel;
            activate = false;
            levelRequirement = _levelrequirement;
            unlocked = _unlocked;

        }

        public Reason CanUpgrade(int points)
        {
            if (points < pointsPerLevel)
                return Reason.NoEnoughtPoints;
            if (level >= maxLevel)
                return Reason.MaxLevelReach;
            if (!unlocked)
                return Reason.NotUnlocked;
            return Reason.CanUpgrade;
        }

        public virtual void Upgrade()
        {
            if (!activate)
            {
                activate = true;
                enable = true;
            }

            level++;
        }

        public virtual void Unlock()
        {
            unlocked = true;
        }

    }
}
