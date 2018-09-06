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
using Terraria.GameInput;
using AnotherRpgMod.Items;

namespace AnotherRpgMod.RPGModule
{
    

    public enum Stat : byte
    {
        Vit, //Vitality : Increase Health Points, Increase Health Regen, Increase defence (light)
        Foc, //Focus : Increase Mana, Increase Critical Rate, Increase Sumon Damage (light)
        Cons, //Constitution : Increase Defences, Increase Health Regen, increase health (light)


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
            public readonly static UInt16 ACTUALSAVEVERSION = 2;
            private int Exp = 0;
            private int skillPoints = 5;
            public void SpentSkillPoints(int value)
            {
                skillPoints -= value;
                Mathf.Clamp(skillPoints, 0, int.MaxValue);
            }
            public int GetSkillPoints
            {
                get{return skillPoints; }
            }
            private RpgStats Stats;
            private SkillTree skilltree;
            public SkillTree GetskillTree { get { return skilltree; } }

            

            private bool initiated = false;

            private int level = 1;
            private int armor;
            public int BaseArmor { get { return armor; } }

            public long EquipedItemXp = 0;
            public long EquipedItemMaxXp = 1;

            public float statMultiplier = 1;

            static public float MAINSTATSMULT = 0.04f;
            static public float SECONDARYTATSMULT = 0.025f;


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
            public int GetStatImproved(Stat stat)
            {
                return Mathf.CeilInt(Stats.GetStat(stat) * GetStatImprovementItem(stat));
            }
            public int GetStat(Stat s)
            {
                return Stats.GetStat(s)+ skilltree.GetStats(s);
            }
            public int GetNaturalStat(Stat s)
            {
                return Stats.GetNaturalStat(s) + skilltree.GetStats(s);
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



            #region WEAPONXP
            public void AddWeaponXp(int damage,Item Xitem )
            {
                if (damage < 0)
                    damage = -damage;
                if (Xitem != null && Xitem.damage > 0 && Xitem.maxStack <= 1)
                {
                    ItemUpdate Item = Xitem.GetGlobalItem<Items.ItemUpdate>();
                    if (Item != null && Item.NeedsSaving(Xitem))
                    {
                        Item.AddExp(Mathf.Ceillong(damage * 0.2f), player, Xitem);
                    }
                }
            }

            public void Leech(int damage)
            {
                int lifeHeal = (int)(player.statLife * GetLifeLeech());
                player.statLife = Mathf.Clamp(player.statLife + lifeHeal, 0, player.statLifeMax2);
                int manaHeal = (int)(player.statManaMax2 * GetManaLeech());
                player.statMana = Mathf.Clamp(player.statMana + manaHeal, 0, player.statManaMax2);
                if (lifeHeal > 0)
                    CombatText.NewText(player.getRect(), new Color(50, 255, 50), "+" + lifeHeal);
                if (manaHeal > 0)
                    CombatText.NewText(player.getRect(), new Color(50, 50, 255), "+" + manaHeal);

                if (Main.netMode == 1)
                {
                    NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }

            int GetBuffAmmount(int[] bufftime)
            {
                int count =0;
                for (int i = 0; i < bufftime.Length; i++)
                {
                    if (bufftime[i] > 0)
                        count++;
                }
                return count;
   
            }

            float GetXpMult()
            {
                float value = 1;
                ItemUpdate instance;
                Item item;
                for (int i = 0; i <= player.armor.Length; i++)
                {
                    if (i == player.armor.Length)
                        item = player.HeldItem;
                    else
                        item = player.armor[i];

                    if (item.netID > 0)
                    {
                        instance = item.GetGlobalItem<ItemUpdate>();
                        if (ItemUtils.HaveModifier(Modifier.Smart, instance.modifier) && !Main.dayTime)
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Smart, instance) * 0.01f;
                        }
                    }
                }
                return value;
            }

