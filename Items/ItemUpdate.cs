using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Utilities;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.Utils;
using AnotherRpgMod.RPGModule;
namespace AnotherRpgMod.Items
{

    
    
    class ItemUpdate : GlobalItem
    {


        string[] AscendName =
        {
            "",
            "Raised ",
            "Mortal ",
            "Raging ",
            "Unleashed ",
            "Rampageous ",
            "Ascended ",
            "transcendental ",
            "Trans-Universal ",
            "Trans-Dimensional "
        };


        public Modifier modifier = Modifier.None;
        public Rarity rarity = Rarity.NONE;
        ItemStats stats = new ItemStats() { Stats = new List<ItemStat>()};


        void Roll(Item item)
        {
            RollInfo info = ModifierManager.RollItem(this, item);
            if (ConfigFile.GetConfig.gpConfig.ItemRarity)
            {
                rarity = info.rarity;
                stats = info.stats;
            }
            if (ConfigFile.GetConfig.gpConfig.ItemRarity)
            {
                modifier = info.modifier;
            }
        }

        public override void PostReforge(Item item)
        {
            Roll(item);
        }
        private bool IsEquiped(Item item)
        {
            if (item.headSlot > 0 || item.legSlot > 0 || item.bodySlot > 0 || item.accessory)
                return true;
            return false;
        }

        public override bool NeedsSaving(Item item)
        {
            if (item.maxStack > 1 && item.ammo < 1)
            {
                if (baseDamage > 0 || IsEquiped(item))
                    return true;
            }
            return false;
        }

        ItemType itemType = ItemType.Other;
        public ItemType Get_ItemType { get { return itemType; } }


        bool init = false;
        int ascendedLevel = 0;
        int baseHealLife = 0;
        int baseHealMana = 0;
        int baseValue = 0;
        Int64 xp = 0;
        int level = 0;

        public int Level { get{ return level; } }
        public int Ascention { get { return ascendedLevel; } }


        List<TooltipLine> AscendToolTip = new List<TooltipLine>();

        public Int64 GetXp { get { return xp; } }
        public Int64 GetMaxXp { get { return GetExpToNextLevel(); } }

        int baseDamage = 0;
        public int BaseDamage { get { return baseDamage; } }

        int baseArmor = 0;
        int baseUseTime = 1;
        int baseMana = 0;

        float lifeLeech = 0;
        float manaLeech = 0;

        public float GetLifeLeech { get { return lifeLeech; } }
        public float GetManaLeech { get { return manaLeech; } }

        public void Ascend()
        {
            ascendedLevel++;
            Main.NewText("weapon ascending !");
            level = 0;
        }

        

        public static Dictionary<Message, List<ItemDataTag>> itemDataTags = new Dictionary<Message, List<ItemDataTag>>()
        {
            { Message.SyncWeapon, new List<ItemDataTag>(){ ItemDataTag.init,ItemDataTag.baseDamage,ItemDataTag.baseArmor,ItemDataTag.baseAutoReuse,ItemDataTag.baseName,ItemDataTag.baseUseTime,ItemDataTag.baseMana,ItemDataTag.level, ItemDataTag.xp, ItemDataTag.ascendedlevel, ItemDataTag.modifier, ItemDataTag.rarity
                , ItemDataTag.statst1, ItemDataTag.stat1
                , ItemDataTag.statst2, ItemDataTag.stat2
                , ItemDataTag.statst3, ItemDataTag.stat3
                , ItemDataTag.statst4, ItemDataTag.stat4
                , ItemDataTag.statst5, ItemDataTag.stat5
                , ItemDataTag.statst6, ItemDataTag.stat6
            } },
        };

