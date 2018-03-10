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
        class StatData
        {
            private int natural;
            private int level;
            private int xp;

            public int AddLevel { get { return level; } }
            public int NaturalLevel { get { return natural; } }
            public int GetLevel { get { return level+ natural; } }
            public int GetXP { get { return xp; } }

            public int XpForLevel()
            {
                return Mathf.CeilInt(level * 0.04f) + 1;
            }
            public void AddXp(int _xp)
            {
                xp += _xp;
                while (xp >= XpForLevel())
                {
                    xp -= XpForLevel();
                    level=level +1;
                }
            }
            public StatData(int _natural, int _level = 0, int _xp = 0)
            {
                natural = _natural;
                xp = _xp;
                level = _level;
            }
            public void LevelUp()
            {
                natural++;
            }
            
        }

        class RpgStats
        {
            readonly int Default = 4;
            private Dictionary<Stat, StatData> ActualStat;

            public RpgStats()
            {
                ActualStat = new Dictionary<Stat, StatData>(8);
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat.Add((Stat)i,new StatData(Default));
                }
            }

            public void SetStats(Stat _stat,int _natural,int _level,int _xp)
            {
                ActualStat[_stat] = new StatData(_natural, _level,_xp);
            }

            public int GetLevelStat(Stat a)
            {
                return ActualStat[a].AddLevel;
            }
            public int GetStat(Stat a)
            {
                return ActualStat[a].GetLevel;
            }
            public int GetNaturalStat(Stat a)
            {
                return ActualStat[a].NaturalLevel;
            }

            public void UpgradeStat(Stat statname,int value = 1)
            {
                ActualStat[statname].AddXp(value);
            }
            public int GetStatXP(Stat statname)
            {
                return ActualStat[statname].GetXP;
            }
            public int GetStatXPMax(Stat statname)
            {
                return ActualStat[statname].XpForLevel();
            }

            public void Reset(int level)
            {
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat[(Stat)i] = new StatData(level + Default-1);
                }
            }

            public void OnLevelUp()
            {
                for (int i = 0; i <= 7; i++)
                {
                    ActualStat[(Stat)i].LevelUp();
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

            public int EquipedItemXp = 0;
            public int EquipedItemMaxXp = 1;

            public float statMultiplier = 1;

            public void SyncLevel(int _level) //only use for sync
            {
                level = _level;
            }
            public void SyncStat(StatData data ,Stat stat) //only use for sync
            {
                Stats.SetStats(stat,level, data.GetLevel, data.GetXP);
            }

            public int GetStatXP(Stat s)
            {
                return Stats.GetStatXP(s);
            }
            public int GetStatXPMax(Stat s)
            {
                return Stats.GetStatXPMax(s);
            }
            public int GetStat(Stat s)
            {
                return Stats.GetStat(s);
            }
            public int GetNaturalStat(Stat s)
            {
                return Stats.GetNaturalStat(s);
            }
            public int GetAddStat(Stat s)
            {
                return Stats.GetLevelStat(s);
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



            private void AddWeaponXp(int damage)
            {
                if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
                {
                    Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                    if (Item != null && Item.NeedsSaving(player.HeldItem))
                    {
                        Item.AddExp(Mathf.CeilInt(damage * 0.2f), player);
                    }
                }
            }

            public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
            {
                AddWeaponXp(damage);
                int lifeHeal = (int)(damage * GetLifeLeech());
                player.statLife += lifeHeal;
                int manaHeal = (int)(player.statManaMax2 * GetManaLeech());
                player.statMana += manaHeal;
                if (lifeHeal>0)
                    CombatText.NewText(player.getRect(), new Color(50, 255, 50),"+"+ lifeHeal);
                if (manaHeal>0)
                    CombatText.NewText(player.getRect(), new Color(50, 50, 255), "+" + manaHeal);
            }

            public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
            {
                AddWeaponXp(damage);
                player.statMana += (int)(player.statManaMax2 * GetManaLeech());
                player.statLife += (int)(damage * GetLifeLeech());
            }

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
                    if (player.HeldItem != null && player.HeldItem.damage>0&& player.HeldItem.maxStack <= 1) { 
                        Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                        if (Item != null && Item.NeedsSaving(player.HeldItem))
                        {
                            EquipedItemXp = Item.GetXp;
                            EquipedItemMaxXp = Item.GetMaxXp;
                        }
                    }
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
            private void SilentLevelUp()
            {
                int pointsToGain = 5 + Mathf.FloorInt(Mathf.Pow(level, 0.5f));
                totalPoints += pointsToGain;
                freePoints += pointsToGain;
                Stats.OnLevelUp();
                level++;
            }
            public void RecalculateStat()
            {
                int _level = level;
                level = 0;
                totalPoints = 0;
                freePoints = 0;
                Stats = new RpgStats();
                for (int i = 0; i< _level; i++)
                {
                    SilentLevelUp();
                }

            }

            public float GetLifeLeech()
            {
                if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
                {
                    Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                    if (Item != null && Item.NeedsSaving(player.HeldItem))
                    {
                        return Item.GetLifeLeech*0.01f;
                    }
                }
                return 0;
            }
            public float GetManaLeech()
            {
                if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
                {
                    Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                    if (Item != null && Item.NeedsSaving(player.HeldItem))
                    {
                        return Item.GetManaLeech * 0.01f;
                    }
                }
                return 0;
            }
            private void LevelUp()
            {
                int pointsToGain = 5 + Mathf.FloorInt(Mathf.Pow(level,0.5f));
                totalPoints += pointsToGain;
                freePoints += pointsToGain; 
                Stats.OnLevelUp();
                CombatText.NewText(player.getRect(), new Color(255, 25, 100), "LEVEL UP !!!!");
                
                level++;
                Main.NewText(player.name + " Is now level : " + level.ToString() + " .Congratulation !", 255, 223, 63);
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
                return 15 * level + 5 * Mathf.CeilInt(Mathf.Pow(level, 1.8f)) + 40;
            }


            public int[] ConvertStatToInt()
            {
                int[] convertedStats = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    convertedStats[i] = GetAddStat((Stat)i);
                }
                return convertedStats;
            }

            public int[] ConvertStatXPToInt()
            {
                int[] convertedStats = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    convertedStats[i] = GetStatXP((Stat)i);
                }
                return convertedStats;
            }

            void LoadStats(int[] _level, int[] _xp)
            {
                if (_xp.Length != 8) //if save is not correct , will try to port
                {

                    RecalculateStat();
                    if (_level.Length != 8) //if port don't work
                    {
                        return;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Stats.UpgradeStat((Stat)i, _level[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Stats.SetStats((Stat)i,level+3,_level[i], _xp[i]);
                    }
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
                    {"StatsXP", ConvertStatXPToInt()},
                    {"totalPoints", totalPoints},
                    {"freePoints", freePoints},
                    {"AnRPGSaveVersion", 1}
                };
            }
            public override void Initialize()
            {
                Stats = new RpgStats();
            }

            public override void Load(TagCompound tag)
            {
                Exp = tag.GetInt("Exp");
                level = tag.GetInt("level");
                LoadStats( tag.GetIntArray("Stats"), tag.GetIntArray("StatsXP"));
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
