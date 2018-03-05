using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using AnotherRpgMod.UI;

namespace AnotherRpgMod
{
    public enum DamageType : byte
    {
        Melee,
        Ranged,
        Throw,
        Magic,
        Summon
    }


    class Arpg : Mod
	{

        public UserInterface customResources;
        public HealthBar healthBar;
        public UserInterface customstats;
        public UserInterface customOpenstats;
        public Stats statMenu;
        public OpenStatsButton openStatMenu;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                customResources = new UserInterface();
                healthBar = new HealthBar();
                HealthBar.visible = true;
                customResources.SetState(healthBar);

                customstats = new UserInterface();
                statMenu = new Stats();
                Stats.visible = false;
                customstats.SetState(statMenu);



                customOpenstats = new UserInterface();
                openStatMenu = new OpenStatsButton();
                OpenStatsButton.visible = true;
                customOpenstats.SetState(openStatMenu);
                

                /*
                
                statMenu = new Stats();
                Stats.visible = true;
                customstats.SetState(statMenu);
                */
            }
        }

        public Arpg()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

        public override void UpdateUI(GameTime gameTime)
        {
            if (customstats != null)
                customstats.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int id = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (id != -1)
            {
                //layers.RemoveAt(id);

                    //Add you own layer
                    layers.Insert(id, new LegacyGameInterfaceLayer(
                        "AnotherRpgMod: Custom Health Bar",
                        delegate
                        {
                            if (HealthBar.visible)
                            {
                                //Update CustomBars
                                customResources.Update(Main._drawInterfaceGameTime);
                                healthBar.Draw(Main.spriteBatch);
                                
                            }
                            return true;
                        }, InterfaceScaleType.UI)
                    );
            }

            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Map / Minimap"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: StatWindows",
                    delegate
                    {
                        if (Stats.visible)
                        {
                            
                            statMenu.Draw(Main.spriteBatch);
                            
                        }
                        if (OpenStatsButton.visible)
                        {
                            customOpenstats.Update(Main._drawInterfaceGameTime);
                            openStatMenu.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    

}
