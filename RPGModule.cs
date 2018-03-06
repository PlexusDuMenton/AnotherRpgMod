using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using AnotherRpgMod.Utils;
using Terraria.ModLoader.IO;

namespace AnotherRpgMod.RPGModule
{
    

    public enum Stat : byte
    {
        Vit, //Vitality : Increase Health Points, Increase Health Regen, Increase defence (light)
        Foc, //Focus : Increase Mana, Increase Critical Rate, Increase Sumon Damage (light)
        Con, //Constitution : Increase Defences, Increase Health Regen, increase health (light)


        Str, //Strength : Increase Melee Damage, Increase Critical Damage, Increase Throw Damage (light)
        Agi, //Agility : Increase Ranged Damage, Increase Critical Damage, Increase Melee Damage (light)
        Dex, //Dexterity : Increase Throw Damage, Increase Critical Rate, Increase Ranged Damage (light)
        Int, //Intelect : Increase Magic Damage, Increase Mana Regen, Increase Mana (light)
        Spr //Spirit : Increase Summon Damage,  Increase Mana Regen, Increase Magic Damage (light)
    }
    

    namespace Entities { 
        class RpgStats
        {
            static int Default = 4;
            private Dictionary<Stat, int> ActualStat;

            public RpgStats()
            {
                ActualStat = new Dictionary<Stat, int>(8);
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat.Add((Stat)i, Default);
                }
            }

            public void SetStats(Stat a,int b)
            {
                ActualStat[a] = b;
            }

            public int GetStat(Stat a)
            {
                return ActualStat[a];
            }
            public int GetNaturalStat(Stat a, int level)
            {
                return level + Default - 1;
            }
            public void UpgradeStat(Stat statname,int value = 1)
            {
                ActualStat[statname] += value;
            }

            public void Reset(int level)
            {
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat[(Stat)i] = level+ Default-1;
                }
            }

