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

    public class VisualConfig
    {
        public float HealthBarYoffSet = 0;
        public float HealthBarScale = 0.75f;
        public float UI_Scale = 0.75f;
        public bool DisplayNpcName = true;
    }

    static public class GPConfigExtension
    {
        static public GamePlayFlag ToGPFlag(this GamePlayConfig config)
        {
            GamePlayFlag GpFlag = GamePlayFlag.NONE;

            if (config.NPCProgress)
                GpFlag = GpFlag | GamePlayFlag.NPCPROGRESS;
            if (config.XPReduction)
                GpFlag = GpFlag | GamePlayFlag.XPREDUTION;
            if (config.NPCRarity)
                GpFlag = GpFlag | GamePlayFlag.NPCRARITY;
            if (config.NPCModifier)
                GpFlag = GpFlag | GamePlayFlag.NPCMODIFIER;
            if (config.BossModifier)
                GpFlag = GpFlag | GamePlayFlag.BOSSMODIFIER;
            if (config.BossClustered)
                GpFlag = GpFlag | GamePlayFlag.BOSSCLUSTERED;
            if (config.RPGPlayer)
                GpFlag = GpFlag | GamePlayFlag.RPGPLAYER;
            if (config.ItemModifier)
                GpFlag = GpFlag | GamePlayFlag.ITEMMODIFIER;
            if (config.ItemRarity)
                GpFlag = GpFlag | GamePlayFlag.ITEMRARITY;
            if (config.LimitNPCGrowth)
                GpFlag = GpFlag | GamePlayFlag.LIMITNPCGROWTH;

            return GpFlag;
        }

        static private bool HaveFlag(GamePlayFlag test, GamePlayFlag tocheck)
        {
            return ((tocheck & test) == tocheck);
        }
        static public GamePlayConfig FromGPFlag(this GamePlayConfig baseConfig, GamePlayFlag GpFlag)
        {

            baseConfig.NPCProgress = HaveFlag(GamePlayFlag.NPCPROGRESS, GpFlag);
            baseConfig.XPReduction = HaveFlag(GamePlayFlag.XPREDUTION, GpFlag);
            baseConfig.NPCRarity = HaveFlag(GamePlayFlag.NPCRARITY, GpFlag);
            baseConfig.NPCModifier = HaveFlag(GamePlayFlag.NPCMODIFIER, GpFlag);
            baseConfig.BossModifier = HaveFlag(GamePlayFlag.BOSSMODIFIER, GpFlag);
            baseConfig.BossClustered = HaveFlag(GamePlayFlag.BOSSCLUSTERED, GpFlag);
            baseConfig.RPGPlayer = HaveFlag(GamePlayFlag.RPGPLAYER, GpFlag);
            baseConfig.ItemModifier = HaveFlag(GamePlayFlag.ITEMMODIFIER, GpFlag);
            baseConfig.ItemRarity = HaveFlag(GamePlayFlag.ITEMRARITY, GpFlag);
            baseConfig.LimitNPCGrowth = HaveFlag(GamePlayFlag.LIMITNPCGROWTH, GpFlag);

            return baseConfig;
        }
    }

    public class GamePlayConfig
    {

        public bool NPCProgress = true;
        public bool XPReduction = true;
        public bool NPCRarity = true;
        public bool NPCModifier = true;
        public bool BossModifier = true;
        public bool BossClustered = false;
        public bool RPGPlayer = true;
        public bool ItemRarity = true;
        public bool ItemModifier = true;
        public bool LimitNPCGrowth = true;
        

        public int XPReductionDelta = 10;
        public float XpMultiplier = 1;
        public float NpclevelMultiplier = 1;
        public float ItemXpMultiplier = 1;
    }

    public class Config
    {
        public VisualConfig vConfig = new VisualConfig();
        public GamePlayConfig gpConfig = new GamePlayConfig();
    }


    public class ConfigFile
    {

        public static string configName = "Settings.json";
        public static string dir = "Mod Configs" + Path.DirectorySeparatorChar + "AnRPG";
        public static string configPath;

        static Config serverConfig = new Config();

        public static Config GetConfig
        {
            get { return serverConfig; }
        }

        public static void Init()
        {
            try
            {
                configPath =(Main.SavePath + Path.DirectorySeparatorChar + dir + Path.DirectorySeparatorChar + configName);
                serverConfig = new Config();
                Load();
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + dir);

                serverConfig = new Config();
                if (File.Exists(configPath))
                {
                    using (StreamReader reader = new StreamReader(configPath))
                    {
                        serverConfig = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                    }
                }
                Save();

            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        
        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + dir);
                File.WriteAllText(configPath, JsonConvert.SerializeObject(serverConfig, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }


    }
}
