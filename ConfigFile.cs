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

    public class Config
    {
        public bool NpcProgress = true;
        public bool XPReduction = true;
        public float XpMultiplier = 1;
        public float NpclevelMultiplier = 1;
    }


    class ConfigFile
    {


        public static string configName = "AnRPG_Settings.json";
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
                configPath =(Main.SavePath + Path.DirectorySeparatorChar + configName);
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
                Directory.CreateDirectory(Main.SavePath);

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
                Directory.CreateDirectory(Main.SavePath);
                File.WriteAllText(configPath, JsonConvert.SerializeObject(serverConfig, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }


    }
}
