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
using System.Reflection;

namespace AnotherRpgMod.RPGModule.Entities
{

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
        private RPGStats Stats;
        private SkillTree skilltree;
        public SkillTree GetskillTree { get { return skilltree; } }
        float damageToApply = 0;


        private bool initiated = false;

        private int level = 1;
        private int armor;
        public int BaseArmor { get { return armor; } }
        public float m_virtualRes = 0;

        public long EquipedItemXp = 0;
        public long EquipedItemMaxXp = 1;

        public float statMultiplier = 1;

        static public float MAINSTATSMULT = 0.0025f;
        static public float SECONDARYTATSMULT = 0.001f;
        public string baseName = "";

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
            return Mathf.CeilInt(GetStat(stat) * GetStatImprovementItem(stat));
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


    public float GetLifeLeechLeft { get { return HealthRegenLeech * LifeLeechDuration * player.statLifeMax2; } }
    private float HealthRegenLeech = 0.1f;

    private float LifeLeechDuration;
    public float LifeLeechMaxDuration = 3;

    #region WEAPONXP
    public void AddWeaponXp(int damage,Item Xitem,float multiplier = 1 )
    {
        if (damage < 0)
            damage = -damage;
        if (Xitem != null && Xitem.damage > 0 && Xitem.maxStack <= 1)
        {
            ItemUpdate Item = Xitem.GetGlobalItem<Items.ItemUpdate>();
            if (Item != null && Item.NeedsSaving(Xitem))
            {
                Item.AddExp(Mathf.Ceillong(damage * multiplier), player, Xitem);
            }
        }
    }

    public void Leech(int damage)
    {
        float lifeHeal = damage * GetLifeLeech();
        int manaHeal = (int) (player.statManaMax2 * GetManaLeech());
        player.GetModPlayer<RPGPlayer>().ApplyReduction(ref lifeHeal);

        if (lifeHeal > 0)
        {
            if (lifeHeal < 1)
                lifeHeal = 1;
            float duration = lifeHeal / (player.statLifeMax2 * HealthRegenLeech);
            float buffer = 0;
            float totalHeal = 0;

            if (LifeLeechDuration < 1)
            {
                buffer = LifeLeechDuration;
                LifeLeechDuration = Mathf.Clamp(LifeLeechDuration + duration, LifeLeechDuration, 1);
                buffer = LifeLeechDuration - buffer;
                totalHeal = lifeHeal * buffer;
                duration -= buffer;
            }
            if (duration > 0)
            {

                int LifeLifeHealInfo = (int)(lifeHeal * 0.2f);
                    
                duration *= 0.2f;

                buffer = LifeLeechDuration;

                LifeLeechDuration = Mathf.Clamp(LifeLeechDuration + duration, LifeLeechDuration, LifeLeechMaxDuration);
                buffer = LifeLeechDuration - buffer;
                totalHeal += lifeHeal * buffer;

            }
            if (totalHeal < 1)
                totalHeal = 1;
            CombatText.NewText(player.getRect(), new Color(64, 255, 64), "+" + (int)totalHeal);

        }

        if (manaHeal > 0)
        {
            player.statMana = Mathf.Clamp(player.statMana + manaHeal, 0, player.statManaMax2);
        }
            
    }

    private float BufferLife;

