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
            ErrorLogger.Log("Class "+ classType.ToString() +"is missing from class List");
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
                new float[7]{ 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0, 0 },
                0,
                0.05f,0
            ),
            new JsonChrClass(
                "Regular",
                new float[7]{ 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0, 0 },
                0,
                0.2f,0.1f
            ),

            //Specialist - Tier 1
            new JsonChrClass(
                "Archer",
                new float[7]{ 0, 0.2f, 0, 0, 0, 0.25f, 0 },
                0,
                0,0,0,0.03f,0,0,0,0
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
                -0.2f,0
            ),
                new JsonChrClass(
                "Spiritualist",
                new float[7]{ 0, 0, 0, 0, 0.4f, 0, 0 },
                0,
                0,0,1,0,0,0,0
            ),
                new JsonChrClass(
                "Mage",
                new float[7]{ 0, 0, 0, 0.4f, 0, 0, 0 },
                0,
                -0.25f,0,0,-0.2f,0,0,0
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
                0.1f,0,0,0,0.3f,0.02f,2
            ),
                new JsonChrClass(
                "Cavalier",
                new float[7]{ 0.2f, 0, 0, 0, 0, 0, 0 },
                0,
                0.5f,0.2f,-0.05f,0,0,0,0,0,0
            ),
                
            //Specialist - Tier 2
            new JsonChrClass(
                "Hunter",
                new float[7]{ 0, 0.4f, 0, 0, 0, 0.4f, 0 },
                0,
                0,0,0,0.28f,0,0,0,0
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
                -0.35f,0
            ),
                new JsonChrClass(
                "Invoker",
                new float[7]{ 0, 0, 0, 0, 0.75f, 0, 0 },
                0,
                0,0,2,0,0,0,0
            ),
                new JsonChrClass(
                "ArchMage",
                new float[7]{ 0, 0, 0, .75f, 0, 0, 0 },
                0,
                -0.35f,0,0,-0.3f,0,0,0
            ),
                new JsonChrClass(
                "Shinobi",
                new float[7]{ 0, 0, .75f, 0, 0, 0, 0 },
                0,
                0,0,0,0.1f,0.2f,0,0,0
            ),
                new JsonChrClass(
                "Templar",
                new float[7]{ 0.1f, 0, 0, 0.4f, 0, 0, 0 },
                0,
                0.2f,0.1f,0,0,0.42f,0.03f,3
            ),
                new JsonChrClass(
                "Knight",
                new float[7]{ 0.5f, 0, 0, 0, 0, 0, 0 },
                0,
                1f,0.45f,-0.1f,0,0,0,0,0,0
            )
            

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
                ErrorLogger.Log(e.ToString());
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
                ErrorLogger.Log(e.ToString());
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
                ErrorLogger.Log(e.ToString());
            }
        }

    }
}