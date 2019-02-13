
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Items;

namespace AnotherRpgMod.Utils
{

    public enum DamageNameTree : byte
    {
        Melee,
        Ranged,
        Throw,
        Magic,
        Summon,
        Bow, //thorium
        Gun //thorium

    }

    class SkillTextures
    {

        static public string GetItemTexture(ItemNode node)
        {
            string path = "AnotherRpgMod/Textures/ItemTree/" + node.GetName;
            return path;
        }

        static public string GetTexture(Node node)
        {
            string path = "AnotherRpgMod/Textures/SkillTree/" + node.GetNodeType + "/";

            string additional = "";

            switch (node.GetNodeType)
            {
                case NodeType.Class:
                    additional += (node as ClassNode).GetClassType;
                    break;
                case NodeType.Damage:
                    additional += (node as DamageNode).GetDamageType;
                    break;
                case NodeType.Speed:
                    additional += (node as SpeedNode).GetDamageType;
                    break;
                case NodeType.Leech:
                    additional += (node as LeechNode).GetLeechType;
                    break;
                case NodeType.Perk:
                    additional += (node as PerkNode).GetPerk;
                    break;
                case NodeType.Immunity:
                    additional += (node as ImmunityNode).GetImmunity;
                    break;
                case NodeType.Stats:
                    additional += (node as StatNode).GetStatType;
                    break;
            }
            path += additional;
            return path;
        }
    }

    
}
