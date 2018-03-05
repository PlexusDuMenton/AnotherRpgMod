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
    class ItemUpdate : GlobalItem
    {
        public override bool UseItem(Item item, Player player)
        {
            RPGPlayer character = player.GetModPlayer<RPGPlayer>();
            if (character != null)
            {
                if (item.healLife > 0)
                {
                    int heal = Mathf.CeilInt(item.healLife * character.GetBonusHeal())- item.healLife;
                    player.statLife = Mathf.Clamp(player.statLife+heal,0, player.statLifeMax2);

                }
                if (item.healMana > 0)
                {
                    int mana = Mathf.CeilInt(item.healMana * character.GetBonusHealMana())- item.healMana;
                    player.statMana = Mathf.Clamp(player.statLife + mana, 0, player.statManaMax2);
                }
            }
            return base.UseItem(item, player);
        }

    }
}
