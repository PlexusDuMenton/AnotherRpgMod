using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.RPGModule;
using AnotherRpgMod.Utils;
using AnotherRpgMod.Items;

namespace AnotherRpgMod.Command
{
    public class SetLevel : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "setlevel"; }
        }

        public override string Usage
        {
            get { return "/setlevel <level>"; }
        }

        public override string Description
        {
            get { return "Sets your character level to the chosen value"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            RPGPlayer character = caller.Player.GetModPlayer<RPGPlayer>();
            int level = Int32.Parse(args[0]) - 1;
            level = Mathf.Clamp(level, 0, 9999);

            character.ResetLevel();
            WorldManager.PlayerLevel = level;


            for (int i = 0; i < level; i++)
            {
                character.commandLevelup();
            }
        }
    }

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
            get { return "Levelup X time"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            RPGPlayer character = caller.Player.GetModPlayer<RPGPlayer>();
            int level = Int32.Parse(args[0]);
            level = Mathf.Clamp(level, 0, 9999);

            character.ResetSkillTree();

            for (int i = 0; i< level; i++)
            {
                character.commandLevelup();
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
                RPGPlayer character = caller.Player.GetModPlayer<RPGPlayer>();
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
            Player Player = caller.Player;
            RPGPlayer character = Player.GetModPlayer<RPGPlayer>();
            
            ItemUpdate item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            float itemvalue = Player.HeldItem.value;
            int cost = Mathf.RoundInt((itemvalue * 0.33333f));

            if (Player.CanAfford(cost))
            {
                Player.BuyItem(cost);
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
            Player Player = caller.Player;
            RPGPlayer character = Player.GetModPlayer<RPGPlayer>();

            ItemUpdate item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            int cost = Player.HeldItem.value;
            if (Player.CanAfford(cost))
            {
                Player.BuyItem(cost);

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




    public class ItemName : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "iname"; }
        }

        public override string Usage
        {
            get { return "/iname <slot>"; }
        }

        public override string Description
        {
            get { return "Get Item Name from slot (to confirm XP transfer)"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player Player = caller.Player;
            RPGPlayer character = Player.GetModPlayer<RPGPlayer>();
            if (args.Length == 0)
            {
                Main.NewText(Description);
                return;
            }
            if (int.TryParse(args[0], out int slot) == false)
            {
                Main.NewText("Slot Number invalid");
                return;
            }
            ItemUpdate item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            ItemUpdate Source = Player.inventory[slot].GetGlobalItem<ItemUpdate>();
            Main.NewText(Player.inventory[slot].Name);


        }
    }

    public class XpTransfer : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "xpt"; }
        }

        public override string Usage
        {
            get { return "/xpt <slot>"; }
        }

        public override string Description
        {
            get { return "eXPerience Transfer from the slot to the held item, have 75% loss"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player Player = caller.Player;
            RPGPlayer character = Player.GetModPlayer<RPGPlayer>();
            if (args.Length == 0)
            {
                Main.NewText(Description);
                return;
            }
            if (int.TryParse(args[0], out int slot) == false) { 
                Main.NewText("Slot Number invalid");
                return;
            }
            ItemUpdate item = Player.HeldItem.GetGlobalItem<ItemUpdate>();
            ItemUpdate Source = Player.inventory[slot].GetGlobalItem<ItemUpdate>();

            if (item == Source)
            {
                Main.NewText("Slot number And Held Items are the same");
                return;
            }


            AnotherRpgMod.source = Source;
            AnotherRpgMod.Transfer = item;
            AnotherRpgMod.XPTvalueA = ItemExtraction.GetTotalEarnedXp(Source);
            AnotherRpgMod.XPTvalueB = ItemExtraction.GetTotalEarnedXp(item);

            float xp = ItemExtraction.GetExtractedXp(false, Source);

            Main.NewText("Transfering "+ xp + " exp from " + Player.inventory[slot].Name + " to " + Player.HeldItem.Name);

            Source.ResetLevelXp();
            item.xPTransfer(xp,Player,Player.HeldItem);

        }
    }

    public class UndoXpTransfer : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "undoxpt"; }
        }

        public override string Usage
        {
            get { return "/undoxpt"; }
        }

        public override string Description
        {
            get { return "Undo The last Exp Transfer (I know you naughty boi would make a mistake ;))"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player Player = caller.Player;
            RPGPlayer character = Player.GetModPlayer<RPGPlayer>();
            if (AnotherRpgMod.source == null)
                return;
            if (AnotherRpgMod.Transfer == null)
                return;

            AnotherRpgMod.source.ResetLevelXp(false);
            AnotherRpgMod.Transfer.ResetLevelXp(false);
            AnotherRpgMod.source.SilentxPTransfer(AnotherRpgMod.XPTvalueA);
            AnotherRpgMod.Transfer.SilentxPTransfer(AnotherRpgMod.XPTvalueB);

            AnotherRpgMod.source = null;
            AnotherRpgMod.Transfer = null;
            AnotherRpgMod.XPTvalueA = 0;
            AnotherRpgMod.XPTvalueB = 0;


        }
    }


    public class WorldLevel : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "WorldRPGInfo"; }
        }

        public override string Usage
        {
            get { return "/WorldRPGInfo"; }
        }

        public override string Description
        {
            get { return "Debug print the world info"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("is world ascended : " + WorldManager.ascended);
            if (WorldManager.ascended)
                Main.NewText("world ascend level : " + WorldManager.ascendedLevelBonus);

            Main.NewText("Number of boss defeated : " + WorldManager.BossDefeated);
            Main.NewText("Number of boss defeated for the first time : " + WorldManager.FirstBossDefeated);
            Main.NewText("Is hardmode ? : " + Main.hardMode);
            Main.NewText("Day : " + WorldManager.Day);
            Main.NewText("Player Level : " + WorldManager.PlayerLevel);
            Main.NewText("Additional NPC Level : " + WorldManager.GetWorldAdditionalLevel());
        }
    }

    public class AscentWorld : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "Ascend"; }
        }

        public override string Usage
        {
            get { return "/Ascend <level>"; }
        }

        public override string Description
        {
            get { return "Ascend world, and add X level to the world"; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            WorldManager.ascended = true;
            int level = Int32.Parse(args[0]);
            if (level < 250) level = 250;
            WorldManager.ascendedLevelBonus = level;

        }
    }

}
