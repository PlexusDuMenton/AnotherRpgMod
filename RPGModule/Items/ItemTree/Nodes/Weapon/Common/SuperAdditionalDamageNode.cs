using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Utils;
using Terraria;

namespace AnotherRpgMod.Items
{
    class SuperAdditionalDamageNode : ItemNode
    {
        new protected string m_Name = "(Rare) Additional Damage";
        new protected string m_Desc = "Add";
        new public float rarityWeight = 0.2f;
        
        public override NodeCategory GetNodeCategory 
        {
            get
            {
                
                return NodeCategory.Flat;
            }
        }

        public override string GetName
        {
            get
            {
                return m_Name;
            }
        }

        public override string GetDesc { get {
                return "Add " + (FlatDamage * Utils.Mathf.Clamp(GetLevel,1,GetMaxLevel)) + " Damage";
            } }

        

        public int FlatDamage;

        public override void Passive(Item item)
        {
            item.GetGlobalItem<ItemUpdate>().DamageFlatBuffer += FlatDamage * GetLevel;
        }

        public override void SetPower(float value)
        {
            FlatDamage = Utils.Mathf.Clamp((int)(value*2),2,999);
            m_MaxLevel = 1;
            m_RequiredPoints = 2 + Utils.Mathf.FloorInt(value * 0.2f);
            power = value;
        }

        public override void LoadValue(string saveValue)
        {
            power = saveValue.SafeFloatParse();
            SetPower(power);
        }

        public override string GetSaveValue()
        {
            return power.ToString();
        }

    }
}
