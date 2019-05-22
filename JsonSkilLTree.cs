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
    
    /*DamageType _damageType
     * bool _flat
     * NodeType _type, 
     * bool _unlocked
     * float _value
     * int _levelrequirement 
     * int _maxLevel
     * int _pointsPerLevel
     */
     public class JsonNodeList
    {
        public JsonNode[] jsonList = {
            // BASE TYPE , POSX, POSY, NEIGHTBOORLIST, SPECIFIC TYPE, ISFLATDAMAGE?, VALUEPERLEVEL, LEVEL REQUIREMENT, MAXLEVEL , POINTS PER LEVEL
            new JsonNode("Class",0,0,true,new int[6]{ 1,2,3,4,5,6},"Tourist",false,0.1f,1,1,1), //0
            new JsonNode("Class",0,250,false,new int[1]{0},"Apprentice",false,0.1f,10,1,1), //1
            new JsonNode("Damage",-100,75,false,new int[4]{0,3,7,8},"Melee",false,0.01f,1,20,1), //2
            new JsonNode("Damage",100,75,false,new int[5]{ 0, 2,4,9,10},"Magic",false,0.01f,1,20,1), //3
            new JsonNode("Damage",100,-50,false,new int[4]{ 0, 3,5,11},"Summon",false,0.01f,1,20,1), //4
            new JsonNode("Damage",0,-111,false,new int[5]{ 0, 5,6,12,13},"Ranged",false,0.01f,1,20,1), //5
            new JsonNode("Damage",-100,-50,false,new int[4]{0,5,2,14},"Throw",false,0.01f,1,20,1), //6

            new JsonNode("Class",-300,100,false,new int[2]{2,8},"Cavalier",false,0.03f,10,1,1), //7
            new JsonNode("Class",-200,200,false,new int[2]{2,7},"SwordMan",false,0.03f,10,1,1), //8
            new JsonNode("Class",300,100,false,new int[2]{3,10},"Mage",false,0.03f,10,1,1), //9
            new JsonNode("Class",200,300,false,new int[2]{3,9},"Acolyte",false,0.03f,10,1,1), //10
            new JsonNode("Class",200,-200,false,new int[1]{4},"Spiritualist",false,0.03f,10,1,1), //11
            new JsonNode("Class",-75,-250,false,new int[2]{5,13},"Archer",false,0.03f,10,1,1), //12
            new JsonNode("Class",75,-250,false,new int[2]{5,12},"Gunner",false,0.03f,10,1,1), //13
            new JsonNode("Class",-200,-150,false,new int[1]{6},"Ninja",false,0.03f,10,1,1), //14

            new JsonNode("Leech",0,350,false,new int[3]{1,23,25},"Life",false,0.025f,25,5,5), //15

            new JsonNode("Class",0,450,false,new int[1]{15},"Regular",false,0.1f,50,1,1), //16

            new JsonNode("Stats",-300,300,false,new int[3]{7,8,10},"Str",false,2,20,15,2), //17

            new JsonNode("Stats",350,250,false,new int[2]{9,10},"Int",false,2,25,15,2), //18
            new JsonNode("Stats",300,-50,false,new int[2]{9,11},"Spr",false,2,25,15,2), //19
            new JsonNode("Stats",0,-325,false,new int[2]{12,13},"Agi",false,2,25,15,2), //20
            new JsonNode("Stats",-200,-250,false,new int[2]{12,14},"Dex",false,2,25,15,2), //21
            
            new JsonNode("Class",-400,200,false,new int[2]{7,17},"Knight",false,0.05f,50,1,5), //22
            new JsonNode("Class",-200,400,false,new int[2]{8,17},"Mercenary",false,0.05f,50,1,5), //23
            new JsonNode("Class",400,100,false,new int[2]{9,18},"ArchMage",false,0.05f,50,1,5), //24
            new JsonNode("Class",200,400,false,new int[2]{10,18},"Templar",false,0.05f,50,1,5), //25
            new JsonNode("Class",400,-200,false,new int[2]{11,19},"Invoker",false,0.05f,50,1,5), //26
            new JsonNode("Class",-75,-400,false,new int[2]{12,20},"Hunter",false,0.05f,50,1,5), //27
            new JsonNode("Class",75,-400,false,new int[2]{13,20},"Gunslinger",false,0.05f,50,1,5), //28
            new JsonNode("Class",-325,-250,false,new int[2]{14,21},"Shinobi",false,0.05f,50,1,5), //29

            new JsonNode("Damage",400,400,false,new int[2]{24,25},"Magic",false,0.02f,75,50,4), //30
            new JsonNode("Damage",450,-50,false,new int[2]{24,26},"Summon",false,0.02f,75,50,4), //31
            new JsonNode("Damage",0,-475,false,new int[2]{27,28},"Ranged",false,0.02f,75,50,4), //32
            new JsonNode("Damage",-500,-400,false,new int[2]{27,29},"Throw",false,0.02f,75,50,4), //33
            new JsonNode("Damage",-400,400,false,new int[3]{22,23,25},"Melee",false,0.02f,75,50,4),//34
            new JsonNode("Leech",0,550,false,new int[1]{16},"Both",false,0.05f,100,10,10), //35

            new JsonNode("Stats",550,550,false,new int[1]{30},"Int",false,10,150,10,5), //36
            new JsonNode("Stats",600,-50,false,new int[1]{31},"Spr",false,10,150,10,5), //37
            new JsonNode("Stats",100,-575,false,new int[1]{32},"Agi",false,10,150,10,5), //38
            new JsonNode("Stats",-625,-400,false,new int[1]{33},"Dex",false,10,150,10,5), //39
            new JsonNode("Stats",-550,550,false,new int[1]{34},"Str",false,10,150,10,5), //40

        };
    }

    public class JsonNode
    {
        public string baseType = "Damage";
        public float posX = 50;
        public float posY = 0;
        public int[] neigthboorlist = { 1, 2 };
        public string specificType = "Magic";
        public bool flatDamage = false;
        public float valuePerLevel = 0.1f;
        public int levelRequirement = 0;
        public int maxLevel = 5;
        public int pointsPerLevel = 1;
        public bool unlocked = false;

        public JsonNode(string baseType,float posX,float posY,bool unlocked, int[] neightboorlist,string specificType,bool flatDamage,float valuePerLevel,int levelRequirement,int maxLevel, int pointsPerLevel)
        {
            this.baseType = baseType;
            this.posX = posX;
            this.posY = posY;
            this.unlocked = unlocked;
            this.neigthboorlist = neightboorlist;
            this.specificType = specificType;
            this.flatDamage = flatDamage;
            this.valuePerLevel = valuePerLevel;
            this.levelRequirement = levelRequirement;
            this.maxLevel = maxLevel;
            this.pointsPerLevel = pointsPerLevel;
        }
    }


    class JsonSkillTree
    {
        static JsonNodeList jsonSkillList;
        static public JsonNodeList GetJsonNodeList { get { return jsonSkillList; } }

        public static string Name = "skillTree.json";
        public static string Dir = "Mod Configs" + Path.DirectorySeparatorChar + "AnRPG";
        public static string cPath;



        public static void Init()
        {
            try
            {
                cPath = (Main.SavePath + Path.DirectorySeparatorChar + Dir + Path.DirectorySeparatorChar + Name);
                jsonSkillList = new JsonNodeList();
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
                Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + Dir);

                jsonSkillList = new JsonNodeList();
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
                Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + Dir);
                File.WriteAllText(cPath, JsonConvert.SerializeObject(jsonSkillList, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                AnotherRpgMod.Instance.Logger.Error(e.ToString());
            }
        }

    }
}
