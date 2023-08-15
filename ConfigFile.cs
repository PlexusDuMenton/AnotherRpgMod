using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Terraria;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.UI;
using AnotherRpgMod.UI;
using Terraria.ID;


namespace AnotherRpgMod
{
    [Flags]
    public enum GamePlayFlag
    {
        NONE = 0x0,
        NPCPROGRESS = 0x1,
        XPREDUTION = 0x2,
        NPCRARITY = 0x4,
        NPCMODIFIER = 0x8,
        BOSSMODIFIER = 0x10,
        BOSSCLUSTERED = 0x20,
        RPGPlayer = 0x40,
        ITEMRARITY = 0x80,
        ITEMMODIFIER = 0x100,
        LIMITNPCGROWTH = 0x200,
        DISPLAYNPCNAME = 0x400,
        BOSSKILLINCREASELEVEL = 0x800
    }

    [Label("AnRPG display config")]
    public class VisualConfig : ModConfig
    {
        // You MUST specify a MultiPlayerSyncMode.
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("HealthBar Offset")]
        [Tooltip("Health Bar Offset on the Y axis")]
        [Range(-500f, 1500f)]
        [Increment(10f)]
        [DefaultValue(100)]
        public float HealthBarYoffSet;

        [Label("Health Bar Scale")]
        [Range(0.1f, 3f)]
        [Increment(.25f)]
        [DefaultValue(0.75f)]
        public float HealthBarScale;

        [Label("Other UI Scale")]
        [Tooltip("Used for the scale of all other UI element from Another RPG Mod")]
        [Range(0.1f, 3f)]
        [Increment(.25f)]
        [DefaultValue(0.75f)]
        public float UI_Scale;

        [Label("Display npc name")]
        [Tooltip("Display NPC information at all time and detailed information when mouse over")]
        [DefaultValue(true)]
        public bool DisplayNpcName;

        [Label("Display Town names")]
        [Tooltip("Display Town Npc information at all time and detailed information when mouse over")]
        [DefaultValue(true)]
        public bool DisplayTownName;

        [Label("Hide Old Bar")]
        [Tooltip("Hide The vanilla game HealthBar")]
        [DefaultValue(true)]
        public bool HideVanillaHB;


