using System;

namespace AnotherRpgMod.RPGModule.Entities
{
    [Flags]
    public enum NPCModifier : int
    {
        None = 0x0,
        Cluster = 0x1, // On death spawn several more of himself
        Size = 0x2, //Changed size and stats
        Berserker = 0x4, //Lower the health, higger the damage, up to 100%
        Golden = 0x8, // + 200% damage, +50% damage, + 50% def, drop way more money than usual
        Vampire = 0x10, // Heal itself on damage
        ArmorBreaker = 0x20, //Ignore 30% of armor
        Dancer = 0x40, //25% to dodge
    }
}
