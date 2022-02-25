using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AnotherRpgMod.Utils;
using Terraria;

namespace AnotherRpgMod.Items
{
    class MagicCostReduction : ItemNode
    {

        new protected string m_Name = "Manacost Reduction";
        new protected string m_Desc = "+ X Projectile";
        new protected NodeCategory m_NodeCategory = NodeCategory.Other;
        new public float rarityWeight = 0.4f;



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
                return "- " + (manaCostReduction*100 *Utils.Mathf.Clamp(GetLevel, 1, GetMaxLevel)) + "% Manacost";
            }
        }

        protected new bool m_isAscend = true;
        public float manaCostReduction;

        public override void PlayerPassive(Item item, Player Player)
        {
            Player.manaCost *= 1 - manaCostReduction* GetLevel;
        }

        public override void SetPower(float value)
        {
            manaCostReduction = Utils.Mathf.Clamp(Utils.Mathf.Round(value * 0.01f, 2), 0.01f, 0.5f);
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
