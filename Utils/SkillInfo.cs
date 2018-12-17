
using AnotherRpgMod.RPGModule;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.Utils
{
    class SkillInfo
    {


        static public string GetClassDescription(ClassType classType)
        {
            JsonChrClass ClassInfo = JsonCharacterClass.GetJsonCharList.GetClass(classType);
            string desc = "";




            bool allDamage = true;
            float damage = ClassInfo.Damage[0];

            int id = 0;
            foreach (float d in ClassInfo.Damage)
            {

                id++;
                if (!allDamage || id > 4)
                    break;
                if (d != damage)
                    allDamage = false;
            }


            if (allDamage && ClassInfo.Damage[0] != 0)
                desc += "+ " + (ClassInfo.Damage[0] * 100) + "% All Damage\n";
            else
            {

                for (int i = 0; i < ClassInfo.Damage.Length; i++)
                {
                    if (ClassInfo.Damage[i] != 0)
                    {
                        if (ClassInfo.Damage[i] > 0)
                            desc += "+ " + (ClassInfo.Damage[i] * 100) + "% " + ((DamageNameTree)i).ToString() + " Damage\n";
                        else
                            desc += "- " + (-ClassInfo.Damage[i] * 100) + "% " + ((DamageNameTree)i).ToString() + " Damage\n";
                    }
                }
            }


            if (ClassInfo.Speed != 0)
            {
                if (ClassInfo.Speed > 0)
                    desc += "+ " + (ClassInfo.Speed * 100) + "% Melee Speed\n";
                else
                    desc += "- " + (-ClassInfo.Speed * 100) + "% Melee Speed\n";
            }

            if (ClassInfo.Health != 0)
            {
                if (ClassInfo.Health > 0)
                    desc += "+ " + (ClassInfo.Health * 100) + "% Health\n";
                else
                    desc += "- " + (-ClassInfo.Health * 100) + "% Health\n";
            }
            if (ClassInfo.Armor != 0)
            {
                if (ClassInfo.Armor > 0)
                    desc += "+ " + (ClassInfo.Armor * 100) + "% Armor\n";
                else
                    desc += "- " + (-ClassInfo.Armor * 100) + "% Armor\n";
            }
            if (ClassInfo.MovementSpeed != 0)
            {
                if (ClassInfo.MovementSpeed > 0)
                    desc += "+ " + (ClassInfo.MovementSpeed * 100) + "% Movement Speed\n";
                else
                    desc += "- " + (-ClassInfo.MovementSpeed * 100) + "% Movement Speed\n";
            }
            if (ClassInfo.Dodge != 0)
            {
                if (ClassInfo.Dodge != 0)
                    desc += "+ " + (ClassInfo.Dodge * 100) + "% Dodge\n";
                else
                    desc += "- " + (-ClassInfo.Dodge * 100) + "% Dodge\n";
            }
            if (ClassInfo.Ammo != 0)
            {
                if (ClassInfo.Ammo < 0)
                    desc += "+ " + (-ClassInfo.Ammo * 100) + "% Ammo consumation\n";
                else
                    desc += "- " + (ClassInfo.Ammo * 100) + "% Ammo consumation\n";
            }
            if (ClassInfo.Summons != 0)
            {
                if (ClassInfo.Summons < 0)
                    desc += "- " + (-ClassInfo.Summons) + " Max Summon\n";
                else
                    desc += "+ " + ClassInfo.Summons + " Max Summon\n";
            }
            if (ClassInfo.ManaCost != 0)
            {
                if (ClassInfo.ManaCost < 0)
                    desc += "- " + (-ClassInfo.ManaCost * 100) + " Mana Cost";
                else
                    desc += "+ " + (ClassInfo.ManaCost * 100) + " Mana Cost";
            }

            if (ClassInfo.ManaShield > 0 && (ClassInfo.ManaEfficiency > 0 || ClassInfo.ManaBaseEfficiency > 0))
            {
                int intelect = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetStat(Stat.Int);
                desc += "Grand Mana Shield : " + (ClassInfo.ManaShield * 100) +
                    "% damage are absorbed by mana (" + (ClassInfo.ManaBaseEfficiency + (intelect * ClassInfo.ManaEfficiency)) + " Damage per mana)";
            }

            return desc;
        }

        static public string GetDesc(Node node)
        {
            string desc = "";

            switch (node.GetNodeType)
            {
                case NodeType.Class:
                    desc += GetClassDescription((node as ClassNode).GetClassType);
                    break;
                case NodeType.Damage:
                    switch ((node as DamageNode).GetFlat)
                    {
                        case true:
                            if (node.GetLevel > 0)
                                desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel)) + " " + (node as DamageNode).GetDamageType + " Damage Multiplier";
                            desc += "\n Next Level:";
                            desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel)) + " " + (node as DamageNode).GetDamageType + " Damage Multiplier";
                            break;
                        default:
                            if (node.GetLevel > 0)
                                desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as DamageNode).GetDamageType + " Damage";
                            desc += "\n Next Level:";
                            desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as DamageNode).GetDamageType + " Damage";
                            break;
                    }
                    break;
                case NodeType.Speed:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as SpeedNode).GetDamageType + " Speed";
                    desc += "\n Next Level:";
                    desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as SpeedNode).GetDamageType + " Speed";
                    break;
                case NodeType.Stats:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel)) + " " + (node as StatNode).GetStatType + "";
                    desc += "\n Next Level:";
                    desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel)) + " " + (node as StatNode).GetStatType + "";
                    break;
                case NodeType.Leech:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as LeechNode).GetLeechType + " Leech";
                    desc += "\n Next Level:";
                    desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as LeechNode).GetLeechType + " Leech";
                    break;
                case NodeType.Perk:
                    switch ((node as PerkNode).GetPerk)
                    {
                        default:
                            desc += "";
                            break;
                    }
                    break;
                case NodeType.Immunity:
                    switch ((node as ImmunityNode).GetImmunity)
                    {
                        default:
                            desc += "";
                            break;
                    }
                    break;
            }

            return desc;
        }
    }
}
