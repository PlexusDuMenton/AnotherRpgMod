using System;

namespace AnotherRpgMod.Items
{
    [Flags]
    public enum Modifier : int
    {
        None = 0x0, //None
        MoonLight = 0x1, //Increase Damage NightTime
        SunLight = 0x2, //Increase Damage DayTime
        Berserker = 0x4, //Increase Damage As Health Is Down
        MagicConnection = 0x8, //Increase Damage with Higger Mana
        Sniper = 0x10, //Increase Damage with Distance
        Brawler = 0x20, //Increase Damage with how close you are
        Piercing = 0x40, //Ignore Part of defense (flat ammounts of defense is ignored, increase with level)
        Savior = 0x80, //Save you when you are about to die (CD reduce with level)
        FireLord = 0x100, //Ennemies Arround you turn on fire (range increase with level)
        Thorny = 0x200, //Reflect Damages Percent
        Smart = 0x400, //Increase XP Gain
        SelfLearning = 0x800, //Increase Weapon XP Gain
        VampiricAura = 0x1000, //Passively steal Health from nearby enemies
        Executor = 0x2000, //Increase Damage as Ennemy is wounder
        Confusion = 0x4000, //Chance to Confuse Ennemy on hit
        Poisones = 0x8000, //Chance to Poison Ennemy on hit
        Venom = 0x10000, //Increase Damage Against poisoned ennemies
        Chaotic = 0x20000, //Damage Per Player Debuff
        Cunning = 0x40000, //Damage Per ennemy Debuff
        BloodSeeker = 0x80000, //Increase Damage and lifeSteal against wounder ennemies
        Cleave = 0x100000, //AOE damage
        Random = 0x200000, //Random Projectile every shoot
    }
}
