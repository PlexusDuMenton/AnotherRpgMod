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
namespace AnotherRpgMod.Command
{
    /*   
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
            level = NPCUtils.Mathf.Clamp(level, 0, 2500);
            for (int i = 0; i< level; i++)
            {
                character.commandLevelup();
            }

            for(int i = 0; i < 8; i++)
            {
                Main.NewText((Stat)i + " : " + character.GetStat((Stat)i), 255, 223, 63);
            }
            Main.NewText(WorldManager.BossDefeated + " original boss degeated", 255, 223, 63);
            
            
        }
    }*/
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
}