        public override void NetSend(Item item, BinaryWriter writer)
        {
            MPDebug.Log(mod, item.Name);
            if (NeedsSaving(item))
            {


                MPDebug.Log(mod, "           __________________________          ");
                MPDebug.Log(mod, Main.netMode);
                MPDebug.Log(mod, item.Name);
                MPDebug.Log(mod, init);
                MPDebug.Log(mod, level);
                MPDebug.Log(mod, ascendedLevel);
                MPDebug.Log(mod, baseDamage);
                MPDebug.Log(mod, baseArmor);
                MPDebug.Log(mod, baseAutoReuse);
                MPDebug.Log(mod, baseUseTime);
                MPDebug.Log(mod, "           __________________________          ");

                writer.Write((byte)Message.SyncWeapon);
                writer.Write(init);

                writer.Write(baseDamage);
                writer.Write(baseArmor);
                writer.Write(baseAutoReuse);
                writer.Write(baseName);
                writer.Write(baseUseTime);
                writer.Write(baseMana);

                writer.Write(level);
                writer.Write(xp);
                writer.Write(ascendedLevel);
                writer.Write((int)modifier);
                writer.Write((int)rarity);




                if (stats.Stats == null)
                    stats.Stats = new List<ItemStat>();

                for (int i = 0;i< 6; i++)
                {
                    if (i< stats.Stats.Count)
                    {
                        writer.Write((sbyte)stats.Stats[i].stat);
                        writer.Write((int)(stats.Stats[i].value*100));
                    }
                    else
                    {
                        writer.Write((sbyte)-1);
                        writer.Write(0);
                    }
                }
            }
            base.NetSend(item, writer);
        }

        public ItemType GetItemTypeCustom(Item item)
        {
            if (item.maxStack > 1)
            {
                if (item.healLife > 0 || item.healMana > 0)
                    return ItemType.Healing;
                return ItemType.Other;
            }
            else
            {
                if (item.accessory)
                    return ItemType.Accessory;
                if (item.bodySlot >0 || item.legSlot > 0 || item.headSlot > 0)
                    return ItemType.Armor;
                if (item.damage > 0)
                    return ItemType.Weapon;
                return ItemType.Other;
            }
        }

