using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.RPGModule
{
    class NodeParent
    {
        Node actualNode;
        List<NodeParent> neighboorNode;
        public List<NodeParent> connectedNeighboor; //used to check for drawing connection lines between neighboor
        public List<NodeParent> GetNeightboor
        {
            get
            {
                return neighboorNode;
            }
        }
        public Node GetNode
        {
            get
            {
                return actualNode;
            }
        }

        public Reason CanUpgrade(int points, int level)
        {
            if (level < actualNode.GetLevelRequirement)
                return Reason.LevelRequirement;
            return actualNode.CanUpgrade(points);
        }
        public bool GetEnable { get { return actualNode.GetEnable; } }
        static ushort TotalID = 0;

        public readonly ushort ID;

        public static void ResetID()
        {
            TotalID = 0;
        } 

        public Vector2 menuPos;
        //comment to my future self as I will surely forget it : 
        // type var => var
        // is equal to 
        // type var {get{return var;}}
        public NodeType GetNodeType
        {
            get
            {
                return actualNode.GetNodeType;
            }
        }
        public int GetLevel
        {
            get
            {
                return actualNode.GetLevel;
            }
        }
        public int GetMaxLevel
        {
            get
            {
                return actualNode.GetMaxLevel;
            }
        }
        public int GetCostPerLevel
        {
            get
            {
                return actualNode.GetCostPerLevel;
            }
        }
        public int GetLevelRequirement
        {
            get
            {
                return actualNode.GetLevelRequirement;
            }
        }
        public bool GetUnlock
        {
            get
            {
                return actualNode.GetUnlock;
            }
        }
        public bool GetActivate
        {
            get
            {
                return actualNode.GetActivate;
            }
        }


        public NodeParent(Node node, Vector2 pos)
        {
            neighboorNode = new List<NodeParent>();
            connectedNeighboor = new List<NodeParent>();
            actualNode = node;
            menuPos = pos;
            ID = TotalID;
            TotalID++;
        }

        public bool CanBeDisable()
        {
            if (actualNode.GetNodeType == NodeType.Class || actualNode.GetNodeType == NodeType.Perk)
                return true;
            return false;
        }

        public void ToggleEnable()
        {
            actualNode.ToggleEnable();
        }


        public void Unlock()
        {
            actualNode.Unlock();
        }

        public void AddNeighboor(NodeParent neighboor)
        {
            neighboorNode.Add(neighboor);
            neighboor.AddNeighboorSimple(this);
        }

        public void AddNeighboorSimple(NodeParent neighboor)
        {
            neighboorNode.Add(neighboor);
        }

        public void Upgrade(bool loading = false)
        {
            if (loading && actualNode.GetNodeType == NodeType.Class)
            {
                (actualNode as ClassNode).loadingUpgrade();
            }
            else
                actualNode.Upgrade();

            for (int i = 0; i < neighboorNode.Count; i++)
            {
                neighboorNode[i].Unlock();
            }
        }
    }
}
