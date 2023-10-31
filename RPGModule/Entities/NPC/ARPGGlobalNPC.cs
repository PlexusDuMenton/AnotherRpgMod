using AnotherRpgMod.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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

        int updateDataFrame = 0;
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
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {

            float damage = modifiers.FinalDamage.Additive * modifiers.FinalDamage.Multiplicative;

            if (HaveModifier(NPCModifier.Vampire))
            {
                npc.HealEffect(Mathf.RoundInt(damage * 0.5f), true);
            }

            if (HaveModifier(NPCModifier.ArmorBreaker))
            {
                float def = target.statDefense;

                float mult = 0.5f;
                if (Main.expertMode)
                    mult = 0.75f;

                modifiers.ArmorPenetration += Mathf.RoundInt(def * 0.3f * mult);

            }
            
            base.ModifyHitPlayer(npc, target, ref modifiers);
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
            
            if (Main.netMode == NetmodeID.Server)
            {
                Main.npcLifeBytes[npc.type] = 4; //Sadly have to UN-optimise Health of ennemy, because it caused npc to dispear if health was over 128 (for small )
            }
            
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (level < 0) {
                    if (!Config.NPCConfig.NPCProgress)
                    {
                        level = 0;
                        tier = 0;
                    }
                    else { 
                        level = Mathf.CeilInt(NPCUtils.GetBaseLevel(npc) * Config.NPCConfig.NpclevelMultiplier);
                        if (npc.townNPC || (npc.damage == 0))
                            tier = Mathf.CeilInt(NPCUtils.GetTierAlly(npc, level) * Config.NPCConfig.NpclevelMultiplier);
                        else if (Config.NPCConfig.NPCProgress)
                            tier = Mathf.CeilInt(NPCUtils.GetTier(npc, level) * Config.NPCConfig.NpclevelMultiplier);
                    }
                    if (!npc.townNPC && !(npc.damage == 0) && (!npc.dontCountMe)) { 
                        Rank = NPCUtils.GetRank(level+tier,npc.boss);
                        modifier = NPCUtils.GetModifier(Rank,npc);
                        if (HaveModifier(NPCModifier.Size))
                        {
                            int maxrng = Main.expertMode ? 50 : 70;
                            int rn = Mathf.RandomInt(0, maxrng);
                            if (npc.boss)
                                rn+=1;
                            /*
                            if (rn < 1)
                            {
                                SetBufferProperty("size", "Growther");
                                SetBufferProperty("GrowtherStep", "Mini");
                            }
                            */
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
                int num41 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Poisoned, 0f, 0f, 120, default(Color), 0.2f);
                Main.dust[num41].noGravity = true;
                Main.dust[num41].fadeIn = 1.9f;
            }
        }


        public override void PostAI(NPC npc)
        {

            base.PostAI(npc);

            Effect(npc);
            if (npc.dontTakeDamage && dontTakeDamageTime > 0)
            {
                dontTakeDamageTime -= NPCUtils.DELTATIME;
                if (dontTakeDamageTime <= 0) { 
                    npc.dontTakeDamage = false;
                    dontTakeDamageTime = 0;
                }
            }


            if (!StatsCreated && Main.netMode != NetmodeID.MultiplayerClient)
            {
                StatsCreated = true;
                SetInit(npc);
                SetStats(npc);
                //MPDebug.Log(mod,"Server Side : \n"+ npc.GetGivenOrTypeNetName()+ " ID : "+ npc.whoAmI + "\nLvl."+ (getLevel+getTier)+"\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + getRank + "\n");
                MPPacketHandler.SendNpcSpawn(Mod,-1, npc, tier, level, this);
                //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
                npc.GivenName = NPCUtils.GetNpcNameChange(npc, tier, level, Rank);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (!StatsFrame0)
                {
                    StatsFrame0 = true;
                }

                else if (!StatsCreated)
                {
                    MPPacketHandler.AskNpcInfo(Mod, npc, Main.myPlayer);
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                
                return;
            }
                
            debuffDamage += NPCUtils.UpdateDOT(npc);
                
            if (debuffDamage > 0) {
                int applydamage = Mathf.FloorInt(debuffDamage);
                debuffDamage -= applydamage;
                npc.life -= applydamage;
                npc.lifeRegen -= applydamage;
                if (npc.life <= 0)
                    npc.checkDead();
            }


            if (HaveModifier(NPCModifier.Berserker))
            {
                npc.damage = Mathf.RoundInt(baseDamage * (2-((float)npc.life/ (float)npc.lifeMax)));
            }

            if (Main.netMode == NetmodeID.Server)
            {
                if (updateDataFrame <= 0)
                {
                    MPPacketHandler.SendNpcUpdate(Mod, npc);
                    updateDataFrame = 32;
                }
                    
                else
                {
                    updateDataFrame--;
                }
            }

            if (npc.life < 0)
                npc.life = 0;
            


            
        }


        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            
            if (HaveModifier(NPCModifier.Dancer))
            {
                if (Mathf.Random(0, 1) < 0.2f)
                {
                    hit.Damage *= 0;
                    damageDone = 0;
                    SoundEngine.PlaySound(SoundID.DoubleJump, npc.position);
                }
            }

            base.OnHitByItem(npc, player, item, hit, damageDone);
            //MPPacketHandler.SendNpcUpdate(Mod, npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (HaveModifier(NPCModifier.Dancer))
            {
                if (Mathf.Random(0, 1) < 0.2f)
                {
                    hit.Damage *= 0;
                    damageDone = 0;
                    SoundEngine.PlaySound(SoundID.DoubleJump, npc.position);
                }
            }
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
            //MPPacketHandler.SendNpcUpdate(Mod, npc);
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
                MPPacketHandler.SendNpcUpdate(Mod, npc);
                return false;
                //NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
            }
            return true;
        }
        public override void OnKill(NPC npc)
        {
            if (npc.townNPC || npc.damage == 0)
            {
                base.OnKill(npc);
                return;
            }

            Player Player = null;
            if (Main.netMode == NetmodeID.SinglePlayer)
                Player = Main.LocalPlayer;
            else if (Main.player[npc.target].active)
                Player = Main.player[npc.target];


            if (Player is not null)
            {
                RPGPlayer rpgPlayer;
                if (!Player.TryGetModPlayer(out rpgPlayer))
                {
                    base.OnKill(npc);
                    return;
                }
            }

            if (HaveModifier(NPCModifier.Cluster) && !HaveBufferProperty("clustered"))
            {
                
                int clusterRN = Mathf.RandomInt(4, 8);
                for (int i = 0; i < clusterRN; i++)

                    NPCUtils.SpawnSized(npc, NPC.GetSource_NaturalSpawn(), specialBuffer);
            }

            
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

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Player.GetModPlayer<RPGPlayer>().AddXp(XPToDrop, xplevel);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                MPPacketHandler.SendXPPacket(Mod, XPToDrop, xplevel);
            }

            base.OnKill(npc);
        }


    }
}
