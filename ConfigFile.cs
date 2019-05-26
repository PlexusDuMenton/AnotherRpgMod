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
        RPGPLAYER = 0x40,
        ITEMRARITY = 0x80,
        ITEMMODIFIER = 0x100,
        LIMITNPCGROWTH = 0x200,
        DISPLAYNPCNAME = 0x400
    }

    [Label("AnRPG display config")]
    public class VisualConfig : ModConfig
    {
        // You MUST specify a MultiplayerSyncMode.
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("HealthBar Offset")]
        [Tooltip("Health Bar Offset on the Y axis")]
        [Range(-500f, 1500f)]
        [Increment(10f)]
        [DefaultValue(100)]
        public float HealthBarYoffSet;

        [Label("Health Bar Scale")]
        [Range(0f, 3f)]
        [Increment(.25f)]
        [DefaultValue(0.75f)]
        public float HealthBarScale;

        [Label("Other UI Scale")]
        [Tooltip("Used for the scale of all other UI element from Another RPG Mod")]
        [Range(0.25f, 3f)]
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


        public override void OnLoaded()
        {
            AnotherRpgMod.visualConfig = this;
        }
        public override void OnChanged()
        {
            AnotherRpgMod.visualConfig = this;
            if (AnotherRpgMod.Instance!= null) { 
                AnotherRpgMod.Instance.healthBar.OnInitialize();
                AnotherRpgMod.Instance.OpenST.OnInitialize();
                AnotherRpgMod.Instance.openStatMenu.OnInitialize();
            }
        }
    }

    [Label("AnRPG gameplay config")]
    public class GamePlayConfig : ModConfig
    {

        // You MUST specify a MultiplayerSyncMode.
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Npc Progression")]
        [Tooltip("Npc level enable/Disable")]
        [DefaultValue(true)]
        public bool NPCProgress;

        [Label("Xp Level Reduction")]
        [Tooltip("Reduce the exp gained when the entity level is too low, look at Xp Reduction delta to change the value")]
        [DefaultValue(true)]
        public bool XPReduction;

        [Label("NPC Rank")]
        [Tooltip("Enable NPC rank, like : Alpha, Legendary ect ...")]
        [DefaultValue(true)]
        public bool NPCRarity;

        [Label("NPC Modifier")]
        [Tooltip("Enable NPC modifier, like : The golden, Clustered ect ...")]
        [DefaultValue(true)]
        public bool NPCModifier;

        [Label("Boss Modifier")]
        [Tooltip("Apply modifier to boss")]
        [DefaultValue(true)]
        public bool BossModifier;

        [Label("Boss Clustered")]
        [Tooltip("Enable Clustered modifier on boss, it's disable since most people don't want an army of boss spawning after killing one")]
        [DefaultValue(false)]
        public bool BossClustered;

        [Label("RPG Player Module")]
        [Tooltip("Enable all player related RPG elements")]
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

        [Label("Limit NPC growth")]
        [Tooltip("Not realy sure if work properly, but prevent npc from level over 100 that the player level (and a bit more) to spawn")]
        [DefaultValue(true)]
        public bool LimitNPCGrowth;

        [Label("Item Tree")]
        [Tooltip("Enable Skill Tree evolution for Item")]
        [DefaultValue(true)]
        public bool ItemTree;

        [Label("Xp Reduction Delta")]
        [Tooltip("Level range at which the xp gain will start to be reduced")]
        [DefaultValue(10)]
        public int XPReductionDelta;

        [Label("Xp Multiplier")]
        [Range(0f, 5f)]
        [Increment(.25f)]
        [DefaultValue(1)]
        public float XpMultiplier;

        [Label("Npc Level Multiplier")]
        [Range(0f, 3f)]
        [Increment(.1f)]
        [DefaultValue(1)]
        public float NpclevelMultiplier;

        [Label("Item Xp Multiplier")]
        [Range(0f, 5f)]
        [Increment(.25f)]
        [DefaultValue(1)]
        public float ItemXpMultiplier;

        public override void OnLoaded()
        {
            AnotherRpgMod.gpConfig = this;
        }
        public override void OnChanged()
        {
            AnotherRpgMod.gpConfig = this;
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

    }
}
