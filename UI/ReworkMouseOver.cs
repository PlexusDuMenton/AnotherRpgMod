using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using AnotherRpgMod.RPGModule.Entities;
using System.Diagnostics;
using log4net.Util;
using Terraria.ID;

namespace AnotherRpgMod.Utils
{

    class ReworkMouseOver : UIState
    {
        UIPanel ScreenPanel;
        public static bool visible = false;

        public static ReworkMouseOver Instance;
        List<NPCInfoUI> NPCName = new List<NPCInfoUI>();
        NPCInfoUI NPCDetails;

        Dictionary<int, Color> ColorDic = new Dictionary<int, Color>()
        {
            {-1,Color.Black },
            {0,Color.Black },
            {1,Color.DarkGray },
            {2,Color.Gray },
            {3,Color.LightGreen },
            {4,Color.Green },
            {5,Color.LightYellow },
            {6,Color.Yellow },
            {7,Color.Orange },
            {8,Color.OrangeRed },
            {9,Color.Red },
            {10,Color.PaleVioletRed },
            {11,Color.Purple },
            {12,Color.BlueViolet }

        };

        public override void Update(GameTime gameTime)
        {
            if (!visible)
                return;
            NPCName = new List<NPCInfoUI>();
            ScreenPanel.RemoveAllChildren();
            NPCDetails = new NPCInfoUI();


            Recalculate();
            GetDetails();
            
            UIText npanel;
            if (NPCDetails.Text != null)
            {
                npanel = new UIText(NPCDetails.Text,0.8f);

                npanel.Left.Set(NPCDetails.Position.X, 0);
                npanel.Top.Set(NPCDetails.Position.Y + 25, 0);
                ScreenPanel.Append(npanel);
            }



            ScreenPanel.Recalculate();
            ScreenPanel.RecalculateChildren();

            base.Update(gameTime);
        }

        public override void OnInitialize()
        {
            Instance = this;
            ScreenPanel = new UIPanel();
            ScreenPanel.Width.Set(Main.screenWidth, 0);
            ScreenPanel.Height.Set(Main.screenHeight, 0);
            ScreenPanel.BackgroundColor = new Color(0, 0, 0, 0);
            ScreenPanel.BorderColor = new Color(0, 0, 0, 0);
            Append(ScreenPanel);
            base.OnInitialize();
        }

