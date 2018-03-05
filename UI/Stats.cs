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
        private UIText PointsLeft = new UIText("");

        private float baseYOffset = 100;
        private float baseXOffset = 100;
        private float YOffset = 35;
        private float XOffset = 100;

        public override void OnInitialize()
        {

           
            statsPanel = new UIPanel();
            statsPanel.SetPadding(0);
            statsPanel.Left.Set(400f, 0f);
            statsPanel.Top.Set(100f, 0f);
            statsPanel.Width.Set(800, 0f);
            statsPanel.Height.Set(400, 0f);
            statsPanel.BackgroundColor = new Color(73, 94, 171);

            statsPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
            statsPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

            PointsLeft = new UIText("Points : 0 / 0");
            PointsLeft.Left.Set(250, 0f);
            PointsLeft.Top.Set(20, 0f);
            PointsLeft.Width.Set(22, 0f);
            PointsLeft.Height.Set(22, 0f);
            statsPanel.Append(PointsLeft);

            Texture2D Button = ModLoader.GetTexture("Terraria/UI/ButtonPlay");
            for (int i = 0; i < 8; i++)
            {
                UIImageButton UpgradeStatButton = new UIImageButton(Button);
                UpgradeStatButton.Left.Set(baseXOffset+ XOffset, 0f);
                UpgradeStatButton.Top.Set(baseYOffset+(YOffset*i), 0f);
                UpgradeStatButton.Width.Set(22, 0f);
                UpgradeStatButton.Height.Set(22, 0f);
                Stat Statused = (Stat)i;
                UpgradeStatButton.OnClick += new MouseEvent((UIMouseEvent, UIElement) => UpgradeStat(UIMouseEvent, UIElement, Statused));
                statsPanel.Append(UpgradeStatButton);

                UpgradeStatText[i] = new UIText((Stat)i + " : " + 50 + " + " + 0);
                UpgradeStatText[i].Left.Set(baseXOffset, 0f);
                UpgradeStatText[i].Top.Set(baseYOffset + (YOffset * i), 0f);
                UpgradeStatText[i].Width.Set(22, 0f);
                UpgradeStatText[i].Height.Set(22, 0f);
                statsPanel.Append(UpgradeStatText[i]);

                UpgradeStatDetails[i] = new UIText("Increase Melee Damage And Slightly Throw Damage");
                UpgradeStatDetails[i].Left.Set(baseXOffset+ (XOffset * 2), 0f);
                UpgradeStatDetails[i].Top.Set(baseYOffset + (YOffset * i), 0f);
                UpgradeStatDetails[i].Width.Set(22, 0f);
                UpgradeStatDetails[i].Height.Set(22, 0f);
                statsPanel.Append(UpgradeStatDetails[i]);
            }
            Append(statsPanel);

        }



        private void UpgradeStat(UIMouseEvent evt, UIElement listeningElement, Stat stat)
        {
            if (Char == null)
            {
                loadchar();
            }
            Main.PlaySound(SoundID.MenuOpen);
            Char.SpendPoints(stat, Amount);
            
        }

        public override void Update(GameTime gameTime)
        {
            if (visible)
                UpdateStats();
            base.Update(gameTime);
        }

        void UpdateStats()
        {
            if (Char == null)
            {
                loadchar();
            }
            for (int i = 0; i < 8; i++)
            {
                UpgradeStatText[i].SetText((Stat)i + " : " + Char.GetNaturalStat((Stat)i) + " + " + Char.GetAddStat((Stat)i));
            }
            for (int i = 0; i < 5; i++)
            {
                UpgradeStatDetails[i+3].SetText((DamageType)i + " Damage Multiplier : " + Char.GetDamageMult((DamageType)i));
            }
            UpgradeStatDetails[0].SetText("Health : "+ Char.player.statLifeMax2 + " - " +(Char.player.statLifeMax / 20) + " Heart x " + Char.GetHealthPerHeart() + " Health Per Heart + 10");
            UpgradeStatDetails[1].SetText("Mana : " + Char.player.statManaMax2 + " - " + (Char.player.statManaMax / 20) + " Mana Crystal x " +Utils.Mathf.Clamp( Char.GetManaPerStar(),0,20) + " Mana per crystal + 4");
            UpgradeStatDetails[2].SetText("Defense : " + Char.player.statDefense + " - " + Char.BaseArmor + " Armor x " + Char.GetDefenceMult() + " Defense Per Armor");
            PointsLeft.SetText("Points : " + Char.FreePtns + " / " + Char.TotalPtns,1,true);
        }

        Vector2 offset;
        public bool dragging = false;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            offset = new Vector2(evt.MousePosition.X - statsPanel.Left.Pixels, evt.MousePosition.Y - statsPanel.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
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
