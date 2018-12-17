using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using AnotherRpgMod.UI;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.Utils;


namespace AnotherRpgMod
{
    public enum DamageType : byte
    {
        Melee,
        Ranged,
        Throw,
        Magic,
        Summon,
        Symphonic, //thorium
        Radiant //thorium

    }

    public enum Message : byte {
        AddXP,
        SyncLevel,
        SyncNPCSpawn,
        SyncNPCUpdate,
        SyncWeapon,
        SyncConfig,
        AskNpc,
        Log
    };

    

    enum SupportedMod
    {
        Thorium, //only suported mod for now
        Calamity,
        DBZMOD
    }

    class AnotherRpgMod : Mod
	{
        public static AnotherRpgMod Instance;
        public UserInterface customResources;
        public HealthBar healthBar;
        public UserInterface customNPCInfo;
        public UserInterface customstats;
        public UserInterface customOpenstats;
        public UserInterface customOpenST;
        public Stats statMenu;
        public OpenStatsButton openStatMenu;
        public OpenSTButton OpenST;
        public UserInterface customSkillTree;
        public SkillTreeUi skillTreeUI;
        public ReworkMouseOver NPCInfo;
        public static ModHotKey StatsHotKey;
        public static ModHotKey SkillTreeHotKey;

        public static int PlayerLevel = 0;
        public static Dictionary<SupportedMod, bool> LoadedMods = new Dictionary<SupportedMod, bool>()
        {
            {SupportedMod.Thorium,false },
            {SupportedMod.Calamity,false },
            //{SupportedMod.Spirit,false }

        };

    
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {

            MPPacketHandler.HandlePacket(reader, whoAmI);
        }
        public override void PreSaveAndQuit()
        {
            Stats.visible = false;
            base.PreSaveAndQuit();
        }
        public override void Load()
        {
            ErrorLogger.Log("Another Rpg Mod " + Version + " Correctly loaded !");
            Instance = this;
            ConfigFile.Init();
            JsonSkillTree.Init();
            JsonCharacterClass.Init();
            LoadedMods[SupportedMod.Thorium] = ModLoader.GetMod("ThoriumMod") != null;
            LoadedMods[SupportedMod.Calamity] = ModLoader.GetMod("CalamityMod") != null;
            StatsHotKey = RegisterHotKey("Open Stats Menu", "C");
            SkillTreeHotKey = RegisterHotKey("Open SkillTree", "X");
            if (!Main.dedServ)
            {

                customNPCInfo = new UserInterface();
                NPCInfo = new ReworkMouseOver();
                ReworkMouseOver.visible = true;
                customNPCInfo.SetState(NPCInfo);

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

                customOpenST = new UserInterface();
                OpenST = new OpenSTButton();
                OpenSTButton.visible = true;
                customOpenST.SetState(OpenST);

                customSkillTree = new UserInterface();
                skillTreeUI = new SkillTreeUi();
                OpenStatsButton.visible = true;
                customSkillTree.SetState(skillTreeUI);

                /*
                
                statMenu = new Stats();
                Stats.visible = true;
                customstats.SetState(statMenu);
                */
            }
            
        }

        public AnotherRpgMod()
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
            if (Main.netMode == 2)
                return;

            //Vanilla: Emote Bubbles
            int mouseid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (mouseid != -1)
            {
                layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: NPC Info detail",
                    delegate
                    {
                        if (HealthBar.visible) { 
                            customNPCInfo.Update(Main._drawInterfaceGameTime);
                            NPCInfo.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }


            int skilltreeid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Map / Minimap"));
            if (skilltreeid != -1)
            {
                //layers.RemoveAt(id);

                //Add you own layer
                layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: Skill Tree",
                    delegate
                    {
                        if (SkillTreeUi.visible)
                        {
                                //Update CustomBars
                                customSkillTree.Update(Main._drawInterfaceGameTime);
                                skillTreeUI.Draw(Main.spriteBatch);

                        }
                        if (OpenSTButton.visible)
                        {
                            customOpenST.Update(Main._drawInterfaceGameTime);
                            OpenST.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.UI)
                );
            }
            

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
