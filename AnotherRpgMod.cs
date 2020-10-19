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
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.Localization;

using AnotherRpgMod.Items;
using Terraria.Graphics;

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
        Radiant, //thorium
        KI,

    }

    public enum Message : byte {
        AddXP,
        SyncLevel,
        SyncNPCSpawn,
        SyncNPCUpdate,
        SyncWeapon,
        AskNpc,
        Log
    };

    

    enum SupportedMod
    {
        Thorium,
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
        public UserInterface customItemTree;

        public UserInterface customSkillTree;

        public OpenStatsButton openStatMenu;
        public OpenSTButton OpenST;
        public Stats statMenu;
        
        public SkillTreeUi skillTreeUI;
        public ItemTreeUi ItemTreeUI;

        public ReworkMouseOver NPCInfo;


        public static ModHotKey StatsHotKey;
        public static ModHotKey SkillTreeHotKey;
        public static ModHotKey ItemTreeHotKey;


        internal static GamePlayConfig gpConfig;
        internal static VisualConfig visualConfig;


        public static ItemUpdate source;
        public static ItemUpdate Transfer;
        public static float XPTvalueA;
        public static float XPTvalueB;

        public static Vector2 zoomValue = new Vector2(1,1);

        private float lastUpdateScreenScale = Main.screenHeight;

        public static int PlayerLevel = 0;
        public static Dictionary<SupportedMod, bool> LoadedMods = new Dictionary<SupportedMod, bool>()
        {
            {SupportedMod.Thorium,false },
            {SupportedMod.Calamity,false },
            {SupportedMod.DBZMOD,false },
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
            
            Instance = this;
            Instance.Logger.Info("Another Rpg Mod " + Version + " Correctly loaded");
            JsonSkillTree.Init();
            JsonCharacterClass.Init();
            LoadedMods[SupportedMod.Thorium] = ModLoader.GetMod("ThoriumMod") != null;
            LoadedMods[SupportedMod.Calamity] = ModLoader.GetMod("CalamityMod") != null;
            LoadedMods[SupportedMod.DBZMOD] = ModLoader.GetMod("DBZMOD") != null;
            
            StatsHotKey = RegisterHotKey("Open Stats Menu", "C");
            SkillTreeHotKey = RegisterHotKey("Open SkillTree", "X");
            ItemTreeHotKey = RegisterHotKey("Open Item Tree", "V");
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

                customItemTree = new UserInterface();
                ItemTreeUI = new ItemTreeUi();
                ItemTreeUi.visible = false;
                customItemTree.SetState(ItemTreeUI);

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

        public override void PostUpdateEverything()
        {
            //Update UI when screen Size Change
            if (lastUpdateScreenScale != Main.screenHeight)
            {
                AnotherRpgMod.Instance.healthBar.Reset();
                AnotherRpgMod.Instance.OpenST.Reset();
                AnotherRpgMod.Instance.openStatMenu.Reset();
            }
            lastUpdateScreenScale = Main.screenHeight;
            base.PostUpdateEverything();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (customstats != null)
                customstats.Update(gameTime);
        }



        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            zoomValue = Transform.Zoom;
            base.ModifyTransformMatrix(ref Transform);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.netMode == NetmodeID.Server)
                return;


            if (HealthBar.visible && Config.gpConfig.RPGPlayer)
            {
                int ressourceid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
                layers.RemoveAt(ressourceid);
            }

            //Vanilla: MouseOver
            int mouseid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (mouseid != -1)
            {
                layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: NPC Info detail",
                    delegate
                    {
                            customNPCInfo.Update(Main._drawInterfaceGameTime);
                            NPCInfo.Draw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.None)
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
                        if (ItemTreeUi.visible)
                        {
                            //Update CustomBars
                            customItemTree.Update(Main._drawInterfaceGameTime);
                            ItemTreeUI.Draw(Main.spriteBatch);

                        }
                        if (OpenSTButton.visible)
                        {
                            customOpenST.Update(Main._drawInterfaceGameTime);
                            OpenST.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.None)
                );
            }
            

            int id = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (id != -1)
            {
                    layers.Insert(id, new LegacyGameInterfaceLayer(
                        "AnotherRpgMod: Custom Health Bar",
                        delegate
                        {
                            if (HealthBar.visible)
                            {
                                //Update CustomBars
                                

                                customOpenST.Update(Main._drawInterfaceGameTime);
                                customOpenstats.Update(Main._drawInterfaceGameTime);
                                customResources.Update(Main._drawInterfaceGameTime);
                                healthBar.Draw(Main.spriteBatch);
                                


                            }
                            return true;
                        }, InterfaceScaleType.None)
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
                    InterfaceScaleType.None)
                );
            }

        }


        private void DrawInterface_Resources_ClearBuffs()
        {
            Main.buffString = "";
            Main.bannerMouseOver = false;
            if (!Main.recBigList)
            {
                Main.recStart = 0;
            }
        }

        private void DrawInterface_Resources_Buffs()
        {
            int BuffID = -1;
            int num2 = 11;
            for (int i = 0; i < 22; i++)
            {
                if (Main.player[Main.myPlayer].buffType[i] > 0)
                {
                    int b = Main.player[Main.myPlayer].buffType[i];
                    int x = 32 + i * 38;
                    int num3 = 76;
                    if (i >= num2)
                    {
                        x = 32 + (i - num2) * 38;
                        num3 += 50;
                    }
                    BuffID = DrawBuffIcon(BuffID, i, b, x, num3);
                }
                else
                {
                    Main.buffAlpha[i] = 0.4f;
                }
            }
            if (BuffID >= 0)
            {
                int num4 = Main.player[Main.myPlayer].buffType[BuffID];
                if (num4 > 0)
                {
                    Main.buffString = Lang.GetBuffDescription(num4);
                    int itemRarity = 0;
                    if (num4 == 26 && Main.expertMode)
                    {
                        Main.buffString = Language.GetTextValue("BuffDescription.WellFed_Expert");
                    }
                    if (num4 == 147)
                    {
                        Main.bannerMouseOver = true;
                    }
                    if (num4 == 94)
                    {
                        int num5 = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
                        Main.buffString = Main.buffString + num5 + "%";
                    }
                    if (Main.meleeBuff[num4])
                    {
                        itemRarity = -10;
                    }
                    BuffLoader.ModifyBuffTip(num4, ref Main.buffString, ref itemRarity);
                }
            }




            int DrawBuffIcon(int drawBuffText, int i, int b, int x, int y)
            {
                //IL_011b: Unknown result type (might be due to invalid IL or missing references)
                if (b == 0)
                {
                    return drawBuffText;
                }
                Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(Main.buffAlpha[i], Main.buffAlpha[i], Main.buffAlpha[i], Main.buffAlpha[i]);
                Main.spriteBatch.Draw(Main.buffTexture[b], new Vector2((float)x, (float)y), new Microsoft.Xna.Framework.Rectangle(0, 0, Main.buffTexture[b].Width, Main.buffTexture[b].Height), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                if (!Main.vanityPet[b] && !Main.lightPet[b] && !Main.buffNoTimeDisplay[b] && (!Main.player[Main.myPlayer].honeyWet || b != 48) && (!Main.player[Main.myPlayer].wet || !Main.expertMode || b != 46) && Main.player[Main.myPlayer].buffTime[i] > 2)
                {
                    string text = Lang.LocalizedDuration(new TimeSpan(0, 0, Main.player[Main.myPlayer].buffTime[i] / 60), true, false);
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontItemStack, text, new Vector2((float)x, (float)(y + Main.buffTexture[b].Height)), color, 0f, default(Vector2), 0.8f, SpriteEffects.None, 0f);
                }
                if (Main.mouseX < x + Main.buffTexture[b].Width && Main.mouseY < y + Main.buffTexture[b].Height && Main.mouseX > x && Main.mouseY > y)
                {
                    drawBuffText = i;
                    Main.buffAlpha[i] += 0.1f;
                    bool flag = Main.mouseRight && Main.mouseRightRelease;
                    if (PlayerInput.UsingGamepad)
                    {
                        flag = (Main.mouseLeft && Main.mouseLeftRelease && Main.playerInventory);
                        if (Main.playerInventory)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                        }
                    }
                    else
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    if (flag)
                    {
                        TryRemovingBuff(i, b);
                    }
                }
                else
                {
                    Main.buffAlpha[i] -= 0.05f;
                }
                if (Main.buffAlpha[i] > 1f)
                {
                    Main.buffAlpha[i] = 1f;
                }
                else if ((double)Main.buffAlpha[i] < 0.4)
                {
                    Main.buffAlpha[i] = 0.4f;
                }
                if (PlayerInput.UsingGamepad && !Main.playerInventory)
                {
                    drawBuffText = -1;
                }
                return drawBuffText;
            }

            void TryRemovingBuff(int i, int b)
            {
                bool flag = false;
                if (!Main.debuff[b] && b != 60 && b != 151)
                {
                    if (Main.player[Main.myPlayer].mount.Active && Main.player[Main.myPlayer].mount.CheckBuff(b))
                    {
                        Main.player[Main.myPlayer].mount.Dismount(Main.player[Main.myPlayer]);
                        flag = true;
                    }
                    if (Main.player[Main.myPlayer].miscEquips[0].buffType == b && !Main.player[Main.myPlayer].hideMisc[0])
                    {
                        Main.player[Main.myPlayer].hideMisc[0] = true;
                    }
                    if (Main.player[Main.myPlayer].miscEquips[1].buffType == b && !Main.player[Main.myPlayer].hideMisc[1])
                    {
                        Main.player[Main.myPlayer].hideMisc[1] = true;
                    }
                    Main.PlaySound(SoundID.MenuTick, -1, -1, 1, 1f, 0f);
                    if (!flag)
                    {
                        Main.player[Main.myPlayer].DelBuff(i);
                    }
                }
            }
        }

    }




    

    

}
