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

            NodeParent _node;
            if (Main.myPlayer >= Main.player.Length)
                return;
            RPGPlayer player = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            player.GetskillTree.ActiveClass = this;

            for (int i = 0; i < player.GetskillTree.nodeList.nodeList.Count; i++)
            {
                _node = player.GetskillTree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (classNode.GetClassType != classType && classNode.GetActivate)
                    {
                        classNode.Disable();
                    }
                }
            }
        }

        public void Disable()
        {
            if (Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass == this)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            enable = false;
        }

        public override void ToggleEnable()
        {
            base.ToggleEnable();
            NodeParent _node;
            if (enable)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = this;
            else
            {
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            }
            for (int i = 0; i < Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList.Count; i++)
            {
                _node = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (classNode.GetClassType != classType && classNode.GetActivate)
                    {
                        classNode.Disable();
                    }

                }
            }
        }
    }
}
