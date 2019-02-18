using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Utils;
using AnotherRpgMod.Items;
namespace AnotherRpgMod.Command
{
    
    public class Level : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "level"; }
        }

        public override string Usage
        {
            get { return "/level <level>"; }
        }

        public override string Description
        {
            get { return "Sets your character level to the chosen value"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            RPGPlayer character = caller.Player.GetModPlayer<RPGPlayer>(mod);
            int level = Int32.Parse(args[0]);
            level = Mathf.Clamp(level, 0, 9999);

            character.ResetSkillTree();

            for (int i = 0; i< level; i++)
            {
                character.commandLevelup();
            }

            for(int i = 0; i < 8; i++)
            {
                Main.NewText((Stat)i + " : " + character.GetStat((Stat)i), 255, 223, 63);
            }
            
            
        }
    }
        public class ResetCommand : ModCommand
        {
            public override CommandType Type
            {
                get { return CommandType.Chat; }
            }

            public override string Command
            {
                get { return "reset"; }
            }

            public override string Usage
            {
                get { return "/reset "; }
            }

            public override string Description
            {
                get { return "Reset your points"; }
            }

            public override void Action(CommandCaller caller, string input, string[] args)
            {
                RPGPlayer character = caller.Player.GetModPlayer<RPGPlayer>(mod);
                character.RecalculateStat();
            }
        }

    public class RarityReroll : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "rarity"; }
        }

        public override string Usage
        {
            get { return "/rarity"; }
        }

        public override string Description
        {
            get { return "Reroll the held item rarity taking as much coins as the goblin tinkerer"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            RPGPlayer character = player.GetModPlayer<RPGPlayer>(mod);
            
            ItemUpdate item = player.HeldItem.GetGlobalItem<ItemUpdate>();
            float itemvalue = player.HeldItem.value;
            int cost = Mathf.RoundInt((itemvalue * 0.33333f));
            AnotherRpgMod.Instance.Logger.Info(cost);

            if (player.CanBuyItem(cost))
            {
                player.BuyItem(cost);
                int plat = 0;
                int gold = 0; 
                int silv = 0; 
                int copp = 0; 

                int costbuffer = cost;
                if (costbuffer >= 1000000)
                {
                    plat = costbuffer / 1000000;
                    costbuffer = -plat * 1000000;
                }
                    
                if (costbuffer > 10000)
                {
                    gold = costbuffer / 10000;
                    costbuffer = -gold * 10000;
                }
                    
                if (costbuffer > 100)
                {
                    silv = costbuffer / 100;
                    costbuffer = -silv * 100;
                }

                if (costbuffer > 1)
                    copp = costbuffer;

                string coststring = "";

                if (plat > 0)
                {   
                    coststring += " " + plat + " " + Lang.inter[15].Value;
                }
                if (gold > 0)
                {
                    coststring += " " + gold + " " + Lang.inter[16].Value;
                }
                if (silv > 0)
                {
                    coststring += " " + silv + " " + Lang.inter[17].Value;
                }
                if (copp > 0)
                {
                    coststring += " " + copp + " " + Lang.inter[18].Value;
                }
                coststring += " used to reroll your item rarity";
                Main.NewText(coststring);

                item.Roll(caller.Player.HeldItem);
            }
            else
            {
                int plat = 0;
                int gold = 0;
                int silv = 0;
                int copp = 0;

                int costbuffer = cost;
                if (costbuffer >= 1000000)
                {
                    plat = costbuffer / 1000000;
                    costbuffer = -plat * 1000000;
                }

                if (costbuffer > 10000)
                {
                    gold = costbuffer / 10000;
                    costbuffer = -gold * 10000;
                }

                if (costbuffer > 100)
                {
                    silv = costbuffer / 100;
                    costbuffer = -silv * 100;
                }

                if (costbuffer > 1)
                    copp = costbuffer;

                string coststring = "need";

                if (plat > 0)
                {
                    coststring += " " + plat + " " + Lang.inter[15].Value;
                }
                if (gold > 0)
                {
                    coststring += " " + gold + " " + Lang.inter[16].Value;
                }
                if (silv > 0)
                {
                    coststring += " " + silv + " " + Lang.inter[17].Value;
                }
                if (copp > 0)
                {
                    coststring += " " + copp + " " + Lang.inter[18].Value;
                }
                coststring += " to reroll your item rarity";
                Main.NewText(coststring);
            }
        }
    }

    public class EvolutionReroll : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "itemtree"; }
        }

        public override string Usage
        {
            get { return "/itemtree"; }
        }

        public override string Description
        {
            get { return "Reroll the held item Evolution Tree for it's entire value of coins"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            RPGPlayer character = player.GetModPlayer<RPGPlayer>(mod);

            ItemUpdate item = player.HeldItem.GetGlobalItem<ItemUpdate>();
            int cost = player.HeldItem.value;
            if (player.CanBuyItem(cost))
            {
                player.BuyItem(cost);

                int plat = 0;
                int gold = 0;
                int silv = 0;
                int copp = 0;

                int costbuffer = cost;
                if (costbuffer >= 1000000)
                {
                    plat = costbuffer / 1000000;
                    costbuffer = -plat * 1000000;
                }

                if (costbuffer > 10000)
                {
                    gold = costbuffer / 10000;
                    costbuffer = -gold * 10000;
                }

                if (costbuffer > 100)
                {
                    silv = costbuffer / 100;
                    costbuffer = -silv * 100;
                }

                if (costbuffer > 1)
                    copp = costbuffer;

                string coststring = "";

                if (plat > 0)
                {
                    coststring += " " + plat + " " + Lang.inter[15].Value;
                }
                if (gold > 0)
                {
                    coststring += " " + gold + " " + Lang.inter[16].Value;
                }
                if (silv > 0)
                {
                    coststring += " " + silv + " " + Lang.inter[17].Value;
                }
                if (copp > 0)
                {
                    coststring += " " + copp + " " + Lang.inter[18].Value;
                }
                coststring += " used to reroll your item evolution tree";
                Main.NewText(coststring);

                item.CompleteReset();
            }
            else
            {
                int plat = 0;
                int gold = 0;
                int silv = 0;
                int copp = 0;

                int costbuffer = cost;
                if (costbuffer >= 1000000)
                {
                    plat = costbuffer / 1000000;
                    costbuffer = -plat * 1000000;
                }

                if (costbuffer > 10000)
                {
                    gold = costbuffer / 10000;
                    costbuffer = -gold * 10000;
                }

                if (costbuffer > 100)
                {
                    silv = costbuffer / 100;
                    costbuffer = -silv * 100;
                }

                if (costbuffer > 1)
                    copp = costbuffer;

                string coststring = "need";

                if (plat > 0)
                {
                    coststring += " " + plat + " " + Lang.inter[15].Value;
                }
                if (gold > 0)
                {
                    coststring += " " + gold + " " + Lang.inter[16].Value;
                }
                if (silv > 0)
                {
                    coststring += " " + silv + " " + Lang.inter[17].Value;
                }
                if (copp > 0)
                {
                    coststring += " " + copp + " " + Lang.inter[18].Value;
                }
                coststring += " to reroll your item evolution tree";
                Main.NewText(coststring);
            }
        }
    }
}