            void UpdateModifier()
            {
                ItemUpdate instance;
                Item item;
                for (int i = 0; i <= player.armor.Length; i++)
                {
                    if (i == player.armor.Length)
                        item = player.HeldItem;
                    else
                        item = player.armor[i];

                    if (item.netID > 0)
                    {


                        instance = item.GetGlobalItem<ItemUpdate>();
                        instance.EquipedUpdateModifier(item, player);
                    }
                }
            }

            float DamageMultiplierFromModifier(NPC target, int damage)
            {
                float value = 1;
                ItemUpdate instance;
                Item item;
                for (int i = 0; i <= player.armor.Length; i++)
                {
                    if (i == player.armor.Length)
                        item = player.HeldItem;
                    else
                        item = player.armor[i];
                    
                    if (item.netID > 0)
                    {

                        
                        instance = item.GetGlobalItem<ItemUpdate>();
                        if (ItemUtils.HaveModifier(Modifier.MoonLight, instance.modifier) && !Main.dayTime)
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.MoonLight, instance) * 0.01f;
                        }
                        if (ItemUtils.HaveModifier(Modifier.SunLight, instance.modifier) && Main.dayTime)
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.SunLight, instance) * 0.01f;
                        }
                        if (ItemUtils.HaveModifier(Modifier.Berserker, instance.modifier))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Berserker, instance)  * (1-(player.statLife/player.statLifeMax2));
                        }
                        if (ItemUtils.HaveModifier(Modifier.MagicConnection, instance.modifier))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.MagicConnection, instance) * 0.01f*player.statMana;
                        }
                        if (ItemUtils.HaveModifier(Modifier.Sniper, instance.modifier))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Sniper, instance) * 0.01f * Vector2.Distance(player.position, target.position);
                        }
                        if (ItemUtils.HaveModifier(Modifier.Brawler, instance.modifier) && Vector2.Distance(player.position, target.position) < 130)
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Brawler, instance) * 0.01f;
                        }
                        if (ItemUtils.HaveModifier(Modifier.Executor, instance.modifier))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Executor, instance) * (1 - (target.life / target.lifeMax));
                        }
                        if (ItemUtils.HaveModifier(Modifier.Venom, instance.modifier) && target.HasBuff(20))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Venom, instance) * 0.01f;
                        }
                        if (ItemUtils.HaveModifier(Modifier.Chaotic, instance.modifier) )
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(player.buffTime);
                        }
                        if (ItemUtils.HaveModifier(Modifier.Cunning, instance.modifier))
                        {
                            value += ItemUtils.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(target.buffTime);
                        }
                        

                        //APPLY DEBUFF
                        if (ItemUtils.HaveModifier(Modifier.Poisones, instance.modifier))
                        {
                            if(Mathf.Random(0,100)< ItemUtils.GetModifierBonus(Modifier.Poisones, instance))
                                target.AddBuff(BuffID.Venom ,360);
                        }
                        if (ItemUtils.HaveModifier(Modifier.Confusion, instance.modifier))
                        {
                            if (Mathf.Random(0, 100) < ItemUtils.GetModifierBonus(Modifier.Confusion, instance))
                                target.AddBuff(BuffID.Confused, 360);
                        }

                        if (ItemUtils.HaveModifier(Modifier.Cleave, instance.modifier))
                        {
                            int damageToApply = (int)(ItemUtils.GetModifierBonus(Modifier.Cleave, instance) * damage * value*0.01f);
                            for (int j = 0; j< Main.npc.Length; j++)
                            {
                                if (Vector2.Distance(Main.npc[j].position, target.position) < 200 && !Main.npc[j].townNPC)
                                {
                                    Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyCleave(Main.npc[j], damageToApply);
                                }
                            }
                        }

                        if (ItemUtils.HaveModifier(Modifier.BloodSeeker, instance.modifier))
                        {
                            if (ItemUtils.GetModifierBonus(Modifier.BloodSeeker, instance)> (target.life / target.lifeMax) * 100)
                            {
                                
                                player.statLife = Mathf.Clamp(player.statLife + (int)(damage * value*0.01f* ItemUtils.GetModifierBonusAlt(Modifier.BloodSeeker, instance)), player.statLife, player.statLifeMax2);
                            }
                        }
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                return value;
            }


            int GetMaxMinion()
            {
                int value = 0;
                ItemUpdate instance;
                Item item;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    item = player.inventory[i];

                    if (item.netID > 0)
                    {
                        instance = item.GetGlobalItem<ItemUpdate>();
                        if (item.summon && instance.Ascention > value)
                            value = instance.Ascention;
                    }
                }
                return value;
            }

            public int ApplyPiercing(float piercing,int def,int damage)
            {
                if (damage < def)
                {
                    damage = Mathf.RoundInt(def + damage * piercing);
                }
                else if (damage > def + damage * piercing)
                {
                    damage += Mathf.RoundInt(def * piercing);
                }
                return damage;
            }

            public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
            {




                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    if (crit)
                    {
                        damage = (int)(0.5f * damage * GetCriticalDamage());
                    }
                }
                damage = (int)(damage * DamageMultiplierFromModifier(target, damage));
                if (ItemUtils.HaveModifier(Modifier.Piercing, player.HeldItem.GetGlobalItem<ItemUpdate>().modifier))
                {
                    damage = ApplyPiercing(ItemUtils.GetModifierBonus(Modifier.Piercing, player.HeldItem.GetGlobalItem<ItemUpdate>()), target.defense, damage);
                }

                if (target.type != 488)
                    AddWeaponXp(damage, item);
                Leech(damage);
                MPPacketHandler.SendNpcUpdate(mod, target);

            }
            public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
            {
                damage = (int)(damage * DamageMultiplierFromModifier(target, damage));
                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    if (crit)
                    {
                        damage = (int)(0.5f * damage * GetCriticalDamage());
                    }
                }
                if (proj.minion && player.HeldItem.summon)
                {
                    if (target.type != 488)
                        AddWeaponXp(damage / proj.penetrate,proj.GetGlobalProjectile<ARPGGlobalProjectile>().itemOrigin);
                }else if(!proj.minion) {
                    if (target.type != 488)
                        AddWeaponXp(damage / proj.penetrate, player.HeldItem);

                }
                

                Leech(damage);
                MPPacketHandler.SendNpcUpdate(mod, target);

            }
            
            #endregion

            #region ARMORXP
            private void AddArmorXp(int damage)
            {
                damage = Mathf.Clamp(damage, 0, player.statLifeMax2);
                Item armorItem;
                for (int i = 0;i< 3; i++)
                {
                    armorItem = player.armor[i];
                    if (armorItem.Name!= "")
                    {
                        armorItem.GetGlobalItem<Items.ItemUpdate>().AddExp(Mathf.Ceillong(Mathf.Log2(damage)), player, armorItem);
                    }
                    
                }
            }

            public void dodge(ref int damage)
            {
                if (skilltree.ActiveClass != null)
                {
                    if (Mathf.RandomInt(0, 99) * 0.01f < JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Dodge)
                    {
                        player.statLife = Mathf.Clamp(player.statLife + damage, 0, player.statLifeMax2);
                        damage = 0;
                    }
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }

            public override void OnHitByNPC(NPC npc, int damage, bool crit)
            {
                AddArmorXp(damage);
                dodge(ref damage);

                damage = ManaShieldReduction(damage);

                if (damage > player.statLife)
                {
                    if (IsSaved())
                        damage = 0;
                }

                base.OnHitByNPC(npc, damage, crit);
            }
            public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
            {
                AddArmorXp(damage);
                dodge(ref damage);
                damage = ManaShieldReduction(damage);

                if (damage > player.statLife)
                {
                    if (IsSaved())
                        damage = 0;
                }

                base.OnHitByProjectile(proj, damage, crit);
            }

            #endregion

            public struct ManaShieldInfo
            {
                public float DamageAbsorbtion;
                public float ManaPerDamage;

                public ManaShieldInfo(float _Absortion,float _ManaPerDamage)
                {
                    DamageAbsorbtion = _Absortion;
                    ManaPerDamage = _ManaPerDamage;

                }
            }
            
            public int ManaShieldReduction(int damage)
            {
                if (skilltree.ActiveClass == null)
                {
                    return damage;
                }
                ManaShieldInfo ShieldInfo = GetManaShieldInfo();
                if (ShieldInfo.DamageAbsorbtion == 0 || damage == 1)
                    return damage;

                float maxDamageAbsorbed = damage * ShieldInfo.DamageAbsorbtion;
                float ManaCost = maxDamageAbsorbed / ShieldInfo.ManaPerDamage;
                ManaCost = Mathf.Clamp(ManaCost, 0, player.statMana);
                if (ManaCost == 0)
                    return damage;
                Main.PlaySound(39);
                int reducedDamage = Mathf.CeilInt(ManaCost * ShieldInfo.ManaPerDamage);
                player.statMana -= Mathf.CeilInt(ManaCost);
                CombatText.NewText(player.getRect(), new Color(64, 196, 255), "-"+ reducedDamage);
                return damage - reducedDamage;
            }

            public ManaShieldInfo GetManaShieldInfo()
            {
                float abs = JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaShield;
                float mp = JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaBaseEfficiency + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaEfficiency * Stats.GetStat(Stat.Int) ;
                return new ManaShieldInfo(abs,mp);
            }
            public float GetDefenceMult()
            {
                    return (GetStatImproved(Stat.Vit) * 0.0175f + GetStatImproved(Stat.Cons) * 0.035f) * statMultiplier + 1f ;
            }

            public bool IsSaved()
            {
                ItemUpdate instance;
                Item item;
                for (int i = 0; i < player.armor.Length; i++)
                {
                    item = player.armor[i];

                    if (item.netID > 0)
                    {
                        instance = item.GetGlobalItem<ItemUpdate>();
                        if (instance != null)
                            if (ItemUtils.HaveModifier(Modifier.Savior, instance.modifier))
                                if (Mathf.Random(0, 100) < ItemUtils.GetModifierBonus(Modifier.Savior, instance))
                                {
                                    CombatText.NewText(player.getRect(), new Color(64, 196, 255), "SAVED !",true,true);
                                    return true;
                                } 

                    }
                }
                return false;
            }

            public float GetStatImprovementItem(Stat s)
            {
                float value = 1;
                ItemUpdate instance;
                Item item;
                for (int i = 0;i< player.armor.Length; i++)
                {
                    item = player.armor[i];
                    
                    if (item.netID>0) {
                        instance = item.GetGlobalItem<ItemUpdate>();
                        if (instance != null)
                            value += instance.GetStat(s)*0.01f;
                        
                    }   
                }
                return value;
            }
            public float GetHealthPerHeart()
            {
                    return (GetStatImproved(Stat.Vit) * 1.5f + GetStatImproved(Stat.Cons) * 0.75f)* statMultiplier;
            }
            public float GetManaPerStar()
            {
                    return (GetStatImproved(Stat.Foc) * 0.2f + GetStatImproved(Stat.Int) * 0.1f) * statMultiplier;
            }

            public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
            {
                base.SyncPlayer(toWho, fromWho, newPlayer);
            }

            public override void SendClientChanges(ModPlayer clientPlayer)
            {

                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)Message.SyncLevel);
                    packet.Write(player.whoAmI);
                    packet.Write(level);
                    packet.Send();
                }
                base.SendClientChanges(clientPlayer);
            }

            public override void PreUpdateBuffs()
            {
                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    if (!initiated)
                    {
                        initiated = true;

                        skilltree.Init();
                    }

                    if (player.HasBuff(24) || player.HasBuff(20) || player.HasBuff(67))
                    {
                        player.statLife -= Mathf.CeilInt(Mathf.Pow(player.statLifeMax2, 0.9f) * 0.05 * NPCUtils.DELTATIME);
                    }
                    if (player.HasBuff(70) || player.HasBuff(39) || player.HasBuff(68))
                    {
                        player.statLife -= Mathf.CeilInt(Mathf.Pow(player.statLifeMax2, 0.9f) * 0.05 * NPCUtils.DELTATIME);
                    }
                    if (player.onFire2 || player.HasBuff(38) || player.breath <= 0)
                    {
                        player.statLife -= Mathf.CeilInt(Mathf.Pow(player.statLifeMax2, 0.9f) * 0.1 * NPCUtils.DELTATIME);
                    }

                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }

                    if (Main.netMode != 2)
                    {

                        player.meleeCrit = (int)(player.meleeCrit + GetCriticalChanceBonus());
                        player.thrownCrit = (int)(player.thrownCrit + GetCriticalChanceBonus());
                        player.magicCrit = (int)(player.magicCrit + GetCriticalChanceBonus());
                        player.rangedCrit = (int)(player.rangedCrit + GetCriticalChanceBonus());

                        player.maxMinions += skilltree.GetSummonSlot();
                        player.maxMinions += GetMaxMinion();

                        if (Arpg.LoadedMods[SupportedMod.Thorium])
                        {

                        }
                    }
                }
                    if (player.HeldItem != null && player.HeldItem.damage>0&& player.HeldItem.maxStack <= 1) { 
                        Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                        if (Item != null && Item.NeedsSaving(player.HeldItem))
                        {
                            
                            EquipedItemXp = Item.GetXp;
                            EquipedItemMaxXp = Item.GetMaxXp;
                        }
                    }

            }
            private void UpdateThoriumDamage(Player player)
            {
                Mod Thorium = ModLoader.GetMod("ThoriumMod");
                //player.GetModPlayer<Thorium.ThoriumPlayer>().symphonicDamage *= GetDamageMult(DamageType.Symphonic);
                //player.GetModPlayer<Thorium.ThoriumPlayer>().radiantBoost *= GetDamageMult(DamageType.Radiant);
            }
            public override void PostUpdateEquips()
            {

                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    if (Main.netMode != 2)
                    {
                        armor = player.statDefense;
                        player.statLifeMax2 = (int)(GetHealthMult() * player.statLifeMax2 * GetHealthPerHeart() / 20) + 10;
                        player.statManaMax2 = (int)(player.statManaMax2 * GetManaPerStar() / 20) + 4;
                        player.statDefense = (int)(GetDefenceMult() * player.statDefense * GetArmorMult());
                        player.meleeDamage *= GetDamageMult(DamageType.Melee, 2);
                        player.thrownDamage *= GetDamageMult(DamageType.Throw, 2);
                        player.rangedDamage *= GetDamageMult(DamageType.Ranged, 2);
                        player.magicDamage *= GetDamageMult(DamageType.Magic, 2);
                        player.minionDamage *= GetDamageMult(DamageType.Summon, 2);
                        if (skilltree.ActiveClass != null)
                        {
                            player.accRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                            player.moveSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                            player.maxRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                            player.meleeSpeed *= 1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Speed;
                            player.manaCost *= 1 - JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaCost;
                        }
                    }
                    player.lifeRegen *= Mathf.FloorInt(GetHealthRegen());
                    player.manaRegenBonus *= Mathf.FloorInt(GetManaRegen());
                }
                UpdateModifier();
                //Issue : After one use of item , player can no longer do anything wiht item&inventory
                //ErrorLogger.Log(player.can);
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
                float X = (GetStatImproved(Stat.Foc) + GetStatImproved(Stat.Dex))*0.05f;
                return X;
            }
            public float GetCriticalDamage()
            {
                float X = (GetStatImproved(Stat.Agi) + GetStatImproved(Stat.Str))*0.005f;
                return 1.4f+X;
            }

            public override void ProcessTriggers(TriggersSet triggersSet)
            {
                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    if (Arpg.StatsHotKey.JustPressed)
                    {
                        Main.PlaySound(SoundID.MenuOpen);
                        UI.Stats.Instance.LoadChar();
                        UI.Stats.visible = !UI.Stats.visible;
                    }
                    if (Arpg.SkillTreeHotKey.JustPressed)
                    {
                        Main.PlaySound(SoundID.MenuOpen);


                        UI.SkillTreeUi.visible = !UI.SkillTreeUi.visible;
                        if (UI.SkillTreeUi.visible)
                            UI.SkillTreeUi.Instance.LoadSkillTree();
                    }
                }
                
            }
            public float GetBonusHeal()
            {
                return GetHealthPerHeart() /20;
            }
            public float GetBonusHealMana()
            {
                return GetManaPerStar()/ 20;
            }

            public float GetHealthRegen()
            {
                return (GetStatImproved(Stat.Vit) + GetStatImproved(Stat.Cons)) * 0.1f * statMultiplier;
            }
            public float GetManaRegen()
            {
                return (GetStatImproved(Stat.Int) + GetStatImproved(Stat.Spr)) * 0.1f * statMultiplier;
            }

            public bool HaveRangedWeapon()
            {
                if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
                {
                    return player.HeldItem.ranged;
                }
                return false;
            }
            public bool HaveBow()
            {
                if (HaveRangedWeapon())
                {
                    if (player.HeldItem.useAmmo == 1)
                        return true;
                }
                return false;
            }
            public float GetHealthMult()
            {
                float mult = 1;
                if (skilltree.ActiveClass != null)
                    mult += JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Health;
                return mult;
            }
            public float GetArmorMult()
            {
                float mult = 1;
                if (skilltree.ActiveClass != null)
                    mult += JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Armor;
                return mult;
            }
            public float GetDamageMult(DamageType type,int skill = 0)
            {
                if (skill == 0) { 
                    switch (type)
                    {
                        case DamageType.Magic:
                            return (GetStatImproved(Stat.Int) * MAINSTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                        case DamageType.Ranged:
                            return (GetStatImproved(Stat.Agi) * MAINSTATSMULT + GetStatImproved(Stat.Dex) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                        case DamageType.Summon:
                            return (GetStatImproved(Stat.Spr) * MAINSTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                        case DamageType.Throw:
                            return (GetStatImproved(Stat.Dex) * MAINSTATSMULT + GetStatImproved(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                        case DamageType.Symphonic:
                            return (GetStatImproved(Stat.Agi) * SECONDARYTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) *statMultiplier + 0.5f;
                        case DamageType.Radiant:
                            return (GetStatImproved(Stat.Int) * SECONDARYTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                        default:
                            return (GetStatImproved(Stat.Str) * MAINSTATSMULT + GetStatImproved(Stat.Agi) * SECONDARYTATSMULT) * statMultiplier + 0.5f;
                    }
                }
                else if ( skill == 2 )
                {
                    return skilltree.GetDamageMult(type) * statMultiplier * GetDamageMult(type, 0) ;
                } 
                else
                {
                    return skilltree.GetDamageMult(type);
                }
            }


            public void CheckExp()
            {
                int actualLevelGained = 0;
                while (this.Exp >= XPToNextLevel())
                {
                    
                    actualLevelGained++;
                    this.Exp -= XPToNextLevel();
                    LevelUp();
                    if (actualLevelGained > 5)
                    {
                        this.Exp = 0;
                    }
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
                if (ConfigFile.GetConfig.gpConfig.RPGPlayer)
                {
                    exp = (int)(exp * GetXpMult());
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
                skillPoints++;
                Stats.OnLevelUp();
                CombatText.NewText(player.getRect(), new Color(255, 25, 100), "LEVEL UP !!!!");
                CombatText.NewText(player.getRect(), new Color(255, 125, 255), "+1 SKILL POINTS",true);
                CombatText.NewText(player.getRect(), new Color(150, 100, 200), "+"+ pointsToGain + " Ability points",true);
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

            public int[] SaveSkills ()
            {
                int[] skillLevels = new int[skilltree.nodeList.nodeList.Count];

                for(int i = 0; i< skillLevels.Length; i++)
                {
                    skillLevels[i] = skilltree.nodeList.nodeList[i].GetLevel;
                }

                return skillLevels;

            }

            public void ResetSkillTree()
            {
                skilltree = new SkillTree();
                skillPoints = level - 1;
                return;
            }

            void LoadSkills(int[] _skillLevel)
            {
                Reason CanUp;
                if (skilltree.nodeList.nodeList.Count <= _skillLevel.Length)
                {
                    ResetSkillTree();
                }
                for (int i = 1; i < _skillLevel.Length; i++)
                {

                    

                    CanUp = skilltree.nodeList.nodeList[i].CanUpgrade(_skillLevel[i]* skilltree.nodeList.nodeList[i].GetCostPerLevel, level);
                    if (!(CanUp == Reason.LevelRequirement || CanUp == Reason.MaxLevelReach))
                    {
                        for (int U = 0; U < _skillLevel[i]; U++)
                        {
                            if (skillPoints- skilltree.nodeList.nodeList[i].GetCostPerLevel >= 0) { 
                                skillPoints -= skilltree.nodeList.nodeList[i].GetCostPerLevel;
                                if (skilltree.nodeList.nodeList[i].GetNodeType != NodeType.Class)
                                    skilltree.nodeList.nodeList[i].Upgrade();
                                else
                                    skilltree.nodeList.nodeList[i].Upgrade(true);

                            }
                        }
                    }
                    else
                    {
                        //ResetSkillTree();
                        return;
                    }
                }
            }

            int GetActiveClassID()
            {
                if (skilltree.ActiveClass == null)
                {
                    return -1;
                }
                if (skilltree.ActiveClass.GetParent == null)
                {
                    return -1;
                }
                return (int)skilltree.ActiveClass.GetParent.ID;
            }

            public override TagCompound Save()
            {
                
                if (Stats == null)
                {
                    Stats = new RpgStats();
                }
                if (skilltree == null)
                {
                    skilltree = new SkillTree();
                }
                return new TagCompound {
                    {"Exp", Exp},
                    {"level", level},
                    {"Stats", ConvertStatToInt()},
                    {"StatsXP", ConvertStatXPToInt()},
                    {"totalPoints", totalPoints},
                    {"freePoints", freePoints},
                    {"skillLevel",SaveSkills() },
                    {"activeClass",GetActiveClassID() },
                    {"AnRPGSkillVersion",SkillTree.SKILLTREEVERSION },
                    {"AnRPGSaveVersion",ACTUALSAVEVERSION }
                };
            }
            public override void Initialize()
            {
                Stats = new RpgStats();
                skilltree = new SkillTree();
            }

            public override void Load(TagCompound tag)
            {
                
                Exp = tag.GetInt("Exp");
                level = tag.GetInt("level");
                LoadStats( tag.GetIntArray("Stats"), tag.GetIntArray("StatsXP"));
                totalPoints = tag.GetInt("totalPoints");
                
                freePoints = tag.GetInt("freePoints");
                skillPoints = level - 1;
                if (tag.GetInt("AnRPGSkillVersion") != SkillTree.SKILLTREEVERSION)
                    ErrorLogger.Log("AnRPG SkillTree Is Outdated, reseting skillTree");
                else
                {
                    LoadSkills(tag.GetIntArray("skillLevel"));
                    if (tag.GetInt("activeClass") < skilltree.nodeList.nodeList.Count && tag.GetInt("activeClass")>0)
                    {

                        if (skilltree.nodeList.nodeList[tag.GetInt("activeClass")].GetNodeType == NodeType.Class)
                            ErrorLogger.Log(tag.GetInt("activeClass") + " " + skilltree.nodeList.nodeList.Count + " " + skilltree.nodeList.nodeList[tag.GetInt("activeClass")].GetNodeType);

                        skilltree.ActiveClass = (ClassNode)skilltree.nodeList.nodeList[tag.GetInt("activeClass")].GetNode;
                    }
                }

            }


            public override void PlayerConnect(Player player)
            {
                
                

                if (Main.netMode == 2)
                    MPPacketHandler.SendConfigFile(mod, ConfigFile.GetConfig.gpConfig);
            }

        }

        
        
        
    }
}