            public void OnLevelUp()
            {
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat[(Stat)i] += 1;
                }
            }
        }

        class RPGPlayer : ModPlayer
        {
            private int Exp = 0;
            private RpgStats Stats;
            private int level = 1;
            private int armor;
            public int BaseArmor { get { return armor; } }

            public float statMultiplier = 1;

            public void SyncLevel(int _level) //only use for sync
            {
                level = _level;
            }
            public void SyncStat(int value,Stat stat) //only use for sync
            {
                Stats.SetStats(stat, value);
            }

            public int GetStat(Stat s)
            {
                return Stats.GetStat(s);
            }
            public int GetNaturalStat(Stat s)
            {
                return Stats.GetNaturalStat(s,level);
            }
            public int GetAddStat(Stat s)
            {
                return Stats.GetStat(s)- Stats.GetNaturalStat(s, level);
            }
            public int GetLevel()
            {
                return level;
            }
            public int GetExp()
            {
                return Exp;
            }

            private int totalPoints = 0;
            private int freePoints = 0;


            public int FreePtns { get { return freePoints; } }
            public int TotalPtns { get { return totalPoints; } }

            private int BASEHEALTHPERHEART = 15;
            private int BASEMANAPERSTAR = 5;


            public float GetDefenceMult()
            {
                    return (Stats.GetStat(Stat.Vit) * 0.02f + Stats.GetStat(Stat.Con) * 0.04f) * statMultiplier + 0.6f;
            }

            public float GetHealthPerHeart()
            {
                    return (Stats.GetStat(Stat.Vit) * 0.5f + Stats.GetStat(Stat.Con) * 0.25f)* statMultiplier + BASEHEALTHPERHEART;
            }
            public float GetManaPerStar()
            {
                    return (Stats.GetStat(Stat.Foc) * 0.2f + Stats.GetStat(Stat.Int) * 0.1f) * statMultiplier + BASEMANAPERSTAR;
            }

            public override void PreUpdateBuffs()
            {
                if (Main.netMode != 2) { 
                    player.meleeDamage *= GetDamageMult(DamageType.Melee);
                    player.thrownDamage *= GetDamageMult(DamageType.Throw);
                    player.rangedDamage *= GetDamageMult(DamageType.Ranged);
                    player.magicDamage *= GetDamageMult(DamageType.Magic);
                    player.minionDamage *= GetDamageMult(DamageType.Summon);
                }

            }
            public override void PostUpdateEquips()
            {
                if (Main.netMode != 2) { 
                    armor = player.statDefense;
                    player.statLifeMax2 = (int)(player.statLifeMax * GetHealthPerHeart() / 20) + 10;
                    player.statManaMax2 = (int)(player.statManaMax * GetManaPerStar() / 20) + 4;
                    player.statDefense = (int)(player.statDefense * GetDefenceMult());

                }
            }




            public void SpendPoints(Stat _stat,int ammount)
            {
                ammount = Mathf.Clamp(ammount, 1, freePoints);
                Stats.UpgradeStat(_stat, ammount);
                freePoints -= ammount;
            }

            public void ResetStats()
            {
                Stats.Reset(level);
                freePoints = totalPoints;
            }

            public float GetCriticalChanceBonus()
            {
                int X = Mathf.CeilInt((Stats.GetStat(Stat.Foc) + Stats.GetStat(Stat.Dex))*0.2);
                return Mathf.Round(Mathf.Log(Mathf.Pow(X,15)),3); // please don't juge weird maths
            }
            public float GetCriticalDamage()
            {
                float X = (Stats.GetStat(Stat.Agi) + Stats.GetStat(Stat.Str))*0.001f;
                return 1+(1-(X/(1+X)));
            }

            public float GetBonusHeal()
            {
                return GetHealthPerHeart() / 20;
            }
            public float GetBonusHealMana()
            {
                return GetManaPerStar() / 20;
            }

            public int GetHealthRegen()
            {
                return Mathf.CeilInt((Stats.GetStat(Stat.Vit) + Stats.GetStat(Stat.Con)) * 0.05f * statMultiplier);
            }
            public int GetManaRegen()
            {
                return Mathf.CeilInt((Stats.GetStat(Stat.Int) + Stats.GetStat(Stat.Spr)) * 0.05f * statMultiplier);
            }

            public float GetDamageMult(DamageType type)
            {
                if (type == DamageType.Melee)
                {
                    return (Stats.GetStat(Stat.Str) * 0.1f+ Stats.GetStat(Stat.Agi) * 0.05f)*statMultiplier + 0.4f;
                }
                else if (type == DamageType.Magic)
                {
                    return (Stats.GetStat(Stat.Int) * 0.1f + Stats.GetStat(Stat.Spr) * 0.05f)*statMultiplier + 0.4f;
                }
                else if (type == DamageType.Ranged)
                {
                    return (Stats.GetStat(Stat.Agi) * 0.1f + Stats.GetStat(Stat.Dex) * 0.05f) * statMultiplier + 0.4f;
                }
                else if (type == DamageType.Summon)
                {
                    return (Stats.GetStat(Stat.Spr) * 0.1f + Stats.GetStat(Stat.Foc) * 0.05f)*statMultiplier + 0.4f;
                }
                else
                {
                    return (Stats.GetStat(Stat.Dex) * 0.1f + Stats.GetStat(Stat.Str)*0.05f)*statMultiplier + 0.4f;
                }
            }


            public void CheckExp()
            {
                while (this.Exp >= XPToNextLevel())
                {
                    this.Exp -= XPToNextLevel();
                    LevelUp();
                }
            }

            int ReduceExp(int xp, int _level)
            {
                int exp = xp;
                if (_level <= level - 5)
                {
                    float expMult = 1 - (level - _level) * 0.1f;
                    exp = (int)(exp * expMult);
                }

                if (exp < 1)
                    exp = 1;

                return exp;
            }
            public void AddXp(int exp,int _level)
            {

                exp = ReduceExp(exp, _level);



                if (exp >= XPToNextLevel() * 0.1f)
                {
                    CombatText.NewText(player.getRect(), new Color(50, 26, 255), exp + " XP !!");
                }
                else
                {
                    CombatText.NewText(player.getRect(), new Color(127, 159, 255), exp + " XP");
                }
                this.Exp += exp;
                CheckExp();
            }
            public void commandLevelup()
            {
                LevelUp();
            }
            private void LevelUp()
            {
                int pointsToGain = 3 + Mathf.FloorInt(Mathf.Pow(level,0.4f));
                totalPoints += pointsToGain;
                freePoints += pointsToGain; 
                Stats.OnLevelUp();
                CombatText.NewText(player.getRect(), new Color(255, 25, 100), "LEVEL UP !!!!");
                Main.NewText(player.name + " Is now level : " + level.ToString() + " .Congratulation !", 255, 223, 63);
                level++;

                if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)Message.SyncLevel);
                    packet.Write(player.whoAmI);
                    packet.Write(level);
                    packet.Send();
                }
            }
            public int XPToNextLevel()
            {
                return 15 * level + 5 * Mathf.CeilInt(Mathf.Pow(level, 2)) + 40;
            }


            public int[] ConvertStatToInt()
            {
                int[] convertedStats = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    convertedStats[i] = Stats.GetStat((Stat)i);
                }
                return convertedStats;
            }

            public void ConvertIntToStat(int[] convertedStats)
            {
                for (int i = 0; i < 8; i++)
                {
                    

                    Stats.SetStats((Stat)i, convertedStats[i]);
                }
            }

            public override TagCompound Save()
            {
                if (Stats == null)
                {
                    Stats = new RpgStats();
                }
                return new TagCompound {
                    {"Exp", Exp},
                    {"level", level},
                    {"Stats", ConvertStatToInt()},
                    {"totalPoints", totalPoints},
                    {"freePoints", freePoints}
                };
            }
            public override void Load(TagCompound tag)
            {
                

                Stats = new RpgStats();

                Exp = tag.GetInt("Exp");
                level = tag.GetInt("level");
                ConvertIntToStat( tag.GetIntArray("Stats"));
                totalPoints = tag.GetInt("totalPoints");
                freePoints = tag.GetInt("freePoints");
            }

            public override void PlayerConnect(Player player)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SyncLevel);
                packet.Write(player.whoAmI);
                packet.Write(level);
                packet.Send();
            }

        }

        
        
        
    }
}