    private void CustomPostUpdates()
    {

        if ( LifeLeechDuration > 0 && player.statLife < player.statLifeMax2)
        {
            LifeLeechDuration -= 1f / 60f;

            float newLife = BufferLife + player.statLife + (player.statLifeMax2 * HealthRegenLeech) / 60;
            int LifeGain = Mathf.Clamp(Mathf.FloorInt(newLife), player.statLife, player.statLifeMax2) ;
            BufferLife = newLife - LifeGain;
            player.statLife = LifeGain;

            if (Main.netMode == 1)
            {
                NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }
        else
        {
            BufferLife = 0;
            LifeLeechDuration = 0;
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
                    if (ModifierManager.HaveModifier(Modifier.Smart, instance.modifier) && !Main.dayTime)
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Smart, instance) * 0.01f;
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
                    if (ModifierManager.HaveModifier(Modifier.MoonLight, instance.modifier) && !Main.dayTime)
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.MoonLight, instance) * 0.01f;
                    }
                    if (ModifierManager.HaveModifier(Modifier.SunLight, instance.modifier) && Main.dayTime)
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.SunLight, instance) * 0.01f;
                    }
                    if (ModifierManager.HaveModifier(Modifier.Berserker, instance.modifier))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Berserker, instance)  * (1-(player.statLife/player.statLifeMax2));
                    }
                    if (ModifierManager.HaveModifier(Modifier.MagicConnection, instance.modifier))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.MagicConnection, instance) * 0.01f*player.statMana;
                    }
                    if (ModifierManager.HaveModifier(Modifier.Sniper, instance.modifier))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Sniper, instance) * 0.01f * Vector2.Distance(player.position, target.position);
                    }
                    if (ModifierManager.HaveModifier(Modifier.Brawler, instance.modifier) && Vector2.Distance(player.position, target.position) < 130)
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Brawler, instance) * 0.01f;
                    }
                    if (ModifierManager.HaveModifier(Modifier.Executor, instance.modifier))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Executor, instance) * (1 - (target.life / target.lifeMax));
                    }
                    if (ModifierManager.HaveModifier(Modifier.Venom, instance.modifier) && target.HasBuff(20))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Venom, instance) * 0.01f;
                    }
                    if (ModifierManager.HaveModifier(Modifier.Chaotic, instance.modifier) )
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(player.buffTime);
                    }
                    if (ModifierManager.HaveModifier(Modifier.Cunning, instance.modifier))
                    {
                        value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(target.buffTime);
                    }
                        

                    //APPLY DEBUFF
                    if (ModifierManager.HaveModifier(Modifier.Poisones, instance.modifier))
                    {
                        if(Mathf.Random(0,100)< ModifierManager.GetModifierBonus(Modifier.Poisones, instance))
                            target.AddBuff(BuffID.Venom ,360);
                    }
                    if (ModifierManager.HaveModifier(Modifier.Confusion, instance.modifier))
                    {
                        if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Confusion, instance))
                            target.AddBuff(BuffID.Confused, 360);
                    }

                    if (ModifierManager.HaveModifier(Modifier.Cleave, instance.modifier))
                    {
                        int damageToApply = (int)(ModifierManager.GetModifierBonus(Modifier.Cleave, instance) * damage * value*0.01f);
                        for (int j = 0; j< Main.npc.Length; j++)
                        {
                            if (Vector2.Distance(Main.npc[j].position, target.position) < 200 && !Main.npc[j].townNPC)
                            {
                                Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyCleave(Main.npc[j], damageToApply);
                            }
                        }
                    }

                    if (ModifierManager.HaveModifier(Modifier.BloodSeeker, instance.modifier))
                    {
                        if (ModifierManager.GetModifierBonus(Modifier.BloodSeeker, instance)> (target.life / target.lifeMax) * 100)
                        {
                            int heal = (int)(damage * value * 0.01f * ModifierManager.GetModifierBonusAlt(Modifier.BloodSeeker, instance));
                            player.GetModPlayer<RPGPlayer>().ApplyReduction(ref heal);
                            player.statLife = Mathf.Clamp(player.statLife + heal, player.statLife, player.statLifeMax2);
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
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




            if (Config.gpConfig.RPGPlayer)
            {
                if (crit)
                {
                    damage = (int)(0.5f * damage * GetCriticalDamage());
                }
            }
            damage = (int)(damage * DamageMultiplierFromModifier(target, damage));
            if (ModifierManager.HaveModifier(Modifier.Piercing, player.HeldItem.GetGlobalItem<ItemUpdate>().modifier))
            {
                damage = ApplyPiercing(ModifierManager.GetModifierBonus(Modifier.Piercing, player.HeldItem.GetGlobalItem<ItemUpdate>()), target.defense, damage);
            }

            if (target.type != 488)
                AddWeaponXp(damage, item);
            Leech(damage);
            MPPacketHandler.SendNpcUpdate(mod, target);

        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * DamageMultiplierFromModifier(target, damage));
            if (Config.gpConfig.RPGPlayer)
            {
                if (crit)
                {
                    damage = (int)(0.5f * damage * GetCriticalDamage());
                }
            }
            if (proj.minion)
            {
                if (target.type != 488)
                {
                    if (player.HeldItem.summon)
                        AddWeaponXp(damage / proj.penetrate, player.HeldItem,1);
                    else
                        AddWeaponXp(damage / proj.penetrate, player.HeldItem, 0.5f);
                            
                }
                        
            }else if(!proj.minion) {
                if (target.type != 488)
                    AddWeaponXp(damage / proj.penetrate, player.HeldItem);

            }
                

            Leech(damage);
            MPPacketHandler.SendNpcUpdate(mod, target);

        }
            
        #endregion

        #region ARMORXP
        private void AddArmorXp(int damage, float multiplier = 1)
    {
            damage = Mathf.Clamp(damage + player.statDefense, 0, player.statLifeMax2);
            Item armorItem;
            for (int i = 0;i< 3; i++)
            {
                armorItem = player.armor[i];
                if (armorItem.Name!= "")
                {
                    armorItem.GetGlobalItem<Items.ItemUpdate>().AddExp(Mathf.Ceillong(damage*multiplier), player, armorItem);
                }
                    
            }
        }

        public void dodge(ref int damage)
        {
            if (skilltree.ActiveClass != null)
            {
                if (Mathf.RandomInt(0, 99) * 0.01f < JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Dodge)
                {
                    int heal = damage;
                    player.GetModPlayer<RPGPlayer>().ApplyReduction(ref damage);
                    player.statLife = Mathf.Clamp(player.statLife + damage, 0, player.statLifeMax2);
                    damage = 0;
                }
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(21, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            ApplyReduction(ref damage);

            base.ModifyHitByNPC(npc, ref damage, ref crit);
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            ApplyReduction(ref damage);
            base.ModifyHitByProjectile(proj, ref damage, ref crit);
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            AddArmorXp(damage);
            dodge(ref damage);

            damage = ManaShieldReduction(damage);

            if (damage > player.statLife)
            {
                if (IsSaved())
                    player.statLife = player.statLifeMax2;
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
                    player.statLife = player.statLifeMax2;
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
                return (GetStatImproved(Stat.Vit) * 0.0055f + GetStatImproved(Stat.Cons) * 0.012f) * statMultiplier + 1f ;
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
                        if (ModifierManager.HaveModifier(Modifier.Savior, instance.modifier))
                            if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Savior, instance))
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
                return (GetStatImproved(Stat.Vit) * 0.5f + GetStatImproved(Stat.Cons) * 0.3f)* statMultiplier + 10;
        }
        public float GetManaPerStar()
        {
                return (GetStatImproved(Stat.Foc) * 0.15f + GetStatImproved(Stat.Int) * 0.075f) * statMultiplier;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            base.SyncPlayer(toWho, fromWho, newPlayer);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        string classnow = "Hobo";
            if (skilltree!= null && skilltree.ActiveClass != null)
            classnow = "" + skilltree.ActiveClass.GetClassType;

            if (Config.gpConfig.RPGPlayer)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SyncLevel);
                packet.Write(player.whoAmI);
                packet.Write(level);
                packet.Write(classnow);
                packet.Send();
        }
            base.SendClientChanges(clientPlayer);
        }

        public override void PreUpdateBuffs()
        {
            if (Config.gpConfig.RPGPlayer)
            {

                if (!initiated)
                {
                    initiated = true;
                    skilltree.Init();
                }
                if (player.HasBuff(24) || player.HasBuff(20) || player.HasBuff(67))
                {
                    damageToApply += Mathf.Clamp(Mathf.Logx(player.statLifeMax2, 1.002f) * 0.05f * NPCUtils.DELTATIME, 0, player.statLifeMax2 * 0.015f * NPCUtils.DELTATIME);
                }
                if (player.HasBuff(70) || player.HasBuff(39) || player.HasBuff(68))
                {
                    damageToApply += Mathf.Clamp(Mathf.Logx(player.statLifeMax2, 1.002f) * 0.08f * NPCUtils.DELTATIME, 0, player.statLifeMax2 * 0.025f * NPCUtils.DELTATIME);
                }
                if (player.onFire2 || player.HasBuff(38) || player.breath <= 0)
                {
                    damageToApply += Mathf.Clamp(Mathf.Logx(player.statLifeMax2, 1.002f)*0.1f * NPCUtils.DELTATIME, 0, player.statLifeMax2 * 0.04f * NPCUtils.DELTATIME);
                }

                if (AnotherRpgMod.LoadedMods[SupportedMod.DBZMOD])
                    IncreaseDBZKi(player);

            if (damageToApply > 1)
                {
                    int BuffDamage = Mathf.FloorInt(damageToApply);
                    damageToApply -= damageToApply;
                    ApplyReduction(ref BuffDamage);
                    player.statLife -= BuffDamage;
                }

                if (Main.netMode != 2)
                {

                    player.meleeCrit = (int)(player.meleeCrit + GetCriticalChanceBonus());
                    player.thrownCrit = (int)(player.thrownCrit + GetCriticalChanceBonus());
                    player.magicCrit = (int)(player.magicCrit + GetCriticalChanceBonus());
                    player.rangedCrit = (int)(player.rangedCrit + GetCriticalChanceBonus());

                    player.maxMinions += skilltree.GetSummonSlot();
                    player.maxMinions += GetMaxMinion();

                    //MODED DAMAGE
                    if (AnotherRpgMod.LoadedMods[SupportedMod.Thorium])
                    {
                        UpdateThoriumDamage(player);
                    }

                    if (AnotherRpgMod.LoadedMods[SupportedMod.Tremor])
                        UpdateTremorDamage(player);

                    if (AnotherRpgMod.LoadedMods[SupportedMod.DBZMOD])
                    {
                        UpdateDBZDamage(player);
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
    private void IncreaseDBZKi(Player player)
    {
        Mod DBZ = ModLoader.GetMod("DBZMOD");
        player.GetModPlayer<DBZMOD.MyPlayer>().kiMaxMult *= Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 10)),1,10);
        player.GetModPlayer<DBZMOD.MyPlayer>().kiChargeRate += Mathf.FloorInt( Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 6)), 0, 25));
    }
    private void UpdateDBZDamage(Player player)
        {
            Mod DBZ = ModLoader.GetMod("DBZMOD");
            player.GetModPlayer<DBZMOD.MyPlayer>().kiDamage *= GetDamageMult(DamageType.KI);
        player.GetModPlayer<DBZMOD.MyPlayer>().kiCrit += (int)GetCriticalChanceBonus();
    }
    private void UpdateThoriumDamage(Player player)
    {
        Mod Thorium = ModLoader.GetMod("thoriumLoaded");
        player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicDamage *= GetDamageMult(DamageType.Symphonic);
        player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantBoost *= GetDamageMult(DamageType.Radiant);

        player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantCrit += (int)GetCriticalChanceBonus();
        player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicCrit += (int)GetCriticalChanceBonus();


    }
    private void UpdateTremorDamage(Player player)
    {
        Mod Tremor = ModLoader.GetMod("Tremor");
        player.GetModPlayer<Tremor.MPlayer>().alchemicalDamage *= GetDamageMult(DamageType.Alchemic);
    }

    public void ApplyReduction(ref int damage,bool heal = false)
    {
        if (m_virtualRes>0)
            CombatText.NewText(player.getRect(), new Color(50, 26, 255,1), "("+damage + ")");
        damage = (int)(damage * (1 - m_virtualRes));
               
    }

    public void ApplyReduction(ref float damage, bool heal = false)
    {
        if (m_virtualRes > 0)
            CombatText.NewText(player.getRect(), new Color(50, 26, 255, 1), "(" + damage + ")");
        damage = (float)(damage * (1 - m_virtualRes));

    }

    public float GetArmorPenetrationMult()
        {
            return 1f + Stats.GetStat(Stat.Dex) * 0.01f;
        }

    public int GetArmorPenetrationAdd()
        {
            return Mathf.FloorInt( Stats.GetStat(Stat.Dex) * 0.1f);
        }

        public override void PostUpdateEquips()
    {
           
        if (Config.gpConfig.RPGPlayer)
        {
            if (Main.netMode != 2)
            {
                m_virtualRes = 0;
                armor = player.statDefense;
                if (Main.netMode == 1)
                {
                    player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * player.statLifeMax2 * GetHealthPerHeart() / 20) + 10,10,Int16.MaxValue);
                    if (player.statLifeMax2>= Int16.MaxValue)
                    {
                        float HealthVirtualMult = Mathf.Clamp((int)(GetHealthMult() * player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue) / player.statLifeMax2;
                        m_virtualRes = 1-(1f / HealthVirtualMult);
                    }
                            
                }
                else
                {
                    player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue);
                }
                
                player.statManaMax2 = (int)(player.statManaMax2 * GetManaPerStar() / 20) + 10;
                player.statDefense = (int)(GetDefenceMult() * player.statDefense * GetArmorMult());
                player.meleeDamage *= GetDamageMult(DamageType.Melee, 2);
                player.thrownDamage *= GetDamageMult(DamageType.Throw, 2);
                player.rangedDamage *= GetDamageMult(DamageType.Ranged, 2);
                player.magicDamage *= GetDamageMult(DamageType.Magic, 2);
                player.minionDamage *= GetDamageMult(DamageType.Summon, 2);

                player.armorPenetration = Mathf.FloorInt( player.armorPenetration*GetArmorPenetrationMult());
                player.armorPenetration += GetArmorPenetrationAdd();


                if (skilltree.ActiveClass != null)
                {
                    player.accRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                    player.moveSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                    player.maxRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
                    player.meleeSpeed *= 1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Speed;
                    player.manaCost *= 1 - JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaCost;


                    if (skilltree.ActiveClass.GetClassType == ClassType.Ninja)
                    {
                        player.thrownVelocity *= 1.2f;
                    }

                    if (skilltree.ActiveClass.GetClassType == ClassType.Shinobi)
                    {
                        player.thrownVelocity *= 1.5f;
                    }
                }
            }
            player.lifeRegen += Mathf.FloorInt(GetHealthRegen());
            player.manaRegenBonus += Mathf.FloorInt(GetManaRegen());
        }
        UpdateModifier();
        //Issue : After one use of item , player can no longer do anything wiht item&inventory
        //ErrorLogger.Log(player.can);
        CustomPostUpdates();
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
        float X = Mathf.Pow(GetStatImproved(Stat.Foc) * statMultiplier + GetStatImproved(Stat.Dex) * statMultiplier, 0.8f) * 0.05f;
        Mathf.Clamp(X, 0, 75);
        return X;
    }
    public float GetCriticalDamage()
    {
        float X = Mathf.Pow(GetStatImproved(Stat.Agi) * statMultiplier + GetStatImproved(Stat.Str) * statMultiplier, 0.8f)*0.005f;
        return 1.4f+X;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (Config.gpConfig.RPGPlayer)
        {
            if (AnotherRpgMod.StatsHotKey.JustPressed)
            {
                Main.PlaySound(SoundID.MenuOpen);
                UI.Stats.Instance.LoadChar();
                UI.Stats.visible = !UI.Stats.visible;
            }
            if (AnotherRpgMod.SkillTreeHotKey.JustPressed)
            {
                Main.PlaySound(SoundID.MenuOpen);


                UI.SkillTreeUi.visible = !UI.SkillTreeUi.visible;
                if (UI.SkillTreeUi.visible)
                    UI.SkillTreeUi.Instance.LoadSkillTree();
            }
        }
            
        if (AnotherRpgMod.ItemTreeHotKey.JustPressed && Config.gpConfig.ItemTree)
        {
            Main.PlaySound(SoundID.MenuOpen);
            UI.ItemTreeUi.visible = !UI.ItemTreeUi.visible;
            if (UI.ItemTreeUi.visible)
            {
                    
                if (ItemUpdate.HaveTree(player.HeldItem))
                    UI.ItemTreeUi.Instance.Open(player.HeldItem.GetGlobalItem<ItemUpdate>());
                else
                    UI.ItemTreeUi.visible = false;
            }

        }

    }
        public float GetBonusHeal()
        {
            if (Config.gpConfig.RPGPlayer)
                return GetHealthPerHeart() /20;
            return 1;
        }
        public float GetBonusHealMana()
        {
            if (Config.gpConfig.RPGPlayer)
                return GetManaPerStar() / 20;
            return 1;
        }

        public float GetHealthRegen()
        {
            return (GetStatImproved(Stat.Vit) + GetStatImproved(Stat.Cons)) * 0.02f * statMultiplier;
        }
        public float GetManaRegen()
        {
            return (GetStatImproved(Stat.Int) + GetStatImproved(Stat.Spr)) * 0.01f * statMultiplier;
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
                if (player.HeldItem.useAmmo == 40)
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
                        return (GetStatImproved(Stat.Int) * MAINSTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                    case DamageType.Ranged:
                        return (GetStatImproved(Stat.Agi) * MAINSTATSMULT + GetStatImproved(Stat.Dex) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                    case DamageType.Summon:
                        return (GetStatImproved(Stat.Spr) * MAINSTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                    case DamageType.Throw:
                        return (GetStatImproved(Stat.Dex) * MAINSTATSMULT + GetStatImproved(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                    case DamageType.Symphonic:
                        return (GetStatImproved(Stat.Agi) * SECONDARYTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) *statMultiplier + 0.8f;
                    case DamageType.Radiant:
                        return (GetStatImproved(Stat.Int) * SECONDARYTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                    case DamageType.Alchemic:
                        return (Stats.GetStat(Stat.Int) * SECONDARYTATSMULT + Stats.GetStat(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                case DamageType.KI:
                        return (GetStatImproved(Stat.Spr) * MAINSTATSMULT + GetStatImproved(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
                default:
                        return (GetStatImproved(Stat.Str) * MAINSTATSMULT + GetStatImproved(Stat.Agi) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
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
            if (Config.gpConfig.RPGPlayer)
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
            SilentLevelUp();
        }
        private void SilentLevelUp()
        {
            int pointsToGain = 5 + Mathf.FloorInt(Mathf.Pow(level, 0.5f));
            totalPoints += pointsToGain;
            freePoints += pointsToGain;
            skillPoints++;
            Stats.OnLevelUp();
            level++;
        }
        public void RecalculateStat()
        {
            int _level = level;
            level = 0;
            totalPoints = 0;
            freePoints = 0;
            Stats = new RPGStats();
            for (int i = 0; i< _level; i++)
            {
                SilentLevelUp();
            }

        }

        public float GetLifeLeech()
        {
            float value = 0;
            if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
            {
                Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                if (Item != null && Item.NeedsSaving(player.HeldItem))
                {
                    if (Config.gpConfig.ItemTree)
                        value = Item.leech;
                    else
                        value = Item.GetLifeLeech*0.01f;
                }
            }
            value += skilltree.GetLeech(LeechType.Life) + skilltree.GetLeech(LeechType.Both);
            return value;
        }
        public float GetManaLeech()
        {
            float value = 0;
            if (player.HeldItem != null && player.HeldItem.damage > 0 && player.HeldItem.maxStack <= 1)
            {
                Items.ItemUpdate Item = player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
                if (Item != null && Item.NeedsSaving(player.HeldItem))
                {
                if (!Config.gpConfig.ItemTree)
                    value = Item.GetManaLeech * 0.01f;
                }
            }
            value += skilltree.GetLeech(LeechType.Magic) + skilltree.GetLeech(LeechType.Both)*0.1f;
            return value;
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
                SendClientChanges(this);
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
            if (skilltree.nodeList.nodeList.Count < _skillLevel.Length)
            {
                ResetSkillTree();
            }
            for (int i = 0; i < _skillLevel.Length; i++)
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
        AnotherRpgMod.Instance.Logger.Info((int)skilltree.ActiveClass.GetParent.ID);
            return (int)skilltree.ActiveClass.GetParent.ID;
        }

        public override TagCompound Save()
        {
            if (Stats == null)
            {
                Stats = new RPGStats();
            }
            if (skilltree == null)
            {
                AnotherRpgMod.Instance.Logger.Warn("save skillTree reset");
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
            if (Stats == null)
                Stats = new RPGStats();
            NodeParent.ResetID();
            if (skilltree == null)
            {
                skilltree = new SkillTree();
            }
                
        }

        public override void Load(TagCompound tag)
        {
            NodeParent.ResetID();
            damageToApply = 0;
            Exp = tag.GetInt("Exp");
            level = tag.GetInt("level");
            LoadStats( tag.GetIntArray("Stats"), tag.GetIntArray("StatsXP"));
            totalPoints = tag.GetInt("totalPoints");
                
            freePoints = tag.GetInt("freePoints");
            skillPoints = level - 1;

            if (tag.GetInt("AnRPGSkillVersion") != SkillTree.SKILLTREEVERSION)
                AnotherRpgMod.Instance.Logger.Warn("AnRPG SkillTree Is Outdated, reseting skillTree");
            else
            {
                LoadSkills(tag.GetIntArray("skillLevel"));
                if (tag.GetInt("activeClass") < skilltree.nodeList.nodeList.Count && tag.GetInt("activeClass")>0)
                {
                    skilltree.ActiveClass = (ClassNode)skilltree.nodeList.nodeList[tag.GetInt("activeClass")].GetNode;
                }
            }
            if (skilltree.ActiveClass == null)
            {
                skilltree.ActiveClass = (ClassNode)skilltree.nodeList.nodeList[0].GetNode;
            }
            
            for (int i = 0; i < skilltree.nodeList.nodeList.Count; i++)
            {
                 NodeParent _node = skilltree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (classNode.GetClassType != skilltree.ActiveClass.GetClassType && classNode.GetActivate)
                    {
                        classNode.Disable(this);
                    }

                }
            }

            NodeParent.ResetID();
            initiated = true;
        }


        public override void PlayerConnect(Player player)
        {
            //player.GetModPlayer<RPGPlayer>().baseName = player.name;
        }

    }

        
        
        
}
