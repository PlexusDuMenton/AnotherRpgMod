using System;

namespace AnotherRpgMod.RPGModule
{
    [Flags]
    public enum Perk // all togleable
    {
        None = 0x0,
        
        //Disabled for now, would be a bit hard to implement for now
        //ArchPriest = 0x1, // on death keep restore 1/25%/50%/75% health, give 1 second of damage immunity, and enter cd (600/450/300/150 seconds)
        
        
        BloodMage = 0x2, // 50/75/90% manacost reduction , lose health on manaUse
        Vampire = 0x4, //0.5/0.75/1% lifesteal , +25/50/100% stats at night, -50/25/10% stats at day)
        DemonEater = 0x8, //Each kill regen 5/10/15% of your max health
        Cupidon = 0x10, //each attack have 5/10/15/20/25 % chance to drop an heart
        StarGatherer = 0x20, //each attack have 5/10/15/20/25 % chance to drop an mana star
        Biologist = 0x40, // potion heal 20/40/60/80/100% more heal/mana
        Berserk = 0x80, //each percent of health missing increase your damage by 1/2/3% 
        Masochist = 0x100, //Set armor to 0, each points of armor increase you damage dealt by 1/2/3
        Survivalist = 0x200, // Reduce Damage by 50% to gain 20/40/60/80/100% of defense
        TheGambler = 0x400, // 50% chance for damage to heal , 50% chance for double damage
        Chlorophyll = 0x800, //50/75/100% more Life & mana Regen at day , +25/50/100% stats at day, -50/25/10% stats at night)

        ManaOverBurst = 0x1600, //Mana cost increase by 10/25/40% of your actual mana but increase the spell damage acordingly
    }
}

