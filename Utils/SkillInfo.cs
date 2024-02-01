
using AnotherRpgMod.RPGModule;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;
using Terraria.Localization;

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
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Masochist") + level + "%";
                case Perk.BloodMage:
                    switch (level)
                        {
                        case 1:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.BloodMage.1");
                        case 2:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.BloodMage.2");
                        case 3:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.BloodMage.3");
                    }
                    break;
                case Perk.Vampire:
                    switch (level)
                    {
                        case 1:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Vampire.1");
                        case 2:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Vampire.2");
                        case 3:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Vampire.3");
                    }
                    break;
                case Perk.Chlorophyll:
                    switch (level)
                    {
                        case 1:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Chlorophyll.1");
                        case 2:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Chlorophyll.2");
                        case 3:
                            return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Chlorophyll.3");
                    }
                    break;
                case Perk.DemonEater:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.DemonEater.1") + level*5 + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.DemonEater.2");
                case Perk.Cupidon:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Cupidon.1") + level * 5 + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Cupidon.2");
                case Perk.StarGatherer:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.StarGatherer.1") + level * 5 + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.StarGatherer.2");
                case Perk.Biologist:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Biologist.1") + level *20+ Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Biologist.2");
                case Perk.Berserk:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Berserk.1") + level + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Berserk.2");
                case Perk.Survivalist:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Survivalist.1") + level*20+ Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Survivalist.2");
                case Perk.TheGambler:
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.TheGambler");
                case Perk.ManaOverBurst:
                    float bonusmanacost = Main.player[Main.myPlayer].statMana * (0.1f + ((float)level - 1) * 0.15f);
                    float multiplier = 1 + (1 - 1 / (bonusmanacost / 1000 + 1));
                    return Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaOverBurst.1") + (10 + 15*(level-1)) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaOverBurst.2") + Mathf.Round(multiplier*100 - 100, 2) + "%";

            }


            return desc;
        }
        static public string GetClassDescription(ClassType classType)
        {
            JsonChrClass ClassInfo = JsonCharacterClass.GetJsonCharList.GetClass(classType);
            string desc = Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Onlyhave");




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
                desc += "+ " + (ClassInfo.Damage[0] * 100) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.AllDamage");
            else
            {

                for (int i = 0; i < ClassInfo.Damage.Length; i++)
                {
                    if (ClassInfo.Damage[i] != 0)
                    {
                        if (ClassInfo.Damage[i] > 0)
                            desc += "+ " + Mathf.Round(ClassInfo.Damage[i] * 100,2) + "% " + ((DamageNameTree)i).ToString() + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Damage1");
                        else
                            desc += "- " + Mathf.Round(-ClassInfo.Damage[i] * 100,2) + "% " + ((DamageNameTree)i).ToString() + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Damage1");
                    }
                }
            }


            if (ClassInfo.Speed != 0)
            {
                if (ClassInfo.Speed > 0)
                    desc += "+ " + Mathf.Round(ClassInfo.Speed * 100,2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.MeleeSpeed");
                else
                    desc += "- " + Mathf.Round(-ClassInfo.Speed * 100,2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.MeleeSpeed");
            }

            if (ClassInfo.Health != 0)
            {
                if (ClassInfo.Health > 0)
                    desc += "+ " + Mathf.Round(ClassInfo.Health * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Health");
                else
                    desc += "- " + Mathf.Round(-ClassInfo.Health * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Health");
            }
            if (ClassInfo.Armor != 0)
            {
                if (ClassInfo.Armor > 0)
                    desc += "+ " + Mathf.Round(ClassInfo.Armor * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Armor");
                else
                    desc += "- " + Mathf.Round(-ClassInfo.Armor * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Armor");
            }
            if (ClassInfo.MovementSpeed != 0)
            {
                if (ClassInfo.MovementSpeed > 0)
                    desc += "+ " + Mathf.Round(ClassInfo.MovementSpeed * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.MovementSpeed");
                else
                    desc += "- " + Mathf.Round(-ClassInfo.MovementSpeed * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.MovementSpeed");
            }
            if (ClassInfo.Dodge != 0)
            {
                if (ClassInfo.Dodge != 0)
                    desc += "+ " + Mathf.Round(ClassInfo.Dodge * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Dodge");
                else
                    desc += "- " + Mathf.Round(-ClassInfo.Dodge * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Dodge");
            }
            if (ClassInfo.Ammo != 0)
            {
                if (ClassInfo.Ammo < 0)
                    desc += "+ " + Mathf.Round(-ClassInfo.Ammo * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Ammo");
                else
                    desc += "- " + Mathf.Round(ClassInfo.Ammo * 100, 2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Ammo");
            }
            if (ClassInfo.Summons != 0)
            {
                if (ClassInfo.Summons < 0)
                    desc += "- " + Mathf.Round(-ClassInfo.Summons, 0) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Summons");
                else
                    desc += "+ " + Mathf.Round(ClassInfo.Summons, 0) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Summons");
            }
            if (ClassInfo.ManaCost != 0)
            {
                if (ClassInfo.ManaCost < 0)
                    desc += "- " + Mathf.Round(-ClassInfo.ManaCost * 100,2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaCost");
                else
                    desc += "+ " + Mathf.Round(ClassInfo.ManaCost * 100,2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaCost");
            }

            if (ClassInfo.ManaShield > 0 && (ClassInfo.ManaEfficiency > 0 || ClassInfo.ManaBaseEfficiency > 0))
            {
                int intelect = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetStat(Stat.Int);
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaShield.1") + Mathf.Round(ClassInfo.ManaShield * 100,2) +
                    Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaShield.2") + Mathf.Round(ClassInfo.ManaBaseEfficiency + (intelect * ClassInfo.ManaEfficiency),2) + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.ManaShield.3");
            }
            if (classType == ClassType.AscendedShadowDancer)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.1");
            }
            if (classType == ClassType.ShadowDancer)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.2");
            }
            if (classType == ClassType.Assassin)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.3");
            }
            if (classType == ClassType.Rogue)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.4");
            }
            if (classType == ClassType.Shinobi)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.5");
            }
            if (classType == ClassType.Ninja)
            {
                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.throwvelocity.6");
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
                                desc += "+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel), 2) + " " + (node as DamageNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.DamageMultiplier");
                            if (node.GetLevel < node.GetMaxLevel)
                            {
                                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel");
                                desc += "\n+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel), 2) + " " + (node as DamageNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.DamageMultiplier");
                            }
                            break;
                        default:
                            if (node.GetLevel > 0)
                                desc += "+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as DamageNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Damage2");
                            if (node.GetLevel < node.GetMaxLevel)
                            {
                                desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel");
                                desc += "\n+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as DamageNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Damage2");
                            }
                            break;
                    }
                    break;
                case NodeType.Speed:
                    if (node.GetLevel > 0)
                        desc += "+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as SpeedNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Speed");
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel");
                        desc += "\n+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as SpeedNode).GetDamageType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Speed");
                    }
                    break;
                case NodeType.Stats:
                    if (node.GetLevel > 0)
                        desc += "+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel), 2) + " " + (node as StatNode).GetStatType + "";
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel");
                        desc += "\n+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel), 2) + " " + (node as StatNode).GetStatType + "";
                    }
                    break;
                case NodeType.Leech:
                    if (node.GetLevel > 0)
                        desc += "+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as LeechNode).GetLeechType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Leech");
                    if (node.GetLevel < node.GetMaxLevel)
                    {
                        desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel");
                        desc += "\n+ " + Mathf.Round(node.GetValue * Mathf.Clamp(node.GetLevel + 1, 0, node.GetMaxLevel) * 100f, 2) + "% " + (node as LeechNode).GetLeechType + Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.Leech");
                    }
                    break;
                case NodeType.Perk:
                    if (node.GetLevel > 0)
                        desc += GetPerkDescription((node as PerkNode).GetPerk,node.GetLevel);
                    if (node.GetLevel < node.GetMaxLevel) { 
                        desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.NextLevel1");
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
                    desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.LimitBreak.1");
                    desc += "+ " +node.GetValue+ Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.LimitBreak.2");
                    desc += Language.GetTextValue("Mods.AnotherRpgMod.SkillInfo.LimitBreak.3");
                    break;
            }

            return desc;
        }
    }
}
