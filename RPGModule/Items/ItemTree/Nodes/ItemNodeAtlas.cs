using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Items;

namespace AnotherRpgMod.Utils
{
    class ItemNodeAtlas
    {

        //Node availible for all items;
        static public List<int> CommonID = new List<int>()
        {
            500,
        };

        //Node availible for armor
        static public List<int> ArmorID = new List<int>()
        {
            300,
        };


        //Node abaible for weapons
        static public List<int> WeaponID = new List<int>()
        {
            0,1,2
        };

        //Node abaible for ranged Weapon only
        static public List<int> RangedID = new List<int>()
        {
            //100
        };

        //Node abaible for melee Weapon only
        static public List<int> MeleeID = new List<int>()
        {
            50
        };

        //Node abaible for magic Weapon only
        static public List<int> MagicID = new List<int>()
        {

        };

        //Node abaible for summon Weapon only
        static public List<int> SummonID = new List<int>()
        {

        };


        
        static public Dictionary<int,Type> Atlas = new Dictionary<int, Type>(){
            {0, typeof(AdditionalDamageNode) },
            {1,typeof(AdditionalDamageNodePercent) },
            {2,typeof(UseTimeNode) },
            {50,typeof(LifeLeech) },


            {100,typeof(AdditionalProjectile) },

            {300,typeof(AdditionalDefenceNode) },


            {500,typeof(BonusExpNode) }
        };

        static public object GetCorrectNode(int AtlasID)
        {
            object Node = Activator.CreateInstance(Atlas[AtlasID]);
            return Node;
        }

        static public int GetID(string nodeType)
        {
            foreach(KeyValuePair<int, Type> entry in Atlas)
            {
                if (entry.Value.Name == nodeType)
                    return entry.Key;
            }
            return 0;
        }

        static public List<int> GetAvailibleNodeList(ItemUpdate source,bool ascend)
        {
            List<int> IDS = CommonID;

            switch (source.Get_ItemType)
            {
                case ItemType.Weapon:
                    IDS.AddRange(WeaponID);
                    switch (source.GetWeaponType)
                    {
                        case WeaponType.ExtendedMelee:
                        case WeaponType.OtherMelee:
                        case WeaponType.Stab:
                        case WeaponType.Spear:
                        case WeaponType.Swing:
                            IDS.AddRange(MeleeID);
                            break;
                        case WeaponType.OtherRanged:
                        case WeaponType.Gun:
                        case WeaponType.Ranged:
                        case WeaponType.Bow:
                            IDS.AddRange(RangedID);
                            break;
                        case WeaponType.Summon:
                            IDS.AddRange(SummonID);
                            break;
                        case WeaponType.Magic:
                            IDS.AddRange(MagicID);
                            break;

                    }
                    break;
                case ItemType.Armor:
                    IDS.AddRange(ArmorID);
                    break;
            }
            List<int> IDR = new List<int>();
            foreach (int n in IDS)
            {
                if (((ItemNode)GetCorrectNode(n)).IsAscend != ascend)
                {
                    IDR.Add(n);
                }
            }

            foreach (int n in IDR)
            {
                IDS.Remove(n);
            }
            return IDS;
        }
    }
}
