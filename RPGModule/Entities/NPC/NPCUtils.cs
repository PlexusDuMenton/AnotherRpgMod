using System;
using Terraria;
using Terraria.ID;
using AnotherRpgMod.Utils;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
namespace AnotherRpgMod.RPGModule.Entities
{
    class NPCUtils
    {
        public static Dictionary<NPCModifier, int> modifierWeight = new Dictionary<NPCModifier, int>()
        {
            { NPCModifier.None,     200},
            { NPCModifier.Cluster,     10},
            { NPCModifier.Size,     200},
            { NPCModifier.Berserker,     100},
            { NPCModifier.Golden,     10},
            //Next is Bugged
            { NPCModifier.Vampire,     50},
             { NPCModifier.ArmorBreaker,     50},
        };

        public static Dictionary<NPCRank, float[]> NPCRankStats = new Dictionary<NPCRank, float[]>()
        {
            //Rank                                  HPMult,   DMGMult,    DefMult
            { NPCRank.Weak,         new float[3]    {0.65f,    0.8f,       0.5f  } },
            { NPCRank.Normal,       new float[3]    {1,       1,          1     } },
            { NPCRank.Alpha,        new float[3]    {1.2f,    1.05f,      1.1f  } },
            { NPCRank.Elite,        new float[3]    {1.4f,    1.10f,       1.2f  } },
            { NPCRank.Legendary,    new float[3]    {2,       1.2f,       1.4f  } },
            { NPCRank.Mythical,     new float[3]    {3.5f,       1.3f,          1.6f  } },
            { NPCRank.Godly,        new float[3]    {5,      1.4f,          1.8f  } },
            { NPCRank.DIO,          new float[3]    {10,      1.5f,           2f  } },
        };

        public static Dictionary<string, float[]> NPCSizeStats = new Dictionary<string, float[]>()
        {
            //Rank                              HPMult,     DMGMult,    DefMult     Size
            { "Mini",         new float[4]      {0.65f,       0.8f,       0.7f,       0.6f    } },
            { "Giant",        new float[4]      {1.8f,       1.05f,       1.05f,       1.2f    } },
            { "Colossus",     new float[4]      {2.2f,         1.10f,       1.10f,       1.5f    } },
            { "Titan",        new float[4]      {2.5f,         1.15f,       1.2f,       1.8f    } },

        };

        public static float DELTATIME = 1f / 60f;
        //Generate/Get level and tier or NPC
        #region LevelGen
        //Get Base Level Based on Stat of npc
        public static int GetBaseLevel(NPC npc)
        {

            if (npc.type == 68 || npc.type == 70 | npc.type == 72)
                return 1;
            int maxLevel = (int)(AnotherRpgMod.PlayerLevel * 1.5f + 100);
            int baselevel = Mathf.HugeCalc((int)((Mathf.Pow(npc.lifeMax / 25, 1.1f) + Mathf.Pow(npc.damage * 0.28f, 1.25f) + Mathf.Pow(npc.defense, 1.5f))),-1);
            
            if (npc.boss)
            {
                float health = npc.lifeMax;


                

                if (Main.expertMode)
                {
                    baselevel = Mathf.HugeCalc((int)(health / 140 + Mathf.Pow(npc.damage * 0.31f, 1.05f) + Mathf.Pow(npc.defense * 0.8f, 1.07f)),-1);
                }
                else
                {
                    baselevel = Mathf.HugeCalc((int)(health / 100 + Mathf.Pow(npc.damage * 0.30f, 1.04f) + Mathf.Pow(npc.defense * 0.7f, 1.05f)),-1) ;
                }

                if ((npc.aiStyle == 6 || npc.aiStyle == 37) && npc.type > 100) { 
                    if (Main.expertMode)
                    {
                        baselevel = Mathf.HugeCalc((int)(Mathf.Pow(health / 650,0.5f) + Mathf.Pow(npc.damage * 0.31f, 1.05f) + Mathf.Pow(npc.defense * 0.8f, 1.07f)), -1);
                    }
                    else
                    {
                        baselevel = Mathf.HugeCalc((int)(Mathf.Pow(health / 500, 0.5f) + Mathf.Pow(npc.damage * 0.31f, 1.05f) + Mathf.Pow(npc.defense * 0.8f, 1.07f)), -1);
                    }
                }

            }

            if (Main.expertMode)
            {
                baselevel = (int)(baselevel * 0.6f);
                if (AnotherRpgMod.LoadedMods[SupportedMod.Thorium])
                {
                    if (baselevel > 15)
                    {
                        baselevel -= 15;
                        Mathf.Clamp(baselevel, 5, int.MaxValue);
                    }

                    baselevel = (int)(baselevel * 0.125f);

                    
                }
            }

            baselevel = WorldManager.GetWorldLevelMultiplier(baselevel);
            if (baselevel < -1)
                return 0;
            Mathf.Clamp(baselevel,0, maxLevel);
            return baselevel;
        }
        //Get Tier bonus from world
        public static int GetWorldTier(NPC npc, int baselevel)
        {
            int BonusLevel = WorldManager.GetWorldAdditionalLevel();

            if (BonusLevel * 2 > baselevel)
            {
                return Mathf.Clamp(BonusLevel, 0, BonusLevel * 2 - baselevel);
            }
            return 0;
        }

