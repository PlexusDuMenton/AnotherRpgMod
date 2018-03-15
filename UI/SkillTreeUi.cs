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
using System.Windows.Forms;
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Utils;

namespace AnotherRpgMod.UI
{
    class SkillTreeUi : UIState
    {

        UIPanel backGround;
        public static SkillTreeUi Instance;
        private SkillTree skillTree;
        public static bool visible = false;


        public void LoadSkillTree()
        {
            skillTree = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree;

        }

        public override void OnInitialize()
        {
            Instance = this;
            backGround = new UIPanel();

            for (int i = 0; i < NodeParent.NodeList.Count; i++)
            {
                SkillInit(NodeParent.NodeList[i]);
            }
        }


        public void SkillInit(NodeParent node)
        {
            DrawSkill(node.menuPos, ModLoader.GetTexture("AnotherRpgMod/Textures/UI/Blank"));
        }

        public void DrawSkill(Vector2 pos,Texture2D tex)
        {
            UIPanel basePanel = new UIPanel();
            basePanel.SetPadding(0);
            basePanel.Left.Set(pos.X, 0f);
            basePanel.Top.Set(pos.Y, 0f);
            basePanel.Width.Set(64, 0f);
            basePanel.Height.Set(64, 0f);
            basePanel.BackgroundColor = new Color(73, 94, 171, 150);

            Skill skillIcon = new Skill(tex);

            backGround.Append(basePanel);
            basePanel.Append(skillIcon);
        }

        public void DrawConnection(Color _color, Vector2 point1, Vector2 points2)
        {
            float angle = 0;
            float distance = 0;
            angle = (float)Math.Atan2(points2.Y, points2.X);
            distance = (float)Math.Sqrt(Math.Pow(points2.X - point1.X, 2) + Math.Pow(points2.Y - point1.Y, 2));
            Connection BG = new Connection(angle, distance, 10)
            {
                color = Color.DarkSlateGray
            };
            Connection connection = new Connection(angle, distance, 6)
            {
                color = Color.Gray
            };
        }

    }

    class Skill : UIElement
    {
        private Texture2D _texture;

        public Skill(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width, 0f);
            Height.Set(_texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(_texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
    }

class Connection : UIElement
    {
        private Texture2D texture = ModLoader.GetTexture("AnotherRpgMod/Textures/UI/Blank");
        public Color color;

        public float rotation;

        public Connection(float rotation, float distance,float height)
        {
            Width.Set(distance, 0f);
            Height.Set(height, 0f);
            this.color = Color.White;
        }

        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point1 = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);

            spriteBatch.Draw(texture, dimensions.Position() + new Vector2(0, texture.Size().Y)/2, new Rectangle(point1.X, point1.Y, width, height), color, 0f, new Vector2(texture.Size().Y,0)*0.5f, 1, SpriteEffects.None, 0f);
        }
    }

}
