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
using Terraria.Audio;
using ReLogic.Content;

namespace AnotherRpgMod.UI
{
    class SkillTreeUi : UIState
    {

        UIPanel backGround;
        UIPanel toolTip;

        List<Connection> allConnection;
        List<SkillPanel> allBasePanel;
        List<SkillText> allText;

        UIText ResetText;


        float zoomMax = 2f;
        float zoomMin = 0.25f;

        float Zoom = 1;
        float UIScale = Config.vConfig.UI_Scale;
        float ScreenMult = (Main.screenHeight / 1080f);

        public float sizeMultplier;
        private void ResetTextHover(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.White;
        }
        private void ResetTextOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ResetText.TextColor = Color.Gray;
        }

        public static SkillTreeUi Instance;

        private UIText skillPointsLeft;

        private SkillTree skillTree;
        public static bool visible = false;

        public Vector2 offSet = new Vector2(Main.screenWidth*0.5f, Main.screenHeight*0.5f)/ Config.vConfig.UI_Scale;
        const int SKILL_SIZE = 64;
        const int CONNECTION_WIDTH = 8;

        public void LoadSkillTree()
        {
            skillTree = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree;
            Init();
        }

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
                    allConnection[i].color = (allConnection[i].neighboor.GetActivate || allConnection[i].node.GetActivate) ? Color.GreenYellow : Color.Gray;
            }
            listSize = allBasePanel.Count;
            NodeParent Node;
            int state = 0; // disactivated and locked
            for (int i = 0; i < listSize; i++)
            {
                Node = allBasePanel[i].node;
                state = 0;
                if (Node.GetEnable) //if the node is enabled
                    state = 3;
                else if (Node.GetActivate && (Node.GetNodeType == NodeType.Class || Node.GetNodeType == NodeType.Perk))
                    state = 2; // if node is unlocked , activated but not enable (like perk or class skill)
                else if (Node.GetUnlock)
                    state = 1; // if node is just unlocked

                switch (state)
                {
                    case 0:
                        allBasePanel[i].skill.color = new Color(255, 150, 150, 255);
                        allBasePanel[i].color = new Color(255, 50, 50, 255);
                        break;
                    case 1:
                        allBasePanel[i].skill.color = new Color(220, 220, 120, 255);
                        allBasePanel[i].color = new Color(160, 180, 50, 255);
                        break;
                    case 2:
                        allBasePanel[i].skill.color = new Color(220, 220, 220, 255);
                        allBasePanel[i].color = new Color(120, 120, 120, 255);
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
                allText[i].SetText(allText[i].node.GetLevel + " / " + allText[i].node.GetMaxLevel, sizeMultplier,false);
            }
        }

        private void ResetStats(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!visible)
                return;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            RPGPlayer rPGPlayer = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            rPGPlayer.ResetSkillTree();
            rPGPlayer.GetskillTree.Init();
            dragging = false;
            LoadSkillTree();
        }

        private void ResetOffset(UIMouseEvent evt, UIElement listeningElement)
        {
            offSet = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f) / Config.vConfig.UI_Scale;
            Zoom = 1;
            Init();
        }

        public void Init()
        {
            //Erase all previous value
            Erase();
            sizeMultplier = Zoom * UIScale * ScreenMult;
            skillTree.ResetConnection();
            allConnection = new List<Connection>();
            allBasePanel = new List<SkillPanel>();
            allText = new List<SkillText>();


            

            backGround = new UIPanel();
            backGround.SetPadding(0);
            backGround.Left.Set(0, 0f);
            backGround.Top.Set(0, 0f);
            backGround.Width.Set(Main.screenWidth, 0f);
            backGround.Height.Set(Main.screenHeight, 0f);
            backGround.BackgroundColor = new Color(73, 94, 171,150);
            Append(backGround);

            RPGPlayer rPGPlayer = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();

            skillPointsLeft = new UIText("Skill Points : " + rPGPlayer.GetSkillPoints + " / " + (rPGPlayer.GetLevel()-1));
            skillPointsLeft.Left.Set(150, 0f);
            skillPointsLeft.Top.Set(150, 0f);
            backGround.Append(skillPointsLeft);

            ResetText = new UIText("RESET", 1* ScreenMult, true)
            {
                TextColor = Color.Gray
            };
            ResetText.Left.Set(150* ScreenMult, 0f);
            ResetText.Top.Set(250* ScreenMult, 0f);
            ResetText.Width.Set(0, 0f);
            ResetText.Height.Set(0, 0f);
            ResetText.OnLeftClick += new MouseEvent(ResetStats);
            ResetText.OnMouseOver += new MouseEvent(ResetTextHover);
            ResetText.OnMouseOut += new MouseEvent(ResetTextOut);
            backGround.Append(ResetText);

            backGround.OnLeftMouseDown += new UIElement.MouseEvent(DragStart);
            backGround.OnLeftMouseUp += new UIElement.MouseEvent(DragEnd);
            backGround.OnScrollWheel += new ScrollWheelEvent(ScrollUpDown);
            backGround.OnMiddleClick += new MouseEvent(ResetOffset);


            Instance = this;
            for (int i = 0; i < skillTree.nodeList.nodeList.Count; i++)
            {
                NodeParent Node = skillTree.nodeList.nodeList[i];
                if (!(Node.GetNodeType == NodeType.LimitBreak && rPGPlayer.GetLevel() < 1000 || Node.GetNode.GetAscended && !skillTree.IsLimitBreak()))
                {
                    SkillInit(skillTree.nodeList.nodeList[i]);
                }
                 
                
            }


            
            //

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
            
            RPGPlayer rPGPlayer = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            skillPointsLeft.SetText("Skill Points : " + rPGPlayer.GetSkillPoints + " / " + (rPGPlayer.GetLevel() - 1));

            Recalculate();
        }

        public void SkillInit(NodeParent node)
        {
            

            DrawSkill(node);
            RPGPlayer rPGPlayer = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            NodeParent neightboor;
            for (int j = 0; j < node.GetNeightboor.Count; j++)
            {
                neightboor = node.GetNeightboor[j];
                if (node.GetNeightboor.Exists(x => x.ID == neightboor.ID) && !(node.connectedNeighboor.Exists(x => x.ID == neightboor.ID)) )
                {

                    if (!(neightboor.GetNodeType == NodeType.LimitBreak && rPGPlayer.GetLevel() < 1000 || neightboor.GetNode.GetAscended && !skillTree.IsLimitBreak()))
                    {
                        DrawConnection((neightboor.GetActivate || node.GetActivate) ? Color.GreenYellow : Color.Gray, node.menuPos, neightboor.menuPos, node, neightboor);
                        node.connectedNeighboor.Add(neightboor);
                        neightboor.connectedNeighboor.Add(node);
                    }

                    
                }
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

        public void DrawSkill(NodeParent node) //Vector2 pos, Texture2D tex,int state)
        {



            SkillPanel basePanel = new SkillPanel(ModContent.Request<Texture2D>("AnotherRpgMod/Textures/UI/skill_blank", AssetRequestMode.ImmediateLoad).Value);
            basePanel.SetPadding(0);
            basePanel.Width.Set(SKILL_SIZE * sizeMultplier, 0f);
            basePanel.Height.Set(SKILL_SIZE * sizeMultplier, 0f);
            Skill skillIcon = new Skill(ModContent.Request<Texture2D>(SkillTextures.GetTexture(node.GetNode), AssetRequestMode.ImmediateLoad).Value);

            skillIcon.Width.Set(SKILL_SIZE * sizeMultplier, 0f);
            skillIcon.Height.Set(SKILL_SIZE * sizeMultplier, 0f);

            int state = 0; // disactivated and locked
            if (node.GetEnable) //if the node is enabled
                state = 3;
            else if (node.GetActivate && (node.GetNodeType == NodeType.Class || node.GetNodeType == NodeType.Perk))
                state = 2; // if node is unlocked , activated but not enable (like perk or class skill)
            else if (node.GetUnlock)
                state = 1; // if node is just unlocked

            switch (state) { 
                case 0:
                    skillIcon.color = new Color(255,150,150,255);
                    basePanel.color = new Color(255, 50, 50, 255);
                    break;
                case 1:
                    skillIcon.color = new Color(220, 220, 120, 255);
                    basePanel.color = new Color(160, 180, 50, 255);
                    break;
                case 2:
                    skillIcon.color = new Color(220, 220, 220, 255);
                    basePanel.color = new Color(120, 120, 120, 255);
                    break;
                default:
                    skillIcon.color = Color.White;
                    basePanel.color = new Color(180, 220, 255, 255);
                    break;
            }
            
            basePanel.basePos = new Vector2(node.menuPos.X, node.menuPos.Y);
            basePanel.node = node;
            basePanel.skill = skillIcon;
            SkillText levelText = new SkillText(node.GetLevel + " / " + node.GetMaxLevel,node,sizeMultplier,false);
            levelText.Left.Set(SKILL_SIZE*0.2f* sizeMultplier, 0);
            levelText.Top.Set(SKILL_SIZE*0.5f* sizeMultplier, 0);
            allText.Add(levelText);
            allBasePanel.Add(basePanel);
            skillIcon.OnMouseOver += new MouseEvent((UIMouseEvent, UIElement) => OpenToolTip(UIMouseEvent, UIElement,node));
            skillIcon.OnLeftClick += new MouseEvent((UIMouseEvent, UIElement) => OnClickNode(UIMouseEvent, UIElement, node));
            skillIcon.OnRightClick += new MouseEvent((UIMouseEvent, UIElement) => OnRightClickNode(UIMouseEvent, UIElement, node));
            skillIcon.OnMouseOut += new MouseEvent(CloseToolTip);
            basePanel.Append(skillIcon);
            basePanel.Append(levelText);
        }

        public void DrawConnection(Color _color, Vector2 point1, Vector2 point2,NodeParent node,NodeParent neighboor)
        {
            float angle = 0;
            float distance = 0;
            //angle = (float)(Math.Atan2(point2.Y- point1.Y, point2.X - point1.X) *(180f/Math.PI));
            angle = (float)(Math.Atan2(point2.Y - point1.Y, point2.X - point1.X) );
            distance = (float)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
            Connection BG = new Connection(angle, distance, 10)
            {
                color = Color.DarkSlateGray
                
            };
            
            Connection connection = new Connection(angle, distance, 6)
            {
                color = _color,
            };
            BG.basePos = new Vector2(point1.X + SKILL_SIZE * 0.5f, point1.Y + SKILL_SIZE * 0.5f);
            connection.basePos = new Vector2(point1.X+ SKILL_SIZE*0.5f, point1.Y+ SKILL_SIZE * 0.5f);
            BG.bg = true;
            BG.neighboor = neighboor;
            BG.node = node;
            connection.neighboor = neighboor;
            connection.node = node;
            allConnection.Add(BG);
            allConnection.Add(connection);

        }


        private void ScrollUpDown(UIScrollWheelEvent evt, UIElement listeningElement)
        {
            Vector2 Center = new Vector2(Main.screenWidth * .5f, Main.screenHeight * .5f);
            float preZoom = Zoom;
            Vector2 mouseoffset = evt.MousePosition - Center;
            
            if (evt.ScrollWheelValue > 0)
            {
                Zoom = Mathf.Clamp(1.1f * Zoom, zoomMin, zoomMax);
                
            }
            else if (evt.ScrollWheelValue < 0)
            {
                Zoom = Mathf.Clamp(0.85f * Zoom, zoomMin, zoomMax);
                
            }
            float ratio = Zoom / preZoom;
            offSet /= ratio;
            if (ratio != 1) { 
                if (evt.ScrollWheelValue > 0)
                    offSet -= mouseoffset * Zoom * 0.1f;
               else
                    offSet += mouseoffset * Zoom * 0.02f;
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

        private void UpdateToolTip(NodeParent node)
        {
            toolTip.Remove();
            toolTip = new UIPanel();

            float unzoomMult = ScreenMult;

            toolTip.Left.Set((node.menuPos.X + SKILL_SIZE * 2 + offSet.X)* sizeMultplier, 0);
            toolTip.Top.Set((node.menuPos.Y - SKILL_SIZE * 2 + offSet.Y)* sizeMultplier, 0);

            float TTWidth = 500;
            if (node.GetNodeType == NodeType.Class)
            {
                ClassType CT = (node.GetNode as ClassNode).GetClassType;
                JsonChrClass ClassInfo = JsonCharacterClass.GetJsonCharList.GetClass(CT);
                if (ClassInfo.ManaShield > 0)
                    TTWidth = 750;
            }
            toolTip.Width.Set(TTWidth * unzoomMult, 0);
            toolTip.Height.Set(350 * unzoomMult, 0);
            toolTip.SetPadding(0);
            toolTip.BackgroundColor = new Color(73, 94, 171, 150);

            UIText Name = new UIText("", 0.5f* unzoomMult, true);
            Name.Left.Set(10* unzoomMult, 0);
            Name.Top.Set(10* unzoomMult, 0);
            switch (node.GetNodeType)
            {
                case NodeType.Class:
                    Name.SetText("Class : " + (node.GetNode as ClassNode).GetClassType, unzoomMult, false);
                    break;
                case NodeType.Perk:
                    Name.SetText("Perk : " + (node.GetNode as PerkNode).GetPerk + "\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel, unzoomMult, false);
                    break;
                case NodeType.Immunity:
                    Name.SetText("Immunity : " + (node.GetNode as ImmunityNode).GetImmunity, unzoomMult, false);
                    break;
                case NodeType.Damage:
                    Name.SetText("Bonus " + (node.GetNode as DamageNode).GetDamageType + " Damage\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel, unzoomMult, false);
                    break;
                case NodeType.Leech:
                    Name.SetText("Bonus " + (node.GetNode as LeechNode).GetLeechType + " Leech\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel, unzoomMult, false);
                    break;
                case NodeType.Speed:
                    Name.SetText("Bonus " + (node.GetNode as SpeedNode).GetDamageType + " Speed\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel, unzoomMult, false);
                    break;
                case NodeType.LimitBreak:
                    Name.SetText("LIMIT BREAK : " + (node.GetNode as LimitBreakNode).LimitBreakType, unzoomMult, false);
                    break;
            }

            UIText info = new UIText("Level Required : " + node.GetLevelRequirement + "\nCost " + node.GetCostPerLevel + " Points Per Level\n");
            info.Left.Set(50* unzoomMult, 0);
            info.Top.Set(80* unzoomMult, 0);

            UIText description = new UIText(SkillInfo.GetDesc(node.GetNode));
            description.Left.Set(50* unzoomMult, 0);
            description.Top.Set(140* unzoomMult, 0);

            SoundEngine.PlaySound(SoundID.MenuTick);

            backGround.Append(toolTip);
            toolTip.Append(Name);
            toolTip.Append(info);
            toolTip.Append(description);


            backGround.Recalculate();
            Recalculate();
        }

        private void OnRightClickNode(UIMouseEvent evt, UIElement listeningElement, NodeParent node)
        {
            if (node.GetNodeType == NodeType.Perk)
            {
                node.GetNode.ToggleEnable();
                if (node.GetEnable == false)
                    SoundEngine.PlaySound(SoundID.MenuClose);
                else
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                UpdateValue();
            }
            else for (int i = 0; i < 5; i++)
            {
                OnClickNode(evt, listeningElement, node);
            }
        }

        private void OnClickNode(UIMouseEvent evt, UIElement listeningElement, NodeParent node)
        {
            if (node.GetActivate)
            {
                if (node.GetNodeType == NodeType.Class)
                {
                    node.ToggleEnable();
                    
                    UpdateValue();
                    if (node.GetEnable == false)
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    else
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                    return;
                }
            }

            RPGPlayer rPGPlayer = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            switch(node.CanUpgrade(rPGPlayer.GetSkillPoints, rPGPlayer.GetLevel()))
            {
                case Reason.CanUpgrade:
                    rPGPlayer.SpentSkillPoints( node.GetCostPerLevel);
                    node.Upgrade();
                    UpdateToolTip(node);
                    UpdateValue();
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                    break;
                default:
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    break;
            }
            
        }

        private void OpenToolTip(UIMouseEvent evt, UIElement listeningElement,NodeParent node)
        {
            if (node == null)
            {
                return;
            }
            toolTip = new UIPanel();
            float unzoomMult = ScreenMult;

            toolTip.Left.Set((node.menuPos.X + SKILL_SIZE*2 + offSet.X)* sizeMultplier, 0);
            toolTip.Top.Set((node.menuPos.Y - SKILL_SIZE * 2 + offSet.Y)* sizeMultplier, 0);

            float TTWidth = 500;
            float TTHeight = 350;
            if (node.GetNodeType == NodeType.Class)
            {
                TTHeight = 400;
                ClassType CT = (node.GetNode as ClassNode).GetClassType;
                JsonChrClass ClassInfo = JsonCharacterClass.GetJsonCharList.GetClass(CT);
                if (ClassInfo.ManaShield > 0)
                    TTWidth = 750;
                if (node.GetNode.GetAscended)
                    TTHeight = 500;
            }
            toolTip.Width.Set(TTWidth * unzoomMult, 0);
            toolTip.Height.Set(TTHeight * unzoomMult, 0);
            toolTip.SetPadding(0);
            toolTip.BackgroundColor = new Color(73, 94, 171, 150);

            UIText Name = new UIText("",0.5f,true);
            Name.Left.Set(10, 0);
            Name.Top.Set(10, 0);
            switch (node.GetNodeType) {
                case NodeType.Class:
                    Name.SetText("Class : "+(node.GetNode as ClassNode).GetClassName);
                    break;
                case NodeType.Perk:
                    Name.SetText("Perk : " + (node.GetNode as PerkNode).GetPerk + " \nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);
                    break;
                case NodeType.Immunity:
                    Name.SetText("Class : " + (node.GetNode as ImmunityNode).GetImmunity);
                    break;
                case NodeType.Damage:
                    Name.SetText("Bonus "+(node.GetNode as DamageNode).GetDamageType + " Damage\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);
                    break;
                case NodeType.Leech:
                    Name.SetText("Bonus " + (node.GetNode as LeechNode).GetLeechType + " Leech\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);
                    break;
                case NodeType.Speed:
                    Name.SetText("Bonus " + (node.GetNode as SpeedNode).GetDamageType + " Speed\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);
                    break;
                case NodeType.Stats:
                    Name.SetText("Bonus " + (node.GetNode as StatNode).GetStatType + " Stats\nLevel : " + node.GetLevel + " / " + node.GetMaxLevel);
                    break;
                case NodeType.LimitBreak:
                    Name.SetText("LIMIT BREAK : " + (node.GetNode as LimitBreakNode).LimitBreakType, unzoomMult, false);
                    break;
            }

            UIText info = new UIText("Level Required : " + node.GetLevelRequirement +"\nCost " + node.GetCostPerLevel + " Points Per Level\n");
            info.Left.Set(50* unzoomMult, 0);
            info.Top.Set(100* unzoomMult, 0);

            UIText description = new UIText(SkillInfo.GetDesc(node.GetNode));
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
