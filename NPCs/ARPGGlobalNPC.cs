using System;
using Terraria;
using Terraria.ModLoader;
using AnotherRpgMod.Utils;
using Microsoft.Xna.Framework;
namespace AnotherRpgMod.RPGModule.Entities
{
    class Utils
    {

        public static int GetBaseLevel(NPC npc)
        {
            
            int baselevel = (int)(npc.lifeMax / 20 + Mathf.Pow(npc.damage * 0.3f, 1.1f) + Mathf.Pow(npc.defense, 1.2f));
            if (npc.boss)
            {
                float health = npc.lifeMax;
                if (npc.aiStyle == 6)
                    health = health * 0.4f;

                if (Main.expertMode)
                {
                    baselevel = (int)(npc.lifeMax / 120 + Mathf.Pow(npc.damage * 0.33f, 1.05f) + Mathf.Pow(npc.defense * 0.8f, 1.07f));
                }
                else
                {
                    baselevel = (int)(npc.lifeMax / 85 + Mathf.Pow(npc.damage * 0.33f, 1.04f) + Mathf.Pow(npc.defense * 0.8f, 1.05f));
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

            int randomlevel = Mathf.RandomInt(-1, 3);
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
        private int basehealth = 0;
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
                packet.Write(tier+level);
                packet.Write(npc.lifeMax);
                packet.Write(npc.damage);
                packet.Write(npc.defense);
                packet.Send();
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.friendly) return;
            if (npc.townNPC) return;
            if (Main.netMode != 1)
            {
                level = Mathf.CeilInt(Utils.GetBaseLevel(npc) * ConfigFile.GetConfig.NpclevelMultiplier);
                if (ConfigFile.GetConfig.NpcProgress)
                    tier = Mathf.CeilInt(Utils.GetTier(npc, level) * ConfigFile.GetConfig.NpclevelMultiplier);

                if (npc.boss)
                {
                    npc.lifeMax = Mathf.FloorInt(npc.lifeMax * (1 + level * 0.5f + tier * 0.8f));
                }
                else
                    npc.lifeMax = Mathf.FloorInt(npc.lifeMax * (1 + level * 0.23f + tier * 0.3f));

                npc.damage = Mathf.FloorInt(npc.damage * (1 + level * 0.04f + tier * 0.05f));
                npc.defense = Mathf.FloorInt(npc.defense * (1 + level * 0.01f + tier * 0.018f));
            }
            base.SetDefaults(npc);
        }
        public override void PostAI(NPC npc)
        {
            if (npc.friendly) return;
            if (npc.townNPC) return;
            if (Main.netMode != 1) { 
                if (StatsCreated == false) {
                    StatsCreated = true;
                    npc.life = npc.lifeMax;

                    if (npc.GivenName == "")
                    {
                        npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.TypeName);
                    }
                    else
                        npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.GivenName);
                    SyncNpc(npc);
                }
            }
        }
        /*
        public override void PostAI(NPC npc)
        {


            if (npc.friendly) return;
            if (npc.townNPC) return;

                if (StatsCreated)
                    return;
                level = Mathf.CeilInt( Utils.GetBaseLevel(npc)*ConfigFile.GetConfig.NpclevelMultiplier);
                if (ConfigFile.GetConfig.NpcProgress)
                    tier = Mathf.CeilInt(Utils.GetTier(npc, level) * ConfigFile.GetConfig.NpclevelMultiplier);
                basehealth = npc.life;
                npc.lifeMax = Mathf.FloorInt(npc.lifeMax * (1 + level * 0.25f + tier * 0.4f));
                npc.life = Mathf.Clamp(Mathf.FloorInt(npc.life * (1 + level * 0.25f + tier * 0.4f)),0, basehealth*5);
                npc.damage = Mathf.FloorInt(npc.damage * (1 + level * 0.05f + tier * 0.08f));
                npc.defense = Mathf.FloorInt(npc.defense * (1 + level * 0.01f + tier * 0.025f));
                if (npc.GivenName == "")
                {
                    npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.TypeName);
                }
                else
                    npc.GivenName = ("Lvl. " + (level + tier) + " " + npc.GivenName);

                //SyncNpc(npc);

                StatsCreated = true;

        }
        */

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
                XPToDrop = (int)(XPToDrop * 1.5f);
            }
            if (npc.GivenName.Contains("godly")|| npc.GivenName.Contains("Legendary"))
            {
                XPToDrop = XPToDrop * 2;
            }
            if (npc.boss)
            {
                XPToDrop = XPToDrop * 2;
                WorldManager.OnBossDefeated(npc);
            }

            XPToDrop = Mathf.CeilInt(XPToDrop * ConfigFile.GetConfig.XpMultiplier);

            int xplevel = level + tier;
            if (!ConfigFile.GetConfig.XPReduction)
            {
                xplevel = 99999;
            }
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AddXP);
                packet.Write(XPToDrop);
                packet.Write(xplevel);
                packet.Send();
            }
            else
                player.GetModPlayer<RPGPlayer>().AddXp(XPToDrop, xplevel);

        }
    }
}
