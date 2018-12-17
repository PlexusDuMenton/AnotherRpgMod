using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherRpgMod.RPGModule
{
    public enum ClassType
    {
        //Tier 0 
        Tourist, // base class + 5% all damage 
        //Tier 1 -lvl 15+ (should be arround EoC)
        Apprentice, // upgrade to tourist , + 15% all damage , +5% health
        Archer, // +20% ranged damage , 5% dodge + 30% bow damage
        Gunner, // +20% ranged damage , 20% chance not to consume ammo + 30% gun damage
        SwordMan, // + 75% melee damage , + 10% melee speed , -20% health
        Spiritualist, // + 50% summon damage , + 1 summon 
        Mage, // +50% magic damage , 20% manacost reduction - 25% health
        Ninja, //+50% throw damage , 20% not to consume ammo 
        Acolyte, // 30% damage to mana (10(+int/50) damage per mana) + 25% magic damage + 10% health
        Cavalier, //+ 20% melee damage , + 50% health ,+ 20% armor , -5% movement speed
        //Tier 1 -lvl 15+ (should be arround EoC)
        Regular, // upgrade to tourist , + 15% all damage , +5% health
        Hunter, // +20% ranged damage , 5% dodge + 30% bow damage
        Gunslinger, // +20% ranged damage , 20% chance not to consume ammo + 30% gun damage
        Mercenary, // + 75% melee damage , + 10% melee speed , -20% health
        Invoker, // + 50% summon damage , + 1 summon 
        ArchMage, // +50% magic damage , 20% manacost reduction - 25% health
        Shinobi, //+50% throw damage , 20% not to consume ammo 
        Templar, // 30% damage to mana (10(+int/50) damage per mana) + 25% magic damage + 10% health
        Knight, //+ 20% melee damage , + 50% health ,+ 20% armor , -5% movement speed



    }
}