        ItemStat ReceivedStat(sbyte stat,int value)
        {
            return new ItemStat((Stat)stat, (float)(value) * 0.01f);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (NeedsSaving(item)) { 
                Message msg = (Message)reader.ReadByte();
                Dictionary<ItemDataTag, object> tags = new Dictionary<ItemDataTag, object>();
                foreach (ItemDataTag tag in itemDataTags[msg])
                {
                    tags.Add(tag, tag.read(reader));
                }
                MPDebug.Log(mod,"           __________________________          ");
                MPDebug.Log(mod, Main.netMode);
                MPDebug.Log(mod, item.Name);
                MPDebug.Log(mod, ItemDataTag.init);
                MPDebug.Log(mod, ItemDataTag.level);
                MPDebug.Log(mod, ItemDataTag.ascendedlevel);
                MPDebug.Log(mod, (int)tags[ItemDataTag.baseDamage]);
                MPDebug.Log(mod, (int)tags[ItemDataTag.baseArmor]);
                MPDebug.Log(mod, (bool)tags[ItemDataTag.baseAutoReuse]);
                MPDebug.Log(mod, (int)tags[ItemDataTag.baseUseTime]);
                MPDebug.Log(mod, "           __________________________          ");
                switch (msg)
                {
                    case Message.SyncWeapon:
                        baseName = (string)tags[ItemDataTag.baseName];
                        init = (bool)tags[ItemDataTag.init];
                        level = (int)tags[ItemDataTag.level];
                        xp = (long)tags[ItemDataTag.xp];
                        ascendedLevel = (int)tags[ItemDataTag.ascendedlevel];
                        
                        baseDamage = (int)tags[ItemDataTag.baseDamage];
                        baseArmor = (int)tags[ItemDataTag.baseArmor];
                        baseAutoReuse = (bool)tags[ItemDataTag.baseAutoReuse];
                        baseUseTime = (int)tags[ItemDataTag.baseUseTime];

                        itemType = GetItemTypeCustom(item);
                        if (itemType == ItemType.Armor)
                            item.defense = getDefense(item);

                        else if (itemType == ItemType.Weapon) { 
                            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                                item.useTime = getUse(item);
                            else
                                item.damage = getDamage(item);
                        }
                        
                        modifier = (Modifier)tags[ItemDataTag.modifier];
                        rarity = (Rarity)tags[ItemDataTag.rarity];


                        stats = new ItemStats
                        {
                            Stats = new List<ItemStat>()
                        };

                        if ((sbyte)tags[ItemDataTag.statst1] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst1], (int)tags[ItemDataTag.stat1]));
                        }
                        if ((sbyte)tags[ItemDataTag.statst2] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst2], (int)tags[ItemDataTag.stat2]));
                        }
                        if ((sbyte)tags[ItemDataTag.statst3] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst3], (int)tags[ItemDataTag.stat3]));
                        }
                        if ((sbyte)tags[ItemDataTag.statst4] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst4], (int)tags[ItemDataTag.stat4]));
                        }
                        if ((sbyte)tags[ItemDataTag.statst5] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst5], (int)tags[ItemDataTag.stat5]));
                        }
                        if ((sbyte)tags[ItemDataTag.statst6] >= 0)
                        {
                            stats.Stats.Add(ReceivedStat((sbyte)tags[ItemDataTag.statst6], (int)tags[ItemDataTag.stat6]));
                        }
                        

                        baseMana = (int)tags[ItemDataTag.baseMana];
                        init = true;
                        break;
                }
            }
            base.NetReceive(item,reader);
        }

        bool baseAutoReuse = false;

        
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int Projectileid = type;
            if (ModifierManager.HaveModifier(Modifier.Random, modifier))
            {
                Projectileid = Mathf.RandomInt(1, 500);
                float spread = 10 * 0.0174f; //20 degree cone
                float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY) * (1 + 0.1f * Main.rand.NextFloat());
                double baseAngle = Math.Atan2(speedX, speedY);
                double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                float spdX = baseSpeed * (float)Math.Sin(randomAngle);
                float spdY = baseSpeed * (float)Math.Cos(randomAngle);
                int projnum = Projectile.NewProjectile(position.X, position.Y, spdX, spdY, Projectileid, damage, knockBack, player.whoAmI);
                Main.projectile[projnum].friendly = true;
                Main.projectile[projnum].hostile = false;

            }
            for (int i = 0; i < ascendedLevel; i++) { 
                float spread = 10 * 0.0174f; //20 degree cone
                float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY) * (1 + 0.1f * Main.rand.NextFloat());
                double baseAngle = Math.Atan2(speedX, speedY);
                double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                float spdX = baseSpeed * (float)Math.Sin(randomAngle);
                float spdY = baseSpeed * (float)Math.Cos(randomAngle);

                int projnum = Projectile.NewProjectile(position.X, position.Y, spdX, spdY, Projectileid, damage, knockBack, player.whoAmI);
                Main.projectile[projnum].friendly = true;
                Main.projectile[projnum].hostile = false;
            }
            return true;
        }
        
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {

            RPGPlayer rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (crit)
            {
                damage = (int)(0.5f * damage * rpgPlayer.GetCriticalDamage());
            }
            if (target.type != 488)
                rpgPlayer.AddWeaponXp(damage,item);
            rpgPlayer.Leech(damage);
            base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
        }

        string baseName = "";
        public override void Load(Item item, TagCompound tag)
        {
            if (init)
                return;
            //item.r
            xp = tag.GetAsLong("Exp");
            level = tag.GetInt("level");
            ascendedLevel = tag.GetInt("ascendedLevel");

            rarity = (Rarity)tag.GetInt("rarity");
            modifier = (Modifier)tag.GetInt("modifier");

            List<ItemStat> itemstatslist = new List<ItemStat>();
            for (int i = 0;i< tag.GetIntArray("stats").Length * 0.5f; i++)
            {

                itemstatslist.Add(new ItemStat((Stat)tag.GetIntArray("stats")[i * 2], tag.GetIntArray("stats")[i * 2 + 1]*0.01f));
            }
            stats = new ItemStats()
            {
                Stats = itemstatslist
            };



            baseDamage = item.damage;
            baseArmor = item.defense;
            baseAutoReuse = item.autoReuse;
            baseName = item.Name;
            baseUseTime = item.useTime;


            itemType = GetItemTypeCustom(item);
            if (itemType == ItemType.Armor)
                item.defense = getDefense(item);
            else if (itemType == ItemType.Weapon) {
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                    item.useTime = getUse(item);
                else
                    item.damage = getDamage(item);
            }
            
            baseValue = item.value;
            baseMana = item.mana;

            if (item.healLife>0)
                baseHealLife = item.healLife;
            if (item.healMana > 0)
                baseHealMana = item.healMana;


            if (rarity == Rarity.NONE)
            {
                Roll(item);
            }
            if (stats.Stats == null)
                stats.Stats = new List<ItemStat>();
            init = true;


            item.SetNameOverride(SetName(item));

        }

        private string SetName(Item item)
        {

            int maxascend = Mathf.Clamp(ascendedLevel, 0, AscendName.Length-1);
            string prefix = "";
            if (rarity != Rarity.NONE)
                prefix += Enum.GetName(typeof(Rarity), rarity) + " ";
            prefix += AscendName[maxascend];
            string sufix = "";
            if (level > 0)
                sufix = " +" + level;

            if (baseName == "")
            {
                baseName = item.Name;
            }

            return (prefix + baseName +sufix );
        }

        public override TagCompound Save(Item item)
        {

            int[] statsArray = new int[0];
            if (stats.Stats != null&& stats.Stats.Count > 0) { 
                statsArray = new int[stats.Stats.Count * 2];
                for (int i = 0; i < stats.Stats.Count;i++)
                {
                    statsArray[(i * 2)] = (int)stats.Stats[i].stat;
                    statsArray[(i*2) + 1] = (int)(stats.Stats[i].value*100);
                }
            }


            return new TagCompound
            {
                {"Exp", xp},
                {"level",level },
                {"ascendedLevel",ascendedLevel },
                {"rarity",(int)rarity },
                {"modifier",(int)modifier },
                {"stats", statsArray}
            };
        }

        
        private float StatLevel(float statsvalue)
        {
            statsvalue = (statsvalue + (1 + level) * statsvalue * (1f / 40f)) * (1 + ascendedLevel * .05f);
            return statsvalue;
        }

        public float GetStatSlot(int slot)
        {
            return StatLevel(stats.Stats[slot].value);
        }

        
        public float GetStat(Stat stat)
        {
            float value = 0;
            for (int i = 0; i < stats.Stats.Count; i++)
            {
                if (stats.Stats[i].stat == stat)
                    value += stats.Stats[i].value;
            }
            return value;
        }
        

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {

            if (item.healLife > 0)
            {
                TooltipLine healtt = tooltips.Find(x => x.Name == "HealLife");
                if (healtt != null)
                {
                    int iheal = tooltips.FindIndex(x => x.Name == "HealLife");
                    healtt = new TooltipLine(mod, "HealLife", healtt.text + "( " + baseHealLife + " )");
                    tooltips[iheal] = healtt;
                }

            }
            if (item.healMana > 0)
            {
                TooltipLine healtt = tooltips.Find(x => x.Name == "HealMana");
                if (healtt != null)
                {
                    int iheal = tooltips.FindIndex(x => x.Name == "HealMana");
                    healtt = new TooltipLine(mod, "HealMana", healtt.text + "( " + baseHealMana + " )");
                    tooltips[iheal] = healtt;
                }
            }

            if (NeedsSaving(item))
            {

                
                if (itemType == ItemType.Accessory || itemType == ItemType.Armor || itemType == ItemType.Weapon) {

                    /*
                    if (itemType == ItemType.Weapon) { 
                        TooltipLine ITT = new TooltipLine(mod, "WeaponType",ItemUtils.GetWeaponType(item).ToString());
                        tooltips.Add(ITT);
                    }*/
                    if (ConfigFile.GetConfig.gpConfig.ItemRarity)
                    {
                        TooltipLine Rtt;
                        if (rarity != Rarity.NONE)
                            Rtt = new TooltipLine(mod, "Rarity", Enum.GetName(typeof(Rarity), rarity))
                            {
                                overrideColor = ModifierManager.GetRarityColor(rarity)
                            };
                        else
                        {
                            Rtt = new TooltipLine(mod, "Rarity", "???")
                            {
                                overrideColor = Color.Pink
                            };
                        }
                        tooltips.Add(Rtt);
                    }

                    if ((itemType == ItemType.Accessory || itemType == ItemType.Armor )&& ConfigFile.GetConfig.gpConfig.ItemRarity) { 
                        if (stats.Stats.Count > 0)
                        {
                            for (int i = 0; i < stats.Stats.Count; i++)
                            {
                                TooltipLine Stt;
                                RPGPlayer p = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                                if (GetStatSlot(i) > 0)
                                    Stt = new TooltipLine(mod, "statsInfo" + i, Enum.GetName(typeof(Stat), stats.Stats[i].stat) + " +" + GetStatSlot(i) + " % ( +" + Mathf.Round(p.GetStat(stats.Stats[i].stat) * GetStatSlot(i) * 0.01f, 1) + " )")
                                    {
                                        overrideColor = Color.LimeGreen
                                    };
                                else
                                    Stt = new TooltipLine(mod, "statsInfo" + i, Enum.GetName(typeof(Stat), stats.Stats[i].stat) + " -" + Math.Abs(GetStatSlot(i)) + " % ( " + Mathf.Round(p.GetStat(stats.Stats[i].stat) * GetStatSlot(i) * 0.01f, 1) + " )")
                                    {
                                        overrideColor = Color.PaleVioletRed
                                    };

                        tooltips.Add(Stt);
                            }
                        }
                    }
                    if (itemType == ItemType.Weapon && ConfigFile.GetConfig.gpConfig.ItemRarity)
                    {
                        TooltipLine Stt;
                        RPGPlayer p = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                        if (ModifierManager.GetRarityDamageBoost(rarity) >0)
                            Stt = new TooltipLine(mod, "statsInfo", "Rarity bonus + " + ModifierManager.GetRarityDamageBoost(rarity) + "% Damage")
                            {
                                overrideColor = ModifierManager.GetRarityColor(rarity)
                            };
                        else
                        {
                            Stt = new TooltipLine(mod, "statsInfo", "Rarity bonus " + ModifierManager.GetRarityDamageBoost(rarity) + "% Damage")
                            {
                                overrideColor = ModifierManager.GetRarityColor(rarity)
                            };
                        }
                        tooltips.Add(Stt);
                    }

                    if (ConfigFile.GetConfig.gpConfig.ItemModifier) { 
                        String[] SList = ModifierManager.GetModifierDesc(this);

                        for (int i = 0; i < SList.Length; i++)
                        {
                            
                            TooltipLine Mtt = new TooltipLine(mod, "Modifier"+i, SList[i])
                            {
                                overrideColor = ModifierManager.GetRarityColor(rarity)
                            };
                            tooltips.Add(Mtt);
                        }
                    }
                }
                //BASE EXP BONUS FROM ITEM
                if (!item.accessory && itemType != ItemType.Healing) { 
                    if (itemType == ItemType.Weapon)
                    {
                        TooltipLine bt = new TooltipLine(mod, "BaseDamage", "" + baseDamage + " Base Damage");
                        tooltips.Add(bt);
                    }
                    else if (itemType == ItemType.Armor)
                    {
                        TooltipLine bt = new TooltipLine(mod, "BaseDefense", "" + baseArmor + " Base Defense");
                        tooltips.Add(bt);
                    }

                    if (level > 0) { 
                        if (itemType == ItemType.Weapon) { 
                            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                            {
                                TooltipLine stt = new TooltipLine(mod, "BonusSpeedstuff", "+" + Math.Round(((Mathf.Pow(1.02, level)-1) * 100 + (Mathf.Pow( 1.1,ascendedLevel)-1)*100),1) + "% Speed")
                                {
                                    isModifier = true
                                };
                                tooltips.Add(stt);
                            }
                            else { 
                                    TooltipLine tt = new TooltipLine(mod, "PrefixDamage", "+" + (((float)level * 5f)+ascendedLevel*50f) + "% Damage")
                                    {
                                        isModifier = true
                                    };
                                    tooltips.Add(tt);
                            }

                        }
                        if (itemType == ItemType.Armor)
                        {
                            TooltipLine tt = new TooltipLine(mod, "PrefixDefense", "+" + Mathf.CeilInt(((float)level * 0.5f) + baseArmor*ascendedLevel * 0.3f) + " Defense")
                            {
                                isModifier = true
                            };
                            tooltips.Add(tt);
                        
                        
                        }

                    }
                    TooltipLine xptt = new TooltipLine(mod, "Experience", "Xp : " + GetXp + "/" + GetMaxXp)
                    {
                        isModifier = true
                    };
                    tooltips.Add(xptt);
                
                    for (int i = 0;i< AscendToolTip.Count; i++)
                    {
                        tooltips.Add(AscendToolTip[i]);
                    }
                }
                
            }
       
 }

        public int getDamage(Item item)
        {
            int _damage = baseDamage;

            _damage = (int)(((float)_damage * (1f+ ascendedLevel*0.5f + (float)level / 20f)) * (1 + ModifierManager.GetRarityDamageBoost(rarity) * 0.01f));
            return _damage;
        }

        public int getUse(Item item)
        {
            int _use = baseUseTime;

            float _useReduction = Mathf.Pow(0.9, ascendedLevel) * Mathf.Pow(0.98, level);
            _use = Mathf.FloorInt( _use* _useReduction);
            return _use;

        }
            public int getDefense(Item item)
        {
            int _defense = baseArmor;

            _defense = Mathf.CeilInt(_defense*(1+ascendedLevel*0.3f) + level*0.5f);
            return _defense;
        }

        public Int64 GetExpToNextLevel()
        {
                if (itemType == ItemType.Weapon)
                {
                    if (level <= 11)
                        return Mathf.Floorlong((level + 1) * 50 + Mathf.Pow(level * (baseDamage * 0.4f) * (1 + 15 / baseUseTime), 2.15f) * Mathf.Pow(1.5f, ascendedLevel));
                    else
                        return Mathf.Floorlong((level + 1) * 50 + Mathf.Pow(level * (baseDamage * 0.4f) * (1 + 15 / baseUseTime), 2.2f) * Mathf.Pow(1.5f, ascendedLevel))*Mathf.Floorlong(1 + level/10);
                }
                else
                    return Mathf.Floorlong((level + 1) * 50 + baseArmor * 10 * level);
            
        }

        public void LevelUp(Player player,Item item)
        {
            xp -= GetExpToNextLevel();
            if (itemType == ItemType.Armor)
                CombatText.NewText(player.getRect(), new Color(255, 26, 255), "Armor upgrade !", true);
            if (itemType == ItemType.Weapon)
                CombatText.NewText(player.getRect(), new Color(255, 26, 255), "Weapon upgrade !",true);
            level++;
            if (level > (ascendedLevel+1) * 10)
            {
                Ascend();
            }
            item.SetNameOverride(SetName(item));
        }
        public void AddExp(Int64 _xp,Player player,Item item)
        {
            if (ModifierManager.HaveModifier(Modifier.SelfLearning, modifier) && !Main.dayTime)
            {
                _xp*= 1 + (long)ModifierManager.GetModifierBonus(Modifier.SelfLearning, this) * (long)0.01f;
            }
            xp += Mathf.Clamp(_xp* (Int64)ConfigFile.GetConfig.gpConfig.ItemXpMultiplier, (Int64)0, Int64.MaxValue);
            while (xp >= GetExpToNextLevel())
            {
                LevelUp(player, item);
            }
        }

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public override bool CloneNewInstances
        {
            get
            {
                return true;
            }
        }

        private void InitItem(Item item,RPGPlayer character)
        {
            

            if (init)
                return;
            init = true;
            if (NeedsSaving(item))
            {
                baseMana = item.mana;
                baseAutoReuse = item.autoReuse;
                baseUseTime = item.useTime;
                baseName = item.Name;
                baseValue = item.value;
                itemType = GetItemTypeCustom(item);
                baseArmor = item.defense;
                baseDamage = item.damage;
                Roll(item);

                
            }

            if (item.healLife > 0)
                baseHealLife = item.healLife;

            if (item.healMana > 0)
                baseHealMana = item.healMana;
            if (stats.Stats == null)
                stats.Stats = new List<ItemStat>();
            
        }

        public override bool ConsumeAmmo(Item item, Player player)
        {
            if (ascendedLevel>0)
                return Main.rand.NextFloat() >= .5f;
            return base.ConsumeAmmo(item, player);
        }


        public void EquipedUpdateModifier(Item item,Player player)
        {
            if (ModifierManager.HaveModifier(Modifier.Thorny, modifier))
            {
                player.thorns += ModifierManager.GetModifierBonus(Modifier.Thorny, this) * 0.01f;
            }
            if (ModifierManager.HaveModifier(Modifier.VampiricAura, modifier))
            {
                for (int j = 0; j < Main.npc.Length; j++)
                {
                    float damageToApply = ModifierManager.GetModifierBonus(Modifier.VampiricAura, this)*(1f/60f);
                    if (Vector2.Distance(Main.npc[j].position, player.position) < 1000 && !Main.npc[j].townNPC)
                    {
                        int heal = Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyVampricAura(Main.npc[j], damageToApply);
                        player.GetModPlayer<RPGPlayer>().ApplyReduction(ref heal);
                        player.statLife = Mathf.Clamp(player.statLife + heal, player.statLife, player.statLifeMax2);

                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(21, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            if (ModifierManager.HaveModifier(Modifier.FireLord, modifier))
            {
                for (int j = 0; j < Main.npc.Length; j++)
                {
                    if (!Main.npc[j].townNPC)
                    {

                        if (Vector2.Distance(Main.npc[j].position, player.position) < ModifierManager.GetModifierBonus(Modifier.FireLord, this))
                        {
                            Main.npc[j].AddBuff(BuffID.OnFire, 15);
                        }
                    }
                }
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (NeedsSaving(item))
            {
                if (rarity == Rarity.NONE)
                {
                    Roll(item);
                }
            }

            RPGPlayer character = player.GetModPlayer<RPGPlayer>();
            if (character != null)
            {

                AscendToolTip = new List<TooltipLine>();
                
                if (NeedsSaving(item) && (level > 0 || ascendedLevel > 0))
                {

                    baseValue = (int)(item.value * (1 + Mathf.Pow(ascendedLevel, 2f) + level * 0.1f));
                    if (itemType == ItemType.Armor)
                    {
                        item.defense = getDefense(item);
                        if (ascendedLevel > 0)
                        {
                            AscendToolTip.Add(new TooltipLine(mod, "Ascding", "Ascending Tier ." + ascendedLevel) { isModifier = true });
                        }
                    }
                }
            }
            InitItem(item, character);
            
        }

        public override void UpdateInventory(Item item, Player player)
        {
            RPGPlayer character = player.GetModPlayer<RPGPlayer>();
            if (NeedsSaving(item))
            {
                item.SetNameOverride(SetName(item));
            }
            if (character != null)
            {
                AscendToolTip = new List<TooltipLine>();
                InitItem(item, character);
                
                

                if (NeedsSaving(item) && (level > 0 || ascendedLevel > 0))
                {
                    
                    
                    baseValue = (int)(item.value * (1 + Mathf.Pow(ascendedLevel,2f) + level*0.1f));
                    if (itemType == ItemType.Armor)
                    {
                        item.defense = getDefense(item);
                        if (ascendedLevel > 0)
                        {
                            AscendToolTip.Add(new TooltipLine(mod, "Ascding", "Ascending Tier ." + ascendedLevel) { isModifier = true });
                        }
                    }
                    if (itemType == ItemType.Weapon) {
                        if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                        {
                            item.useTime = getUse(item);
                        }
                        else
                            item.damage = getDamage(item);
                        
                        if (ascendedLevel > 0)
                        {
                            AscendToolTip.Add(new TooltipLine(mod, "Ascding", "Ascending Tier ." + ascendedLevel) { isModifier = true });

                            if (!baseAutoReuse && !item.magic && !item.summon)
                                AscendToolTip.Add(new TooltipLine(mod, "AscdAutoSwing", "Have AutoUse") { isModifier = true });
                            if (item.ranged)
                                AscendToolTip.Add(new TooltipLine(mod, "ascdProjectile", "+ " + ascendedLevel + " Projectiles") { isModifier = true });

                            if ((baseAutoReuse ) || (ascendedLevel > 1))
                            {
                                    AscendToolTip.Add(new TooltipLine(mod, "AscdAutoSwingBonus", "+ 40% attack speed") { isModifier = true });
                                    item.useTime = Mathf.CeilInt(baseUseTime * 0.6f);
                            }
                            if (baseMana > 0)
                            {
                                AscendToolTip.Add(new TooltipLine(mod, "AscdAManaUse", "50% Mana Reduction") { isModifier = true });
                                item.mana = Mathf.CeilInt(baseMana * 0.5f);
                            }
                            if (item.summon ) {
                                AscendToolTip.Add(new TooltipLine(mod, "AscdMaxMinion", "Max minion +"+ ascendedLevel) { isModifier = true });
                                /*
                                if (player.HeldItem == item)
                                    player.maxMinions+=ascendedLevel;
                                    */
                            }
                            if (!item.magic&&!item.summon)
                            item.autoReuse = true;
                        }
                        if (ascendedLevel > 1)
                        {
                        
                            if (item.melee)
                            {
                                for (int i = 0; i < ascendedLevel; i++)
                                {
                                    lifeLeech = i * 0.5f;
                                }
                                AscendToolTip.Add(new TooltipLine(mod, "AscdLifeLeech", "+ " + lifeLeech + "% LifeLeech") { isModifier = true });

                            }
                            if (item.magic)
                            {
                                for (int i = 0; i < ascendedLevel; i++)
                                {
                                    manaLeech = i;
                                }
                                AscendToolTip.Add(new TooltipLine(mod, "AscdManaLeech", "+ " + manaLeech + "% ManaLeech") { isModifier = true });
                            }
                            if (item.summon)
                            {

                                AscendToolTip.Add(new TooltipLine(mod, "AscdMinionDamage", "+ 50% Minion Damage") { isModifier = true });
                                player.minionDamage *= 1.5f;
                            }
                            if (item.useAmmo > 0)
                            {
                                AscendToolTip.Add(new TooltipLine(mod, "AscdMinionDamage", "50% Chance Not to use ammo") { isModifier = true });
                            }

                        }
                    }
                    
                }

                if (GetItemTypeCustom(item) == ItemType.Healing)
                {
                    if (item.healLife > 0)
                        item.healLife = Mathf.CeilInt(baseHealLife * character.GetBonusHeal());

                    if (item.healMana > 0)
                        item.healMana = Mathf.CeilInt(baseHealMana * character.GetBonusHealMana());
                }




            }
            
        }

    }
}
