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
        public static ItemDataTag xp = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag ascendedlevel = new ItemDataTag(reader => reader.ReadInt32());

        public Func<BinaryReader, object> read;

        public ItemDataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
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
            return item.maxStack == 1 && (item.damage > 0);
        }



        bool init = false;
        int ascendedLevel = 0;
        int baseHealLife = 0;
        int baseHealMana = 0;

        int xp = 0;
        int level = 0;

        List<TooltipLine> AscendToolTip = new List<TooltipLine>();

        public int GetXp { get { return xp; } }
        public int GetMaxXp { get { return GetExpToNextLevel(); } }

        public void Ascend()
        {
            ascendedLevel++;
            Main.NewText("weapon ascending !");
            level = 0;
        }

        int baseDamage = 0;
        int baseUseTime = 0;
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

        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (NeedsSaving(item)) { 
                Message msg = (Message)reader.ReadByte();
                Dictionary<ItemDataTag, object> tags = new Dictionary<ItemDataTag, object>();
                foreach (ItemDataTag tag in itemDataTags[msg])
                {
                    tags.Add(tag, tag.read(reader));
                }
                ErrorLogger.Log(msg);
                switch (msg)
                {
                    case Message.SyncWeapon:
                        ErrorLogger.Log(item.Name);
                        baseName = item.Name;
                        level = (int)tags[ItemDataTag.level];
                        xp = (int)tags[ItemDataTag.xp];
                        ascendedLevel = (int)tags[ItemDataTag.ascendedlevel];
                        baseDamage = item.damage;
                        baseAutoReuse = item.autoReuse;

                        baseUseTime = item.useTime;
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
            xp = tag.GetInt("Exp");
            level = tag.GetInt("level");
            ascendedLevel = tag.GetInt("ascendedLevel");
            baseDamage = item.damage;
            baseAutoReuse = item.autoReuse;
            baseName = item.Name;
            baseUseTime = item.useTime;
            item.damage = getDamage(item);
            baseMana = item.mana;
            init = true;
            if (level > 0)
                item.SetNameOverride(SetName());
        }

        private string SetName()
        {

            int maxascend = Mathf.Clamp(ascendedLevel, 0, AscendName.Length);
            return (AscendName[maxascend] + "" + baseName + " +" + level);
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

        


        public int GetExpToNextLevel()
        {
            //return 5; // for debug
            return Mathf.FloorInt((level+1) * 50 + Mathf.Pow(level * (baseDamage*0.5f), 2.2f));
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (NeedsSaving(item))
            {
                if (level > 0)
                    if (tooltips.Find(x => x.Name == "PrefixDamage") == null && level > 0)
                    {
                        TooltipLine tt = new TooltipLine(mod, "PrefixDamage", "+" + (((float)level * 10f)+ascendedLevel) + "% Damage")
                        {
                            isModifier = true
                        };
                        tooltips.Add(tt);
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

            _damage = (int)((float)_damage * (1f+ ascendedLevel + (float)level / 10f));
            return _damage;
        }

        

        public void LevelUp(Player player)
        {
            xp -= GetExpToNextLevel();
            CombatText.NewText(player.getRect(), new Color(255, 26, 255), "Weapon upgrade !");
            level++;
            if (level > (ascendedLevel+1) * 10)
            {
                Ascend();
            }
        }
        public void AddExp(int _xp,Player player)
        {
            xp += _xp;
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


        public override void UpdateInventory(Item item, Player player)
        {
            RPGPlayer character = player.GetModPlayer<RPGPlayer>();
            if (character != null)
            {
                AscendToolTip = new List<TooltipLine>();
                if (NeedsSaving(item) && (level > 0 || ascendedLevel > 0))
                {
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
                                    lifeLeech = i * 7.5f;
                                }
                                AscendToolTip.Add(new TooltipLine(mod, "AscdLifeLeech", "+ "+ lifeLeech + "% LifeLeech") { isModifier = true });
                                
                            }
                            if (item.magic)
                            {
                                for (int i = 0; i < ascendedLevel; i++)
                                {
                                    manaLeech = i * 7.5f;
                                }
                                AscendToolTip.Add(new TooltipLine(mod, "AscdManaLeech", "+ " + manaLeech + "% ManaLeech") { isModifier = true });
                            }
                            if (item.summon)
                            {

                                AscendToolTip.Add(new TooltipLine(mod, "AscdMinionDamage", "+ 50% Minion Damage") { isModifier = true });
                                player.minionDamage *= 1.5f;
                            }
                            if (item.useAmmo>0)
                            {
                            AscendToolTip.Add(new TooltipLine(mod, "AscdMinionDamage", "50% Chance Not to use ammo") { isModifier = true });
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
