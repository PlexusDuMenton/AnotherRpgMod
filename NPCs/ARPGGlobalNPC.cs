using System;
using Terraria;
using Terraria.ModLoader;
using AnotherRpgMod.Utils;

namespace AnotherRpgMod.RPGModule.Entities
{
    class Utils
    {

        public static int GetBaseLevel(NPC npc)
        {
            
            int baselevel = (int)(npc.lifeMax / 30 + Mathf.Pow(npc.damage * 0.3f, 1.1f) + Mathf.Pow(npc.defense, 1.2f));
            if (npc.boss)
            {
                float health = npc.lifeMax;
                if (npc.aiStyle == 6)
                    health = health * 0.2f;

                if (Main.expertMode)
                {
                    baselevel = (int)(npc.lifeMax / 150 + Mathf.Pow(npc.damage * 0.2f, 1.05f) + Mathf.Pow(npc.defense * 0.8f, 1.07f));
                }
                else
                {
                    baselevel = (int)(npc.lifeMax / 100 + Mathf.Pow(npc.damage * 0.2f, 1.03f) + Mathf.Pow(npc.defense * 0.8f, 1.05f));
                }
                
            }
            
            if (Main.expertMode)
            {
                float levelmultiplier = Mathf.Clamp(baselevel / 50, 0.5f, 1);
                baselevel = (int)(baselevel * 0.6f);
            }
            
            return baselevel;
        }

        public static int GetTier(NPC npc,int baselevel)
        {
            int BonusLevel = WorldManager.GetWorldAdditionalLevel();
            int randomlevel = Mathf.RandomInt(-1, 2 + (int)(baselevel * 0.1f));
            if (BonusLevel*2 > baselevel)
            {
                return randomlevel = Mathf.Clamp(randomlevel + BonusLevel, randomlevel,BonusLevel*2 - baselevel) ;
            }
            return randomlevel;
        }
        public static int GetExp(NPC npc)
        {
            return Mathf.CeilInt(npc.lifeMax / 20 + npc.damage/2 + npc.defense);
        }
    }



    class ARPGGlobalNPC : GlobalNPC
    {

        private bool StatsCreated = false;
        private int level;
        private int tier;
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        private void SyncNpc(NPC npc) // only sync tier since it's the only random value, rest is calculed on client
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SyncNPC);
                packet.Write(npc.whoAmI);
                packet.Write(tier);
                packet.Send();
            }
        }

        public override void PostAI(NPC npc)
        {
            if (npc.friendly) return;
            if (npc.townNPC) return;

            if (Main.netMode !=1) { 
                if (StatsCreated)
                    return;
                level = Utils.GetBaseLevel(npc);
                tier = Utils.GetTier(npc,level);
                npc.lifeMax = Mathf.FloorInt(npc.lifeMax * (1 + level * 0.25f + tier*0.4f));
                npc.life = npc.lifeMax;
                npc.damage = Mathf.FloorInt(npc.damage * (1 + level * 0.05f + tier * 0.08f));
                npc.defense = Mathf.FloorInt(npc.defense * (1 + level * 0.01f + tier * 0.025f));
                if (npc.GivenName == "")
                {
                    npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.TypeName);
                }
                else
                    npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.GivenName);

                SyncNpc(npc);

                StatsCreated = true;
            }
        }


        public override void NPCLoot(NPC npc)
        {
            if (npc.friendly) return;
            if (npc.townNPC) return;

            Player player = Array.Find(Main.player, p => p.active);
            if (Main.netMode == 0) player = Main.LocalPlayer; //if local , well it's simple

            else if (Main.player[npc.target].active)
                player = Main.player[npc.target];
            else
                return;
            int XPToDrop = Utils.GetExp(npc);
            if (npc.rarity > 0)
            {
                XPToDrop = (int)(XPToDrop * 3f);
            }
            if (npc.boss)
            {
                XPToDrop = XPToDrop * 10;
                WorldManager.OnBossDefeated(npc);
            }

            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AddXP);
                packet.Write(XPToDrop);
                packet.Write(level + tier);
                packet.Send();
            }
            else
                player.GetModPlayer<RPGPlayer>().AddXp(XPToDrop, level + tier);

        }
    }
}
