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

namespace AnotherRpgMod.UI
{
    internal enum Mode
    {
        HP,
        MANA,
        XP,
        Breath
    }

    class HealthBar : UIState
    {

        private Player player;

        Ressource hp;
        Ressource xp;
        Ressource mana;
        RessourceBreath breath;
        UIOverlay Overlay;

        public UIElement MainPanel1;
        public UIElement MainPanel2;
        public UIElement MainPanel3;
        public UIElement MainPanel4;
        public UIElement MainPanel5;

        private UIText health;
        private UIText manatext;
        private UIText xptext;
        private UIText Level;

        public static bool visible = false;

        public override void Update(GameTime gameTime)
        {
            Player player = Main.player[Main.myPlayer]; //Get Player

            health.SetText("" + player.statLife + " | " + player.statLifeMax2); //Set Life


            manatext.SetText("" + player.statMana + " | " + player.statManaMax2); //Set Mana
            xptext.SetText("" + (float)player.GetModPlayer<RPGPlayer>().GetExp() + " | " + (float)player.GetModPlayer<RPGPlayer>().XPToNextLevel()); //Set Mana
            Level.SetText("Lvl. " + (float)player.GetModPlayer<RPGPlayer>().GetLevel());



            Recalculate();

            base.Update(gameTime);
        }



