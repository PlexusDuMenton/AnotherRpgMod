using System;
using Terraria;
using Terraria.ModLoader;
using AnotherRpgMod.Utils;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        DIO = 7 // All modifier, + 2400% hp, + 400 damage + 100%def
    }
    [Flags]
    public enum NPCModifier:int
    {
        None = 0x0,
        Cluster = 0x1, // On death spawn several mini version of himself (35% of original stats)
        Size = 0x2, //Changed size ans stats
        Berserker = 0x4, //Lower the health, higger the stat, up to 100%
        Golden = 0x8, // + 200% damage, +50% damage, + 50% def, drop way more money than usual
        Vampire = 0x10, // Heal itself on damage (10% of damage are converted to health)
        ArmorBreaker = 0x20 //Ignore 30% of armor
    }
    


    //======================================================================================================================================================
    //======================================================================================================================================================
    //======================================================================================================================================================
    //======================================================================================================================================================

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
        

        public override void AI(Projectile projectile)
        {

            
            if (init)
                return;
            if (projectile.friendly)
                return;
            if (projectile.npcProj)
            { 
                if (projectile.owner > Main.npc.Length)
                    return;
                NPC owner = Main.npc[projectile.owner];
                if (owner.GivenName == "")
                    return;
                ARPGGlobalNPC ownerGlobal = owner.GetGlobalNPC<ARPGGlobalNPC>();

                    projectile.damage = owner.damage;

            }
            else
            {
                Player p = Main.player[projectile.owner];
                itemOrigin = p.HeldItem;
            }
            init = true;

        }
    }


    //======================================================================================================================================================
    //======================================================================================================================================================
    //======================================================================================================================================================
    //======================================================================================================================================================


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
                    ErrorLogger.Log(
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
                Main.npcLifeBytes[npc.type] = 4; //Sadly have to UN-optimise Health of enemy, because it caused npc to dispear if health was over 128 (for small )
            }
            if (!ConfigFile.GetConfig.gpConfig.NPCProgress)
            {
                return;
            }   

            if (Main.netMode != 1)
            {
                if (level < 0) { 
                    level = Mathf.CeilInt(NPCUtils.GetBaseLevel(npc) * ConfigFile.GetConfig.gpConfig.NpclevelMultiplier);

                    if (npc.townNPC || (npc.damage == 0))
                        tier = Mathf.CeilInt(NPCUtils.GetTierAlly(npc, level) * ConfigFile.GetConfig.gpConfig.NpclevelMultiplier);
                    else if (ConfigFile.GetConfig.gpConfig.NPCProgress)
                        tier = Mathf.CeilInt(NPCUtils.GetTier(npc, level) * ConfigFile.GetConfig.gpConfig.NpclevelMultiplier);

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
                                SetBufferProperty("size", "Growth");
                                SetBufferProperty("GrowthStep", "Mini");
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
                NetMessage.SendData(23, -1, -1, null, npc.whoAmI);

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
                    //ErrorLogger.Log("NPC not generated ! : \n" + npc.GetGivenOrTypeNetName());
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
                    npc.damage = baseDamage * (2-(npc.life/npc.lifeMax));
                }

                //MPPacketHandler.SendNpcUpdate(mod,npc);
                
            }

        }
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            
            base.OnHitByItem(npc, player, item, damage, knockback, crit);
            MPPacketHandler.SendNpcUpdate(mod, npc);
            NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            MPPacketHandler.SendNpcUpdate(mod, npc);
            NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
        }

        public override bool CheckDead(NPC npc)
        {
            if (HaveBufferProperty("GrowthStep"))
            {
                switch (GetBufferProperty("GrowthStep"))
                {
                    case "Mini":
                        SetBufferProperty("GrowthStep", "Normal");
                        break;
                    case "Normal":
                        SetBufferProperty("GrowthStep", "Giant");
                        break;
                    case "Giant":
                        SetBufferProperty("GrowthStep", "Colossus");
                        break;
                    case "Colossus":
                        SetBufferProperty("GrowthStep", "Titan");
                        break;
                    case "Titan":
                        return false;
                }
                npc = NPCUtils.SizeShiftMult(npc, GetBufferProperty("GrowthStep"));
                npc.life = npc.lifeMax;
                MPPacketHandler.SendNpcUpdate(mod, npc);
                NetMessage.SendData(23, -1, -1, null, npc.whoAmI);
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

            XPToDrop = Mathf.CeilInt(XPToDrop * ConfigFile.GetConfig.gpConfig.XpMultiplier);

            int xplevel = level + tier;
            if (!ConfigFile.GetConfig.gpConfig.XPReduction)
            {
                xplevel = 99999;
            }
            MPPacketHandler.SendXPPacket(mod, XPToDrop, xplevel);
            if (Main.netMode == 0)
                player.GetModPlayer<RPGPlayer>().AddXp(XPToDrop, xplevel);
           

        }
    }
}
