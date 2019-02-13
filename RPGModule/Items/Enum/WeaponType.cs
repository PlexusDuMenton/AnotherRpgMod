using System;
namespace AnotherRpgMod.Items
{
    [Flags]
    public enum WeaponType : int
    {
        NONE = 0,
        Stab = 0x1,
        Swing = 0x2,
        Spear = 0x4,
        Melee = 0x7,
        OtherMelee = 0x8,
        ExtendedMelee = 0xF,
        Throw = 0x10,
        Bow = 0x20,
        Gun = 0x40,
        OtherRanged = 0x80,
        Ranged = 0xF0,
        Magic = 0x100,
        Summon = 0x200,
        Other = 0x400,

    }
}
