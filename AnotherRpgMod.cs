using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Graphics;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using AnotherRpgMod.UI;
using MonoMod.Cil;
using AnotherRpgMod.Utils;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.Audio;


using AnotherRpgMod.Items;
using Terraria.GameContent;

namespace AnotherRpgMod
{
    public enum DamageType : byte
    {
        Melee,
        Ranged,
        Throw,
        Magic,
        Summon,
        Symphonic, //thorium
        Radiant, //thorium
        KI,

    }

    public enum Message : byte {
        AddXP,
        SyncLevel,
        SyncPlayerHealth,
        SyncNPCSpawn,
        SyncNPCUpdate,
        SyncWeapon,
        AskNpc,
        Log,
        syncWorld,
    };

    

    enum SupportedMod
    {
        Thorium,
        Calamity,
        DBZMOD
    }

    class AnotherRpgMod : Mod
	{
        public static AnotherRpgMod Instance;
        public UserInterface customResources;
        public HealthBar healthBar;

        public UserInterface customNPCInfo;
        public UserInterface customNPCName;
        public UserInterface customstats;
        public UserInterface customOpenstats;
        public UserInterface customOpenST;
        public UserInterface customItemTree;

        public UserInterface customSkillTree;

        public OpenStatsButton openStatMenu;
        public OpenSTButton OpenST;
        public Stats statMenu;
        
        public SkillTreeUi skillTreeUI;
        public ItemTreeUi ItemTreeUI;

        public ReworkMouseOver NPCInfo;
        public NPCNameUI NPCName;

        public static ModKeybind StatsHotKey;
        public static ModKeybind SkillTreeHotKey;
        public static ModKeybind ItemTreeHotKey;

        public static ModKeybind HelmetItemTreeHotKey;
        public static ModKeybind ChestItemTreeHotKey;
        public static ModKeybind LegsItemTreeHotKey;


        internal static GamePlayConfig gpConfig;
        internal static NPCConfig NPCConfig;
        internal static VisualConfig visualConfig;


        public static ItemUpdate source;
        public static ItemUpdate Transfer;
        public static float XPTvalueA;
        public static float XPTvalueB;

        public static Vector2 zoomValue = new Vector2(1,1);

        public float lastUpdateScreenScale = Main.screenHeight;

        public static Dictionary<SupportedMod, bool> LoadedMods = new Dictionary<SupportedMod, bool>()
        {
            {SupportedMod.Thorium,false },
            {SupportedMod.Calamity,false },
            {SupportedMod.DBZMOD,false },
            //{SupportedMod.Spirit,false }

        };

    
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {

            MPPacketHandler.HandlePacket(reader, whoAmI);
        }
        


        private void Player_Update(ILContext il)
        {
            try
            {
                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.Before,
                                                    i => i.MatchLdfld("Terraria.Player", "statManaMax2"),
                                                    i => i.MatchLdcI4(400)))
                {
                    Logger.Error("Can't find this damn mana instruction D:");
                    return;
                }