        public override void OnInitialize()
        {
            player = Main.player[Main.myPlayer];



            MainPanel1 = new UIElement();
            MainPanel1.SetPadding(0);
            MainPanel1.Left.Set(0, 0f);
            MainPanel1.Width.Set(840f, 0f);
            MainPanel1.Height.Set(393f, 0f);

            MainPanel1.Top.Set(Main.screenHeight - MainPanel1.Height.Pixels, 0f);

            MainPanel2 = new PanelBar(Mode.HP, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/HealthBar"));
            MainPanel2.SetPadding(0);
            MainPanel2.Left.Set(14, 0f);
            MainPanel2.Width.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/HealthBar").Width, 0f);
            MainPanel2.Height.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/HealthBar").Height, 0f);
            MainPanel2.Top.Set(Main.screenHeight - MainPanel1.Height.Pixels + 54 + 262, 0f);

            MainPanel3 = new PanelBar(Mode.MANA, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/ManaBar"));
            MainPanel3.SetPadding(0);
            MainPanel3.Left.Set(31, 0f);
            MainPanel3.Width.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/ManaBar").Width, 0f);
            MainPanel3.Height.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/ManaBar").Height, 0f);
            MainPanel3.Top.Set(Main.screenHeight - MainPanel1.Height.Pixels + 90 + 262, 0f);

            MainPanel4 = new PanelBar(Mode.XP, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/XPBar"));
            MainPanel4.SetPadding(0);
            MainPanel4.Left.Set(44, 0f);
            MainPanel4.Width.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/XPBar").Width, 0f);
            MainPanel4.Height.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/XPBar").Height, 0f);
            MainPanel4.Top.Set(Main.screenHeight - MainPanel1.Height.Pixels + 113 + 262, 0f);


            MainPanel5 = new PanelBar(Mode.Breath, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/BreathBar"));
            MainPanel5.SetPadding(0);
            MainPanel5.Left.Set(5, 0f);
            MainPanel5.Width.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/BreathBar").Width, 0f);
            MainPanel5.Height.Set(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/BreathBar").Height, 0f);
            MainPanel5.Top.Set(Main.screenHeight - MainPanel1.Height.Pixels + 64, 0f);


            Overlay = new UIOverlay(ModLoader.GetTexture("AnotherRpgMod/Textures/UI/OverlayHealthBar"));
            MainPanel1.Append(Overlay);

            hp = new Ressource(Mode.HP, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/HealthBar"));
            MainPanel2.Append(hp);

            mana = new Ressource(Mode.MANA, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/ManaBar"));
            MainPanel3.Append(mana);

            xp = new Ressource(Mode.XP, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/XPBar"));
            MainPanel4.Append(xp);

            breath = new RessourceBreath(Mode.Breath, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/BreathBar"));
            MainPanel5.Append(breath);



            base.Append(MainPanel2);
            base.Append(MainPanel3);
            base.Append(MainPanel4);
            base.Append(MainPanel5);
            base.Append(MainPanel1);

            health = new UIText("0|0", 1.2f);
            manatext = new UIText("0|0");
            xptext = new UIText("0|0");
            Level = new UIText("Lvl. 1", 0.6f, true);

            health.Width.Set(840f, 0f);
            health.Height.Set(131f, 0f);
            manatext.Width.Set(840f, 0f);
            manatext.Height.Set(131f, 0f);
            xptext.Width.Set(840f, 0f);
            xptext.Height.Set(131f, 0f);
            Level.Width.Set(840f, 0f);
            Level.Height.Set(131f, 0f);

            health.Left.Set(0, 0f);
            health.Top.Set(65 + 262, 0f);
            manatext.Left.Set(0, 0f);
            manatext.Top.Set(90 + 262, 0f);
            xptext.Left.Set(0, 0f);
            xptext.Top.Set(112 + 262, 0f);

            Level.Left.Set(-290, 0f);
            Level.Top.Set(285, 0f);

            MainPanel1.Append(health);
            MainPanel1.Append(manatext);
            MainPanel1.Append(xptext);
            MainPanel1.Append(Level);

            //Texture2D OverlayTexture = ModLoader.GetTexture("AnotherRpgMod/Assets/UI/OverlayHealthBar");

        }
    }




    class RessourceBreath : UIElement
    {
        private Texture2D _texture;
        public float ImageScale = 1f;
        public Color color;

        private Mode stat;
        private float width;
        private float height;

        public RessourceBreath(Mode stat, Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
            height = _texture.Height;
            Left.Set(0, 0f);
            Top.Set(0, 0f);
            this.color = Color.White;
            this.stat = stat;
        }

        public void SetImage(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.player[Main.myPlayer];
            float quotient = 1f;
            //Calculate quotient

            quotient = (float)player.breath / (float)player.breathMax;

            this.Height.Set(quotient * height, 0f);
            Top.Set((1 - quotient) * height, 0);
            Recalculate(); // recalculate the position and size

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point1 = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);

            spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f, new Rectangle(point1.X, point1.Y, width, height), color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
        }
    }

    class Ressource : UIElement
    {
        private Texture2D _texture;
        public float ImageScale = 1f;
        public Color color;

        private Mode stat;
        private float width;
        private float height;

        public Ressource(Mode stat, Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
            width = _texture.Width;
            Left.Set(0, 0f);
            Top.Set(0, 0f);
            this.color = Color.White;
            this.stat = stat;
        }

        public void SetImage(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.player[Main.myPlayer];
            float quotient = 1f;
            //Calculate quotient
            switch (stat)
            {
                case Mode.HP:
                    quotient = (float)player.statLife / (float)player.statLifeMax2;
                    break;

                case Mode.MANA:
                    quotient = (float)player.statMana / (float)player.statManaMax2;
                    break;
                case Mode.XP:
                    quotient = (float)player.GetModPlayer<RPGPlayer>().GetExp() / (float)player.GetModPlayer<RPGPlayer>().XPToNextLevel();
                    break;

                default:
                    break;
            }

            this.Left.Set(-(1 - quotient) * width, 0f);
            Recalculate(); // recalculate the position and size

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();


            spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f, null, color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
        }
    }

    class UIOverlay : UIElement
    {
        private Texture2D _texture;
        public float ImageScale = 1f;

        public UIOverlay(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
            Left.Set(0, 0f);
            Top.Set(0, 0f);
        }

        public void SetImage(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(_texture, dimensions.Position() + _texture.Size() * (1f - ImageScale) / 2f, null, Color.White, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
        }
    }


    class PanelBar : UIElement
    {
        private Mode stat;
        private float width;


        public PanelBar(Mode stat, Texture2D texture)
        {
            this.stat = stat;
            width = texture.Width;

        }



    }
}


