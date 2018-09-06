using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AnotherRpgMod.RPGModule;
using Terraria;
using AnotherRpgMod.RPGModule.Entities;

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
        //Tier 0 
        Tourist, // base class + 5% all damage 
        //Tier 1 -lvl 15+ (should be arround EoC)
        Apprentice, // upgrade to tourist , + 15% all damage , +5% health
        Archer, // +20% ranged damage , 5% dodge + 30% bow damage
        Gunner, // +20% ranged damage , 20% chance not to consume ammo + 30% gun damage
        SwordMan, // + 75% melee damage , + 10% melee speed , -20% health
        Spiritualist, // + 50% summon damage , + 1 summon 
        Mage, // +50% magic damage , 20% manacost reduction - 25% health
        Ninja, //+50% throw damage , 20% not to consume ammo 
        Acolyte, // 30% damage to mana (10(+int/50) damage per mana) + 25% magic damage + 10% health
        Cavalier, //+ 20% melee damage , + 50% health ,+ 20% armor , -5% movement speed
        //Tier 1 -lvl 15+ (should be arround EoC)
        Regular, // upgrade to tourist , + 15% all damage , +5% health
        Hunter, // +20% ranged damage , 5% dodge + 30% bow damage
        Gunslinger, // +20% ranged damage , 20% chance not to consume ammo + 30% gun damage
        Mercenary, // + 75% melee damage , + 10% melee speed , -20% health
        Invoker, // + 50% summon damage , + 1 summon 
        ArchMage, // +50% magic damage , 20% manacost reduction - 25% health
        Shinobi, //+50% throw damage , 20% not to consume ammo 
        Templar, // 30% damage to mana (10(+int/50) damage per mana) + 25% magic damage + 10% health
        Knight, //+ 20% melee damage , + 50% health ,+ 20% armor , -5% movement speed



    }
    [Flags]
    public enum Perk // all togleable
    {
        None = 0x0,
        ArchPriest=0x1, // on death keep restore 1/25%/50%/75% health, give 1 second of damage immunity, and enter cd (600/450/300/150 seconds)
        BloodMage=0x2, // 50/75/90% manacost reduction , lose health on manaUse
        Vampire = 0x4, //0.05/0.075/0.1% lifesteal , +50/75/100% stats at night, -50/25/0% stats at day)
        DemonEater = 0x8, //Each kill regen 5/10/15% of your max health
        Cupidon = 0x10, //each attack have 5/10/15/20 % chance to drop an heart, increase pick range of heart
        StarGatherer = 0x20, //each attack have 5/10/15/20 % chance to drop an mana star, increase pick range of stars
        Biologist = 0x40, // potion heal 20/40/60/80/100% more heal/mana
        Berserk = 0x80, //each percent of health missing increase your damage by 1/2/3% 
        Masochist = 0x100, //Set armor to 0, each points of armor increase you damage dealt by 1/2/3
        Survivalist = 0x200, // Reduce Damage by 50% to gain 15/30/45% of defense
        TheGambler = 0x400, // 50% chance for damage to heal , 50% chance for heal to damage
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
        Leech,
        Immunity,
        Class,
        Perk,
        Stats
    }





        //Agility       Tanking
    //          MELEE

                   //

    // RANGED           MAGIC
    //          

    





    //          MELEE


    //THROW                 SUMMON


    //RANGED                RANGED         

    class Node
    {
        protected NodeType Type;

        
        protected int level = 0;
        protected NodeParent Parent;

        protected int maxLevel = 1;
        protected int pointsPerLevel = 1;
        protected float value = 0;
        protected int levelRequirement = 0;
        protected bool unlocked = false;

        public NodeType GetNodeType
        {
            get
            {
                return Type;
            }
        }
        public NodeParent GetParent { get { return Parent; } }
        public void SetParrent(NodeParent Parent)
        {
            this.Parent = Parent;
        }
        public int GetLevel
        {
            get
            {
                return level;
            }
        }
        public float GetValue
        {
            get
            {
                return value;
            }
        }
        public int GetMaxLevel
        {
            get
            {
                return maxLevel;
            }
        }
        public int GetCostPerLevel
        {
            get
            {
                return pointsPerLevel;
            }
        }
        public int GetLevelRequirement
        {
            get
            {
                return levelRequirement;
            }
        }
        public bool GetUnlock
        {
            get
            {
                return unlocked;
            }
        }
        public bool GetActivate
        {
            get
            {
                return activate;
            }
        }
        public bool GetEnable
        {
            get
            {
                return enable;
            }
        }

        

        protected bool activate = false;

        protected bool enable = false;

        public virtual void ToggleEnable()
        {
            enable = !enable;
        }

        public Node(NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0,  int _maxLevel = 1, int _pointsPerLevel = 1)
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
            if (!unlocked)
                return Reason.NotUnlocked;
            return Reason.CanUpgrade;
        }

        public virtual void Upgrade()
        {
            if (!activate)
            {
                activate = true;
                enable = true;
            }
            
            level++;
        }

        public virtual void Unlock()
        {
            unlocked = true;
        }

    }










    #region NodeChild
    class PerkNode : Node
    {
        Perk perk;
        public Perk GetPerk
        {
            get
            {
                return perk;
            }
        }
        public PerkNode(Perk _perk,NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type,_unlocked,_value,_levelrequirement,_maxLevel,_pointsPerLevel)
        {
            perk = _perk;
        }
    }

    class DamageNode : Node
    {
        DamageType damageType;
        bool flat;
        public bool GetFlat
            {
                get
                {
                    return flat;
                }
            }
        public DamageType GetDamageType
            {
                get
                {
                    return damageType;
                }
            }
        public DamageNode(DamageType _damageType,bool _flat, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement,  _maxLevel, _pointsPerLevel)
        {
            damageType = _damageType;
            flat = _flat;
        }

        public float GetDamage()
        {
            return value * level;
        }
    }

    class StatNode : Node
    {
        Stat StatType;
        bool flat;
        public bool GetFlat
        {
            get
            {
                return flat;
            }
        }
        public Stat GetStatType
        {
            get
            {
                return StatType;
            }
        }
        public StatNode(Stat _statType, bool _flat, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement, _maxLevel, _pointsPerLevel)
        {
            StatType = _statType;
            flat = _flat;
        }

        public float GetDamage()
        {
            return value * level;
        }
    }

    class ClassNode : Node
    {
        ClassType classType;
        public ClassType GetClassType
                {
                    get
                    {
                        return classType;
                    }
                }
        public ClassNode(ClassType _classType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0,int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement,  _maxLevel, _pointsPerLevel)
        {
            classType = _classType;
        }

        public void loadingUpgrade()
        {
            base.Upgrade();
        }

        public override void Upgrade()
        {
            base.Upgrade();

            NodeParent _node;
            if (Main.myPlayer >= Main.player.Length)
                return;
            RPGPlayer player = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            player.GetskillTree.ActiveClass = this;

            for (int i = 0; i < player.GetskillTree.nodeList.nodeList.Count; i++)
            {
                _node = player.GetskillTree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (classNode.GetClassType != classType && classNode.GetActivate)
                    {
                        classNode.Disable();
                    }
                }
            }
        }

        public void Disable()
        {
            if (Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass == this)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            enable = false;
        }

        public override void ToggleEnable()
        {
            base.ToggleEnable();
            NodeParent _node;
            if (enable)
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = this;
            else
            {
                Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.ActiveClass = null;
            }
            for (int i = 0; i < Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList.Count; i++)
            {
                _node = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>().GetskillTree.nodeList.nodeList[i];
                if (_node.GetNodeType == NodeType.Class)
                {
                    ClassNode classNode = (ClassNode)_node.GetNode;
                    if (classNode.GetClassType != classType && classNode.GetActivate)
                    {
                        classNode.Disable();
                    }

                }
            }
        }
    }

    class SpeedNode : Node
    {
        DamageType damageType;
        public DamageType GetDamageType
                {
                    get
                    {
                        return damageType;
                    }
                }
        public SpeedNode(DamageType _damageType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0,int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement,  _maxLevel, _pointsPerLevel)
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
        public LeechType GetLeechType
                    {
                        get
                        {
                            return leechType;
                        }
                    }
        public LeechNode(LeechType _leechType, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0, int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement,  _maxLevel, _pointsPerLevel)
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
        public Immunity GetImmunity
                        {
                            get
                            {
                                return immunity;
                            }
                        }
        public ImmunityNode(Immunity _immunity, NodeType _type, bool _unlocked = false, float _value = 1, int _levelrequirement = 0,  int _maxLevel = 1, int _pointsPerLevel = 1) : base(_type, _unlocked, _value, _levelrequirement,  _maxLevel, _pointsPerLevel)
        {
            immunity = _immunity;
        }

    }

    #endregion

    class NodeParent
    {
        Node actualNode;
        List<NodeParent> neighboorNode;
        public List<NodeParent> connectedNeighboor; //used to check for drawing connection lines between neighboor
        public List<NodeParent> GetNeightboor
        {
            get
            {
                return neighboorNode;
            }
        }
        public Node GetNode
        {
            get
            {
                return actualNode;
            }
        }

        public Reason CanUpgrade(int points,int level)
        {
            if (level < actualNode.GetLevelRequirement)
                return Reason.LevelRequirement;
            return actualNode.CanUpgrade( points);
        }
        public bool GetEnable { get { return actualNode.GetEnable; } }
        static ushort TotalID = 0;

        public readonly ushort ID;

        

        public Vector2 menuPos;
        //comment to my future self as I will surely forget it : 
        // type var => var
        // is equal to 
        // type var {get{return var;}}
        public NodeType GetNodeType
        {
            get
            {
                return actualNode.GetNodeType;
            }
        }
        public int GetLevel
        {
            get
            {
                return actualNode.GetLevel;
            }
        }
        public int GetMaxLevel
        {
            get
            {
                return actualNode.GetMaxLevel;
            }
        }
        public int GetCostPerLevel
        {
            get
            {
                return actualNode.GetCostPerLevel;
            }
        }
        public int GetLevelRequirement
        {
            get
            {
                return actualNode.GetLevelRequirement;
            }
        }
        public bool GetUnlock
        {
            get
            {
                return actualNode.GetUnlock;
            }
        }
        public bool GetActivate
        {
            get
            {
                return actualNode.GetActivate;
            }
        }


        public NodeParent(Node node,Vector2 pos)
        {
            neighboorNode = new List<NodeParent>();
            connectedNeighboor = new List<NodeParent>();
            actualNode = node;
            menuPos = pos;
            ID = TotalID;
            TotalID++;
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


        public void Unlock()
        {
            actualNode.Unlock();
        }

        public void AddNeighboor(NodeParent neighboor)
        {
            neighboorNode.Add(neighboor);
            neighboor.AddNeighboorSimple(this);
        }

        public void AddNeighboorSimple(NodeParent neighboor)
        {
            neighboorNode.Add(neighboor);
        }

        public void Upgrade(bool loading = false)
        {
            if (loading && actualNode.GetNodeType == NodeType.Class)
            {
                (actualNode as ClassNode).loadingUpgrade();
            }
            else
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
        public List<NodeParent> nodeList;

        private List<DamageNode> meleeDamage;
        private List<DamageNode> rangedDamage;
        private List<DamageNode> magicDamage;
        private List<DamageNode> summonDamage;
        private List<DamageNode> throwDamage;

        private List<StatNode> StatsNodes;

        private List<SpeedNode> meleeSpeed;
        private List<SpeedNode> rangedSpeed;
        private List<SpeedNode> magicSpeed;

        private List<LeechNode> leechs;
        public List<LeechNode> GetLeech
        {
            get
            {
                return leechs;
            }
        }

        private List<ClassNode> classes;
        private List<PerkNode> perks;
        private List<ImmunityNode> immunities;

        public NodeList()
        {
            nodeList = new List<NodeParent>();

            meleeDamage = new List<DamageNode>();
            rangedDamage = new List<DamageNode>();
            magicDamage = new List<DamageNode>();
            summonDamage = new List<DamageNode>();
            throwDamage = new List<DamageNode>();

            meleeSpeed = new List<SpeedNode>();
            rangedSpeed = new List<SpeedNode>();
            magicSpeed = new List<SpeedNode>();
            leechs = new List<LeechNode>();

            StatsNodes = new List<StatNode>();

            classes = new List<ClassNode>();
            perks = new List<PerkNode>();
            immunities = new List<ImmunityNode>();

        }

        public List<StatNode> GetStatsList
        {
            get
            {
                return StatsNodes;
            }
        }

        public List<ClassNode> GetClasses
        {
            get
            {
                return classes;
            }
        }
        public List<PerkNode> GetPerks
        {
            get
            {
                return perks;
            }
        }
        public List<ImmunityNode> GetImmunities
        {
            get
            {
                return immunities;
            }
        }

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
            NodeParent nodeParent = new NodeParent(node, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
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
            NodeParent nodeParent = new NodeParent(node, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
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
            NodeParent nodeParent = new NodeParent(perk, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            perks.Add(perk);
        }

        public void AddNode(LeechNode leech)
        {
            NodeParent nodeParent = new NodeParent(leech, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            leechs.Add(leech);
        }

        public void AddNode(ImmunityNode immunity)
        {
            NodeParent nodeParent = new NodeParent(immunity, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            immunities.Add(immunity);
        }

        public void AddNode(ClassNode classnode)
        {
            NodeParent nodeParent = new NodeParent(classnode, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            classes.Add(classnode);
        }
        public void AddNode(StatNode statnode)
        {
            NodeParent nodeParent = new NodeParent(statnode, Vector2.Zero);
            nodeParent.GetNode.SetParrent(nodeParent);
            nodeList.Add(nodeParent);
            StatsNodes.Add(statnode);
        }

    }

    class SkillTree
    {
        public ClassNode ActiveClass;
        public NodeList nodeList;

        public int GetStats(Stat stat)
        {
            int value = 0;
            foreach (StatNode node in nodeList.GetStatsList)
            {
                if (node.GetStatType == stat)
                    value += (int)(node.GetValue * node.GetLevel);
            }
            return value;
        }

        private float CalcDamage(List<DamageNode> _list, bool flat)
        {
            float value = 0;
            foreach (DamageNode node in _list)
            {
                if (node.GetFlat == flat)
                    value += node.GetValue* node.GetLevel;
            }
            return value;
        }

        private float CalcSpeed(List<SpeedNode> _list)
        {
            float value = 0;
            foreach (SpeedNode node in _list)
            {
                value += node.GetValue * node.GetLevel;
            }
            return value;
        }

        private float CalcLeech(List<LeechNode> _list,LeechType _type)
        {
            float value = 0;
            foreach (LeechNode node in _list)
            {
                if (node.GetLeechType == LeechType.Both || node.GetLeechType == _type)
                    value += node.GetValue * node.GetLevel;
            }
            return value;
        }

       public int GetSummonSlot()
        {
            int slot = 0;
            if (ActiveClass == null)
                return 0;
            switch (ActiveClass.GetClassType)
            {
                case (ClassType.Spiritualist):
                    slot = 1;
                    break;
                case (ClassType.Invoker):
                    slot = 2;
                    break;
            }
            return slot;
        }

        private float GetClassDamage(DamageType _type)
        {
            float value = 1;
            RPGPlayer pEntity = Main.player[Main.myPlayer].GetModPlayer<RPGPlayer>();
            if (ActiveClass == null)
            {
                return 1;
            }
            JsonChrClass actualClass = JsonCharacterClass.GetJsonCharList.GetClass(ActiveClass.GetClassType);
            value += actualClass.Damage[(int)_type];
            if (_type == DamageType.Ranged)
            {
                if (pEntity.HaveBow())
                    value += actualClass.Damage[5];
                if(pEntity.HaveRangedWeapon() && !pEntity.HaveBow())
                    value += actualClass.Damage[6];
            }
            return value;
        }
        public float GetDamageMult(DamageType _type)
        {
            float value = 0;

            value += CalcDamage(nodeList.GetDamageList(_type), false);
            value += GetClassDamage(_type);
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

        public float GetLeech(LeechType _leechType)
        {
            float value = 0;

            value += CalcLeech(nodeList.GetLeech, _leechType);

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

        public void  ResetConnection()
        {
            int count = nodeList.nodeList.Count;
            for (int i = 0;i< count; i++)
            {
                nodeList.nodeList[i].connectedNeighboor = new List<NodeParent>();
            }
        }
        public SkillTree()
        {

            nodeList = new NodeList();
            JsonNodeList NodeSaved = JsonSkilLTree.GetJsonNodeList;

            NodeType nodeT;
            ClassType classT;
            DamageType damageT;
            LeechType leechT;
            Immunity immunityT;
            Stat StatT;
            Perk perkT;

            int i = 0;
            foreach (JsonNode actualNode in NodeSaved.jsonList)
            {
                nodeT = (NodeType)Enum.Parse(typeof(NodeType), actualNode.baseType);
                switch (nodeT)
                {
                    case (NodeType.Damage):
                        damageT = (DamageType)Enum.Parse(typeof(DamageType), actualNode.specificType);
                        nodeList.AddNode(new DamageNode(damageT, actualNode.flatDamage, NodeType.Damage, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Class):
                        classT = (ClassType)Enum.Parse(typeof(ClassType), actualNode.specificType);
                        nodeList.AddNode(new ClassNode(classT, NodeType.Class, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, 1, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Speed):
                        damageT = (DamageType)Enum.Parse(typeof(DamageType), actualNode.specificType);
                        nodeList.AddNode(new SpeedNode(damageT, NodeType.Speed, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Immunity):
                        immunityT = (Immunity)Enum.Parse(typeof(Immunity), actualNode.specificType);
                        nodeList.AddNode(new ImmunityNode(immunityT, NodeType.Immunity, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Leech):
                        leechT = (LeechType)Enum.Parse(typeof(LeechType), actualNode.specificType);
                        nodeList.AddNode(new LeechNode(leechT, NodeType.Leech, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Perk):
                        perkT = (Perk)Enum.Parse(typeof(Perk), actualNode.specificType);
                        nodeList.AddNode(new PerkNode(perkT, NodeType.Perk, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                    case (NodeType.Stats):
                        StatT = (Stat)Enum.Parse(typeof(Stat), actualNode.specificType);
                        nodeList.AddNode(new StatNode(StatT, actualNode.flatDamage, NodeType.Stats, actualNode.unlocked, actualNode.valuePerLevel, actualNode.levelRequirement, actualNode.maxLevel, actualNode.pointsPerLevel));
                        break;
                }

                nodeList.nodeList[i].menuPos = new Vector2(actualNode.posX, actualNode.posY);
                i++;
            }
            i = 0;
            foreach (JsonNode actualNode in NodeSaved.jsonList)
            {
                
                foreach (int nbID in actualNode.neigthboorlist)
                {
                    nodeList.nodeList[i].AddNeighboor(nodeList.nodeList[nbID]);

                }

                i++;
            }
        }


        public readonly static int SKILLTREEVERSION = 1;
        public void Init()
        {
            nodeList.nodeList[0].Upgrade();
        }
    }
}