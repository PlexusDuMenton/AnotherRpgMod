using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Utils;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AnotherRpgMod.Items
{

    class Branch
    {
        List<int> m_nodeID;

        public void Add(int ID)
        {
            m_nodeID.Add(ID);
        }
        public int GetLast()
        {
            if (IsEmpty())
                return 0;
            return m_nodeID.Last();
        }
        public int GetLastandBefore()
        {
            if (m_nodeID.Count>1)
                return m_nodeID[m_nodeID.Count - Mathf.RandomInt(1,2)];

            return GetLast();
        }
        public bool IsEmpty()
        {
            if (m_nodeID.Count == 0)
            {
                return true;
            }
            return false;
        }

        public Branch()
        {
            m_nodeID = new List<int>();
        }

        static public float GetPosition(int index)
        {
            switch (index)
            {
                case 1:
                    return -75;
                case 2:
                    return 75;
                case 3:
                    return -175;
                case 4:
                    return 175;
                case 5:
                    return -300;
                case 6:
                    return 300;
                default:
                    return 0;


            }
        }

        static public int GetIdFromPosition(float PosX)
        {
            switch ((int)PosX)
            {
                case -75:
                    return 1;
                case 75:
                    return 2;
                case -175:
                    return 3;
                case 175:
                    return  4;
                case -300:
                    return 5;
                case 300:
                    return  6;
                default:
                    return 0;


            }
        }

        static public int GetRandomNearbyBranches(int index, int limit,List<Branch> BList,bool noEmpty)
        {
            List<int> branches = GetNearbyBranchesID(index);
            List<int> idToDel = new List<int>();
            foreach (int id in branches)
            {
                if (noEmpty && (id >= BList.Count || id < BList.Count && BList[id].IsEmpty()))
                    idToDel.Add(id);
                if (id > limit)
                {
                    idToDel.Add(id);
                }
            }
            foreach(int id in idToDel)
            {
                branches.Remove(id);
            }
            if (branches.Count == 0)
            {
                branches = GetNearbyBranchesID(index);

                return GetRandomNearbyBranches(branches[Mathf.RandomInt(0, branches.Count - 1)],limit, BList, noEmpty);
            }
            if (branches.Count == 1)
            {
                return branches[0];
            }
            return branches[Mathf.RandomInt(0, 1)];
        }

        static public List<int> GetNearbyBranchesID(int index)
        {
            List<int> branches = new List<int>();
            switch (index)
            {
                case 0:
                    branches = new List<int> { 1,2};
                    break;
                case 1:
                    branches = new List<int> { 0, 3 };
                    break;
                case 3:
                    branches = new List<int> { 1, 5 };
                    break;
                case 5:
                    branches = new List<int> { 3};
                    break;
                case 2:
                    branches = new List<int> { 0, 4 };
                    break;
                case 4:
                    branches = new List<int> { 2, 6 };
                    break;
                case 6:
                    branches = new List<int> { 4 };
                    break;
                default:
                    return new List<int>{0};   
            }
            return branches;
            
        }
    }

    class ItemSkillTree
    {

        //How to save : 
        //optimised to : 
        //;ID,Neighboor1:2,state,level,maxlevel,required,posx:posy,specificvalue1:2:3:5:7;
        //Convert to Json, to save

        private int HaveNodeAtPos(Vector2 pos)
        {
            for(int i = 0;i< m_nodeList.Count; i++)
            {
                if (m_nodeList[i].GetPos == pos)
                    return i;
            }
            return -1;
        }

        public static string ConvertToString(ItemSkillTree skilltree)
        {
            string save = "";
            for (int i = 0; i < skilltree.GetSize; i++)
            {
                if (i > 0)
                    save += ";";
                save += ItemNodeAtlas.GetID(skilltree.GetNode(i).GetType().Name) + ",";
                for (int j = 0; j < skilltree.GetNode(i).GetNeighboor.Count; j++)
                {
                    
                    save += skilltree.GetNode(i).GetNeighboor[j];
                    if (j < skilltree.GetNode(i).GetNeighboor.Count-1)
                        save += ':';
                }

                save += "," + skilltree.GetNode(i).GetState + ","+ skilltree.GetNode(i).GetLevel+ "," + skilltree.GetNode(i).GetMaxLevel + ","+ skilltree.GetNode(i).GetRequiredPoints + "," + skilltree.GetNode(i).GetPos.X + ":"+ skilltree.GetNode(i).GetPos.Y+",";

                AnotherRpgMod.Instance.Logger.Info(skilltree.GetNode(i).GetSaveValue());
                save += skilltree.GetNode(i).GetSaveValue();

            }

            return save;
        }

        public static ItemSkillTree ConvertToTree(string save, ItemUpdate source)
        {
            string[] nodeListSave;
            string[] nodeDetails;
            ItemSkillTree tree = new ItemSkillTree();
            nodeListSave = save.Split(';');
            tree.m_ItemSource = source;
            string[] a;
            ItemNode bufferNode;

            foreach (string nodeSave in nodeListSave)
            {
                
                bufferNode = null;
                nodeDetails = nodeSave.Split(',');
                if (nodeDetails.Length != 8)
                {
                    AnotherRpgMod.Instance.Logger.Warn("Item tree corrupted, reseting tree");
                    tree = new ItemSkillTree();
                    tree.Init(source);
                    return tree;
                }

                bufferNode = (ItemNode)ItemNodeAtlas.GetCorrectNode(int.Parse(nodeDetails[0]));
                
                a = nodeDetails[6].Split(':');
                bufferNode.Init(tree, tree.GetSize, int.Parse(nodeDetails[4]), int.Parse(nodeDetails[5]), new Vector2(int.Parse(a[0]), int.Parse(a[1])));

                //Ignore it if there is no neighboor
                if (nodeDetails[1] != "")
                {
                    a = nodeDetails[1].Split(':');
                    foreach (string neightboor in a)
                    {
                        bufferNode.AddNeightboor(int.Parse(neightboor));
                    }
                }
                bufferNode.ForceLockNode(int.Parse(nodeDetails[2]));
                bufferNode.SetLevel = int.Parse(nodeDetails[3]);
                AnotherRpgMod.Instance.Logger.Info(nodeDetails[7]);
                if (nodeDetails[7] != "")
                    bufferNode.LoadValue(nodeDetails[7]);
                tree.AddNode(bufferNode);
            }

            return tree;
        }

        protected List<ItemNode> m_nodeList;
        protected ItemUpdate m_ItemSource;


        private int m_ActualPossibleBranch;

        private const int MAXBRANCH = 7;
        private const int MINBRANCH = 3;

        

        public ItemSkillTree()
        {
            m_ItemSource = new ItemUpdate();
            m_nodeList = new List<ItemNode>();
        }

        public void Init(ItemUpdate source)
        {
            m_ItemSource = source;
            m_nodeList = new List<ItemNode>();
            GenerateTree();
        }

        #region StatsFunctions

        public void ApplyFlatPassives(Item item)
        {

            foreach(ItemNode n in m_nodeList)
            {
                if (n.GetLevel > 0 && n.GetNodeCategory == NodeCategory.Flat)
                {
                    n.Passive(item);
                }
            }

        }

        public void ApplyMultiplierPassives(Item item)
        {

            foreach (ItemNode n in m_nodeList)
            {
                
                if (n.GetLevel>0 && n.GetNodeCategory == NodeCategory.Multiplier)
                {
                    n.Passive(item);
                }
            }

        }

        public void ApplyOtherPassives(Item item)
        {

            foreach (ItemNode n in m_nodeList)
            {
                if (n.GetLevel > 0 && n.GetNodeCategory == NodeCategory.Other)
                {
                    n.Passive(item);
                }
            }

        }

        #endregion



        #region GeneralIntractionFunction
        public void AddNode(ItemNode node)
        {
            m_nodeList.Add(node);
        }

        public int GetSize
        {
            get
            {
                return m_nodeList.Count;
            }

        }

        public void Reset(bool Complete)
        {
            if (Complete)
            {
                m_nodeList = new List<ItemNode>();
                Init(m_ItemSource);
            }
            else
            {
                for (int i = 0; i < m_nodeList.Count; i++)
                {
                    m_nodeList[i].Reset();
                }
                m_nodeList[0].ForceLockNode(3);
            }
        }

        public void BuildConnection()
        {
            foreach (ItemNode node in m_nodeList)
            {
                node.ShareNeightboor();
            }
        }

        public ItemNode GetNode(int ID)
        {
            if (m_nodeList[ID] != null)
            {
                return m_nodeList[ID];
            }
            return null;
        }
        #endregion

        #region ExtendFunctions
        private Branch GetEntireBranch(int ID)
        {
            Branch branch = new Branch();

            foreach (ItemNode node in m_nodeList)
            {
                if (Branch.GetIdFromPosition(node.GetPos.X) == Branch.GetPosition(ID))
                {
                    branch.Add(node.GetId);
                }
            }

            return branch;
        }

        private int GetBranchesCount()
        {
            int branches = 0;
            bool[] detectedBranches = new bool[7];
            foreach (ItemNode node in m_nodeList)
            {
                if (detectedBranches[Branch.GetIdFromPosition(node.GetPos.X)] == false)
                {
                    detectedBranches[Branch.GetIdFromPosition(node.GetPos.X)] = true;
                    branches++;
                }
            }
            return branches;
        }

        private int GetYPos()
        {
            float higgestYPos = 0;

            foreach (ItemNode node in m_nodeList)
            {
                if (node.GetPos.Y > higgestYPos)
                {
                    higgestYPos = node.GetPos.Y;
                }
            }

            return (int)higgestYPos / 100;
        }
        #endregion

        #region Generation

        public void ExtendTree(int Node)
        {
            //Init all value to continue building the tree
            int yPos = GetYPos();
            List<int> IDS = ItemNodeAtlas.GetAvailibleNodeList(m_ItemSource, false);

            int brancheAmm = Mathf.RandomInt(
                Mathf.Clamp(GetBranchesCount(), Mathf.RoundInt(MINBRANCH + m_ItemSource.GetCapLevel() * 0.02f),MAXBRANCH)
                , MAXBRANCH
                );

            List<Branch> Branches = new List<Branch>(brancheAmm);
            for (int i = 0; i < brancheAmm; i++)
            {
                Branches[i] = GetEntireBranch(i);
            }

            int brancheID = 0;
            int connectionBranch;
            Vector2 pos;
            for (int i = 0; i < Node; i++)
            {
                //20% chance to add a new branches
                if (Mathf.RandomInt(0, 5) >= 4 && Branches.Count < brancheAmm)
                {
                    Branches.Add(new Branch());
                }

                //if we reached all branches, then we goes to the layer
                if (brancheID >= Branches.Count)
                {
                    brancheID = 0;
                    yPos++;
                }
                if (Mathf.RandomInt(0, 5) < 4 || (brancheID == Branches.Count - 1 ))
                {

                    connectionBranch = brancheID;

                    if (Branches[brancheID].IsEmpty())
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                    else if (Mathf.RandomInt(0, 5) >= 4)
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);



                    pos = new Vector2(Branch.GetPosition(brancheID), yPos * 100);



                    int a = HaveNodeAtPos(pos);
                    if (a > 0)
                    {
                        m_nodeList[a].AddNeightboor(Branches[brancheID].GetLastandBefore());
                    }
                    else
                    {
                        Branches[brancheID].Add(AddNewRandomNode(Branches[connectionBranch].GetLastandBefore(), pos, IDS, false));
                    }

                    //add neightboor
                    if (Mathf.RandomInt(0, 5) >= 4)
                    {
                        connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                        int id = Branches[connectionBranch].GetLastandBefore();
                        if (!m_nodeList[Branches[brancheID].GetLast()].GetNeighboor.Contains(id))
                            m_nodeList[Branches[brancheID].GetLast()].AddNeightboor(id);
                    }

                }
                //Go through all the node one by one
                brancheID += 1;

            }


        }

        private void GenerateTree()
        {
            int NodeGoal = Mathf.CeilInt(Mathf.Pow(m_ItemSource.baseCap/3f,0.95)*1.25f);
            List<int> IDS = ItemNodeAtlas.GetAvailibleNodeList(m_ItemSource,false);
            int yPos = 0; //Normal skilltree will allways goes down, Ascend will goes up
            int minbranche = MINBRANCH;
            minbranche = Mathf.Clamp(Mathf.FloorInt(MINBRANCH + m_ItemSource.GetCapLevel() * 0.02f),MINBRANCH,MAXBRANCH);
            int brancheAmm = Mathf.RandomInt(minbranche, MAXBRANCH);
            List<Branch> Branches = new List<Branch>
            {
                new Branch(),
                new Branch(),
                new Branch()
            };

            //branches placement : 
            // 3 1 0 2 4
            int brancheID = 0;
            bool emptyLevel = true;
            int connectionBranch;
            Vector2 pos;
            for (int i = 0; i < NodeGoal; i++)
            {
                //20% chance to add a new branches
                if (Mathf.RandomInt(0, 5) >= 4 && Branches.Count < brancheAmm)
                {
                    Branches.Add(new Branch());
                }


                //Used to init the first node
                
                if (i == 0)
                {
                    Branches[0].Add(AddNewRandomNode(-1,new Vector2(0,0), IDS, false));
                    yPos ++;
                    emptyLevel = true;
                }
                else
                {
                    
                    //if we reached all branches, then we goes to the layer
                    if (brancheID >= Branches.Count)
                    {
                        brancheID = 0;
                        yPos++;
                    }
                    if (Mathf.RandomInt(0, 5) < 4 ||(brancheID== Branches.Count-1 && emptyLevel))
                    {
                        emptyLevel = false;

                        connectionBranch = brancheID;
                        
                        if (Branches[brancheID].IsEmpty())
                            connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count,Branches, true);
                        else if (Mathf.RandomInt(0, 5) >= 4)
                            connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);



                        pos = new Vector2(Branch.GetPosition(brancheID), yPos * 100);
                        
                        

                        int a = HaveNodeAtPos(pos);
                        if (a > 0)
                        {
                            m_nodeList[a].AddNeightboor(Branches[brancheID].GetLastandBefore());
                        }
                        else
                        {
                            Branches[brancheID].Add(AddNewRandomNode(Branches[connectionBranch].GetLastandBefore(), pos, IDS, false));
                        }

                        //add neightboor
                        if (Mathf.RandomInt(0, 5) >= 4)
                        {
                            connectionBranch = Branch.GetRandomNearbyBranches(brancheID, Branches.Count, Branches, true);
                            int id = Branches[connectionBranch].GetLastandBefore();
                            if (!m_nodeList[Branches[brancheID].GetLast()].GetNeighboor.Contains(id))
                                m_nodeList[Branches[brancheID].GetLast()].AddNeightboor(id);
                        }

                    }
                    //Go through all the node one by one
                    brancheID += 1;

                }
            }
            BuildConnection();  
            m_nodeList[0].UnlockStep(3);
        }

        public int AddNewRandomNode(int Source,Vector2 position , List<int> IDList, bool Ascend = false )
        {

            
            int ID = IDList.Count;
            ItemNode Node = (ItemNode)ItemNodeAtlas.GetCorrectNode(IDList[Mathf.RandomInt(0, ID)]);

            if (!Ascend) { 
                float power = Mathf.Clamp((position.Y / 250) + (Math.Abs(position.X) / 300) + Mathf.Random(-0.35f, 0.35f),0,45);
                int level = 3 + (int)power;
                int requirement = Mathf.FloorInt(1 + power * 0.75f);
                Node.Init(this,m_nodeList.Count, level, requirement, position); 
                Node.SetPower(1+power);

                if (Source != -1)
                {
                    Node.AddNeightboor(Source);
                }
                m_nodeList.Add(Node);
            }


            return m_nodeList.Count-1;
        }

        #endregion
    }
}
