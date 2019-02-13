using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.Utils;
using Terraria.ModLoader;
namespace AnotherRpgMod.Items
{

    //What to save : 
    //[Position,{ID,{Neighboor:1},state,level,maxlevel,{specific node values:4:5}]
    //Convert to Json, to save
    class ItemNode
    {
        protected int m_ID;
        protected List<int> m_ID_Neightboor;
        protected ItemSkillTree m_SkillTreeParent;

        public List<int> GetNeighboor { get { return m_ID_Neightboor; } }
        public ItemSkillTree GetParent { get { return m_SkillTreeParent; } }
        public int GetId { get { return m_ID; } }


        protected NodeCategory m_NodeCategory = NodeCategory.Other;
        virtual public NodeCategory GetNodeCategory
        {
            get
            {
                return m_NodeCategory;
            }
        }

        protected int m_RequiredPoints;
        protected bool m_isAscend = false;

        public bool IsAscend { get { return m_isAscend; } }
        public int GetRequiredPoints { get { return m_RequiredPoints; } }

        protected string m_Name = "Blank";
        protected string m_Desc = "Blank Node used to make more node, if you see that, report it :p";
        
        virtual public string GetName { get { return m_Name; } }
        virtual public string GetDesc { get { return m_Desc; } }

        protected int m_MaxLevel;
        protected int m_Level;

        public int GetLevel { get { return m_Level; } }
        public int GetMaxLevel { get { return m_MaxLevel; } }

        public int SetLevel { set { m_Level = Mathf.Clamp(value,0,m_MaxLevel); } }
        public int SetMaxLevel { set { m_MaxLevel = Mathf.Clamp(value, 0, int.MaxValue); } }
        public int SetRequired { set { m_RequiredPoints = Mathf.Clamp(value, 1, int.MaxValue); } }

        public virtual void SetPower(float value)
        {

        }

        public void SetPos (float posx, float posy)
        {
            m_position = new Vector2(posx, posy);
        }

        //0 = invisible, 1 = unknow, 2 = locked, 3 = unlocked, 4 = active;
        protected int m_LockState;

        public int GetState
        {
            get{ return m_LockState; }
        }

        protected Vector2 m_position;
        public Vector2 GetPos { get { return m_position; } }


        virtual public void Reset()
        {
            m_Level = 0;
            if (m_LockState >1)
                m_LockState = 2;
        }

        virtual public string GetSaveValue()
        {
            return "";
        }
        virtual public void LoadValue(string saveValue)
        {

        }

        virtual public void ShareNeightboor()
        {
            foreach (int Node in m_ID_Neightboor)
            {
                m_SkillTreeParent.GetNode(Node).AddNeightboor(m_ID);
            }
        }
        virtual public void AddNeightboor(int ID)
        {
            if (!m_ID_Neightboor.Contains(ID))
                m_ID_Neightboor.Add(ID);
        }


        public void ForceLockNode(int lockValue)
        {
            m_LockState = lockValue;
        }

        static int MININT = 0;

        virtual public void UnlockStep (int step)
        {
            if (m_LockState < Mathf.Clamp(step, MININT, 3))
                m_LockState = Mathf.Clamp(step, MININT, 3);
            step -= 1;
            if (step > 0)
                foreach (int ID in m_ID_Neightboor)
                {
                    m_SkillTreeParent.GetNode(ID).UnlockStep(step);
                }
        }

        virtual public void Activate()
        {
            m_LockState = 4;
            m_Level = 0;
        }

        virtual public ItemReason CanAddLevel(int Points)
        {
            if (m_LockState < 3)
                return ItemReason.Locked;
            if (m_Level >= m_MaxLevel)
                return ItemReason.MaxLevel;
            if (Points < m_RequiredPoints)
                return ItemReason.NotEnougtPoint;
            return ItemReason.CanUpgrade;
        }
        virtual public void AddLevel()
        {
            if (m_LockState == 3)
            {
                Activate();
                UnlockStep(4);
            }
                
            m_Level++;
        }
        /// <summary>
        /// Passive : 
        /// Called each frame to modify weapons only attribute;
        /// </summary>
        /// <param name="item"></param>
        virtual public void Passive(Item item)
        {

        }

        /// <summary>
        /// Passive : 
        /// Called each frame to modify player linked attributes;
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        virtual public void PlayerPassive(Item item, Player player)
        {

        }

        //;ID,Neighboor1:2,state,level,maxlevel,required,posx:posy,specificvalue1:2:3:5:7;
        virtual public void Init(ItemSkillTree IST,int ID, int maxlevel,int required,Vector2 pos)
        {
            m_ID_Neightboor = new List<int>();
            m_SkillTreeParent = IST;
            m_ID = ID;
            m_Level= 0;
            m_LockState = MININT;
            m_MaxLevel = maxlevel;
            m_RequiredPoints = required;
            m_position = pos;
        }

        public ItemNode()
        {
            m_ID_Neightboor = new List<int>();
            m_SkillTreeParent = new ItemSkillTree();
        }

    }

    class ItemNodeAdvanced : ItemNode
    {

        virtual public void OnShoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

        }

        //virtual public void On
        
    }
}
