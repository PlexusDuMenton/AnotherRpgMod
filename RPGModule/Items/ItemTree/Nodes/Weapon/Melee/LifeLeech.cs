using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Utils;
using Terraria;

namespace AnotherRpgMod.Items
{
    class LifeLeech : ItemNode
    {

        new protected string m_Name = "Life Leech";
        new protected string m_Desc = "+ 0.X% Life Leech";
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

        public override string GetDesc
        {
            get
            {
                return "Restore " + (leech * Utils.Mathf.Clamp(GetLevel, 1, GetMaxLevel)) + "% Health each attack";
            }
        }

        public float leech;

        public override void Passive(Item item)
        {
            item.GetGlobalItem<ItemUpdate>().leech += (leech * GetLevel) * 0.01f;
        }

        public override void SetPower(float value)
        {
            leech = Utils.Mathf.Clamp(Utils.Mathf.Round(value*0.2f,2), 0.2f, 50);
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
