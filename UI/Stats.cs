using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ID;
using Terraria;
using System;
using Terraria.ModLoader;
using AnotherRpgMod.RPGModule.Entities;
using System.Reflection;
using Terraria.GameInput;
using Terraria.Localization;
using AnotherRpgMod.RPGModule;


namespace AnotherRpgMod.UI
{

    class OpenStatsButton : UIState
    {
        public static bool visible = true;
        public UIElement OpenStatsPanel;

        public override void OnInitialize()
        {
            OpenStatsPanel = new UIElement();
            OpenStatsPanel.SetPadding(0);
            OpenStatsPanel.Left.Set(57, 0f);
            OpenStatsPanel.Top.Set(933, 0f);
            OpenStatsPanel.Width.Set(32, 0f);
            OpenStatsPanel.Height.Set(64, 0f);

            Texture2D Button = ModLoader.GetTexture("AnotherRpgMod/Textures/UI/character");
            UIImageButton OpenButton = new UIImageButton(Button);
            OpenButton.Left.Set(0, 0f);
            OpenButton.Top.Set(0, 0f);
            OpenButton.Width.Set(32, 0f);
            OpenButton.Height.Set(64, 0f);
            OpenButton.OnClick += new MouseEvent(OpenStatMenu);
            OpenStatsPanel.Append(OpenButton);
            base.Append(OpenStatsPanel);
        }
        private void OpenStatMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuOpen);
            Stats.visible = !Stats.visible;
        }
    }
    class Stats : UIState
    {
        public static Stats Instance;
        public UIPanel statsPanel;
        private RPGPlayer Char;
        public static bool visible = false;

        

        private int Amount = 1;

        private void loadchar()
        {
            Char = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
        }

        private UIText[] UpgradeStatText = new UIText[8];
        private UIText[] UpgradeStatDetails = new UIText[8];
        private UIText[] UpgradeStatOver = new UIText[8];
        private UIText PointsLeft = new UIText("");

        UIText ResetText;

        private float baseYOffset = 100;
        private float baseXOffset = 50;
        private float YOffset = 35;
        private float XOffset = 120;

        private void ResetTextHover(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.White;
        }
        private void ResetTextOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.Gray;
        }

        public override void OnInitialize()
        {

            Instance = this;
            statsPanel = new UIPanel();
            statsPanel.SetPadding(0);
            statsPanel.Left.Set(400f, 0f);
            statsPanel.Top.Set(100f, 0f);
            statsPanel.Width.Set(900, 0f);
            statsPanel.Height.Set(400, 0f);
            statsPanel.BackgroundColor = new Color(73, 94, 171,150);

            statsPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
            statsPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

            PointsLeft = new UIText("Points : 0 / 0");
            PointsLeft.Left.Set(250, 0f);
            PointsLeft.Top.Set(20, 0f);
            PointsLeft.Width.Set(0, 0f);
            PointsLeft.Height.Set(0, 0f);
            statsPanel.Append(PointsLeft);

            
            
            ResetText = new UIText("RESET", 1, true)
            {
                TextColor = Color.Gray
            };
            ResetText.Left.Set(50, 0f);
            ResetText.Top.Set(20, 0f);
            ResetText.Width.Set(0, 0f);
            ResetText.Height.Set(0, 0f);
            ResetText.OnClick += new MouseEvent(ResetStats);
            ResetText.OnMouseOver += new MouseEvent(ResetTextHover);
            ResetText.OnMouseOut += new MouseEvent(ResetTextOut);
            statsPanel.Append(ResetText);

            Texture2D Button = ModLoader.GetTexture("Terraria/UI/ButtonPlay");
            for (int i = 0; i < 8; i++)
            {
                UIImageButton UpgradeStatButton = new UIImageButton(Button);
               
                UpgradeStatButton.Left.Set(baseXOffset+ XOffset, 0f);
                UpgradeStatButton.Top.Set(baseYOffset+(YOffset*i), 0f);
                UpgradeStatButton.Width.Set(22, 0f);
                UpgradeStatButton.Height.Set(22, 0f);
                Stat Statused = (Stat)i;
                UpgradeStatButton.OnMouseOver += new MouseEvent((UIMouseEvent, UIElement) => UpdateStat(UIMouseEvent, UIElement, Statused));
                UpgradeStatButton.OnMouseOut += new MouseEvent(ResetOver);
                UpgradeStatButton.OnClick += new MouseEvent((UIMouseEvent, UIElement) => UpgradeStat(UIMouseEvent, UIElement, Statused,1));
                UpgradeStatButton.OnRightClick += new MouseEvent((UIMouseEvent, UIElement) => UpgradeStat(UIMouseEvent, UIElement, Statused, 5));
                UpgradeStatButton.OnMiddleClick += new MouseEvent((UIMouseEvent, UIElement) => UpgradeStat(UIMouseEvent, UIElement, Statused, 25));
                statsPanel.Append(UpgradeStatButton);

                
                UpgradeStatText[i] = new UIText("0");
                UpgradeStatText[i].SetText("Mana : 10 + 10");
                UpgradeStatText[i].Left.Set(baseXOffset, 0f);
                UpgradeStatText[i].Top.Set(baseYOffset + (YOffset * i), 0f);
                UpgradeStatText[i].HAlign = 0f;
                UpgradeStatText[i].VAlign = 0f;
                UpgradeStatText[i].MinWidth.Set(150, 0);
                UpgradeStatText[i].MaxWidth.Set(150, 0);
                statsPanel.Append(UpgradeStatText[i]);

                UpgradeStatDetails[i] = new UIText("");
                if (i < 3)
                {
                    
                    UpgradeStatDetails[i].SetText("Health : 100 - 5 Heart x 20 Health Per Heart");
                }
                UpgradeStatDetails[i].SetText("Melee Damage Multiplier : 1");
                UpgradeStatDetails[i].Left.Set(baseXOffset+ (XOffset * 1.5f), 0f);
                UpgradeStatDetails[i].Top.Set(baseYOffset + (YOffset * i), 0f);
                UpgradeStatDetails[i].HAlign = 0f;
                UpgradeStatDetails[i].VAlign = 0f;
                UpgradeStatDetails[i].MinWidth.Set(300f, 0);
                UpgradeStatDetails[i].MaxWidth.Set(300f, 0);
                statsPanel.Append(UpgradeStatDetails[i]);

                UpgradeStatOver[i] = new UIText("")
                {
                    TextColor = Color.Aqua
                };
                UpgradeStatOver[i].SetText("");
                UpgradeStatOver[i].Left.Set(baseXOffset + (XOffset * 5.3f), 0f);
                UpgradeStatOver[i].Top.Set(baseYOffset + (YOffset * i), 0f);
                UpgradeStatOver[i].HAlign = 0f;
                UpgradeStatOver[i].VAlign = 0f;
                UpgradeStatOver[i].MinWidth.Set(20f,0);
                UpgradeStatOver[i].MaxWidth.Set(20f, 0);
                statsPanel.Append(UpgradeStatOver[i]);
            }
            Append(statsPanel);
            


        }

        private Color MainColor = new Color(75,75, 255);
        private Color SecondaryColor = new Color(150, 150, 255);

        public void UpdateStat(UIMouseEvent evt, UIElement listeningElement, Stat stat)
        {
            Recalculate();
            for (int i = 0; i < 8; i++)
            {
                UpgradeStatOver[i].TextColor = SecondaryColor;
            }
            CheckChar();
            switch (stat)
            {
                case (Stat.Vit):
                    UpgradeStatOver[0].SetText("+ " + ((Char.player.statLifeMax / 20) * 0.5f * Char.statMultiplier) + " Hp");
                    UpgradeStatOver[0].TextColor = MainColor;
                    UpgradeStatOver[2].SetText("+ " + (Char.BaseArmor*0.02f) + " Armor");
                    UpgradeStatOver[2].TextColor = SecondaryColor;
                    break;
                case (Stat.Foc):
                    UpgradeStatOver[1].SetText("+ " + ((Char.player.statManaMax / 20) * 0.5f * Char.statMultiplier) + " Mana");
                    UpgradeStatOver[1].TextColor = MainColor;
                    UpgradeStatOver[7].SetText("+ " + (0.05 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[7].TextColor = SecondaryColor;
                    break;
                case (Stat.Con):
                    UpgradeStatOver[2].SetText("+ " + (Char.BaseArmor * 0.04f) + " Armor");
                    UpgradeStatOver[2].TextColor = MainColor;
                    UpgradeStatOver[0].SetText("+ " + ((Char.player.statLifeMax / 20) * 0.25f * Char.statMultiplier) + " Hp");
                    UpgradeStatOver[0].TextColor = SecondaryColor;
                    break;
                case (Stat.Str):
                    UpgradeStatOver[3].SetText("+ " +(0.1*Char.statMultiplier)+ " Multiplier");
                    UpgradeStatOver[3].TextColor = MainColor;
                    UpgradeStatOver[5].SetText("+ " + (0.05 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[5].TextColor = SecondaryColor;
                    break;
                case (Stat.Agi):
                    UpgradeStatOver[4].SetText("+ " + (0.1 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[4].TextColor = MainColor;
                    UpgradeStatOver[3].SetText("+ " + (0.05 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[3].TextColor = SecondaryColor;
                    break;
                case (Stat.Dex):
                    UpgradeStatOver[5].SetText("+ " + (0.1 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[5].TextColor = MainColor;
                    UpgradeStatOver[4].SetText("+ " + (0.05 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[4].TextColor = SecondaryColor;
                    break;
                case (Stat.Int):
                    UpgradeStatOver[6].SetText("+ " + (0.1 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[6].TextColor = MainColor;
                    UpgradeStatOver[1].SetText("+ " + ((Char.player.statManaMax / 20) * 0.25f * Char.statMultiplier) + " Mana");
                    UpgradeStatOver[1].TextColor = SecondaryColor;
                    break;
                case (Stat.Spr):
                    UpgradeStatOver[7].SetText("+ " + (0.1 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[7].TextColor = MainColor;
                    UpgradeStatOver[6].SetText("+ " + (0.05 * Char.statMultiplier) + " Multiplier");
                    UpgradeStatOver[6].TextColor = SecondaryColor;
                    break;
            }
        }
        public void ResetOver(UIMouseEvent evt, UIElement listeningElement)
        {
            for (int i = 0; i < 8; i++)
            {
                UpgradeStatOver[i].SetText("");
            }
        }
        private void ResetStats(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            Main.PlaySound(SoundID.MenuOpen);
            Char.ResetStats();
        }
        public void CheckChar()
        {
            if (Char == null)
            {
                loadchar();
            }
        }

        private void UpgradeStat(UIMouseEvent evt, UIElement listeningElement, Stat stat,int amount)
        {
            Main.PlaySound(SoundID.MenuOpen);
            Char.SpendPoints(stat, amount);
            
        }

        public override void Update(GameTime gameTime)
        {
            if (visible)
                UpdateStats();
                Recalculate();
            base.Update(gameTime);
        }

        void UpdateStats()
        {
            CheckChar();
            for (int i = 0; i < 8; i++)
            {
                UpgradeStatText[i].SetText((Stat)i + " : " + Char.GetNaturalStat((Stat)i) + " + " + Char.GetAddStat((Stat)i));
            }
            for (int i = 0; i < 5; i++)
            {
                UpgradeStatDetails[i+3].SetText((DamageType)i + " Damage Multiplier : " + Char.GetDamageMult((DamageType)i));
            }
            UpgradeStatDetails[0].SetText("Health : "+ Char.player.statLifeMax2 + " - " +(Char.player.statLifeMax / 20) + " Heart x " + Char.GetHealthPerHeart() + " Health Per Heart + 10");
            UpgradeStatDetails[1].SetText("Mana : " + Char.player.statManaMax2 + " - " + (Char.player.statManaMax / 20) + " Crystal x " +Utils.Mathf.Clamp( Char.GetManaPerStar(),0,20) + " Mana per crystal + 4");
            UpgradeStatDetails[2].SetText("Defense : " + Char.player.statDefense + " - " + Char.BaseArmor + " Armor x " + Char.GetDefenceMult() + " Defense Per Armor");
            PointsLeft.SetText("Points : " + Char.FreePtns + " / " + Char.TotalPtns,1,true);

        }

        Vector2 offset;
        public bool dragging = false;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            offset = new Vector2(evt.MousePosition.X - statsPanel.Left.Pixels, evt.MousePosition.Y - statsPanel.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            Vector2 end = evt.MousePosition;
            dragging = false;

            statsPanel.Left.Set(end.X - offset.X, 0f);
            statsPanel.Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (statsPanel.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (dragging)
            {
                statsPanel.Left.Set(MousePosition.X - offset.X, 0f);
                statsPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }
        }
    }



}
