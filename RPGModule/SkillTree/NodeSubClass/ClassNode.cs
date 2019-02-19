using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;
namespace AnotherRpgMod.RPGModule
{
    class ClassNode : Node
    {
        ClassType classType;
        public ClassType GetClassType
        {
            get
            {
                return classType;
            }
        }
        public ClassNode(ClassType _classType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            classType = _classType;
        }

        public void loadingUpgrade()
        {
            base.Upgrade();
        }

        public override void Upgrade()
        {
            base.Upgrade();
            UpdateClass();
        }

        public void Disable(RPGPlayer p)
        {
            if (p.GetskillTree.ActiveClass == this)
                p.GetskillTree.ActiveClass = null;
            enable = false;
        }

        public void Disable()
        {
            if (Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass == this)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            enable = false;
        }

        public void UpdateClass()
        {
            NodeParent _node;
            ClassType Active = ClassType.Hobo;
            if (Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass != null)
                Active = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass.GetClassType;
            for (int i = 0; i < Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList.Count; i++)
            {
                _node = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (Active != classNode.GetClassType && classNode.GetActivate)
                    {
                        classNode.Disable();
                    }

                }
            }
        }

        public override void ToggleEnable()
        {
            base.ToggleEnable();
            
            if (enable)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = this;
            else
            {
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            }
            
            RPGPlayer player = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            

            UpdateClass();
            if (Main.netMode == 1)
                player.SendClientChanges(player);
        }


    }
}
