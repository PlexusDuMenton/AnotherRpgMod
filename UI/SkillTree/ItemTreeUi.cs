using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ID;
using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using AnotherRpgMod.RPGModule.Entities;
using System.Reflection;
using Terraria.GameInput;
using Terraria.Localization;
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Utils;
using AnotherRpgMod.Items;
using Terraria.Audio;
using ReLogic.Content;

namespace AnotherRpgMod.UI
{
    class ItemTreeUi : UIState
    {

        UIPanel backGround;
        UIPanel toolTip;

        List<ItemConnection> allConnection;
        List<ItemSkillPanel> allBasePanel;
        List<ItemSkillText> allText;

        UIText ResetText;
        float zoomMax = 2f;
        float zoomMin = 0.5f;

        float iZoom = 1;
        static float uIScale = 1;
        float screenMult = (Main.screenHeight / 1080f);


        public float sizeMultplier;
        private void ResetTextHover(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.White;
        }
        private void ResetTextOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.Gray;
        }

        public static ItemTreeUi Instance;

        private UIText skillPointsLeft;

        protected ItemUpdate m_itemSource;

        public static bool visible = false;

        public Vector2 offSet = new Vector2(Main.screenWidth*0.5f, Main.screenHeight*0.5f)/ Config.vConfig.UI_Scale;
        const int SKILL_SIZE = 64;
        const int CONNECTION_WIDTH = 8;

        public override void OnInitialize()
        {
            Instance = this;
        }

        public void Erase()
        {
            if (backGround != null) { 
                backGround.RemoveAllChildren();
                backGround.Remove();
                }
        }

        public void UpdateValue()
        {
            int listSize = allConnection.Count;
            for (int i = 0; i < listSize; i++)
            {
                if (!allConnection[i].bg) 
                    allConnection[i].color = ( m_itemSource.GetItemTree.GetNode(allConnection[i].neighboorID).GetState ==4 || m_itemSource.GetItemTree.GetNode(allConnection[i].nodeID).GetState == 4) ? Color.GreenYellow : Color.DarkRed;

                allConnection[i].Hidden = (m_itemSource.GetItemTree.GetNode(allConnection[i].neighboorID).GetState == 0 || m_itemSource.GetItemTree.GetNode(allConnection[i].nodeID).GetState == 0);
            }
            listSize = allBasePanel.Count;

            for (int i = 0; i < listSize; i++)
            {
                if (allBasePanel[i].node.GetState > 1 )
                    allBasePanel[i].skill.SetTexture =  ModContent.Request<Texture2D>(SkillTextures.GetItemTexture(allBasePanel[i].node), AssetRequestMode.ImmediateLoad).Value;
                allBasePanel[i].Hidden = false;
                allBasePanel[i].skill.Hidden = false;
                switch (allBasePanel[i].node.GetState)
                {
                    case 1:
                        
                        allBasePanel[i].skill.color = new Color(255, 255, 255, 255);
                        allBasePanel[i].color = new Color(255, 255, 255, 255);
                        break;
                    case 2:
                        allBasePanel[i].skill.color = new Color(220, 220, 120, 255);
                        allBasePanel[i].color = new Color(160, 180, 50, 255);
                        break;
                    case 3:

                        allBasePanel[i].skill.color = new Color(220, 220, 220, 255);
                        allBasePanel[i].color = new Color(120, 120, 120, 255);
                        break;
                    case 0:
                        allBasePanel[i].Hidden = true;
                        allBasePanel[i].skill.Hidden = true;
                        break;
                    default:
                        allBasePanel[i].skill.color = Color.White;
                        allBasePanel[i].color = new Color(180, 220, 255, 255);
                        break;
                }
            }
            
            listSize = allText.Count;
            for (int i = 0; i < listSize; i++)
            {
                if (allBasePanel[i].Hidden)
                    allText[i].SetText("");
                else 
                    allText[i].SetText(allText[i].node.GetLevel + " / " + allText[i].node.GetMaxLevel, sizeMultplier,false);
            }
        }