        //Tier are bonus level either random or from world
        public static int GetTier(NPC npc, int baselevel)
        {
            if (baselevel < 0)
                baselevel = 0;
            int rand = Mathf.RandomInt(0, 4 + Mathf.CeilInt(baselevel * 0.1f));
            return Mathf.HugeCalc(rand + GetWorldTier(npc, baselevel + rand),-1);
        }
        public static int GetTierAlly(NPC npc, int baselevel)
        {
            return WorldManager.GetWorldAdditionalLevel();
        }

        //get actual rank of the monster
        public static NPCRank GetRank(int level)
        {
            if (!ConfigFile.GetConfig.gpConfig.NPCRarity)
                return NPCRank.Normal;
            if (level < 1)
                level = 1; 
            float rarityBooster = (float)Math.Log(level) + 1;
            int rn = Mathf.RandomInt(0, (500-Mathf.Clamp((int)Mathf.Pow(level,0.75f),0,350))*10);
            if (rn <= 1* rarityBooster)
                return NPCRank.DIO;
            if (rn <= 8* rarityBooster)
                return NPCRank.Godly;
            if (rn <= 20* rarityBooster)
                return NPCRank.Mythical;
            if (rn < 75* rarityBooster)
                return NPCRank.Legendary;
            if (rn < 200* Mathf.Pow(rarityBooster,0.75f))
                return NPCRank.Elite;
            if (rn < 1000* Mathf.Pow(rarityBooster, 0.5f))
                return NPCRank.Alpha;
            if (rn < 3500 * Mathf.Pow(rarityBooster, 0.25f))
                return NPCRank.Normal;
            return NPCRank.Weak;

        }

        #endregion

        public static int GetExp(NPC npc)
        {
            return Mathf.CeilInt(Math.Pow(npc.lifeMax / 10 + npc.damage + npc.defense * 1.5f, 0.95f));
        }

        private static NPCModifier AddRandomModifier(List<NPCModifier> pool)
        {
            int totalWeight = 0;
            for(int i =0; i < pool.Count; i++)
                totalWeight += modifierWeight[pool[i]];

            int rn = Mathf.RandomInt(0, totalWeight);
            int checkingWeight = 0;
            for(int i = 0; i < pool.Count; i++)
            {
                if (rn < checkingWeight+modifierWeight[pool[i]])
                    return pool[i];
                checkingWeight +=modifierWeight[pool[i]];
                
            }
            return pool[pool.Count-1];

        }

