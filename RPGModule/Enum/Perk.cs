using System;

namespace AnotherRpgMod.RPGModule
{
    [Flags]
    public enum Perk // all togleable
    {
        None = 0x0,
        ArchPriest = 0x1, // on death keep restore 1/25%/50%/75% health, give 1 second of damage immunity, and enter cd (600/450/300/150 seconds)
        BloodMage = 0x2, // 50/75/90% manacost reduction , lose health on manaUse
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
}
