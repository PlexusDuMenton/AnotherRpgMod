using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class NodeList //prefer to make this class to stock in different set each type of node to speedup
                   //looking for the entire tree would take lot of time
    {
        public List<NodeParent> nodeList;

        private List<DamageNode> meleeDamage;
        private List<DamageNode> rangedDamage;
        private List<DamageNode> magicDamage;
        private List<DamageNode> summonDamage;
        private List<DamageNode> throwDamage;

        private List<StatNode> StatsNodes;
        private List<LimitBreakNode> LBNodes;

        private List<SpeedNode> meleeSpeed;
        private List<SpeedNode> rangedSpeed;
        private List<SpeedNode> magicSpeed;

        private List<LeechNode> leechs;
        public List<LeechNode> GetLeech
        {
            get
            {
                return leechs;
            }
        }

        private List<ClassNode> classes;
        private List<PerkNode> perks;
        private List<ImmunityNode> immunities;

        public NodeList()
        {
            nodeList = new List<NodeParent>();

            meleeDamage = new List<DamageNode>();
            rangedDamage = new List<DamageNode>();
            magicDamage = new List<DamageNode>();
            summonDamage = new List<DamageNode>();
            throwDamage = new List<DamageNode>();

            meleeSpeed = new List<SpeedNode>();
            rangedSpeed = new List<SpeedNode>();
            magicSpeed = new List<SpeedNode>();
            leechs = new List<LeechNode>();

            StatsNodes = new List<StatNode>();
            LBNodes = new List<LimitBreakNode>();

            classes = new List<ClassNode>();
            perks = new List<PerkNode>();
            immunities = new List<ImmunityNode>();

        }

        public List<StatNode> GetStatsList
        {
            get
            {
                return StatsNodes;
            }
        }

        public List<LimitBreakNode> GetLBList
        {
            get
            {
                return LBNodes;
            }
        }

        public List<ClassNode> GetClasses
        {
            get
            {
                return classes;
            }
        }
        public List<PerkNode> GetPerks
        {
            get
            {
                return perks;
            }
        }

        public PerkNode GetPerk(Perk perk)
        {
            foreach(PerkNode Pnode in perks)
            {
                if (Pnode.GetPerk == perk)
                    return Pnode;
            }
            return null;
        }

        public List<ImmunityNode> GetImmunities
        {
            get
            {
                return immunities;
            }
        }

        public List<DamageNode> GetDamageList(DamageType _type)
        {
            switch (_type)
            {
                case DamageType.Magic:
                    return magicDamage;
                case DamageType.Melee:
                    return meleeDamage;
                case DamageType.Ranged:
                    return rangedDamage;
                case DamageType.Throw:
                    return throwDamage;
                default:
                    return summonDamage;
            }
        }

        public void AddNode(DamageNode node)
        {
            NodeParent nodeParent = new NodeParent(node, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            switch (node.GetDamageType)
            {
                case DamageType.Magic:
                    magicDamage.Add(node);
                    break;
                case DamageType.Melee:
                    meleeDamage.Add(node);
                    break;
                case DamageType.Ranged:
                    rangedDamage.Add(node);
                    break;
                case DamageType.Throw:
                    throwDamage.Add(node);
                    break;
                default:
                    summonDamage.Add(node);
                    break;
            }
        }

        public void AddNode(SpeedNode node)
        {
            NodeParent nodeParent = new NodeParent(node, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            switch (node.GetDamageType)
            {
                case DamageType.Magic:
                    magicSpeed.Add(node);
                    break;
                case DamageType.Melee:
                    meleeSpeed.Add(node);
                    break;
                case DamageType.Ranged:
                    rangedSpeed.Add(node);
                    break;
                default:
                    return;
            }
        }

        public void AddNode(PerkNode perk)
        {
            NodeParent nodeParent = new NodeParent(perk, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            perks.Add(perk);
        }

        public void AddNode(LeechNode leech)
        {
            NodeParent nodeParent = new NodeParent(leech, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            leechs.Add(leech);
        }

        public void AddNode(ImmunityNode immunity)
        {
            NodeParent nodeParent = new NodeParent(immunity, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            immunities.Add(immunity);
        }

        public void AddNode(ClassNode classnode)
        {
            NodeParent nodeParent = new NodeParent(classnode, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            classes.Add(classnode);
        }
        public void AddNode(StatNode statnode)
        {
            NodeParent nodeParent = new NodeParent(statnode, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            StatsNodes.Add(statnode);
        }

        public void AddNode(LimitBreakNode LBnode)
        {
            NodeParent nodeParent = new NodeParent(LBnode, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            LBNodes.Add(LBnode);
        }

    }

}
