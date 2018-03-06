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
        bool init = false;
        int baseHealLife = 0;
        int baseHealMana = 0;

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

            if (item.healLife > 0)
                baseHealLife = item.healLife;

            if (item.healMana > 0)
                baseHealMana = item.healMana;
            init = true;
        }

        public override void UpdateInventory(Item item, Player player)
        {
            RPGPlayer character = player.GetModPlayer<RPGPlayer>();
            if (character != null)
            {
                InitItem(item, character);       
                if (item.healLife > 0)
                    item.healLife = Mathf.CeilInt(baseHealLife * character.GetBonusHeal());
                if (item.healMana > 0)
                    baseHealMana = Mathf.CeilInt(baseHealMana * character.GetBonusHealMana());
                
                
            }
            
        }

    }
}
