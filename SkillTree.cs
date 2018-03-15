using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AnotherRpgMod.RPGModule
{
    //Gonna be fun !
    public enum Immunity
    {
        OnFire,

    }

    public enum Reason
    {
        CanUpgrade,
        NoEnoughtPoints,
        MaxLevelReach,
        NotUnlocked,
        LevelRequirement
    }

    public enum ClassType
    {
        Tourist, // base class + 5% all damage
        Ranger, // +100% ranged , 20% chance not to consume ammo -25% others
        Duelist, // + 100% melee , + 20% health - 25% others
        Summoner, // + 100% summon , + 2 summon , - 25% other class
        Magus, // +100% magic , 20% manacost reduction - 25% other
        Ninja, //+100% throw , 20% not to consume ammo - 25%other
        OneManArmy, // +5 summon , + 200% summon damage, all other damage type are reset to 1

    }

    public enum Perk // all togleable
    {
        Imortalisis, // on death keep health at 1, give 1 second of damage immunity, and enter cd (600/450/300/150 seconds)
        BloodMage, // 50/75/90% manacost reduction , lose health on manaUse
        Vampire, //0.5/0.75/0.1% lifesteal , +50/75/100% stats at night, 50/75/100% stats at day)
        DemonEater, //Each kill regen 5/10/15% of your max health
        Cupidon, //each attack have 5/10/15/20 % chance to drop an heart, increase pick range of heart
        StarGatherer, //each attack have 5/10/15/20 % chance to drop an mana star, increase pick range of stars
        Biologist, // potion heal 20/40/60/80/100% more heal/mana
        Berserk, //each percent of health missing increase your damage by 1/2/3% 
        Masochist, //Set armor to 0, each points of armor increase you damage dealt by 1/2/3
        Survivalist, // Reduce Damage by 50% to gain 15/30/45% of defense
    }

    public enum LeechType
    {
        Life,
        Magic,
        Both
    }

    public enum NodeType
    {
        Speed,
        Damage,
        LifeLeech,
        MagicLeech,
        Immunity,
        Class,
        Perk
    }










    //          MELEE


    //THROW                 SUMMON


    //RANGED                RANGED         

    class Node
    {
        protected NodeType Type;

        
        protected int level = 0;


        protected int maxLevel = 1;
        protected int pointsPerLevel = 1;
        protected float value = 0;
        protected int levelRequirement = 0;
        protected bool unlocked = false;

        public NodeType GetNodeType => Type;
        public int GetLevel => level;
        public float GetValue => value;
        public int GetMaxLevel => maxLevel;
        public int GetCostPerLevel => pointsPerLevel;
        public int GetLevelRequirement => levelRequirement;
        public bool GetUnlock => unlocked;
        public bool GetActivate => activate;
        public bool GetEnable => enable;


        protected bool activate = false;

        protected bool enable = true;

        public void ToggleEnable()
        {
            enable = !enable;
        }

        public Node(NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1)
        {
            
            Type = _type;
            value = _value;
            maxLevel = _maxLevel;
            pointsPerLevel = _pointsPerLevel;
            activate = false;
            levelRequirement = _levelrequirement;
            unlocked = _unlocked;
            
        }

        public Reason CanUpgrade(int points)
        {
            if (points < pointsPerLevel)
                return Reason.NoEnoughtPoints;
            if (level >= maxLevel)
                return Reason.MaxLevelReach;
            return Reason.CanUpgrade;
        }

        public void Upgrade()
        {
            if (!activate)
                activate = true;
            level++;
        }

        public void Unlock()
        {
            unlocked = true;
        }

    }










    #region NodeChild
    class PerkNode : Node
    {
        Perk perk;
        public Perk GetPerk => perk;
        public PerkNode(Perk _perk,NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type,_unlocked,_levelrequirement,_value,_maxLevel,_pointsPerLevel)
        {
            perk = _perk;
        }
    }

    class DamageNode : Node
    {
        DamageType damageType;
        bool flat;
        public bool GetFlat => flat;
        public DamageType GetDamageType => damageType;
        public DamageNode(DamageType _damageType,bool _flat, NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _levelrequirement, _value, _maxLevel, _pointsPerLevel)
        {
            damageType = _damageType;
        }

        public float GetDamage()
        {
            return value * level;
        }
    }

    class ClassNode : Node
    {
        ClassType classType;
        public ClassType GetClassType => classType;
        public ClassNode(ClassType _classType, NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _levelrequirement, _value, _maxLevel, _pointsPerLevel)
        {
            classType = _classType;
        }
    }

    class SpeedNode : Node
    {
        DamageType damageType;
        public DamageType GetDamageType => damageType;
        public SpeedNode(DamageType _damageType, NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _levelrequirement, _value, _maxLevel, _pointsPerLevel)
        {
            damageType = _damageType;
        }

        public float GetSpeed()
        {
            return value * level;
        }
    }

    class LeechNode : Node
    {
        LeechType leechType;
        public LeechType GetLeechType => leechType;
        public LeechNode(LeechType _leechType, NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _levelrequirement, _value, _maxLevel, _pointsPerLevel)
        {
            leechType = _leechType;
        }

        public float GetLeech()
        {
            return value * level;
        }
    }

    class ImmunityNode : Node
    {
        Immunity immunity;
        public Immunity GetImmunity => immunity;
        public ImmunityNode(Immunity _immunity, NodeType _type, bool _unlocked = false, int _levelrequirement = 0, int _value = 1, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _levelrequirement, _value, _maxLevel, _pointsPerLevel)
        {
            immunity = _immunity;
        }

    }

    #endregion

    class NodeParent
    {
        Node actualNode;
        List<NodeParent> neighboorNode;

        static ushort TotalID = 0;
        public static List<NodeParent> NodeList;

        public readonly ushort ID;

        public List<NodeParent> connectedNeighboor; //used to check for drawing connection lines between neighboor

        public Vector2 menuPos;
        //comment to my future self as I will surely forget it : 
        // type var => var
        // is equal to 
        // type var {get{return var;}}
        public NodeType GetNodeType => actualNode.GetNodeType;
        public int GetLevel => actualNode.GetLevel; 
        public int GetMaxLevel => actualNode.GetMaxLevel; 
        public int GetCostPerLevel => actualNode.GetCostPerLevel; 
        public int GetLevelRequirement => actualNode.GetLevelRequirement;
        public bool GetUnlock => actualNode.GetUnlock;
        public bool GetActivate => actualNode.GetActivate;


        public NodeParent(Node node,Vector2 pos)
        {
            actualNode = node;
            menuPos = pos;
            ID = TotalID;
            TotalID++;
            NodeList.Add(this);
        }

        public bool CanBeDisable()
        {
            if (actualNode.GetNodeType == NodeType.Class || actualNode.GetNodeType == NodeType.Perk)
                return true;
            return false;
        }

        public void ToggleEnable()
        {
            actualNode.ToggleEnable();
        }

        public Reason CanUpgrade(int points)
        {
            return actualNode.CanUpgrade(points);
        }

        public void Unlock()
        {
            actualNode.Unlock();
        }

        public void AddNeighboor(NodeParent neighboor)
        {
            neighboorNode.Add(neighboor);
        }

        public void Upgrade()
        {
            actualNode.Upgrade();
            for (int i = 0; i< neighboorNode.Count; i++)
            {
                neighboorNode[i].Unlock();
            }
        }
    }

    class NodeList //prefer to make this class to stock in different set each type of node to speedup
        //looking for the entire tree would take lot of time
    {
        private List<DamageNode> meleeDamage;
        private List<DamageNode> rangedDamage;
        private List<DamageNode> magicDamage;
        private List<DamageNode> summonDamage;
        private List<DamageNode> throwDamage;

        private List<SpeedNode> meleeSpeed;
        private List<SpeedNode> rangedSpeed;
        private List<SpeedNode> magicSpeed;

        private List<LeechNode> leechs;
        public List<LeechNode> GetLeech => leechs;


        private List<ClassNode> classes;
        private List<PerkNode> perks;
        private List<ImmunityNode> immunities;

        public List<ClassNode> GetClasses => classes;
        public List<PerkNode> GetPerks => perks;
        public List<ImmunityNode> GetImmunities => immunities;

        public List<DamageNode> GetDamageList(DamageType _type)
        {
            switch (_type)
            {
                case DamageType.Magic:
                    return magicDamage;
                case DamageType.Melee:
                    return meleeDamage;
                case DamageType.Ranged:
                    return rangedDamage;
                case DamageType.Throw:
                    return throwDamage;
                default:
                    return summonDamage;
            }
        }

        public void AddNode(DamageNode node)
        {
            switch (node.GetDamageType)
            {
                case DamageType.Magic:
                    magicDamage.Add(node);
                    break;
                case DamageType.Melee:
                    meleeDamage.Add(node);
                    break;
                case DamageType.Ranged:
                    rangedDamage.Add(node);
                    break;
                case DamageType.Throw:
                    throwDamage.Add(node);
                    break;
                default:
                    summonDamage.Add(node);
                    break;
            }
        }

        public void AddNode(SpeedNode node)
        {
            switch (node.GetDamageType)
            {
                case DamageType.Magic:
                    magicSpeed.Add(node);
                    break;
                case DamageType.Melee:
                    meleeSpeed.Add(node);
                    break;
                case DamageType.Ranged:
                    rangedSpeed.Add(node);
                    break;
                default:
                    return;
            }
        }

        public void AddNode(PerkNode perk)
        {
            perks.Add(perk);
        }

        public void AddNode(LeechNode leech)
        {
            leechs.Add(leech);
        }

        public void AddNode(ImmunityNode immunity)
        {
            immunities.Add(immunity);
        }

        public void AddNode(ClassNode classnode)
        {
            classes.Add(classnode);
        }


    }

    class SkillTree
    {
        NodeList nodeList;
        private float CalcDamage(List<DamageNode> _list, bool flat)
        {
            float value = 0;
            foreach (DamageNode node in _list)
            {
                if (node.GetFlat == flat)
                    value += node.GetValue;
            }
            return value;
        }

        private float CalcSpeed(List<SpeedNode> _list)
        {
            float value = 0;
            foreach (SpeedNode node in _list)
            {
                value += node.GetValue;
            }
            return value;
        }

        private float CalcLeech(List<LeechNode> _list,LeechType _type)
        {
            float value = 0;
            foreach (LeechNode node in _list)
            {
                if (node.GetLeechType == LeechType.Both || node.GetLeechType == _type)
                    value += node.GetValue;
            }
            return value;
        }


        public float GetDamageMult(DamageType _type)
        {
            float value = 1;

            value += CalcDamage(nodeList.GetDamageList(_type), false);

            return value;
        }

        public int GetDamageFlat(DamageType _type)
        {
            float value = 0;

            value += CalcDamage(nodeList.GetDamageList(_type), true);

            return (int)value;
        }
        public float GetDamageSpeed(DamageType _type)
        {
            float value = 0;

            value += CalcDamage(nodeList.GetDamageList(_type), true);

            return value;
        }

        public float GetLeech(LeechType _Leechtype)
        {
            float value = 0;

            value += CalcLeech(nodeList.GetLeech, _Leechtype);

            return value;
        }

        public bool HavePerk(Perk _perk)
        {
            List<PerkNode> list = nodeList.GetPerks;
            for (int i = 0;i< list.Count; i++)
            {
                if (list[i].GetPerk == _perk && list[i].GetEnable)
                    return true;
            }
            return false;
        }

        public bool HaveImmunity(Immunity _immunity)
        {
            List<ImmunityNode> list = nodeList.GetImmunities;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetImmunity == _immunity && list[i].GetEnable)
                    return true;
            }
            return false;
        }

        public Node GetSkillByID(int ID)
        {
            if (ID > Node.Nodes.Count)
                return null;

            return Node.Nodes[ID];
        }

        public SkillTree()
        {

            nodeList = new NodeList();
        }
    }
}