                cursor.Next.Next.Operand = 100000;
            }
            catch
            {
                Logger.Error("another mod is editiong TerrariaPlayer StatmanaMax 2, can't edit mana cap");
            }
        }


        public override void Unload()
        {
            base.Unload();

            Terraria.IL_Player.Update -= Player_Update;
            JsonSkillTree.Unload();
            JsonCharacterClass.Unload();

            StatsHotKey = null;
            SkillTreeHotKey = null;
            ItemTreeHotKey = null;

            HelmetItemTreeHotKey = null;
            ChestItemTreeHotKey = null;
            LegsItemTreeHotKey = null;
            if (!Main.dedServ)
            {
                ItemTreeUi.Instance = null;
                SkillTreeUi.Instance = null;
                Stats.Instance = null;


                visualConfig = null;
                NPCConfig = null;
                gpConfig = null;
                LoadedMods.Clear();
                LoadedMods = null;
                NPCNameUI.Instance = null;
                ReworkMouseOver.Instance = null;

                source = null;
                Transfer = null;



                customNPCInfo = null;
                NPCInfo = null;
                customNPCName = null;
                NPCName = null;
                customResources = null;
                healthBar = null;
                customstats = null;
                statMenu = null;
                customOpenstats = null;
                openStatMenu = null;
                customOpenST = null;
                OpenST = null;
                customSkillTree = null;
                skillTreeUI = null;
                customItemTree = null;
                ItemTreeUI = null;

                
            }


            //Instance.Logger.Info("Another Rpg Mod " + Version + " Correctly Unloaded");

            Instance = null;


            
        }
        public override void Load()
        {
            Terraria.IL_Player.Update += Player_Update;

            Instance = this;
            Instance.Logger.Info("Another Rpg Mod " + Version + " Correctly loaded");
            JsonSkillTree.Init();
            JsonCharacterClass.Init();
            //LoadedMods[SupportedMod.Thorium] = ModLoader.GetMod("ThoriumMod") != null;
            //LoadedMods[SupportedMod.Calamity] = ModLoader.GetMod("CalamityMod") != null;
            //LoadedMods[SupportedMod.DBZMOD] = ModLoader.GetMod("DBZMOD") != null;
            
            StatsHotKey = KeybindLoader.RegisterKeybind(this,"Open Stats Menu", "C");
            SkillTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open SkillTree", "X");
            ItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Item Tree", "V");
            HelmetItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Helmet Item Tree", "NumPad1");
            ChestItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Chest Item Tree", "NumPad2");
            LegsItemTreeHotKey = KeybindLoader.RegisterKeybind(this, "Open Legs Item Tree", "NumPad3");
            if (!Main.dedServ)
            {
                
                customNPCInfo = new UserInterface();
                NPCInfo = new ReworkMouseOver();
                ReworkMouseOver.visible = true;
                customNPCInfo.SetState(NPCInfo);
                
                customNPCName = new UserInterface();
                NPCName = new NPCNameUI();
                NPCNameUI.visible = true;
                customNPCName.SetState(NPCName);

                customResources = new UserInterface();
                healthBar = new HealthBar();
                HealthBar.visible = true;
                customResources.SetState(healthBar);

                customstats = new UserInterface();
                statMenu = new Stats();
                Stats.visible = false;
                customstats.SetState(statMenu);

                customOpenstats = new UserInterface();
                openStatMenu = new OpenStatsButton();
                OpenStatsButton.visible = true;
                customOpenstats.SetState(openStatMenu);

                customOpenST = new UserInterface();
                OpenST = new OpenSTButton();
                OpenSTButton.visible = true;
                customOpenST.SetState(OpenST);

                customSkillTree = new UserInterface();
                skillTreeUI = new SkillTreeUi();
                OpenStatsButton.visible = true;
                customSkillTree.SetState(skillTreeUI);

                customItemTree = new UserInterface();
                ItemTreeUI = new ItemTreeUi();
                ItemTreeUi.visible = false;
                customItemTree.SetState(ItemTreeUI);

                /*
                
                statMenu = new Stats();
                Stats.visible = true;
                customstats.SetState(statMenu);
                */
            }
            
        }

        
        

        


        private void DrawInterface_Resources_ClearBuffs()
        {
            Main.buffString = "";
            Main.bannerMouseOver = false;
            if (!Main.recBigList)
            {
                Main.recStart = 0;
            }
        }
        
        
        public void DrawInterface_Resources_Buffs()
        {
            Main.recBigList = false;
            int BuffID = -1;
            int num1 = 11;
            for (int buffSlot = 0; buffSlot < Player.MaxBuffs; buffSlot++)
            {
                if (Main.player[Main.myPlayer].buffType[buffSlot] <= 0)
                {
                    Main.buffAlpha[buffSlot] = 0.4f;
                }
                else
                {
                    int num2 = Main.player[Main.myPlayer].buffType[buffSlot];
                    int x = 32 + buffSlot * 38;
                    int y = 76;
                    if (buffSlot >= num1)
                    {
                        x = 32 + Math.Abs(buffSlot % 11) * 38;
                        y = y + 50 * (buffSlot / 11);
                    }
                    BuffID = Main.DrawBuffIcon(BuffID, buffSlot, x, y);
                }
            }
            if (BuffID >= 0)
            {
                int num5 = Main.player[Main.myPlayer].buffType[BuffID];
                if (num5 > 0)
                {
                    string buffName = Lang.GetBuffName(num5);
                    string buffTooltip = Main.GetBuffTooltip(Main.player[Main.myPlayer], num5);
                    if (num5 == 147)
                    {
                        Main.bannerMouseOver = true;
                    }
                    int rarity = 0;
                    if (Main.meleeBuff[num5])
                    {
                        rarity = -10;
                    }
                    Main.instance.MouseText(buffName, buffTooltip, rarity,0);
                }
            }
        }
        public static int DrawBuffIcon(int drawBuffText, int buffSlotOnPlayer, int x, int y)
        {
            int num1;
            int buffId = Main.player[Main.myPlayer].buffType[buffSlotOnPlayer];
            if (buffId != 0)
            {
                Color color = new Color(Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer]);
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D value = TextureAssets.Buff[buffId].Value;
                Vector2 position = new Vector2((float)x, (float)y);
                Rectangle? nullable = new Rectangle?(new Rectangle(0, 0, TextureAssets.Buff[buffId].Width(), TextureAssets.Buff[buffId].Height()));
                Vector2 origin = new Vector2();
                spriteBatch.Draw(value, position, nullable, color, 0f, origin, 1f, 0, 0f);
                if (Main.TryGetBuffTime(buffSlotOnPlayer, out var buffTimeValue) && buffTimeValue > 2)
                {
                    string str = Lang.LocalizedDuration(new TimeSpan(0, 0, buffTimeValue / 60), true, false);
                    SpriteBatch buffTimerBatch = Main.spriteBatch;
                    DynamicSpriteFont dynamicSpriteFont = FontAssets.ItemStack.Value;
                    Vector2 timerPosition = new Vector2((float)x, (float)(y + TextureAssets.Buff[buffId].Height()));
                    origin = new Vector2();
                    DynamicSpriteFontExtensionMethods.DrawString(buffTimerBatch, dynamicSpriteFont, str, timerPosition, color, 0f, origin, 0.8f, 0, 0f);
                }
                if ((Main.mouseX >= x + TextureAssets.Buff[buffId].Width() || Main.mouseY >= y + TextureAssets.Buff[buffId].Height() || Main.mouseX <= x || Main.mouseY <= y))
                {
                    Main.buffAlpha[buffSlotOnPlayer] -= 0.05f;
                }
                else
                {
                    drawBuffText = buffSlotOnPlayer;
                    Main.buffAlpha[buffSlotOnPlayer] += 0.1f;
                    bool flag = (Main.mouseRight && Main.mouseRightRelease);
                    if (!PlayerInput.UsingGamepad)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                    }
                    else
                    {
                        flag = (Main.mouseLeft && Main.mouseLeftRelease && Main.playerInventory);
                        if (Main.playerInventory)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                        }
                    }
                    if (flag)
                    {
                        Main.TryRemovingBuff(buffSlotOnPlayer, buffId);
                    }
                }
                if (Main.buffAlpha[buffSlotOnPlayer] > 1f)
                {
                    Main.buffAlpha[buffSlotOnPlayer] = 1f;
                }
                else if ((double)Main.buffAlpha[buffSlotOnPlayer] < 0.4)
                {
                    Main.buffAlpha[buffSlotOnPlayer] = 0.4f;
                }
                if ((PlayerInput.UsingGamepad && !Main.playerInventory))
                {
                    drawBuffText = -1;
                }
                num1 = drawBuffText;
            }
            else
            {
                num1 = drawBuffText;
            }
            return num1;
        }
    }
}
