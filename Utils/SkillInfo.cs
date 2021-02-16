
using AnotherRpgMod.RPGModule;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.Utils
{
    class SkillInfo
    {

        static public string GetPerkDescription(Perk PerkType,int level = 1)
        {
            string desc = "";

            switch (PerkType)
            {
                case Perk.Masochist:
                    return "Set armor to 0 \nEach armor point Increase your damage by " + level + "%";
                case Perk.BloodMage:
                    switch (level)
                        {
                        case 1:
                            return "Reduce mana cost by 50% but the saved mana consume health";
                        case 2:
                            return "Reduce mana cost by 75% but the saved mana consume health";
                        case 3:
                            return "Reduce mana cost by 90% but the saved mana consume health";
                    }
                    break;
                case Perk.Vampire:
                    switch (level)
                    {
                        case 1:
                            return "Give 0.5% LifeSteal at night\nIncrease stats by 25% at night\nReduce stat by 50% at day";
                        case 2:
                            return "Give 0.75% LifeSteal at night\nIncrease stats by 50% at night\nReduce stat by 25% at day";
                        case 3:
                            return "Give 1% LifeSteal at night\nIncrease stats by 100% at night\nReduce stat by 10% at day";
                    }
                    break;
                case Perk.Chlorophyll:
                    switch (level)
                    {
                        case 1:
                            return "+50% health and mana regen at day\nIncrease stats by 25% at day\nReduce stat by 50% at night";
                        case 2:
                            return "+75% health and mana regen at day\nIncrease stats by 50% at day\nReduce stat by 25% at night";
                        case 3:
                            return "+100% health and mana regen at day\nIncrease stats by 100% at day\nReduce stat by 10% at night";
                    }
                    break;
                case Perk.DemonEater:
                    return "Each kill regen " + level*5 + "% of your max health";
                case Perk.Cupidon:
                    return "Each attack have " + level * 5 + "% chance to drop an heart";
                case Perk.StarGatherer:
                    return "Each attack have " + level * 5 + "% chance to drop a star";
                case Perk.Biologist:
                    return "Potions heal "+level *20+"% more heal";
                case Perk.Berserk:
                    return "each percent of health missing increase your damage by "+ level +"%";
                case Perk.Survivalist:
                    return "Reduce Damage by 50% to gain "+level*20+"% of defense";
                case Perk.TheGambler:
                    return "50% for damage receive to heal, 50% to double damage";
                case Perk.ManaOverBurst:
                    float bonusmanacost = Main.player[Main.myPlayer].statMana * (0.1f + ((float)level - 1) * 0.15f);
                    float multiplier = 1 + (1 - 1 / (bonusmanacost / 1000 + 1));
                    return "Increase mana cost by " + (10 + 15*(level-1)) + "% of current mana\nIncrease magic damage by " + Mathf.Round(multiplier*100 - 100, 2) + "%";

            }


            return desc;
        }
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
                    desc += "- " + (-ClassInfo.ManaCost * 100) + " % Mana Cost\n";
                else
                    desc += "+ " + (ClassInfo.ManaCost * 100) + " % Mana Cost\n";
            }

            if (ClassInfo.ManaShield > 0 && (ClassInfo.ManaEfficiency > 0 || ClassInfo.ManaBaseEfficiency > 0))
            {
                int intelect = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetStat(Stat.Int);
                desc += "Grand Mana Shield : " + (ClassInfo.ManaShield * 100) +
                    "% damage absorbed by mana (" + (ClassInfo.ManaBaseEfficiency + (intelect * ClassInfo.ManaEfficiency)) + " Damage per mana)\n";
            }
            if (classType == ClassType.AscendedShadowDancer)
            {
                desc += "+200% throw velocity\n";
            }
            if (classType == ClassType.ShadowDancer)
            {
                desc += "+150% throw velocity\n";
            }
            if (classType == ClassType.Assassin)
            {
                desc += "+100% throw velocity\n";
            }
            if (classType == ClassType.Rogue)
            {
                desc += "+75% throw velocity\n";
            }
            if (classType == ClassType.Shinobi)
            {
                desc += "+50% throw velocity\n";
            }
            if (classType == ClassType.Ninja)
            {
                desc += "+20% throw velocity\n";
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
                            if (node.GetLevel < node.GetMaxLevel)
                            {
                                desc += "\n Next Level :";
                                desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel)) + " " + (node as DamageNode).GetDamageType + " Damage Multiplier";
                            }
                            break;
                        default:
                            if (node.GetLevel > 0)
                                desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as DamageNode).GetDamageType + " Damage";
                            if (node.GetLevel < node.GetMaxLevel)
                            {
                                desc += "\n Next Level :";
                                desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as DamageNode).GetDamageType + " Damage";
                            }
                            break;
                    }
                    break;
                case NodeType.Speed:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as SpeedNode).GetDamageType + " Speed";
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += "\n Next Level :";
                        desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as SpeedNode).GetDamageType + " Speed";
                    }
                    break;
                case NodeType.Stats:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel)) + " " + (node as StatNode).GetStatType + "";
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += "\n Next Level :";
                        desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel)) + " " + (node as StatNode).GetStatType + "";
                    }
                    break;
                case NodeType.Leech:
                    if (node.GetLevel > 0)
                        desc += "+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f) + "% " + (node as LeechNode).GetLeechType + " Leech";
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += "\n Next Level :";
                        desc += "\n+ " + (node.GetValue * (float)Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f) + "% " + (node as LeechNode).GetLeechType + " Leech";
                    }
                    break;
                case NodeType.Perk:
                    if (node.GetLevel > 0)
                        desc += GetPerkDescription((node as PerkNode).GetPerk,node.GetLevel);
                    if (node.GetLevel < node.GetMaxLevel) { 
                        desc += "\n Next Level :\n";
                        desc += GetPerkDescription((node as PerkNode).GetPerk, node.GetLevel +1);
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
                case NodeType.LimitBreak:
                    desc += "Unlock Unfathomable Power\n";
                    desc += "+ " +node.GetValue+ " All Stat\n";
                    desc += "Allow Exp Gain after level 1000\n";
                    break;
            }

            return desc;
        }
    }
}
