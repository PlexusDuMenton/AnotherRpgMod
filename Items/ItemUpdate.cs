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
namespace AnotherRpgMod.Items
{

    public class ItemDataTag
    {
        public static ItemDataTag level = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag xp = new ItemDataTag(reader => reader.ReadInt64());
        public static ItemDataTag ascendedlevel = new ItemDataTag(reader => reader.ReadInt32());

        public Func<BinaryReader, object> read;

        public ItemDataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }

    internal enum ItemType
    {
        Other,
        Healing,
        Accessory,
        Weapon,
        Armor
    }
    class ItemUpdate : GlobalItem
    {

        string[] AscendName =
        {
            "",
            "Ascended ",
            "transcendental ",
            "Trans-Universal ",
            "Trans-Dimensional "
        };

        public override bool NeedsSaving(Item item)
        {
            return item.maxStack == 1 && (item.damage > 0 || (item.defense > 0 && !item.accessory));
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

        List<TooltipLine> AscendToolTip = new List<TooltipLine>();

        public Int64 GetXp { get { return xp; } }
        public Int64 GetMaxXp { get { return GetExpToNextLevel(); } }

        public void Ascend()
        {
            ascendedLevel++;
            Main.NewText("weapon ascending !");
            level = 0;
        }

        int baseDamage = 0;
        int baseArmor = 0;
        int baseUseTime = 1;
        int baseMana = 0;

        float lifeLeech = 0;
        float manaLeech = 0;

        public float GetLifeLeech { get { return lifeLeech; } }
        public float GetManaLeech { get { return manaLeech; } }

        public static Dictionary<Message, List<ItemDataTag>> itemDataTags = new Dictionary<Message, List<ItemDataTag>>()
        {
            { Message.SyncWeapon, new List<ItemDataTag>(){ ItemDataTag.level, ItemDataTag.xp, ItemDataTag.ascendedlevel } },
        };

        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (NeedsSaving(item))
            {
                writer.Write((byte)Message.SyncWeapon);
                writer.Write(level);
                writer.Write(xp);
                writer.Write(ascendedLevel);
            }
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
                if (item.defense > 0)
                    return ItemType.Armor;
                if (item.damage > 0)
                    return ItemType.Weapon;
                return ItemType.Other;
            }
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
                switch (msg)
                {
                    case Message.SyncWeapon:
                        baseName = item.Name;
                        level = (int)tags[ItemDataTag.level];
                        xp = (long)tags[ItemDataTag.xp];
                        ascendedLevel = (int)tags[ItemDataTag.ascendedlevel];
                        baseDamage = item.damage;
                        baseArmor = item.defense;
                        
                        baseAutoReuse = item.autoReuse;

                        baseUseTime = item.useTime;
                        itemType = GetItemTypeCustom(item);
                        if (itemType == ItemType.Armor)
                            item.defense = getDefense(item);
                        else if (itemType == ItemType.Weapon)
                            item.damage = getDamage(item);
                        baseMana = item.mana;
                        init = true;
                        break;
                }
            }
        }

        bool baseAutoReuse = false;
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < ascendedLevel; i++) { 
                float spread = 10 * 0.0174f; //20 degree cone
                float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY) * (1 + 0.1f * Main.rand.NextFloat());
                double baseAngle = Math.Atan2(speedX, speedY);
                double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                float spdX = baseSpeed * (float)Math.Sin(randomAngle);
                float spdY = baseSpeed * (float)Math.Cos(randomAngle);

                Projectile.NewProjectile(position.X, position.Y, spdX, spdY, type, damage, knockBack, player.whoAmI);
            }
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
        string baseName = "";
        public override void Load(Item item, TagCompound tag)
        {
            //item.r
            xp = tag.GetAsLong("Exp");
            level = tag.GetInt("level");
            ascendedLevel = tag.GetInt("ascendedLevel");
            baseDamage = item.damage;
            baseArmor = item.defense;
            baseAutoReuse = item.autoReuse;
            baseName = item.Name;
            baseUseTime = item.useTime;
            itemType = GetItemTypeCustom(item);
            if (itemType == ItemType.Armor)
                item.defense = getDefense(item);
            else if (itemType == ItemType.Weapon)
                item.damage = getDamage(item);
            baseValue = item.value;
            baseMana = item.mana;
            
            init = true;
            if (level > 0)
                item.SetNameOverride(SetName());
        }

        private string SetName()
        {

            int maxascend = Mathf.Clamp(ascendedLevel, 0, AscendName.Length-1);

            return (AscendName[maxascend] + "" + baseName + " +" + level );
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound
            {
                {"Exp", xp},
                {"level",level },
                {"ascendedLevel",ascendedLevel }
            };
        }

        


        

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (NeedsSaving(item))
            {
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
                    
                        if (tooltips.Find(x => x.Name == "PrefixDamage") == null)
                        {
                            TooltipLine tt = new TooltipLine(mod, "PrefixDamage", "+" + (((float)level * 5f)+ascendedLevel*0f) + "% Damage")
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

        public int getDamage(Item item)
        {
            int _damage = baseDamage;

            _damage = (int)((float)_damage * (1f+ ascendedLevel*0.5f + (float)level / 20f));
            return _damage;
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
                return Mathf.Floorlong((level + 1) * 50 + Mathf.Pow(level * (baseDamage * 0.5f) * (1 + 15 / baseUseTime), 2.25f) * Mathf.Pow(ascendedLevel + 1,2));
            else
                return Mathf.Floorlong((level + 1) * 50 + baseArmor * 10 * level);
        }

        public void LevelUp(Player player)
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
        }
        public void AddExp(Int64 _xp,Player player)
        {
            xp += Mathf.Clamp(_xp,(Int64)0, Int64.MaxValue);
            while (xp >= GetExpToNextLevel())
            {
                LevelUp(player);
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
            }
            if (item.healLife > 0)
                baseHealLife = item.healLife;

            if (item.healMana > 0)
                baseHealMana = item.healMana;
            init = true;
        }

        public override bool ConsumeAmmo(Item item, Player player)
        {
            if (ascendedLevel>0)
                return Main.rand.NextFloat() >= .5f;
            return base.ConsumeAmmo(item, player);
        }

        public override void UpdateEquip(Item item, Player player)
        {
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
            if (character != null)
            {
                AscendToolTip = new List<TooltipLine>();
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
                        item.damage = getDamage(item);
                        if (ascendedLevel > 0)
                        {
                            AscendToolTip.Add(new TooltipLine(mod, "Ascding", "Ascending Tier ." + ascendedLevel) { isModifier = true });

                            if (!baseAutoReuse && !item.magic && !item.summon)
                                AscendToolTip.Add(new TooltipLine(mod, "AscdAutoSwing", "Have AutoUse") { isModifier = true });
                            if (item.shoot > 0)
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
                            if (item.summon) {
                                AscendToolTip.Add(new TooltipLine(mod, "AscdMaxMinion", "Max minion +"+ ascendedLevel) { isModifier = true });
                                player.maxMinions++;
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
                                    lifeLeech = i * 0.2f;
                                }
                                AscendToolTip.Add(new TooltipLine(mod, "AscdLifeLeech", "+ " + lifeLeech + "% LifeLeech") { isModifier = true });

                            }
                            if (item.magic)
                            {
                                for (int i = 0; i < ascendedLevel; i++)
                                {
                                    manaLeech = i * 2.5f;
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
                    item.SetNameOverride(SetName());
                }


                InitItem(item, character);       
                if (item.healLife > 0)
                    item.healLife = Mathf.CeilInt(baseHealLife * character.GetBonusHeal());

                if (item.healMana > 0)
                    baseHealMana = Mathf.CeilInt(baseHealMana * character.GetBonusHealMana());
                
                
            }
            
        }

    }
}