        public void GetDetails()
        {
            PlayerInput.SetZoom_Unscaled();
            PlayerInput.SetZoom_MouseInWorld();
            Rectangle mouseRectangle = new Rectangle((int)((float)Main.mouseX + Main.screenPosition.X), (int)((float)Main.mouseY + Main.screenPosition.Y), 1, 1);
            if (Main.player[Main.myPlayer].gravDir == -1f)
            {
                mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
            }
            Rectangle screenRectangle = new Rectangle((int)(Main.screenPosition.X), (int)(Main.screenPosition.Y), Main.screenWidth, Main.screenHeight);
            PlayerInput.SetZoom_UI();
            IngameOptions.MouseOver();
            IngameFancyUI.MouseOver();

            Main.HoveringOverAnNPC = false;
            NPC ActualNPC;
            string TempText = "";
            UIText panel;
            for (int k = 0; k < 200; k++)
            {
                ActualNPC = Main.npc[k];
                if (ActualNPC.active)
                {
                    Rectangle NPCPos = new Rectangle((int)ActualNPC.Bottom.X - ActualNPC.frame.Width / 2, (int)ActualNPC.Bottom.Y - ActualNPC.frame.Height, ActualNPC.frame.Width, ActualNPC.frame.Height);
                    if (ActualNPC.type >= NPCID.WyvernHead && ActualNPC.type <= NPCID.WyvernTail)
                    {
                        NPCPos = new Rectangle((int)(ActualNPC.position.X + ActualNPC.width * 0.5 - 32.0), (int)(ActualNPC.position.Y + ActualNPC.height * 0.5 - 32.0), 64, 64);
                    }
                    bool IsMouseOver = mouseRectangle.Intersects(NPCPos);
                    bool IsOnScreen = screenRectangle.Intersects(NPCPos);
                    if (IsMouseOver)
                    {
                        ARPGGlobalNPC Rnpc = ActualNPC.GetGlobalNPC<ARPGGlobalNPC>();
                        string preFix = "";
                        if (Rnpc.getLevel >= 0 && ActualNPC.damage > 0)
                        {
                            preFix = "Level : " + (Rnpc.getLevel + Rnpc.getTier) + "\n";

                            if (NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) > 0)
                                preFix += "World bonus level : +" + NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) + "\n";
                        }
                        TempText = "";

                        if (Main.npc[k].lifeMax > 1 && !Main.npc[k].dontTakeDamage)
                        {
                            TempText = string.Concat(new object[]
                            {
                                preFix,
                                "Rank : ",
                                (NPCRank)Rnpc.getRank,
                                "\nDamage : ",
                                Main.npc[k].damage,
                                "\nDef : ",
                                Main.npc[k].defense
                            });
                        }
                        panel = new UIText(TempText);


                        NPCDetails= new NPCInfoUI(TempText, Color.White, new Vector2(Main.mouseX, Main.mouseY));
                    }
                    else if (IsOnScreen && ((Config.vConfig.DisplayNpcName && !ActualNPC.townNPC) ||(Config.vConfig.DisplayTownName && ActualNPC.townNPC)))
                    {
                        
                        TempText = ActualNPC.GivenOrTypeName;
                        RPGModule.Entities.ARPGGlobalNPC Rnpc = ActualNPC.GetGlobalNPC<RPGModule.Entities.ARPGGlobalNPC>();
                        string preFix = "";
                        if (Rnpc.getLevel >= 0 && ActualNPC.damage>0)
                        {
                            preFix = "Lvl." + (Rnpc.getLevel + Rnpc.getTier);
                            /*
                            if (NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) > 0)
                                preFix += "( + " + RPGModule.Entities.NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) + " )";
                            */
                        }

                        TempText = "";

                        if (Main.npc[k].lifeMax > 1 && !Main.npc[k].dontTakeDamage)
                        {
                            TempText = string.Concat(new object[]       
                            {           
                                preFix,
                                " | ",
                                //ActualNPC.GivenOrTypeName,
                                //": ",
                                Main.npc[k].life,
                                "/",
                                Main.npc[k].lifeMax
                                        
                            });
                        }
                        int NpcColor = 0;
                        int LevelOffSet = (ActualNPC.GetGlobalNPC<ARPGGlobalNPC>().getLevel + ActualNPC.GetGlobalNPC<RPGModule.Entities.ARPGGlobalNPC>().getTier) - Main.LocalPlayer.GetModPlayer<RPGModule.Entities.RPGPlayer>().GetLevel();
                        if (LevelOffSet < -100)
                            NpcColor = 0;
                        else if (LevelOffSet < -50)
                            NpcColor = 1;
                        else if (LevelOffSet < -15)
                            NpcColor = 2;
                        else if (LevelOffSet < -5)
                            NpcColor = 3;
                        else if (LevelOffSet < 5)
                            NpcColor = 4;
                        else if (LevelOffSet < 20)
                            NpcColor = 5;
                        else if (LevelOffSet < 40)
                            NpcColor = 6;
                        else if (LevelOffSet < 75)
                            NpcColor = 7;
                        else if (LevelOffSet < 100)
                            NpcColor = 8;
                        else
                            NpcColor = 9;

                        switch (ActualNPC.GetGlobalNPC<ARPGGlobalNPC>().getRank)
                        {
                            case 0:
                                NpcColor--;
                                break;
                            case 2:
                            case 3:
                                NpcColor++;
                                break;
                            case 4:
                            case 5:
                                NpcColor+=2;
                                break;
                            case 6:
                            case 7:
                                NpcColor+=3;
                                break;

                        }

                        if (ActualNPC.townNPC)
                        {
                            NpcColor = 0;
                        }
                        NPCName.Add(new NPCInfoUI(TempText, ColorDic[NpcColor],new Vector2(ActualNPC.Bottom.X - Main.screenPosition.X, ActualNPC.Bottom.Y- Main.screenPosition.Y)));
                        
                    }

                }

            }

        }
    }


    class NPCNameUI : UIState
    {
        UIPanel ScreenPanel;
        public static bool visible = false;

        public static NPCNameUI Instance;
        List<NPCInfoUI> NPCName = new List<NPCInfoUI>();
        NPCInfoUI NPCDetails;

        Dictionary<int, Color> ColorDic = new Dictionary<int, Color>()
        {
            {-1,Color.Black },
            {0,Color.Black },
            {1,Color.DarkGray },
            {2,Color.Gray },
            {3,Color.LightGreen },
            {4,Color.Green },
            {5,Color.LightYellow },
            {6,Color.Yellow },
            {7,Color.Orange },
            {8,Color.OrangeRed },
            {9,Color.Red },
            {10,Color.PaleVioletRed },
            {11,Color.Purple },
            {12,Color.BlueViolet }

        };

        public override void Update(GameTime gameTime)
        {
            if (!visible)
                return;
            NPCName = new List<NPCInfoUI>();
            ScreenPanel.RemoveAllChildren();

            GetDetails();

            Recalculate();

            UIText npanel;
            for (int i = 0; i < NPCName.Count; i++)
            {
                if (NPCName[i].Color != Color.Black)
                    npanel = new UIText(NPCName[i].Text, 0.9f / Mathf.Pow(AnotherRpgMod.zoomValue.Y, 0.5f))
                    {
                        TextColor = NPCName[i].Color
                    };
                else
                {
                    npanel = new UIText(NPCName[i].Text, 0.75f / Mathf.Pow(AnotherRpgMod.zoomValue.Y, 0.5f))
                    {
                        TextColor = Color.DarkGray
                    };
                }

                Vector2 TextPosition = new Vector2(NPCName[i].Position.X + 10, NPCName[i].Position.Y - 3);

                npanel.Left.Set(TextPosition.X, 0);
                npanel.Top.Set(TextPosition.Y, 0);
                ScreenPanel.Append(npanel);
            }

            ScreenPanel.Recalculate();
            ScreenPanel.RecalculateChildren();

            base.Update(gameTime);
        }

        public override void OnInitialize()
        {
            Instance = this;
            ScreenPanel = new UIPanel();
            ScreenPanel.Width.Set(Main.screenWidth, 0);
            ScreenPanel.Height.Set(Main.screenHeight, 0);
            ScreenPanel.BackgroundColor = new Color(0, 0, 0, 0);
            ScreenPanel.BorderColor = new Color(0, 0, 0, 0);
            Append(ScreenPanel);
            base.OnInitialize();
        }


        public void GetDetails()
        {
            PlayerInput.SetZoom_Unscaled();
            PlayerInput.SetZoom_MouseInWorld();
            Rectangle mouseRectangle = new Rectangle((int)((float)Main.mouseX + Main.screenPosition.X), (int)((float)Main.mouseY + Main.screenPosition.Y), 1, 1);
            if (Main.player[Main.myPlayer].gravDir == -1f)
            {
                mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
            }
            Rectangle screenRectangle = new Rectangle((int)(Main.screenPosition.X), (int)(Main.screenPosition.Y), Main.screenWidth, Main.screenHeight);
            PlayerInput.SetZoom_UI();
            IngameOptions.MouseOver();
            IngameFancyUI.MouseOver();

            Main.HoveringOverAnNPC = false;
            NPC ActualNPC;
            string TempText = "";
            UIText panel;
            for (int k = 0; k < 200; k++)
            {
                ActualNPC = Main.npc[k];
                if (ActualNPC.active && !ActualNPC.dontCountMe)
                {
                    int type = Main.npc[k].type;
                    Rectangle NPCPos = new Rectangle((int)ActualNPC.Bottom.X - ActualNPC.frame.Width / 2, (int)ActualNPC.Bottom.Y - ActualNPC.frame.Height, ActualNPC.frame.Width, ActualNPC.frame.Height);
                    if (ActualNPC.type >= NPCID.WyvernHead && ActualNPC.type <= NPCID.WyvernTail)
                    {
                        NPCPos = new Rectangle((int)(ActualNPC.position.X + ActualNPC.width * 0.5 - 32.0), (int)(ActualNPC.position.Y + ActualNPC.height * 0.5 - 32.0), 64, 64);
                    }
                    bool IsMouseOver = mouseRectangle.Intersects(NPCPos);
                    bool IsOnScreen = screenRectangle.Intersects(NPCPos);
                    if (IsMouseOver)
                    {

                        ARPGGlobalNPC Rnpc = ActualNPC.GetGlobalNPC<ARPGGlobalNPC>();
                        string preFix = "";
                        if (Rnpc.getLevel >= 0 && ActualNPC.damage > 0)
                        {
                            preFix = "Level : " + (Rnpc.getLevel + Rnpc.getTier) + "\n";

                            if (NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) > 0)
                                preFix += "World bonus level : +" + NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) + "\n";
                        }
                        TempText = "";

                        if (Main.npc[k].lifeMax > 1 && !Main.npc[k].dontTakeDamage)
                        {
                            TempText = string.Concat(new object[]
                            {
                                preFix,
                                "Rank : ",
                                (NPCRank)Rnpc.getRank,
                                "\nDamage : ",
                                Main.npc[k].damage,
                                "\nDef : ",
                                Main.npc[k].defense
                            });
                        }
                        panel = new UIText(TempText);


                        NPCDetails = new NPCInfoUI(TempText, Color.White, new Vector2(Main.mouseX, Main.mouseY));
                    }
                    else if (IsOnScreen && ((Config.vConfig.DisplayNpcName && !ActualNPC.townNPC) || (Config.vConfig.DisplayTownName && ActualNPC.townNPC)))
                    {
                        TempText = ActualNPC.GivenOrTypeName;
                        RPGModule.Entities.ARPGGlobalNPC Rnpc = ActualNPC.GetGlobalNPC<RPGModule.Entities.ARPGGlobalNPC>();
                        string preFix = "";
                        if (Rnpc.getLevel >= 0 && ActualNPC.damage > 0)
                        {
                            preFix = "Lvl." + (Rnpc.getLevel + Rnpc.getTier);
                            /*
                            if (NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) > 0)
                                preFix += "( + " + RPGModule.Entities.NPCUtils.GetWorldTier(ActualNPC, Rnpc.getLevel) + " )";
                            */
                        }

                        TempText = "";

                        if (Main.npc[k].lifeMax > 1 && !Main.npc[k].dontTakeDamage)
                        {
                            TempText = string.Concat(new object[]
                            {
                                preFix,
                                " | ",
                                //ActualNPC.GivenOrTypeName,
                                //": ",
                                Main.npc[k].life,
                                "/",
                                Main.npc[k].lifeMax

                            });
                        }
                        int NpcColor = 0;
                        int LevelOffSet = (ActualNPC.GetGlobalNPC<ARPGGlobalNPC>().getLevel + ActualNPC.GetGlobalNPC<RPGModule.Entities.ARPGGlobalNPC>().getTier) - Main.LocalPlayer.GetModPlayer<RPGModule.Entities.RPGPlayer>().GetLevel();
                        if (LevelOffSet < -100)
                            NpcColor = 0;
                        else if (LevelOffSet < -50)
                            NpcColor = 1;
                        else if (LevelOffSet < -15)
                            NpcColor = 2;
                        else if (LevelOffSet < -5)
                            NpcColor = 3;
                        else if (LevelOffSet < 5)
                            NpcColor = 4;
                        else if (LevelOffSet < 20)
                            NpcColor = 5;
                        else if (LevelOffSet < 40)
                            NpcColor = 6;
                        else if (LevelOffSet < 75)
                            NpcColor = 7;
                        else if (LevelOffSet < 100)
                            NpcColor = 8;
                        else
                            NpcColor = 9;

                        switch (ActualNPC.GetGlobalNPC<ARPGGlobalNPC>().getRank)
                        {
                            case 0:
                                NpcColor--;
                                break;
                            case 2:
                            case 3:
                                NpcColor++;
                                break;
                            case 4:
                            case 5:
                                NpcColor += 2;
                                break;
                            case 6:
                            case 7:
                                NpcColor += 3;
                                break;

                        }

                        if (ActualNPC.townNPC)
                        {
                            NpcColor = 0;
                        }
                        NPCName.Add(new NPCInfoUI(TempText, ColorDic[NpcColor], new Vector2(ActualNPC.Bottom.X - Main.screenPosition.X, ActualNPC.Bottom.Y - Main.screenPosition.Y)));

                    }

                }

            }

        }

    }


    struct NPCInfoUI
    {
        public string Text;
        public Color Color;
        public Vector2 Position;

        public NPCInfoUI(string text,Color color,Vector2 position)
        {
            Text = text;
            Color = color;
            Position = position;
        }
    }
}

