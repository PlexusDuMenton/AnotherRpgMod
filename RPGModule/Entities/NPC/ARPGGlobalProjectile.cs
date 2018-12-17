using System;
using Terraria;
using Terraria.ModLoader;
using AnotherRpgMod.Utils;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AnotherRpgMod.RPGModule.Entities
{
    class ARPGGlobalProjectile : GlobalProjectile
    {
        bool init = false;

        public Item itemOrigin;

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if (projectile.owner > Main.npc.Length)
                return;
            NPC owner = Main.npc[projectile.owner];
            if (owner.GivenName == "")
                return;

            damage = owner.damage;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {

            if (projectile.npcProj)
            {
                if (projectile.owner > Main.npc.Length)
                    return;
                NPC owner = Main.npc[projectile.owner];
                if (owner.GivenName == "")
                    return;

                damage = owner.damage;
            }

        }


        public override void AI(Projectile projectile)
        {


            if (init)
                return;
            if (projectile.friendly)
                return;

            if (!projectile.npcProj && projectile.minion)
            {
                Player p = Main.player[projectile.owner];
                itemOrigin = p.HeldItem;
            }
            init = true;

        }
    }
}
