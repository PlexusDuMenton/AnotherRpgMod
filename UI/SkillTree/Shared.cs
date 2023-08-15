using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ModLoader;
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Items;
using ReLogic.Content;

namespace AnotherRpgMod.UI
{
    class Skill : UIElement
    {
        private Texture2D _texture;
        public Color color = Color.White;
        public Skill(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero, SkillTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
        }
    }
    class ItemSkill : UIElement
    {
        public bool Hidden = false;
        private Texture2D _texture;
        public Texture2D SetTexture { set { _texture = value; } }
        public Color color = Color.White;
        public ItemSkill(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Hidden)
                return;
            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero, ItemTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
        }
    }

    class SkillText : UIText
    {
        public NodeParent node;

        public SkillText(string text, NodeParent node, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            this.node = node;
        }
    }

    class ItemSkillText : UIText
    {
        public ItemNode node;

        public ItemSkillText(string text, ItemNode node, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            this.node = node;
        }
    }

    class ItemSkillPanel : UIPanel
    {
        public ItemNode node;
        public ItemSkill skill;
        public Vector2 basePos;
        private Texture2D _texture;
        public Color color = Color.White;
        public bool Hidden = false;
        public ItemSkillPanel(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width * ItemTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(_texture.Height * ItemTreeUi.Instance.sizeMultplier, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            if (Hidden)
                return;
            spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero, ItemTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
        }
    }


    class SkillPanel : UIPanel
    {
        public NodeParent node;
        public Skill skill;
        public Vector2 basePos;
        private Texture2D _texture;
        public Color color = Color.White;
        public SkillPanel(Texture2D texture)
        {
            _texture = texture;
            Width.Set(_texture.Width * SkillTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(_texture.Height * SkillTreeUi.Instance.sizeMultplier, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(_texture, dimensions.Position(), null, color, 0f, Vector2.Zero, SkillTreeUi.Instance.sizeMultplier, SpriteEffects.None, 0f);
        }
    }


    class ItemConnection : UIElement
    {
        public bool Hidden = false;
        public int nodeID;
        public int neighboorID;
        private Texture2D texture = ModContent.Request<Texture2D>("AnotherRpgMod/Textures/UI/Blank", AssetRequestMode.ImmediateLoad).Value;
        public Color color;
        public Vector2 basePos;
        public bool bg = false;
        public float m_rotation;

        public ItemConnection(float rotation, float distance, float height)
        {
            Width.Set(distance * ItemTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(height * ItemTreeUi.Instance.sizeMultplier, 0f);
            m_rotation = rotation;
            this.color = Color.White;
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Hidden)
                return;
            CalculatedStyle dimensions = GetDimensions();
            Point point1 = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);

            spriteBatch.Draw(texture, dimensions.Position(), new Rectangle(point1.X, point1.Y, width, height), color, m_rotation, bg ? new Vector2(0, 5) : new Vector2(0, 3), 1, SpriteEffects.None, 0f);
        }
    }

    class Connection : UIElement
    {
        public NodeParent node;
        public NodeParent neighboor;
        private Texture2D texture = ModContent.Request<Texture2D>("AnotherRpgMod/Textures/UI/Blank", AssetRequestMode.ImmediateLoad).Value;
        public Color color;
        public Vector2 basePos;
        public bool bg = false;
        public float m_rotation;

        public Connection(float rotation, float distance, float height)
        {
            Width.Set(distance * SkillTreeUi.Instance.sizeMultplier, 0f);
            Height.Set(height * SkillTreeUi.Instance.sizeMultplier, 0f);
            m_rotation = rotation;
            this.color = Color.White;
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point1 = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);

            spriteBatch.Draw(texture, dimensions.Position(), new Rectangle(point1.X, point1.Y, width, height), color, m_rotation, bg ? new Vector2(0, 5) : new Vector2(0, 3), 1, SpriteEffects.None, 0f);
        }
    }
}
