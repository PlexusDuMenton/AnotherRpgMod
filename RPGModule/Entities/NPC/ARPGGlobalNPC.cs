using AnotherRpgMod.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AnotherRpgMod.RPGModule.Entities
{
    class ARPGGlobalNPC : GlobalNPC
    {

        public bool StatsCreated = false;
        public bool DontCreateStats = false;
        public bool StatsFrame0 = false;
        public Dictionary<string, string> specialBuffer = new Dictionary<string, string>();
        private int level = -1;
        public int getLevel { get { return level; } }
        private int tier = 0;
        public int getTier { get { return tier; } }
        int baseDamage = 0;
        public float dontTakeDamageTime = 0;
        public float debuffDamage = 0;

        public int getRank { get { return (int)Rank; } }

        private NPCRank Rank = NPCRank.Normal;
        public NPCModifier modifier = 0;

        
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        private float VauraMemory = 0;

        public void ApplyCleave(NPC npc, int damage)
        {
            npc.life = Mathf.Clamp(npc.life - damage, 0, npc.lifeMax);
            Dust.NewDust(npc.Center - npc.Size * 0.5f, npc.width, npc.height, 90, 0f, 0f, 0, new Color(255, 50, 0), 0.1f);
            if (npc.life == 0)
                npc.checkDead();
        }

        public override void TownNPCAttackProj(NPC npc, ref int projType, ref int attackDelay)
        {
            base.TownNPCAttackProj(npc, ref projType, ref attackDelay);
        }
        public int ApplyVampricAura(NPC npc, float damage)
        {
            int value = 0;
            VauraMemory += damage;
            value = Mathf.FloorInt(VauraMemory);
            VauraMemory -= value;
            npc.life = Mathf.Clamp(npc.life-value, 0, npc.lifeMax);
            Dust.NewDust(npc.Center- npc.Size*0.5f, npc.width, npc.height, 90, 0f, 0f, 0, new Color(255, 50, 0), 0.1f);
            if (npc.life == 0)
                npc.checkDead();
            return value;
        }

        #region Buffers
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (HaveModifier(NPCModifier.Vampire))
            {
                npc.life = Mathf.Clamp(Mathf.CeilInt(damage * 0.5f), npc.life, npc.lifeMax);
            }

            if (HaveModifier(NPCModifier.ArmorBreaker))
            {
                int def = target.statDefense;
                if (damage < def)
                {
                    damage = Mathf.RoundInt(def + damage * 0.3f);
                }
                else if (damage > def + damage * 0.3f)
                {
                    damage += Mathf.RoundInt(def * 0.3f);
                }

            }
            
            base.ModifyHitPlayer(npc, target, ref damage, ref crit);
        }

        public string GetBufferProperty(string property)
            {
                try
                {
                    return specialBuffer[property];
                }
                catch (System.Exception exception)
                {
                AnotherRpgMod.Instance.Logger.Error(
                        "[" + this.GetType().FullName + "] GetBufferProperty(" +
                        property + "): " + exception.Message
                    );

                    return null;
                }
            }

            public bool HaveBufferProperty(string property)
            {
                if (specialBuffer.ContainsKey(property))
                    return true;
                return false;
            }

            public void SetBufferProperty(string property, string value)
            {
                if (specialBuffer.ContainsKey(property))
                    specialBuffer[property] = value;
                else
                    specialBuffer.Add(property, value);
            }


        #endregion


        public bool HaveModifier(NPCModifier _modifier)
        {
            return ((_modifier & modifier) == _modifier);
        }


        public void SetLevelTier(int level, int tier,byte rank)
        {
            this.level = level;
            this.tier = tier;
            
            Rank = (NPCRank)rank;
        }

        public void SetStats(NPC npc)
        {
            npc = NPCUtils.SetNPCStats(npc, level, tier,Rank);
            baseDamage = npc.damage;
            
        }
        public void SetInit(NPC npc)
        {
            
            if (Main.netMode == 2)
            {
                Main.npcLifeBytes[npc.type] = 4; //Sadly have to UN-optimise Health of ennemy, because it caused npc to dispear if health was over 128 (for small )
            }
            

            if (Main.netMode != 1)
            {
                if (level < 0) {
                    if (!Config.gpConfig.NPCProgress)
                    {
                        level = 0;
                        tier = 0;
                    }
                    else { 
                        level = Mathf.CeilInt(NPCUtils.GetBaseLevel(npc) * Config.gpConfig.NpclevelMultiplier);
                        if (npc.townNPC || (npc.damage == 0))
                            tier = Mathf.CeilInt(NPCUtils.GetTierAlly(npc, level) * Config.gpConfig.NpclevelMultiplier);
                        else if (Config.gpConfig.NPCProgress)
                            tier = Mathf.CeilInt(NPCUtils.GetTier(npc, level) * Config.gpConfig.NpclevelMultiplier);
                    }
                    if (!npc.townNPC && !(npc.damage == 0) && (!npc.dontCountMe)) { 
                        Rank = NPCUtils.GetRank(level+tier);
                        modifier = NPCUtils.GetModifier(Rank,npc);
                        if (HaveModifier(NPCModifier.Size))
                        {
                            int maxrng = Main.expertMode ? 50 : 70;
                            int rn = Mathf.RandomInt(0, maxrng);
                            if (npc.boss)
                                rn+=1;
                            if (rn < 1)
                            {
                                SetBufferProperty("size", "Growther");
                                SetBufferProperty("GrowtherStep", "Mini");
                            }
                            else if (rn < 3)
                                SetBufferProperty("size", "Titan");
                            else if (rn < 6)
                                SetBufferProperty("size", "Colossus");
                            else if (rn < 20)
                                SetBufferProperty("size", "Giant");
                            else
                                SetBufferProperty("size", "Mini");
                        }
                    }
                }
            }
        }
        private void Effect(NPC npc)
        {
            if (HaveModifier(NPCModifier.Golden))
            {
                Dust.NewDust(npc.Center - npc.Size * 0.5f, npc.width, npc.height, 244, 0f, 0f, 0, new Color(255, 255, 255), 0.05f);
                Lighting.AddLight(npc.Center, 1f, .8f, .5f);
            }
                

            if (HaveModifier(NPCModifier.Berserker))
            {
                Dust.NewDust(npc.Center - npc.Size * 0.5f, npc.width, npc.height, 90, 0f, 0f, 0, new Color(255, 255, 255), 0.5f); 
                Lighting.AddLight(npc.Center, 0.3f, .0f, .0f);
            }

            if (HaveModifier(NPCModifier.Cluster)){
                Dust.NewDust(npc.Center - npc.Size * 0.5f, npc.width, npc.height, 91, 0f, 0f, 0, new Color(255, 255, 255), 0.5f);
                Lighting.AddLight(npc.Center, 0.2f, .1f, .3f);
            }

            if (npc.HasBuff(Terraria.ID.BuffID.Venom))
            {
                int num41 = Dust.NewDust(npc.position, npc.width, npc.height, 46, 0f, 0f, 120, default(Color), 0.2f);
                Main.dust[num41].noGravity = true;
                Main.dust[num41].fadeIn = 1.9f;
            }
        }

        public override void PostAI(NPC npc)
        {

            Effect(npc);
            if (npc.dontTakeDamage && dontTakeDamageTime > 0)
            {
                dontTakeDamageTime -= NPCUtils.DELTATIME;
                if (dontTakeDamageTime <= 0) { 
                    npc.dontTakeDamage = false;
                    dontTakeDamageTime = 0;
                }
            }

            if (!StatsCreated && Main.netMode != 1) {
                StatsCreated = true;
                SetInit(npc);
                SetStats(npc);
                //MPDebug.Log(mod,"Server Side : \n"+ npc.GetGivenOrTypeNetName()+ " ID : "+ npc.whoAmI + "\nLvl."+ (getLevel+getTier)+"\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + getRank + "\n");
                MPPacketHandler.SendNpcSpawn(mod, npc, tier, level, this);
                //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);

                npc.GivenName = NPCUtils.GetNpcNameChange(npc, tier, level, Rank);
            }

            if (Main.netMode == 1)
            {
                if (!StatsFrame0)
                {
                    StatsFrame0 = true;
                }

                else if(!StatsCreated)
                {
                    StatsCreated = true;
                    MPPacketHandler.AskNpcInfo(mod, npc);
                }
            }



            if (Main.netMode != 1)
            {
                
                debuffDamage += NPCUtils.UpdateDOT(npc);
                int applydamage = 0;
                
                if (debuffDamage > 0) { 
                    applydamage = Mathf.FloorInt(debuffDamage);
                    debuffDamage -= applydamage;
                    npc.life -= applydamage;
                    npc.lifeRegen -= applydamage;
                    if (npc.life <= 0)
                        npc.checkDead();
                }


                if (HaveModifier(NPCModifier.Berserker))
                {
                    npc.damage = Mathf.RoundInt((float)baseDamage * (2-((float)npc.life/ (float)npc.lifeMax)));
                }

                //MPPacketHandler.SendNpcUpdate(mod,npc);
                
            }

        }
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            
            base.OnHitByItem(npc, player, item, damage, knockback, crit);
            MPPacketHandler.SendNpcUpdate(mod, npc);
            //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            MPPacketHandler.SendNpcUpdate(mod, npc);
            //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
        }

        public override bool CheckDead(NPC npc)
        {
            if (HaveBufferProperty("GrowtherStep"))
            {
                switch (GetBufferProperty("GrowtherStep"))
                {
                    case "Mini":
                        SetBufferProperty("GrowtherStep", "Normal");
                        break;
                    case "Normal":
                        SetBufferProperty("GrowtherStep", "Giant");
                        break;
                    case "Giant":
                        SetBufferProperty("GrowtherStep", "Colossus");
                        break;
                    case "Colossus":
                        SetBufferProperty("GrowtherStep", "Titan");
                        break;
                    case "Titan":
                        return false;
                }
                npc = NPCUtils.SizeShiftMult(npc, GetBufferProperty("GrowtherStep"));
                npc.life = npc.lifeMax;
                MPPacketHandler.SendNpcUpdate(mod, npc);
                //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
            }
            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            
            if (HaveModifier(NPCModifier.Cluster) && !HaveBufferProperty("clustered"))
            {
                int clusterRN = Mathf.RandomInt(4, 8);
                for (int i = 0; i < clusterRN; i++)
                    NPCUtils.SpawnSized(npc, specialBuffer);

            }

            if (npc.damage == 0) return;
            if (npc.townNPC) return;

            Player player = Array.Find(Main.player, p => p.active);
            if (Main.netMode == 0)
                player = Main.LocalPlayer; //if local , well it's simple
            else if (Main.player[npc.target].active)
                player = Main.player[npc.target];
            else
                return;
            int XPToDrop = NPCUtils.GetExp(npc);
            if (npc.rarity > 0)
            {
                XPToDrop = (int)(XPToDrop * 1.5f);
            }
            if (npc.boss)
            {
                XPToDrop = XPToDrop * 2;
                WorldManager.OnBossDefeated(npc);
            }

            XPToDrop = Mathf.CeilInt(XPToDrop * Config.gpConfig.XpMultiplier);

            int xplevel = level + tier;
            if (!Config.gpConfig.XPReduction)
            {
                xplevel = int.MaxValue;
            }
            MPPacketHandler.SendXPPacket(mod, XPToDrop, xplevel);
            if (Main.netMode == 0)
                player.GetModPlayer<RPGPlayer>().AddXp(XPToDrop, xplevel);
           

        }
    }
}