        public static NPCModifier GetModifier(NPCRank rank,NPC npc)
        {
            if (npc.dontCountMe)
                return NPCModifier.None;
            if (!ConfigFile.GetConfig.gpConfig.NPCRarity)
                return NPCModifier.None;

            if (npc.boss && !ConfigFile.GetConfig.gpConfig.BossModifier)
                return NPCModifier.None;

            int maxModifier = 1;
            switch (rank)
            {
                case NPCRank.Weak:
                    return 0;
                case NPCRank.Normal:
                    maxModifier = (Mathf.Random(0, 3) < 1) ? 0 : 1;
                    break;
                case NPCRank.Alpha:
                    maxModifier = 1;
                    break;
                case NPCRank.Elite:
                case NPCRank.Legendary:
                    maxModifier = 2;
                    break;
                case NPCRank.Mythical:
                    maxModifier = 3;
                    break;
                case NPCRank.Godly:
                    maxModifier = 4;
                    break;
                case NPCRank.DIO:
                    maxModifier = (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length;
                    break;
            }
            NPCModifier modifiers = 0;
            //if npc.aiStyle == 3 

            

            List<NPCModifier> modifiersPool = (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).ToList();
            if (npc.boss ) {
                modifiersPool.Remove(NPCModifier.Size);
                if (maxModifier == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                    maxModifier -= 1;
                if (!ConfigFile.GetConfig.gpConfig.BossClustered) { 
                    modifiersPool.Remove(NPCModifier.Cluster);
                    if (maxModifier+1 == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                        maxModifier -= 1;
                }
            }
            else if (npc.type == 13 || npc.type == 14 || npc.type == 15){
                modifiersPool.Remove(NPCModifier.Cluster);
                modifiersPool.Remove(NPCModifier.Size);
                if (maxModifier == (Enum.GetValues(typeof(NPCModifier)) as NPCModifier[]).Length)
                    maxModifier -= 2;
            }

            for (int i = 0; i < maxModifier; i++)
            {
                NPCModifier modid = AddRandomModifier(modifiersPool);
                modifiers = modifiers | modid;
                modifiersPool.Remove(modid);
            }

            return modifiers;
        }

        public static string GetNpcNameChange(NPC npc, int tier, int level, NPCRank rank)
        {

            string name = npc.GivenOrTypeName;
            if (npc.townNPC)
                return name;
            if (name == "")
                name = npc.GivenName;
            if (name == "")
                name = npc.TypeName;
            if (name == "")
                return name;



            string sufix = " the ";
            string Prefix = "";
                /*
            if (ConfigFile.GetConfig.gpConfig.NPCProgress)
                Prefix+= "Lvl. " + (tier + level) + " ";
            if (WorldManager.GetWorldAdditionalLevel() > 0)
                Prefix += "(+" + GetWorldTier(npc,level) + ") ";
                */
            switch (rank)
            {
                case NPCRank.Weak:
                    Prefix += "Weak ";
                    break;
                case NPCRank.Alpha:
                    Prefix += "Alpha ";
                    break;
                case NPCRank.Elite:
                    Prefix += "Elite ";
                    break;
                case NPCRank.Legendary:
                    Prefix += "Legendary ";
                    break;
                case NPCRank.Mythical:
                    Prefix += "Mythical ";
                    break;
                case NPCRank.Godly:
                    Prefix += "Godly ";
                    break;
                case NPCRank.DIO:
                    Prefix += "Kono Dio Da ";
                    break;
            }

            ARPGGlobalNPC anpc = npc.GetGlobalNPC<ARPGGlobalNPC>();
            if (anpc.HaveModifier(NPCModifier.Cluster))
                sufix += "Clustered ";
            if (anpc.HaveModifier(NPCModifier.Golden))
                sufix += "Golden ";
            if (anpc.HaveModifier(NPCModifier.ArmorBreaker))
                sufix += "Armor Breaker ";
            if (anpc.HaveModifier(NPCModifier.Berserker))
                sufix += "Berserker ";
            if (anpc.HaveModifier(NPCModifier.Vampire))
                sufix += "Vampire ";

            if (anpc.HaveModifier(NPCModifier.Size))
            {
                string size = (string)anpc.GetBufferProperty("size");
                Prefix += (size + " ");
            }
            if (sufix == " the ")
                sufix = "";
            return Prefix + name + sufix;

        }

        public static NPC SetRankStat(NPC npc, NPCRank rank)
        {
            if (rank == NPCRank.Normal)
                return npc;
            if (rank == NPCRank.Weak)
            {
                npc.lifeMax = Mathf.CeilInt(npc.lifeMax * NPCRankStats[rank][0]);
                npc.damage = Mathf.CeilInt(npc.damage * NPCRankStats[rank][1]);
                npc.defense = Mathf.CeilInt(npc.defense * NPCRankStats[rank][2]);
            }
            else { 
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCRankStats[rank][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCRankStats[rank][1]),npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCRankStats[rank][2]),npc.defense);
            }
            npc.life = npc.lifeMax;

            return npc;
        }

        public static NPC SetSizeStat(NPC npc, string size)
        {
            if (size == "Growther")
                size = (string)npc.GetGlobalNPC<ARPGGlobalNPC>().GetBufferProperty("GrowtherStep");

            if (size == "Normal")
                return npc;
            if (size == "Mini")
            {
                npc.lifeMax = Mathf.CeilInt(npc.lifeMax * NPCSizeStats[size][0]);
                npc.damage = Mathf.CeilInt(npc.damage * NPCSizeStats[size][1]);
                npc.defense = Mathf.CeilInt(npc.defense * NPCSizeStats[size][2]);
            }
            else
            {
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCSizeStats[size][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCSizeStats[size][1]), npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCSizeStats[size][2]), npc.defense);
            }
            

