using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AnotherRpgMod.Items
{
    class AdditionalDefenceNode : ItemNode
    {
        new protected string m_Name = "Additional Defence";
        new protected string m_Desc = "Add";


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
                return "Add " + (FlatDef * Utils.Mathf.Clamp(GetLevel,1,GetMaxLevel)) + " Defences";
            } }

        

        public int FlatDef;

        public override void Passive(Item item)
        {
            item.GetGlobalItem<ItemUpdate>().DefenceFlatBuffer += FlatDef * GetLevel;
        }

        public override void SetPower(float value)
        {
            FlatDef = Utils.Mathf.Clamp((int)value,1,999);
        }

        public override void LoadValue(string saveValue)
        {
            power = float.Parse(saveValue);
            SetPower(power);
        }

        public override string GetSaveValue()
        {
            return FlatDef.ToString();
        }

    }
}
