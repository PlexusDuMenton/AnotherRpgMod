using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using AnotherRpgMod.Utils;

namespace AnotherRpgMod.Items
{
    class AscendedAdditionalDamageNode : ItemNode
    {
        new protected string m_Name = "(Ascended) Additional Damage";
        new protected string m_Desc = "Add";
        new public float rarityWeight = 0.05f;
        new protected bool m_isAscend = true;

        public override bool IsAscend
        {
            get
            {
                return m_isAscend;
            }
        }

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
            FlatDamage = Utils.Mathf.Clamp((int)value*8,8,999);
            m_MaxLevel = 1;
            m_RequiredPoints = 1;
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
