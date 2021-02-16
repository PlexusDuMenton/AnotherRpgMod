
namespace AnotherRpgMod.RPGModule.Entities
{
    public enum NPCRank
    {
        Weak = 0, // -50% stats,
        Normal = 1, // normal stats
        Alpha = 2, // +40% hp , 15% damage, 10% def
        Elite = 3, // + 80% hp, 40% damage, 20% def
        Legendary = 4, //+ 200% hp, 60% damage, 40% def
        Mythical = 5, // + 600 %health, 100% damage, 60% def
        Godly = 6, // + 1400% health; 200% damage, 80% def
        DIO = 7, // All modifier, + 2400% hp, + 400 damage + 100%def
        LimitBreaked = 8,
        Raised = 9,
        Ascended = 10,
        HighAscended = 11,
        PeakAscended = 12,
        Transcendental = 13,
        TransDimensional = 14,
        DioAboveHeaven = 999
    }
}
