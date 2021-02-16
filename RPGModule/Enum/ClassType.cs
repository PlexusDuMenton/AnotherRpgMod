using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherRpgMod.RPGModule
{
    public enum ClassType
    {
        Hobo,
        //Tier 0 
        Tourist, // base class + 5% all damage 
        //Tier 1 
        Apprentice, // upgrade to tourist , + 15% all damage , +5% health
        Archer, // +20% ranged damage , 5% dodge + 30% bow damage
        Gunner, // +20% ranged damage , 20% chance not to consume ammo + 30% gun damage
        SwordMan, // + 75% melee damage , + 10% melee speed , -20% health
        Spiritualist, // + 50% summon damage , + 1 summon 
        Mage, // +50% magic damage , 20% manacost reduction - 25% health
        Ninja, //+50% throw damage , 20% not to consume ammo 
        Acolyte, // 30% damage to mana (10(+int/50) damage per mana) + 25% magic damage + 10% health
        Cavalier, //+ 20% melee damage , + 50% health ,+ 20% armor , -5% movement speed

        //Tier 2 
        Regular, //Upgrade to Apprentice
        Hunter,
        Gunslinger,
        Mercenary,
        Invoker,
        ArchMage,
        Shinobi,
        Monk,
        Knight,

        //Tier 3
        Expert,
        Ranger,
        Spitfire,
        SwordMaster,
        Summoner,
        Arcanist,
        Rogue,
        Templar,
        IronKnight,

        //Tier 4
        Master,
        Marksman,
        Sniper,
        Champion,
        SoulBinder,
        Warlock,
        Assassin,
        Paladin,
        Montain,
        //Tier 5

        PerfectBeing,

        WindWalker,
        Hitman,
        SwordSaint,
        SoulLord,
        Mystic,
        ShadowDancer,
        Deity,
        Fortress,

        //Tier 6
        Ascended, //Perfect Class
        AscendedWindWalker,
        AscendedHitman,
        AscendedSwordSaint,
        AscendedSoulLord,
        AscendedMystic,
        AscendedShadowDancer,
        AscendedDeity,
        AscendedFortress,

        /*
        //Hybrid
        ManaBinder, // T3 Summon & Magic damage , summon gain damage from int and foc
        ManaShaper, // T4
        ManaOverLord, //T5

        Shepherd, // Summon and Ranged , summon gain damage from dex and agi
        Druid, // T4
        Protector, // T5

        
        MagicSwordMan,//Magic and Melee (int & foc would increase melee damage)

        Bandit, // Melee and throw

        Miner, // Low Damage, Huge Health and mana, Low/Verylow Knockback, Melee speed bonus
        */

    }
}
