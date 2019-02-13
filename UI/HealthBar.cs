using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ID;
using Terraria;
using System;
using System.Collections.Generic;
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
        Weapon,
        Breath
    }


    class RessourceInfo
    {
        public Texture2D texture;
        public Vector2 position;
        private Vector2 baseSize;
        public Vector2 size;

        public RessourceInfo(Texture2D _texture, Vector2 _position,float scale = 1f)
        {
            texture = _texture;
            position = _position;
            baseSize = _texture.Size();
            size = baseSize * scale;
        }

        public void ChangeSize(float scale)
        {
            size = baseSize * scale;
        }


    }

    class HealthBar : UIState
    {

        

        private Player player;

        float YDefaultOffSet = -Config.vConfig.HealthBarYoffSet;
        float scale = Config.vConfig.HealthBarScale;
        float baseUiHeight = 393f;

        Dictionary<Mode, RessourceInfo> RessourceTexture;

        Ressource[] ressourcebar = new Ressource[4];

        RessourceBreath breath;
        UIOverlay Overlay;



        public UIElement[] MainPanel = new UIElement[6];

        private UIText health;
        private UIText manatext;
        private UIText xptext;
        private UIText Level;

        public static bool visible = false;
        public bool hiden = false;

        public override void Update(GameTime gameTime)
        {

            if (!Config.gpConfig.RPGPlayer)
            {
                RemoveAllChildren();
                hiden = true;
                return;
            }

            if (hiden)
            {
                hiden = false;
                OnInitialize();
            }

            Player player = Main.player[Main.myPlayer]; //Get Player

            if (player.GetModPlayer<RPGPlayer>().m_virtualRes > 0)
            {
                health.Left.Set(450 * scale, 0f);
                health.SetText("" + player.statLife + " | " + player.statLifeMax2 + " (" + player.statLifeMax2/(1- player.GetModPlayer<RPGPlayer>().m_virtualRes)+")"); //Set Life
            }
            else
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

            

            float[] baseUiOffset =
            {
                (105*scale),
                (69*scale ),
                (46*scale ),
                (33*scale ),
                (357*scale )
            };

            RessourceTexture = new Dictionary<Mode, RessourceInfo>()
            {
                { Mode.HP, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/HealthBar"),new Vector2(14*scale,Main.screenHeight + YDefaultOffSet - baseUiOffset[0]),scale)},
                { Mode.MANA, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/ManaBar"),new Vector2(31*scale,Main.screenHeight  + YDefaultOffSet - baseUiOffset[1]),scale)},
                { Mode.XP, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/XPBar"),new Vector2(44*scale,Main.screenHeight + YDefaultOffSet -baseUiOffset[2]),scale)},
                { Mode.Weapon, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/WeaponBar"),new Vector2(50*scale,Main.screenHeight + YDefaultOffSet - baseUiOffset[3]),scale)},
                { Mode.Breath, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/BreathBar"),new Vector2(5*scale,Main.screenHeight + YDefaultOffSet - baseUiOffset[4]),scale)}

            };


            MainPanel[0] = new UIElement();
            MainPanel[0].SetPadding(0);
            MainPanel[0].Width.Set(840f, 0f);
            MainPanel[0].Height.Set(baseUiHeight, 0f);
            MainPanel[0].HAlign = 0;
            MainPanel[0].VAlign = 0;
            MainPanel[0].Left.Set(0, 0f);
            MainPanel[0].Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet, 0f);

            Overlay = new UIOverlay(ModContent.GetTexture("AnotherRpgMod/Textures/UI/OverlayHealthBar"));
            Overlay.ImageScale = scale;
            Overlay.HAlign = 0;
            Overlay.VAlign = 0;
            MainPanel[0].Append(Overlay);

            for (int i = 0; i < 5; i++)
            {
                MainPanel[i + 1] = new PanelBar((Mode)i, RessourceTexture[(Mode)i].texture);
                if (i > 3) {
                    breath = new RessourceBreath((Mode)i, RessourceTexture[(Mode)i].texture);
                }
                else
                {
                    ressourcebar[i] = new Ressource((Mode)i, RessourceTexture[(Mode)i].texture);
                }
                MainPanel[i + 1].HAlign = 0;
                MainPanel[i + 1].VAlign = 0;
                MainPanel[i + 1].SetPadding(0);
                
                MainPanel[i + 1].Width.Set(RessourceTexture[(Mode)i].size.X, 0f);
                MainPanel[i + 1].Height.Set(RessourceTexture[(Mode)i].size.Y, 0f);
                MainPanel[i + 1].Left.Set(RessourceTexture[(Mode)i].position.X, 0f);
                MainPanel[i + 1].Top.Set(RessourceTexture[(Mode)i].position.Y, 0f);

                if (i > 3)
                {
                    breath.ImageScale = scale;
                    MainPanel[i + 1].Append(breath);
                }
                else
                {
                    ressourcebar[i].ImageScale = scale;
                    MainPanel[i + 1].Append(ressourcebar[i]);
                }

                base.Append(MainPanel[i + 1]);


            }

            base.Append(MainPanel[0]);

            health = new UIText("0|0", 1.3f* scale);
            manatext = new UIText("0|0", scale);
            xptext = new UIText("0|0", scale);
            Level = new UIText("Lvl. 1", 0.7f* scale, true);


            health.Left.Set( 500 * scale, 0f);
            health.Top.Set(MainPanel[0].Height.Pixels  - 99 * scale, 0f);
            manatext.Left.Set(420* scale, 0f);
            manatext.Top.Set(MainPanel[0].Height.Pixels  - 69 * scale, 0f);
            xptext.Left.Set(420* scale, 0f);
            xptext.Top.Set(MainPanel[0].Height.Pixels  - 47 * scale, 0f);

            Level.Left.Set(135 * scale, 0f);
            Level.HAlign = 0;
            Level.Top.Set(MainPanel[0].Height.Pixels  - 136 * scale, 0f);

            MainPanel[0].Append(health);
            MainPanel[0].Append(manatext);
            MainPanel[0].Append(xptext);
            MainPanel[0].Append(Level);

            Recalculate();
            //Texture2D OverlayTexture = ModLoader.GetTexture("AnotherRpgMod/Assets/UI/OverlayHealthBar");

        }
    }




    class RessourceBreath : UIElement
    {
        private Texture2D _texture;
        public float ImageScale = 1f;
        public Color color;

        private Mode stat;
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
            VAlign = 0;
            HAlign = 0;

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

            this.Height.Set(quotient * height*ImageScale, 0f);
            Top.Set((1 - quotient) * height * ImageScale, 0);
            Recalculate(); // recalculate the position and size

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 point1 = new Vector2((float)dimensions.X, (float)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width * 1 / ImageScale) ;
            int height = (int)Math.Ceiling(dimensions.Height * 1 / ImageScale) ;

            spriteBatch.Draw(_texture, dimensions.Position() , new Rectangle((int)point1.X, (int)point1.Y, width, height), color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
        }
    }

    class Ressource : UIElement
    {
        private Texture2D _texture;
        public float ImageScale = 1f;
        public Color color;

        private Mode stat;
        private float width;

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
            VAlign = 0;
            HAlign = 0;
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
                case Mode.Weapon:
                    quotient = (float)player.GetModPlayer<RPGPlayer>().EquipedItemXp / (float)player.GetModPlayer<RPGPlayer>().EquipedItemMaxXp;
                    break;

                default:
                    break;
            }

            this.Left.Set(-(1 - quotient) * width * ImageScale, 0f);
            Recalculate(); // recalculate the position and size

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();


            spriteBatch.Draw(_texture, dimensions.Position() , null, color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
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
            VAlign = 0;
            HAlign = 0;
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

            spriteBatch.Draw(_texture, dimensions.Position() + new Vector2(0, _texture.Size().Y) * (1f - ImageScale), null, Color.White, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
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
            VAlign = 0;
            HAlign = 0;
        }



    }
}


