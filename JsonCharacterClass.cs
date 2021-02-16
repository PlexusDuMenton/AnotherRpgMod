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
using AnotherRpgMod.RPGModule;
namespace AnotherRpgMod
{

    /*DamageType _damageType
     * bool _flat
     * NodeType _type, 
     * bool _unlocked
     * float _value
     * int _levelrequirement 
     * int _maxLevel
     * int _pointsPerLevel
     */
    public class JsonChrClassList
    {
        public JsonChrClass GetClass(ClassType classType)
        {
            ClassType ClassName;
            for (int i = 0;i< jsonList.Length; i++)
            {
                ClassName = (ClassType)Enum.Parse(typeof(ClassType), jsonList[i].Name);
                if (ClassName == classType)
                {
                    return jsonList[i];
                }
            }
            AnotherRpgMod.Instance.Logger.Warn("Class "+ classType.ToString() +"is missing from class List");
            return jsonList[0];
        }

        public JsonChrClass[] jsonList = {
            
            //Jack of all trade
            new JsonChrClass(
                "Tourist",
                new float[7]{ 0.05f, 0.05f, 0.05f, 0.05f, 0.05f , 0 , 0 },
                0,0,0
                
            ),
            new JsonChrClass(
                "Apprentice",
                new float[7]{ 0.125f, 0.125f, 0.125f, 0.125f, 0.125f, 0, 0 },
                0,
                0.05f,0
            ),
            new JsonChrClass(
                "Regular",
                new float[7]{ 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0, 0 },
                0,
                0.15f,0.1f
            ),
            new JsonChrClass(
                "Expert",
                new float[7]{ 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0, 0 },
                0,
                0.25f,0.15f,0,-0.05f,0.1f,0.0001f,0.1f
            ),
            new JsonChrClass(
                "Master",
                new float[7]{ 0.7f, 0.7f, 0.7f, 0.7f, 0.7f, 0, 0 },
                0,
                0.4f,0.2f,1,-0.1f,0.15f,0.00025f,0.25f
            ),
            new JsonChrClass(
                "PerfectBeing",
                new float[7]{ 1, 1, 1, 1, 1, 0, 0 },
                0,
                1f,0.25f,1,-0.25f,0.25f,0.0005f,0.5f
            ),
            //T6 
            new JsonChrClass(
                "Ascended",
                new float[7]{ 4, 4, 4, 4, 4, 0, 0 },
                0,
                1.5f,0.5f,2,-0.75f,0.75f,0.002f,1f
            ),

            //Specialist - Tier 1
            new JsonChrClass(
                "Archer",
                new float[7]{ 0, 0.2f, 0, 0, 0, 0.25f, 0 },
                0,
                -0.15f,0,0,0.03f,0,0,0,0
            ),
                new JsonChrClass(
                "Gunner",
                new float[7]{ 0, 0.2f, 0, 0, 0, 0, 0.25f },
                0,
                0,0,0,0,0.2f,0,0,0
            ),
                new JsonChrClass(
                "SwordMan",
                new float[7]{ 0.6f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.1f,-0.05f,0,0.05f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "Spiritualist",
                new float[7]{ 0, 0, 0, 0, 0.5f, 0, 0 },
                0,
                0,0,1,0,0,0,0
            ),
                new JsonChrClass(
                "Mage",
                new float[7]{ 0, 0, 0, 0.4f, 0, 0, 0 },
                0,
                -0.15f,0,0,-0.2f,0,0,0
            ),
                new JsonChrClass(
                "Ninja",
                new float[7]{ 0, 0, 0.4f, 0, 0, 0, 0 },
                0,
                0,0,0,0.05f,0.1f,0,0,0
            ),
                new JsonChrClass(
                "Acolyte",
                new float[7]{ 0, 0, 0, 0.25f, 0, 0, 0 },
                0,
                0.1f,0,0,0,0.2f,.00025f,0.1f
            ),
                new JsonChrClass(
                "Cavalier",
                new float[7]{ 0.25f, 0, 0, 0, 0, 0, 0 },
                0,
                0.25f,0.1f,0,0,0,0,0,0,0
            ),
                
            //Specialist - Tier 2
            new JsonChrClass(
                "Hunter",
                new float[7]{ 0, 0.4f, 0, 0, 0, 0.4f, 0 },
                0,
                -0.3f,0,0,0.08f,0,0,0,0
            ),
                new JsonChrClass(
                "Gunslinger",
                new float[7]{ 0, 0.35f, 0, 0, 0, 0, 0.45f },
                0,
                0,0,0,0,0.3f,0,0,0
            ),
                new JsonChrClass(
                "Mercenary",
                new float[7]{ 1f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.2f,-0.1f,0,0.1f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "Invoker",
                new float[7]{ 0, 0, 0, 0, 0.85f, 0, 0 },
                0,
                -0.1f,0,1,0,0,0,0
            ),
                new JsonChrClass(
                "ArchMage",
                new float[7]{ 0, 0, 0, .75f, 0, 0, 0 },
                0,
                -0.2f,0,0,-0.3f,0,0,0
            ),
                new JsonChrClass(
                "Shinobi",
                new float[7]{ 0, 0, .75f, 0, 0, 0, 0 },
                0,
                0,0,0,0.1f,0.2f,0,0,0
            ),
                new JsonChrClass(
                "Monk",
                new float[7]{ 0.1f, 0, 0, 0.4f, 0, 0, 0 },
                0,
                0.2f,0.1f,0,0,0.35f,0.0005f,0.2f
            ),
                new JsonChrClass(
                "Knight",
                new float[7]{ 0.5f, 0, 0, 0, 0, 0, 0 },
                0,
                0.4f,0.2f,0,0,0,0,0,0,0
            ),

            //Specialist - Tier 3
            new JsonChrClass(
                "Ranger",
                new float[7]{ 0, 0.6f, 0, 0, 0, 0.6f, 0 },
                0,
                -0.4f,0,0,0.15f,0,0,0,0
            ),
                new JsonChrClass(
                "Spitfire",
                new float[7]{ 0, 0.5f, 0, 0, 0, 0, 0.5f },
                0,
                0,0,0,0,0.45f,0,0,0
            ),
                new JsonChrClass(
                "SwordMaster",
                new float[7]{ 1.5f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.3f,-0.15f,0.1f,0.2f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "Summoner",
                new float[7]{ 0, 0, 0, 0, 1.25f, 0, 0 },
                0,
                -0.15f,0,2,0,0,0,0
            ),
                new JsonChrClass(
                "Arcanist",
                new float[7]{ 0, 0, 0, 1.25f, 0, 0, 0 },
                0,
                -0.35f,0,0,-0.5f,0,0,0
            ),
                new JsonChrClass(
                "Rogue",
                new float[7]{ 0, 0, 1f, 0, 0, 0, 0 },
                0,
                0,0,0,0.25f,0.4f,0,0,0
            ),
                new JsonChrClass(
                "Templar",
                new float[7]{ 0.3f, 0, 0, 0.6f, 0, 0, 0 },
                0,
                0.3f,0.15f,0,0,0.5f,0.00075f,0.3f
            ),
                new JsonChrClass(
                "IronKnight",
                new float[7]{ 0.8f, 0, 0, 0, 0, 0, 0 },
                0,
                0.5f,0.3f,0,0,0,0,0,0,0
            ),

            //Specialist - Tier 4
            new JsonChrClass(
                "Marksman",
                new float[7]{ 0, 0.8f, 0, 0, 0, 0.8f, 0 },
                0,
                -0.5f,0,0,0.25f,0,0,0,0
            ),
                new JsonChrClass(
                "Sniper",
                new float[7]{ 0, 0.7f, 0, 0, 0, 0, 0.7f },
                0,
                -0.1f,0,0,0.1f,0.6f,0,0,0
            ),
                new JsonChrClass(
                "Champion",
                new float[7]{ 2f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.4f,-0.2f,0.15f,0.35f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "SoulBinder",
                new float[7]{ 0, 0, 0, 0, 1.6f, 0, 0 },
                0,
                -0.2f,0,0,0,0,2,-0.10f,0.1f,0.00025f,0.1f
            ),
                new JsonChrClass(
                "Warlock",
                new float[7]{ 0, 0, 0, 1.5f, 0, 0, 0 },
                0,
                -0.45f,0,0,-0.7f,0,0,0
            ),
                new JsonChrClass(
                "Assassin",
                new float[7]{ 0, 0, 1.5f, 0, 0, 0, 0 },
                0,
                0,0,0,0.5f,0.5f,0,0,0
            ),
                new JsonChrClass(
                "Paladin",
                new float[7]{ 0.75f, 0, 0, 0.8f, 0, 0, 0 },
                0,
                0.4f,0.25f,0,0,0.65f,0.001f,1
            ),
                new JsonChrClass(
                "Montain",
                new float[7]{ 1.25f, 0, 0, 0, 0, 0, 0 },
                0,
                0.6f,0.4f,-0.05f,0,0,0,0,0,0
            ),

            //Specialist - Tier 5
            new JsonChrClass(
                "WindWalker",
                new float[7]{ 0, 1.2f, 0, 0, 0, 1.2f, 0 },
                0,
                -0.65f,0,0,0.3f,0.3f,0,0,0
            ),
                new JsonChrClass(
                "Hitman",
                new float[7]{ 0, 1.2f, 0, 0, 0, 0, 1.2f },
                0,
                -0.25f,0,0,0.25f,0.75f,0,0,0
            ),
                new JsonChrClass(
                "SwordSaint",
                new float[7]{ 3f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.5f,-0.25f,0.2f,0.5f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "SoulLord",
                new float[7]{ 0, 0, 0, 0, 2.5f, 0, 0 },
                0,
                -0.3f,0,0.2f,0,0,2,-0.25f,0.2f,0.00035f,0.15f
            ),
                new JsonChrClass(
                "Mystic",
                new float[7]{ 0, 0, 0, 2.5f, 0, 0, 0 },
                0,
                -0.25f,0,0,-0.9f,0.25f,0.0005f,0.25f
            ),
                new JsonChrClass(
                "ShadowDancer",
                new float[7]{ 0, 0, 2.5f, 0, 0, 0, 0 },
                0,
                0,0,0,0.75f,0.75f,0,0,0
            ),
                new JsonChrClass(
                "Deity",
                new float[7]{ 1f, 0, 0, 1f, 0, 0, 0 },
                0,
                0.5f,0.35f,0,0,0.8f,0.002f,0.5f
            ),
                new JsonChrClass(
                "Fortress",
                new float[7]{ 1.75f, 0, 0, 0, 0, 0, 0 },
                0,
                1f,0.5f,-0.05f,0,0,0,0,0,0
            ),

            //Ascended Specialist / T6
            new JsonChrClass(
                "AscendedWindWalker",
                new float[7]{ 0, 4f, 0, 0, 0, 4f, 0 },
                0,
                -0.25f,0,0,0.75f,1f,0,0,0
            ),
                new JsonChrClass(
                "AscendedHitman",
                new float[7]{ 0, 4f, 0, 0, 0, 0, 4f },
                0,
                0.4f,0.4f,0,0.6f,1f,0,0,0
            ),
                new JsonChrClass(
                "AscendedSwordSaint",
                new float[7]{ 19f, 0, 0, 0, 0, 0, 0 },
                0,
                -0.75f,0,0.5f,0.99f,0,0,0,0,0,0
            ),
                new JsonChrClass(
                "AscendedSoulLord",
                new float[7]{ 0, 0, 0, 0, 9f, 0, 0 },
                0,
                1,1f,0.35f,0,0,2,-0.75f,0.75f,0.02f,10
            ),
                new JsonChrClass(
                "AscendedMystic",
                new float[7]{ 0, 0, 0, 9f, 0, 0, 0 },
                0,
                -0.25f,1,0,-1,0,0,0
            ),
                new JsonChrClass(
                "AscendedShadowDancer",
                new float[7]{ 0, 0, 9f, 0, 0, 0, 0 },
                0,
                0,0,0,0.95f,1,0,0,0
            ),
                new JsonChrClass(
                "AscendedDeity",
                new float[7]{ 4f, 0, 0, 4f, 0, 0, 0 },
                0,
                4f,4f,0,0,1,0.5f,100
            ),
                new JsonChrClass(
                "AscendedFortress",
                new float[7]{ 3f, 0, 0, 0, 0, 0, 0 },
                0,
                14f,14f,0,0.1f,0,0,0,0,0
            ),
        };
    }

    public class JsonChrClass
    {
        public string Name;
        public float[] Damage; //Melee,Ranged,Throw,Magic,Summon,Bow,Gun
        public float Speed; //Melee speed
        public float Health;
        public float Armor;
        public float MovementSpeed;
        public float Dodge;
        public float Ammo;
        public int Summons;
        public float ManaCost;
        public float ManaShield;
        public float ManaEfficiency;
        public float ManaBaseEfficiency;


        public JsonChrClass(string Name, float[] Damage, float Speed,
            float Health = 0,float Armor=0, float MovementSpeed = 0, float Dodge = 0, float Ammo = 0,
            int Summons = 0, float ManaCost = 0, 
            float ManaShield = 0,float ManaEfficiency = 0, float ManaBaseEfficiency = 0
        )
        {
            this.Name = Name;
            this.Damage = Damage;
            this.Speed = Speed;
            this.Health = Health;
            this.Armor = Armor;
            this.MovementSpeed = MovementSpeed;
            this.Dodge = Dodge;
            this.Ammo = Ammo;
            this.Summons = Summons;
            this.ManaCost = ManaCost;
            this.ManaShield = ManaShield;
            this.ManaEfficiency = ManaEfficiency;
            this.ManaBaseEfficiency = ManaBaseEfficiency;
        }
        public JsonChrClass(string Name, float[] Damage, float Speed,float Health =0, float Armor =0,
            int Summons = 0, float ManaCost = 0, 
            float ManaShield = 0, float ManaEfficiency = 0, float ManaBaseEfficiency = 0) : this(Name, Damage, Speed, Health, Armor,0, 0, 0, Summons, ManaCost, ManaShield, ManaEfficiency, ManaBaseEfficiency)
        { }
        public JsonChrClass(string Name, float[] Damage, float Speed, float Health = 0, float Armor = 0) : this(Name, Damage, Speed, Health, Armor, 0, 0, 0, 0, 0, 0, 0)
        { }
    }


    class JsonCharacterClass
    {
        static JsonChrClassList jsonCCList;
        static public JsonChrClassList GetJsonCharList { get { return jsonCCList; } }

        public static string Name = "skillClass.json";
        public static string cPath;



        public static void Init()
        {
            try
            {
                cPath = (Main.SavePath + Path.DirectorySeparatorChar + Name);
                jsonCCList = new JsonChrClassList();
                Load();
            }
            catch (SystemException e)
            {
                AnotherRpgMod.Instance.Logger.Error(e.ToString());
            }
        }

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);

                jsonCCList = new JsonChrClassList();
                //disable Load for Now
                /*
                if (File.Exists(cPath))
                {
                    using (StreamReader reader = new StreamReader(cPath))
                    {

                        jsonSkillList = JsonConvert.DeserializeObject<JsonNodeList>(reader.ReadToEnd());
                    }
                }
                */
                Save();

            }
            catch (SystemException e)
            {
                AnotherRpgMod.Instance.Logger.Error(e.ToString());
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);
                File.WriteAllText(cPath, JsonConvert.SerializeObject(jsonCCList, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                AnotherRpgMod.Instance.Logger.Error(e.ToString());
            }
        }

    }
}