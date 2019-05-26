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
        Leech,
        MANA,
        XP,
        Weapon,
        Breath,
        
    }

    class BuffIcon : UIElement
    {
        private Texture2D _texture;
        public Color color;
        public float ImageScale = 1f;

        public BuffIcon(Texture2D texture)
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

            spriteBatch.Draw(_texture, dimensions.Position() + new Vector2(0, _texture.Size().Y) * (1f - ImageScale), null, color, 0f, Vector2.Zero, ImageScale, SpriteEffects.None, 0f);
        }
    }


    class RessourceInfo
    {
        public Texture2D texture;
        public Vector2 position;
        private Vector2 baseSize;
        public Vector2 size;
        public Color color;

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

        Ressource[] ressourcebar = new Ressource[5];

        RessourceBreath breath;
        UIOverlay Overlay;



        public UIElement[] MainPanel = new UIElement[7];

        public UIElement buffPanel;
        public UIElement buffTTPanel;
        public List<BuffIcon> BuffList;

        private UIText health;
        private UIText manatext;
        private UIText xptext;
        private UIText Level;

        public static bool visible = false;
        public bool hiden = false;
        public override void Update(GameTime gameTime)
        {

            UpdateBuffList();
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
            buffPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
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

        
        private void UpdateBuffList()
        {
            buffPanel.RemoveAllChildren();
            buffTTPanel.RemoveAllChildren();
            int rowLimit = 11;
            for (int i = 0; i < Main.player[Main.myPlayer].buffType.Length; i++)
            {

                if (Main.player[Main.myPlayer].buffType[i] > 0)
                {
                    int buffType = Main.player[Main.myPlayer].buffType[i];
                    int x_pos = 32 + i * 38;
                    int y_pos = 76;
                    if (i >= rowLimit)
                    {
                        y_pos -= 50;
                        x_pos = 32 + (i - rowLimit) * 38;
                    }
                    DrawBuff(buffType,i, x_pos, y_pos);
                    
                }
            }
        }

        private void DrawBuff(int type,int i,int x,int y)
        {
            BuffIcon buffIcon = new BuffIcon(Main.buffTexture[type]);
            buffIcon.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
            
            buffIcon.Left.Set(x, 0f);
            buffIcon.Top.Set(y, 0f);
            if (!Main.vanityPet[type] && 
                !Main.lightPet[type] && 
                !Main.buffNoTimeDisplay[type] && 
                (!Main.player[Main.myPlayer].honeyWet || type != 48) && 
                (!Main.player[Main.myPlayer].wet || !Main.expertMode || type != 46) && 
                Main.player[Main.myPlayer].buffTime[i] > 2)
            {
                string text = Lang.LocalizedDuration(new TimeSpan(0, 0, Main.player[Main.myPlayer].buffTime[i] / 60), true, false);
                UIText uIText = new UIText(text, scale);
                uIText.Top.Set(Main.buffTexture[type].Height,0);
                buffIcon.Append(uIText);
                
                //buffIcon.MouseOver() += draw
            }
            
            if (Main.mouseX-buffPanel.Left.Pixels < x + Main.buffTexture[type].Width && Main.mouseY -buffPanel.Top.Pixels < y + Main.buffTexture[type].Height && Main.mouseX- buffPanel.Left.Pixels > x && Main.mouseY- buffPanel.Top.Pixels > y)
            {
                DrawBuffToolTip(type, buffIcon);
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    RemoveBuff(i, type);
                }
            }
            buffPanel.Append(buffIcon);
        }
        private void RemoveBuff(int id, int type)
        {
            AnotherRpgMod.Instance.Logger.Info("Remove buff");
            bool flag = false;
            if (!Main.debuff[type] && type != 60 && type != 151)
            {
                if (Main.player[Main.myPlayer].mount.Active && Main.player[Main.myPlayer].mount.CheckBuff(type))
                {
                    Main.player[Main.myPlayer].mount.Dismount(Main.player[Main.myPlayer]);
                    flag = true;
                }
                if (Main.player[Main.myPlayer].miscEquips[0].buffType == type && !Main.player[Main.myPlayer].hideMisc[0])
                {
                    Main.player[Main.myPlayer].hideMisc[0] = true;
                }
                if (Main.player[Main.myPlayer].miscEquips[1].buffType == type && !Main.player[Main.myPlayer].hideMisc[1])
                {
                    Main.player[Main.myPlayer].hideMisc[1] = true;
                }
                Main.PlaySound(12, -1, -1, 1, 1f, 0f);
                if (!flag)
                {
                    Main.player[Main.myPlayer].DelBuff(id);
                }
            }
        }


        private void DrawBuffToolTip(int id,BuffIcon icon)
        {
            int mouseY = Main.lastMouseY - (int)buffPanel.Top.Pixels;
            int mouseX = Main.lastMouseX - (int)buffPanel.Left.Pixels;
            icon.color = new Color(1, 1, 1, 1f);
            string buffDesc = Lang.GetBuffDescription(id);
            if (id == 26 && Main.expertMode)
            {
                buffDesc = Language.GetTextValue("BuffDescription.WellFed_Expert");
            }
            if (id == 94)
            {
                int percentManaSick = (int)(Main.player[Main.myPlayer].manaSickReduction * 100f) + 1;
                buffDesc = Main.buffString + percentManaSick + "%";
            }

            string buffName = Lang.GetBuffName(id);
            UIText TText = new UIText(buffName, scale,true);
            TText.Top.Set(mouseY -60, 0);
            TText.Left.Set(mouseX + 20, 0);
            UIText DescText = new UIText(buffDesc, scale);
            DescText.Top.Set(mouseY - 30, 0);
            DescText.Left.Set(mouseX + 20, 0);

            buffTTPanel.Append(TText);
            buffTTPanel.Append(DescText);
            if (id == 147)
            {
                string bannerTT = "";
                for (int l = 0; l < NPCLoader.NPCCount; l++)
                {
                    if (Item.BannerToNPC(l) != 0 && Main.player[Main.myPlayer].NPCBannerBuff[l])
                    {
                        bannerTT += "\n" + Lang.GetNPCNameValue(Item.BannerToNPC(l));
                    }
                }
                    
                UIText BText = new UIText(bannerTT, scale);
                BText.Top.Set(mouseY - 20, 0);
                BText.Left.Set(mouseX + 20, 0);
                BText.TextColor = Color.Green;
                buffTTPanel.Append(BText);
            }

        }
        
        public void Erase()
        {

            base.RemoveAllChildren();
        }

        public override void OnInitialize()
        {
            Erase();

            buffTTPanel = new UIElement();
            buffTTPanel.Left.Set(10 * scale, 0f);
            buffTTPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
            buffTTPanel.Width.Set(0, 0f);
            buffTTPanel.Height.Set(0, 0f);

            buffPanel = new UIElement();
            buffPanel.Left.Set(10 * scale, 0f);
            buffPanel.Top.Set(Main.screenHeight - baseUiHeight + YDefaultOffSet + 185 * scale, 0f);
            buffPanel.Width.Set(1000, 0f);
            buffPanel.Height.Set(400, 0f);
            YDefaultOffSet = -Config.vConfig.HealthBarYoffSet;
            scale = Config.vConfig.HealthBarScale;

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
                { Mode.Leech, new RessourceInfo(ModContent.GetTexture("AnotherRpgMod/Textures/UI/LeechBar"),new Vector2(14*scale,Main.screenHeight + YDefaultOffSet - baseUiOffset[0]),scale)},
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

            for (int i = 0; i < 6; i++)
            {
                MainPanel[i + 1] = new PanelBar((Mode)i, RessourceTexture[(Mode)i].texture);
                if ((Mode)i == Mode.Breath) {
                    breath = new RessourceBreath((Mode)i, RessourceTexture[(Mode)i].texture);
                }
                else
                {
                    ressourcebar[i] = new Ressource((Mode)i, RessourceTexture[(Mode)i].texture, Color.White);
                }
                MainPanel[i + 1].HAlign = 0;
                MainPanel[i + 1].VAlign = 0;
                MainPanel[i + 1].SetPadding(0);
                
                MainPanel[i + 1].Width.Set(RessourceTexture[(Mode)i].size.X, 0f);
                MainPanel[i + 1].Height.Set(RessourceTexture[(Mode)i].size.Y, 0f);
                MainPanel[i + 1].Left.Set(RessourceTexture[(Mode)i].position.X, 0f);
                MainPanel[i + 1].Top.Set(RessourceTexture[(Mode)i].position.Y, 0f);

                if ((Mode)i == Mode.Breath)
                {
                    breath.ImageScale = scale;
                    MainPanel[i + 1].Append(breath);
                }
                else
                {
                    ressourcebar[i].ImageScale = scale;
                    if (i == 0)
                        ressourcebar[i].color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                    MainPanel[i + 1].Append(ressourcebar[i]);
                }

                base.Append(MainPanel[i + 1]);


            }
            ;
            
            base.Append(MainPanel[0]);
            base.Append(buffPanel);
            base.Append(buffTTPanel);


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

        public Ressource(Mode stat, Texture2D texture, Color col)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
            width = _texture.Width;
            Left.Set(0, 0f);
            Top.Set(0, 0f);
            this.color = col;
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
                case Mode.Leech:
                    quotient = (float)Utils.Mathf.Clamp(player.statLife + player.GetModPlayer<RPGPlayer>().GetLifeLeechLeft,0, player.statLifeMax2) / (float)player.statLifeMax2;
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