        public override void OnLoaded()
        {
            AnotherRpgMod.visualConfig = this;
        }
        public override void OnChanged()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AnotherRpgMod.visualConfig = this;
                if (AnotherRpgMod.Instance!= null) { 
                    AnotherRpgMod.Instance.healthBar.Reset();
                    AnotherRpgMod.Instance.OpenST.Reset();
                    AnotherRpgMod.Instance.openStatMenu.Reset();
                
                }
            }
        }
    }

    [Label("AnRPG gameplay config")]
    public class GamePlayConfig : ModConfig
    {

        // You MUST specify a MultiPlayerSyncMode.
        public override ConfigScope Mode => ConfigScope.ServerSide;

        

        [Label("Xp Level Reduction")]
        [Tooltip("Reduce the exp gained when the entity level is too low, look at Xp Reduction delta to change the value")]
        [DefaultValue(true)]
        public bool XPReduction;

        [Label("RPG Player Module")]
        [Tooltip("Enable all Player related RPG elements")]
        [DefaultValue(true)]
        public bool RPGPlayer;


        [Label("Item Rarity")]
        [Tooltip("Enable item rarity like : Broken, Masterpiece, Legendary...")]
        [DefaultValue(true)]
        public bool ItemRarity;

        [Label("Item Modifier")]
        [Tooltip("Enable Item modifier like : Bonus damage based on distance...")]
        [DefaultValue(true)]
        public bool ItemModifier;

        [Label("Item Tree")]
        [Tooltip("Enable Skill Tree evolution for Item")]
        [DefaultValue(true)]
        public bool ItemTree;

        [Label("Item Ascension Limit")]
        [Tooltip("Cap the maximum ascension based on the world progress")]
        [DefaultValue(true)]
        public bool AscendLimit;

        [Label("Item Ascension Limit Per Boss")]
        [Tooltip("How much the ascend limit Increase Per boss, 0.5 mean one for two each boss killed, '2' mean two for each boss killed")]
        [Range(0.1F, 10F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float AscendLimitPerBoss;

        [Label("Xp Reduction Delta")]
        [Tooltip("Level range at which the xp gain will start to be reduced")]
        [DefaultValue(10)]
        public int XPReductionDelta;

        [Label("Xp Multiplier")]
        [Tooltip("Multiply all xp gain for Player by this value")]
        [Range(0.1F, 50F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float XpMultiplier;

        [Label("Item Xp Multiplier")]
        [Tooltip("Multiply all xp gain for weapon by this value")]
        [Range(0.1F, 50F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float ItemXpMultiplier;

        [Label("Life Leech Cooldown")]
        [Tooltip("CoolDown between Each LifeLeech procc")]
        [Range(0.01f, 5F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float LifeLeechCD;

        [Label("Vanity give stat")]
        [Tooltip("Allow vanity object to give stat")]
        [DefaultValue(false)]
        public bool VanityGiveStat;

        [Label("Use Custom SkillTree")]
        [Tooltip("When true, it'll use the JsonSkillTree in \"Documents\\My Games\\Terraria\\ModLoader\\Mod Configs\\AnRPG\" ")]
        [DefaultValue(false)]
        public bool UseCustomSkillTree;



       

        public override void OnLoaded()
        {
            AnotherRpgMod.gpConfig = this;
        }
        public override void OnChanged()
        {
            AnotherRpgMod.gpConfig = this;
        }
    }
    [Label("AnRPG Enemies config")]
    public class NPCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Npc Progression")]
        [Tooltip("Npc level enable/Disable")]
        [DefaultValue(true)]
        public bool NPCProgress;

        [Label("NPC Rank")]
        [Tooltip("Enable NPC rank, like : Alpha, Legendary ect ...")]
        [DefaultValue(true)]
        public bool NPCRarity;

        [Label("NPC Modifier")]
        [Tooltip("Enable NPC modifier, like : The golden, Clustered ect ...")]
        [DefaultValue(true)]
        public bool NPCModifier;

        [Label("Boss Rarity")]
        [Tooltip("Apply Rarity to boss")]
        [DefaultValue(true)]
        public bool BossRarity;

        [Label("Boss Modifier")]
        [Tooltip("Apply modifier to boss")]
        [DefaultValue(true)]
        public bool BossModifier;

        [Label("Boss Clustered")]
        [Tooltip("Enable Clustered modifier on boss, it's disable since most people don't want an army of boss spawning after killing one")]
        [DefaultValue(false)]
        public bool BossClustered;

        [Label("Limit NPC growth")]
        [Tooltip("If activated cap max npc level arround the Player level based on Limit NPC growth Value")]
        [DefaultValue(true)]
        public bool LimitNPCGrowth;

        
        [Label("Limit NPC growth Value")]
        [Tooltip("If Limit Npc Growth is actiaved, limit npc level by your level + this value + level X Growth Percent")]
        [Range(0, int.MaxValue)]
        [Increment(10)]
        [DefaultValue(20)]
        public int LimitNPCGrowthValue;

        
        [Label("Limit NPC growth Percent")]
        [Tooltip("If Limit Npc Growth is actiaved, limit npc level by your level + Growth Value + level X Growth Percent ")]
        [Range(0f, 200f)]
        [Increment(5f)]
        [DefaultValue(20f)]
        public float LimitNPCGrowthPercent;

        [Label("Npc Level Multiplier")]
        [Tooltip("Multiply all npc level by this value")]
        [Range(0.1F, 50F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float NpclevelMultiplier;

        [Label("Npc Projectile Level")]
        [Tooltip("Used as a workd-arround to scale NPC projectile, tweak this value if needed")]
        [Range(1, 2500)]
        [Increment(10)]
        [DefaultValue(10)]
        public int NPCProjectileDamageLevel;

        [Label("Npc Damage Multiplier")]
        [Tooltip("Multiply all npc Damage by this value")]
        [Range(0.1F, 50F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float NpcDamageMultiplier;

        [Label("Npc Health Multiplier")]
        [Tooltip("Multiply all npc Health by this value")]
        [Range(0.1F, 50F)]
        [Increment(.25f)]
        [DefaultValue(1f)]
        public float NpcHealthMultiplier;

        [Label("Boss Health multiplier")]
        [Tooltip("Multiplier to boss health")]
        [Range(0.1f, 10f)]
        [Increment(.1f)]
        [DefaultValue(1f)]
        public float BossHealthMultiplier;


        [Label("Each Boss Kill NPC growth")]
        [Tooltip("When True, Each boss kill will increase the global level (instead of just the first kill) ")]
        [DefaultValue(false)]
        public bool BossKillLevelIncrease;

        [Label("NPC growth Per Boss")]
        [Tooltip("How many level the world gain when killing a boss")]
        [Range(1, int.MaxValue)]
        [Increment(5)]
        [DefaultValue(15)]
        public int NPCGrowthValue;

        [Label("NPC growth OnHardMode")]
        [Tooltip("How many level the world gain when entering hardmode")]
        [Range(1, int.MaxValue)]
        [Increment(5)]
        [DefaultValue(50)]
        public int NPCGrowthHardMode;

        [Label("NPC growth Hard Mode Percent")]
        [Tooltip("Multiply the world level by this value (applied before \"NPC growth OnHardMode\")")]
        [Range(1f, 10f)]
        [Increment(0.1f)]
        [DefaultValue(1.1f)]
        public float NPCGrowthHardModePercent;

        


        public override void OnLoaded()
        {
            AnotherRpgMod.NPCConfig = this;
        }
        public override void OnChanged()
        {
            AnotherRpgMod.NPCConfig = this;

            JsonSkillTree.Load();
            JsonCharacterClass.Load();
        }
    }


    public class Config
    {
        static public VisualConfig vConfig
        {
            get { return AnotherRpgMod.visualConfig; }
        }
        static public GamePlayConfig gpConfig
        {
            get { return AnotherRpgMod.gpConfig; }
        }
        static public NPCConfig NPCConfig
        {
            get { return AnotherRpgMod.NPCConfig; }
        }

    }
}