            npc.scale *= NPCSizeStats[size][3];

            npc.life = npc.lifeMax;

            return npc;
        }

        public static NPC SizeShiftMult(NPC npc, string size)
        {
            if (size == "Mini")
            {
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax / NPCSizeStats["Mini"][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage / NPCSizeStats["Mini"][1]), npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense / NPCSizeStats["Mini"][2]), npc.defense);

                npc.scale /= NPCSizeStats["Mini"][3];
            }
            else if (size == "Normal")
            {
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt(npc.lifeMax * NPCSizeStats["Giant"][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt(npc.damage * NPCSizeStats["Giant"][1]), npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt(npc.defense * NPCSizeStats["Giant"][2]), npc.defense);

                npc.scale *= NPCSizeStats["Giant"][3];
            }
            else if (size == "Giant")
            {
                
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt((npc.lifeMax / NPCSizeStats["Giant"][0]) * NPCSizeStats["Colossus"][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt((npc.damage / NPCSizeStats["Giant"][1]) * NPCSizeStats["Colossus"][1]),npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt((npc.defense / NPCSizeStats["Giant"][2]) * NPCSizeStats["Colossus"][2]),npc.defense);

                npc.scale *= NPCSizeStats["Colossus"][3] / NPCSizeStats["Giant"][3];
            }
            else if (size == "Colossus")
            {
                npc.lifeMax = Mathf.HugeCalc(Mathf.CeilInt((npc.lifeMax / NPCSizeStats["Colossus"][0]) * NPCSizeStats["Titan"][0]), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.CeilInt((npc.damage / NPCSizeStats["Colossus"][1]) * NPCSizeStats["Titan"][1]),npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.CeilInt((npc.defense / NPCSizeStats["Colossus"][2]) * NPCSizeStats["Titan"][2]),npc.defense);

                npc.scale *= NPCSizeStats["Titan"][3] / NPCSizeStats["Colossus"][3];
            }

            return npc;
        }

        public static NPC SetModifierStat(NPC npc)
        {
            ARPGGlobalNPC ArNpc = npc.GetGlobalNPC<ARPGGlobalNPC>();
            if (ArNpc.HaveModifier(NPCModifier.Golden))
            {
                
                npc.lifeMax = Mathf.HugeCalc((int)(npc.lifeMax * 3f), npc.lifeMax);
                npc.damage = Mathf.HugeCalc((int)(npc.damage * 1.5f),npc.damage);
                npc.value += Item.buyPrice(0, 1, 50, 0);
                npc.value *= 2*ArNpc.getRank;
            }

            if (ArNpc.HaveModifier(NPCModifier.Size))
                npc = SetSizeStat(npc, (string)ArNpc.GetBufferProperty("size"));

            if (ArNpc.HaveModifier(NPCModifier.Vampire))
                npc.lifeMax = Mathf.HugeCalc((int)(npc.lifeMax * 1.5f), npc.lifeMax);
                npc.damage = Mathf.HugeCalc((int)(npc.damage * 1.3f),npc.damage);

            if (ArNpc.HaveModifier(NPCModifier.Berserker))
                npc.color = Color.Lerp(npc.color ,new Color(1.0f, 0.0f, 0.0f),0.3f);
            if (ArNpc.HaveModifier(NPCModifier.Golden))
                npc.color = Color.Lerp(npc.color, new Color(1.0f,0.8f,0.5f),0.8f);



            npc.life = npc.lifeMax;

            return npc;

        }



        static public void SpawnSized(NPC npc, Dictionary<string, string> buffer)
        {
             int n = NPC.NewNPC(
                (int)npc.position.X,
                (int)npc.position.Y,
                npc.type
                );
            Main.npc[n].velocity.X = Mathf.RandomInt(-8, 8);
            Main.npc[n].velocity.Y = Mathf.RandomInt(-8, 3);
            Main.npc[n].GetGlobalNPC<ARPGGlobalNPC>().SetBufferProperty("clustered", "true");
        }

        public static float UpdateDOT (NPC npc)
        {
            float DoTDamage = 0;
            int life = npc.life;
            if (life > 1)
            {
                if (npc.HasBuff(BuffID.OnFire))
                {
                    DoTDamage += Mathf.Clamp(  Mathf.Logx(npc.lifeMax,1.002f) * 0.25f * DELTATIME , 0 , npc.lifeMax*0.05f * DELTATIME);
                }
                if (npc.HasBuff(BuffID.Burning))
                {
                    DoTDamage += Mathf.Clamp(Mathf.Logx(npc.lifeMax, 1.002f) * 0.05f * DELTATIME, 0, npc.lifeMax * 0.02f * DELTATIME);
                }
                if (npc.HasBuff(BuffID.Frostburn))
                {

                    DoTDamage += Mathf.Clamp(Mathf.Logx(npc.lifeMax, 1.002f) * 0.25f * DELTATIME, 0, npc.lifeMax * 0.05f * DELTATIME);
                }
                if (npc.HasBuff(BuffID.Venom))
                {

                    DoTDamage += Mathf.Clamp(Mathf.Logx(npc.lifeMax, 1.002f) * 0.25f * DELTATIME, 0, npc.lifeMax * 0.05f * DELTATIME);
                }
            }
            return DoTDamage;
        }

        public static NPC SetNPCStats(NPC npc, int level, int tier,NPCRank rank)
        {
            if (npc == null)
                return npc;

            if (!ConfigFile.GetConfig.gpConfig.NPCProgress)
            {
                npc = SetRankStat(npc, rank);
                npc = SetModifierStat(npc);
                return npc;
            }

            if (npc.townNPC || npc.damage == 0)
            {
                npc.lifeMax = Mathf.HugeCalc(Mathf.FloorInt(npc.lifeMax * (1 + (tier + level) * 0.1f)), npc.lifeMax);
                npc.damage = Mathf.HugeCalc(Mathf.FloorInt(npc.damage * Mathf.Pow(1 + level * 0.08f + tier * 0.1f, 0.95f)), npc.damage);
                npc.defense = Mathf.HugeCalc(Mathf.FloorInt(npc.defense * (1 + level * 0.012f + tier * 0.02f)), npc.defense);
                npc.life = npc.lifeMax;

                return npc;
            }
            else
            {
                if (npc.boss)
                {
                    if (Mathf.HugeCalc(Mathf.FloorInt(npc.damage * Mathf.Pow(1 + level * 0.025f + tier * 0.04f, 0.95f)), npc.damage) < 250000)
                        npc.damage = Mathf.HugeCalc(Mathf.FloorInt(npc.damage * Mathf.Pow(1 + level * 0.025f + tier * 0.04f, 0.95f)), npc.damage);
                    else
                    {
                        npc.damage = Mathf.FloorInt(250000 * Mathf.Logx(1 + level * 0.10f + tier * 0.30f, 7.5f));
                    }
                    npc.lifeMax = Mathf.HugeCalc(Mathf.FloorInt(npc.lifeMax * (1 + level * 0.05f + tier*0.1)), npc.lifeMax);
                }
                else
                {
                    npc.damage = Mathf.HugeCalc(Mathf.FloorInt(npc.damage * Mathf.Pow(1 + level * 0.12f + tier * 0.15f, 0.85f)), npc.damage);
                    npc.lifeMax = Mathf.HugeCalc(Mathf.FloorInt(npc.lifeMax * (1 + level * 0.065f + tier * 0.1f)), npc.lifeMax);
                }

                npc.value = npc.value * (1 + (level + tier) * 0.01f) * Mathf.Clamp((int)rank,1,5);
                npc.defense = Mathf.HugeCalc(Mathf.FloorInt(npc.defense * (1 + level * 0.01f + tier * 0.02f)), npc.defense);

                npc.life = npc.lifeMax;

                npc = SetRankStat(npc, rank);
                npc = SetModifierStat(npc);
            }
            return npc;
        }
    }
}