        private void ResetStats(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            SoundEngine.PlaySound(SoundID.MenuOpen);
                m_itemSource.ResetTree();
                Init();
                

            dragging = false;
            
        }

        public void Open(ItemUpdate _item)
        {
            m_itemSource = _item;
            Init();
        }




        public void Init()
        {
            //Erase all previous value
            Erase();

            if (m_itemSource == null)
            {
                visible = false;
                return;
            }

            uIScale = Config.vConfig.UI_Scale;

            sizeMultplier = iZoom * uIScale * screenMult;
            allConnection = new List<ItemConnection>();
            allBasePanel = new List<ItemSkillPanel>();
            allText = new List<ItemSkillText>();

            backGround = new UIPanel();




            backGround.OnLeftMouseDown += new UIElement.MouseEvent(DragStart);
            backGround.OnLeftMouseUp += new UIElement.MouseEvent(DragEnd);
            backGround.OnMiddleClick += new MouseEvent(ResetOffset);
            backGround.OnScrollWheel += new ScrollWheelEvent(iScrollUpDown);

            backGround.SetPadding(0);
            backGround.Left.Set(0, 0f);
            backGround.Top.Set(0, 0f);
            backGround.Width.Set(Main.screenWidth, 0f);
            backGround.Height.Set(Main.screenHeight, 0f);
            backGround.BackgroundColor = new Color(73, 94, 171,150);
            Append(backGround);
            string points = "Evolution Points : " + m_itemSource.GetEvolutionPoints + " / " + m_itemSource.GetMaxEvolutionPoints;
            if (m_itemSource.Ascention > 0)
            {
                points += "\nAscend Points : " + m_itemSource.GetAscendPoints + " / " + m_itemSource.GetMaxAscendPoints;
            }
            skillPointsLeft = new UIText(points);
            skillPointsLeft.Left.Set(150, 0f);
            skillPointsLeft.Top.Set(150, 0f);
            backGround.Append(skillPointsLeft);

            ResetText = new UIText("RESET", 1* screenMult, true)
            {
                TextColor = Color.Gray
            };
            ResetText.Left.Set(150* screenMult, 0f);
            ResetText.Top.Set(250* screenMult, 0f);
            ResetText.Width.Set(0, 0f);
            ResetText.Height.Set(0, 0f);
            ResetText.OnLeftClick += new MouseEvent(ResetStats);
            ResetText.OnMouseOver += new MouseEvent(ResetTextHover);
            ResetText.OnMouseOut += new MouseEvent(ResetTextOut);
            backGround.Append(ResetText);

            

            Instance = this;
            for (int i = 0; i < m_itemSource.GetItemTree.GetSize; i++)
            {

                SkillInit(m_itemSource.GetItemTree.GetNode(i));
                
            }


            int listSize = allConnection.Count;
            for (int i = 0; i < listSize; i++)
            {
                backGround.Append(allConnection[i]);
            }
            listSize = allBasePanel.Count;
            for (int i = 0; i < listSize; i++)
            {
                backGround.Append(allBasePanel[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            int listSize = allConnection.Count;
            for (int i = 0;i< listSize; i++)
            {
                allConnection[i].Left.Set((allConnection[i].basePos.X + offSet.X)* sizeMultplier, 0);
                allConnection[i].Top.Set((allConnection[i].basePos.Y + offSet.Y)* sizeMultplier, 0);
            }
            listSize = allBasePanel.Count;
            for (int i = 0; i < listSize; i++)
            {
                allBasePanel[i].Left.Set((allBasePanel[i].basePos.X + offSet.X)*sizeMultplier, 0);
                allBasePanel[i].Top.Set((allBasePanel[i].basePos.Y + offSet.Y)*sizeMultplier, 0);
            }

            string points = "Evolution Points : " + m_itemSource.GetEvolutionPoints + " / " + m_itemSource.GetMaxEvolutionPoints;
            if (m_itemSource.Ascention > 0)
            {
                points+= "\nAscend Points : " + m_itemSource.GetAscendPoints + " / " + m_itemSource.GetMaxAscendPoints;
            }
            skillPointsLeft.SetText(points);

            UpdateValue();
            Recalculate();
        }

        public void SkillInit(ItemNode node)
        {
            

            DrawSkill(node);

            ItemNode neightboor;
            for (int j = 0; j < node.GetNeighboor.Count; j++)
            {
                neightboor = node.GetParent.GetNode(node.GetNeighboor[j]);
                if (neightboor!= null)
                {
                    Color ConnectionColor = (neightboor.GetState == 4 || node.GetState == 4) ? Color.GreenYellow : Color.DarkRed;
                    
                    DrawConnection(ConnectionColor, node.GetPos, neightboor.GetPos, node,neightboor, (neightboor.GetState == 0 || node.GetState == 0));
                }
            }

            
        }
            
        public void DrawSkill(ItemNode node) //Vector2 pos, Texture2D tex,int state)
        {



            ItemSkillPanel basePanel = new ItemSkillPanel(ModContent.Request<Texture2D>("AnotherRpgMod/Textures/UI/skill_blank", AssetRequestMode.ImmediateLoad).Value);
            basePanel.SetPadding(0);
            basePanel.Width.Set(SKILL_SIZE * sizeMultplier, 0f);
            basePanel.Height.Set(SKILL_SIZE * sizeMultplier, 0f);
            ItemSkill skillIcon = new ItemSkill(ModContent.Request<Texture2D>("AnotherRpgMod/Textures/ItemTree/unknow", AssetRequestMode.ImmediateLoad).Value);
            if (node.GetState >1)
                skillIcon = new ItemSkill(ModContent.Request<Texture2D>(SkillTextures.GetItemTexture(node)).Value);

            skillIcon.Width.Set(SKILL_SIZE * sizeMultplier, 0f);
            skillIcon.Height.Set(SKILL_SIZE * sizeMultplier, 0f);


            switch (node.GetState) { 
                case 1:
                    basePanel.Hidden = false;
                    skillIcon.Hidden = false;
                    skillIcon.color = new Color(255,255,255,255);
                    basePanel.color = new Color(255, 255, 255, 255);
                    break;
                case 2:
                    skillIcon.color = new Color(220, 220, 120, 255);
                    basePanel.color = new Color(160, 180, 50, 255);
                    break;
                case 3:
                    skillIcon.color = new Color(220, 220, 220, 255);
                    basePanel.color = new Color(120, 120, 120, 255);
                    break;
                case 0:
                    basePanel.Hidden = true;
                    skillIcon.Hidden = true;
                    break;
                default:
                    skillIcon.color = Color.White;
                    basePanel.color = new Color(180, 220, 255, 255);
                    break;
            }
            
            basePanel.basePos = new Vector2(node.GetPos.X, node.GetPos.Y);
            basePanel.node = node;
            basePanel.skill = skillIcon;
            ItemSkillText levelText = new ItemSkillText(node.GetLevel + " / " + node.GetMaxLevel,node,sizeMultplier,false);
            if (basePanel.Hidden)
                levelText.SetText("");
            levelText.Left.Set(SKILL_SIZE*0.2f* sizeMultplier, 0);
            levelText.Top.Set(SKILL_SIZE*0.5f* sizeMultplier, 0);
            allText.Add(levelText);
            allBasePanel.Add(basePanel);
            skillIcon.OnMouseOver += new MouseEvent((UIMouseEvent, UIElement) => OpenToolTip(UIMouseEvent, UIElement,node,skillIcon.Hidden));
            skillIcon.OnLeftClick += new MouseEvent((UIMouseEvent, UIElement) => OnClickNode(UIMouseEvent, UIElement, node));
            skillIcon.OnRightClick += new MouseEvent((UIMouseEvent, UIElement) => OnRightClickNode(UIMouseEvent, UIElement, node));
            skillIcon.OnMouseOut += new MouseEvent(CloseToolTip);
            basePanel.Append(skillIcon);
            basePanel.Append(levelText);
        }

        public void DrawConnection(Color _color, Vector2 point1, Vector2 point2, ItemNode node, ItemNode neighboor,bool hiden)
        {
            
            float angle = 0;
            float distance = 0;
            //angle = (float)(Math.Atan2(point2.Y- point1.Y, point2.X - point1.X) *(180f/Math.PI));
            angle = (float)(Math.Atan2(point2.Y - point1.Y, point2.X - point1.X) );
            distance = (float)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
            ItemConnection BG = new ItemConnection(angle, distance, 10)
            {
                color = Color.DarkSlateGray,
                Hidden = hiden

            };
            
            ItemConnection connection = new ItemConnection(angle, distance, 6)
            {
                color = _color,
            };
            BG.basePos = new Vector2(point1.X + SKILL_SIZE * 0.5f, point1.Y + SKILL_SIZE * 0.5f);
            connection.basePos = new Vector2(point1.X+ SKILL_SIZE*0.5f, point1.Y+ SKILL_SIZE * 0.5f);
            connection.nodeID = node.GetId;
            connection.neighboorID = neighboor.GetId;
            BG.bg = true;
            allConnection.Add(connection);
        }


        private void iScrollUpDown(UIScrollWheelEvent evt, UIElement listeningElement)
        {
            Vector2 Center = new Vector2(Main.screenWidth * .5f, Main.screenHeight * .5f);
            float preZoom = iZoom;
            Vector2 mouseoffset = evt.MousePosition - Center;
            
            if (evt.ScrollWheelValue > 0)
            {
                iZoom = Mathf.Clamp(1.1f * iZoom, zoomMin, zoomMax);
                
            }
            else if (evt.ScrollWheelValue < 0)
            {
                iZoom = Mathf.Clamp(0.85f * iZoom, zoomMin, zoomMax);
                
            }
            float ratio = iZoom / preZoom;
            offSet /= ratio;
            if (ratio != 1) { 
                if (evt.ScrollWheelValue > 0)
                    offSet -= mouseoffset * iZoom * 0.1f;
               else
                    offSet += mouseoffset * iZoom * 0.02f;
            }

            Init();
        }

        public bool dragging = false;
        Vector2 regOffSet;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            regOffSet = new Vector2(evt.MousePosition.X, evt.MousePosition.Y);

            if (Main.keyState.PressingShift())
            {
                if ((Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl)))
                {
                    offSet.X = 0;
                    offSet.Y = 0;
                }
            }

            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            Vector2 end = evt.MousePosition;
            dragging = false;

            offSet.X += (end.X - regOffSet.X)/sizeMultplier;
            offSet.Y += (end.Y - regOffSet.Y)/ sizeMultplier;
            
            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (backGround.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (dragging)
            {
                offSet.X += (MousePosition.X - regOffSet.X);
                offSet.Y += (MousePosition.Y - regOffSet.Y);
                regOffSet = new Vector2(MousePosition.X, MousePosition.Y);
                Recalculate();
            }
        }

        private void UpdateToolTip(ItemNode node)
        {
            toolTip.Remove();
            toolTip = new UIPanel();

            float unzoomMult = screenMult;

            toolTip.Left.Set((node.GetPos.X + SKILL_SIZE * 2 + offSet.X)* sizeMultplier, 0);
            toolTip.Top.Set((node.GetPos.Y - SKILL_SIZE * 2 + offSet.Y)* sizeMultplier, 0);

            toolTip.Width.Set(550* unzoomMult, 0);
            toolTip.Height.Set(300* unzoomMult, 0);
            toolTip.SetPadding(0);
            toolTip.BackgroundColor = new Color(73, 94, 171, 150);

            UIText Name = new UIText("", 0.5f* unzoomMult, true);
            Name.Left.Set(10* unzoomMult, 0);
            Name.Top.Set(10* unzoomMult, 0);

            Name.SetText(node.GetName + "\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);

            string costText = " State : " + node.GetState + "\n";

            costText += "Cost " + node.GetRequiredPoints;

            if (node.IsAscend)
                costText += " Ascendence Points\n";
            else
                costText += " Evolution Points\n";

            UIText info = new UIText(costText);
            info.Left.Set(50* unzoomMult, 0);
            info.Top.Set(100* unzoomMult, 0);

            UIText description = new UIText(node.GetDesc);
            description.Left.Set(50* unzoomMult, 0);
            description.Top.Set(170* unzoomMult, 0);

            SoundEngine.PlaySound(SoundID.MenuTick);

            backGround.Append(toolTip);
            toolTip.Append(Name);
            toolTip.Append(info);
            toolTip.Append(description);


            backGround.Recalculate();
            Recalculate();
        }

        private void OnRightClickNode(UIMouseEvent evt, UIElement listeningElement, ItemNode node)
        {
            for (int i = 0; i < 5; i++)
            {
                OnClickNode(evt, listeningElement, node);
            }
        }

        private void OnClickNode(UIMouseEvent evt, UIElement listeningElement, ItemNode node)
        {
            int points = 0;
            if (node.IsAscend)
                points = m_itemSource.GetAscendPoints;
            else
                points = m_itemSource.GetEvolutionPoints;
            switch(node.CanAddLevel(points))
            { 
                case ItemReason.CanUpgrade:
                    m_itemSource.SpendPoints( node.GetRequiredPoints,node.IsAscend);
                    node.AddLevel();

                    UpdateToolTip(node);
                    UpdateValue();
                    SoundEngine.PlaySound(SoundID.CoinPickup);
                    break;
                default:
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    break;
            }
            
        }

        private void ResetOffset(UIMouseEvent evt, UIElement listeningElement)
        {
            offSet = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f) / Config.vConfig.UI_Scale;
            iZoom = 1;
            Init();
        }

        private void OpenToolTip(UIMouseEvent evt, UIElement listeningElement,ItemNode node,bool Hidden)
        {
            if (node == null)
            {
                return;
            }
            if (Hidden)
                return;
            toolTip = new UIPanel();
            float unzoomMult = screenMult;


            Vector2 pos = node.GetPos;
            toolTip.Left.Set((pos.X + SKILL_SIZE*2 + offSet.X)* sizeMultplier, 0);
            toolTip.Top.Set((pos.Y - SKILL_SIZE * 2 + offSet.Y)* sizeMultplier, 0);

            toolTip.Width.Set(550* unzoomMult, 0);
            toolTip.Height.Set(300* unzoomMult, 0);
            toolTip.SetPadding(0);
            toolTip.BackgroundColor = new Color(73, 94, 171, 150);

            UIText Name = new UIText("",0.5f,true);
            Name.Left.Set(10, 0);
            Name.Top.Set(10, 0);
            if(node.GetState <= 1)
                Name.SetText("???\nLevel : ?? / ??");
            else
                Name.SetText(node.GetName +"\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);

            string costText = "Cost " + node.GetRequiredPoints;
            if (node.IsAscend)
                costText += " Ascendence Points\n";
            else
                costText += " Evolution Points\n";

            if (node.GetState <= 1)
                costText = "Cost ??? Points";

            UIText info = new UIText(costText);
            info.Left.Set(50* unzoomMult, 0);
            info.Top.Set(100* unzoomMult, 0);

            string desc = node.GetDesc;

            if (node.GetState <= 1)
                desc = "Unknow Bonus\nUnlock a close node to know what this one does !";
            UIText description = new UIText(desc);
            description.Left.Set(50* unzoomMult, 0);
            description.Top.Set(170* unzoomMult, 0);
            
            SoundEngine.PlaySound(SoundID.MenuTick);

            backGround.Append(toolTip);
            toolTip.Append(Name);
            toolTip.Append(info);
            toolTip.Append(description);


            backGround.Recalculate();
        }

        private void CloseToolTip(UIMouseEvent evt, UIElement listeningElement)
        {
            toolTip.Remove();
            Recalculate();
        }


    }
}
