using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.IO;
using System.IO;
using Terraria.ID;
using AnotherRpgMod.UI;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.UI;

namespace AnotherRpgMod.Utils
{
    class WorldManager : ModSystem
    {
        public static readonly Dictionary<Message, List<WorldDataTag>> worldDataTag = new Dictionary<Message, List<WorldDataTag>>()
        {
            { Message.syncWorld, new List<WorldDataTag>(){
                WorldDataTag.Day,WorldDataTag.FirstBossDefeated,WorldDataTag.BossDefeated,WorldDataTag.lastDayTime,WorldDataTag.ascended,WorldDataTag.ascendedLevel,WorldDataTag.PlayerLevel,
            } },
        };

        public static int BossDefeated = 0;
        public static int FirstBossDefeated = 0;
        public static int Day = 1;
        private static bool lastDayTime = true;

        public static bool ascended = false;
        public static int ascendedLevelBonus = 0;

        public static int PlayerLevel = 1;

        public static List<int> BossDefeatedList;

        public static WorldManager instance;

        public static void OnBossDefeated(NPC npc)
        {
            if (BossDefeated < FirstBossDefeated)
                BossDefeated = FirstBossDefeated;

            BossDefeated++;
            if (BossDefeatedList.Exists(x => x == npc.type))
            {
                return;
            }
            BossDefeatedList.Add(npc.type);
            FirstBossDefeated++;
            Main.NewText("The world grow stronger..", 144, 32, 185);
        }

        public override void PostWorldGen()
        {
            instance = this;
            BossDefeated = 0;
            BossDefeatedList = new List<int>();
        }


        public static int GetWorldLevelMultiplier(int Level)
        {
            float baseLevelMult = 0.6f;
            if (Main.hardMode)
                baseLevelMult = 1;
            baseLevelMult += Day * 0.05f;
            if (!Main.expertMode)
            {
                 Mathf.Clamp(baseLevelMult, 0, 1);
            }
            return Mathf.FloorInt(baseLevelMult * Level) + ascendedLevelBonus;
        } 

        public static int GetMaximumAscend()
        {
            if (!Config.gpConfig.AscendLimit)
            {
                return 999;
            }

            float limit = 0;


            if (Config.NPCConfig.BossKillLevelIncrease)
            {
                limit = (BossDefeated * Config.gpConfig.AscendLimitPerBoss);
            }
            else
                limit = (FirstBossDefeated * Config.gpConfig.AscendLimitPerBoss);




            if (Main.hardMode && limit < 5)
            {
                limit = 5;
                if (NPC.downedPlantBoss && limit < 15)
                    limit = 15;
                if (NPC.downedMoonlord)
                    return 999;
            }

            return Mathf.FloorInt(limit);
        }

        public static float GetHealthMultiplierAscendCap()
        {
            float limit = 0;

            if (Config.NPCConfig.BossKillLevelIncrease)
            {
                limit = (BossDefeated * Config.gpConfig.AscendLimitPerBoss);
            }
            else
                limit = (FirstBossDefeated * Config.gpConfig.AscendLimitPerBoss);

            return 1+ Mathf.FloorInt(limit)*0.5f;
        }





        public override void PostUpdateTime()
        {
            if (Main.dayTime != lastDayTime)
            {
                lastDayTime = Main.dayTime;
                if (Main.dayTime)
                {
                    Day++;

                }

            }
            base.PostUpdateTime();
        }


        public static int GetWorldAdditionalLevel()
        {
            int bonuslevel = 0;

            if (Config.NPCConfig.BossKillLevelIncrease)
            {
                bonuslevel = (int)(BossDefeated * Config.NPCConfig.NPCGrowthValue);
            }
            else
                bonuslevel = (int)(FirstBossDefeated * Config.NPCConfig.NPCGrowthValue);
            

            if (Main.hardMode)
            {
                bonuslevel = Mathf.CeilInt(bonuslevel * Config.NPCConfig.NPCGrowthHardModePercent);
                bonuslevel += Config.NPCConfig.NPCGrowthHardMode;
            }
            
            return bonuslevel;
        }


        public override void SaveWorldData(TagCompound tag)
        {
            //AnotherRpgMod.Instance.Logger.Info("Is it saving World Data ? ....");
            base.SaveWorldData(tag);
            if (BossDefeatedList == null)
            {
                BossDefeatedList = new List<int>();
            }
            tag.Add("BossDefeated", BossDefeated);
            tag.Add("FirstBossDefeated", FirstBossDefeated);
            tag.Add("BossDefeatedList", ConvertToInt(BossDefeatedList));
            tag.Add("day", Day);
            tag.Add("ascended", ascended);
            tag.Add("ascendedLevel", ascendedLevelBonus);
            tag.Add("PlayerLevel", PlayerLevel);
        }

        private int[] ConvertToInt(List<int> list)
        {
            int[] newList = new int[list.Count];
            for(int i = 0;i< list.Count; i++)
            {
                newList[i] = list[i];
            }
            return newList;
        }

        private void ConvertToList(int[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                BossDefeatedList.Add(list[i]);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            instance = this;
            //AnotherRpgMod.Instance.Logger.Info(tag);
            //AnotherRpgMod.Instance.Logger.Info("Load World Data");
            BossDefeatedList = new List<int>();
            FirstBossDefeated = tag.GetInt("FirstBossDefeated");
            BossDefeated = tag.GetInt("BossDefeated");
            Day = tag.GetInt("day");
            ConvertToList(tag.GetIntArray("BossDefeatedList"));
            ascended = tag.GetBool("ascended");
            ascendedLevelBonus = tag.GetInt("ascendedLevel");
            if (Main.netMode == NetmodeID.SinglePlayer)
                PlayerLevel = tag.GetInt("PlayerLevel");
            else
            {
                PlayerLevel = 0;
            }
        }

        public void NetUpdateWorld()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)Message.syncWorld);
                packet.Write(Day);
                packet.Write(FirstBossDefeated);
                packet.Write(BossDefeated);
                packet.Write(lastDayTime);
                packet.Write(ascended);
                packet.Write(ascendedLevelBonus);
                packet.Write(PlayerLevel);
                packet.Send();
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)Message.syncWorld);
            writer.Write(Day);
            writer.Write(FirstBossDefeated);
            writer.Write(BossDefeated);
            writer.Write(lastDayTime);
            writer.Write(ascended);
            writer.Write(ascendedLevelBonus);
            writer.Write(PlayerLevel);
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {

            Message msg;
            msg = (Message)reader.ReadByte();
            if (msg == Message.syncWorld) {
                Dictionary<WorldDataTag, object> tags = new Dictionary<WorldDataTag, object>();
                foreach (WorldDataTag tag in worldDataTag[msg])
                {
                    tags.Add(tag, tag.read(reader));
                }
                Day = (int)tags[WorldDataTag.Day];
                FirstBossDefeated = (int)tags[WorldDataTag.FirstBossDefeated];
                BossDefeated = (int)tags[WorldDataTag.BossDefeated];
                lastDayTime = (bool)tags[WorldDataTag.lastDayTime];
                ascended = (bool)tags[WorldDataTag.ascended];
                ascendedLevelBonus = (int)tags[WorldDataTag.ascendedLevel];
                PlayerLevel = (int)tags[WorldDataTag.PlayerLevel];
            }



            base.NetReceive(reader);
        }


        public override void PreSaveAndQuit()
        {

            Stats.visible = false;
            base.PreSaveAndQuit();
        }
        public override void PostUpdateEverything()
        {
            //Update UI when screen Size Change
            if (Main.netMode != NetmodeID.Server)
            {
                if (Math.Abs(AnotherRpgMod.Instance.lastUpdateScreenScale - Main.screenHeight) > 0.01f)
                {
                    AnotherRpgMod.Instance.healthBar.Reset();
                    AnotherRpgMod.Instance.OpenST.Reset();
                    AnotherRpgMod.Instance.openStatMenu.Reset();
                }
                AnotherRpgMod.Instance.lastUpdateScreenScale = Main.screenHeight;
            }
            base.PostUpdateEverything();
        }
        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            if (AnotherRpgMod.Instance.customstats != null)
                AnotherRpgMod.Instance.customstats.Update(gameTime);
        }


        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            AnotherRpgMod.zoomValue = Transform.Zoom;
            base.ModifyTransformMatrix(ref Transform);
        }


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
            if (Main.netMode == NetmodeID.Server)
                return;


            if (HealthBar.visible && Config.gpConfig.RPGPlayer && Config.vConfig.HideVanillaHB)
            {
                int ressourceid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
                layers.RemoveAt(ressourceid);
            }

            //Vanilla: MouseOver
            int mouseid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (mouseid != -1)
            {
                layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: NPC Mouse Info",
                    delegate
                    {
                        AnotherRpgMod.Instance.customNPCInfo.Update(Main._drawInterfaceGameTime);
                        AnotherRpgMod.Instance.NPCInfo.Draw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(mouseid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: NPC Name Info",
                    delegate
                    {
                        AnotherRpgMod.Instance.customNPCName.Update(Main._drawInterfaceGameTime);
                        AnotherRpgMod.Instance.NPCName.Draw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }


            int skilltreeid = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 2"));
            if (skilltreeid != -1)
            {
                //layers.RemoveAt(id);

                //Add you own layer
                layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: StatWindows",
                    delegate
                    {
                        if (Stats.visible)
                        {

                            AnotherRpgMod.Instance.statMenu.Draw(Main.spriteBatch);

                        }
                        if (OpenStatsButton.visible)
                        {
                            AnotherRpgMod.Instance.customOpenstats.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.openStatMenu.Draw(Main.spriteBatch);
                        }

                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: Skill Tree",
                    delegate
                    {
                        if (OpenSTButton.visible)
                        {
                            AnotherRpgMod.Instance.customOpenST.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.OpenST.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.None)
                );

                layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: Skill Tree",
                    delegate
                    {
                        if (ItemTreeUi.visible)
                        {
                            //Update Item Tree
                            AnotherRpgMod.Instance.customItemTree.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.ItemTreeUI.Draw(Main.spriteBatch);

                        }
                        return true;
                    }, InterfaceScaleType.None)
                );

                layers.Insert(skilltreeid, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: Skill Tree",
                    delegate
                    {
                        if (SkillTreeUi.visible)
                        {
                            //Update Skill Tree
                            AnotherRpgMod.Instance.customSkillTree.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.skillTreeUI.Draw(Main.spriteBatch);

                        }
                        return true;
                    }, InterfaceScaleType.None)
                );
            }



            int id = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (id != -1)
            {
                layers.Insert(id, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: Custom Health Bar",
                    delegate
                    {
                        if (HealthBar.visible)
                        {
                            //Update CustomBars
                            AnotherRpgMod.Instance.customOpenST.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.customOpenstats.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.customResources.Update(Main._drawInterfaceGameTime);
                            AnotherRpgMod.Instance.healthBar.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.None)
                );
            }

        }

    }

    
    


    public class WorldDataTag
    {
        public static WorldDataTag Day = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag FirstBossDefeated = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag BossDefeated = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag lastDayTime = new WorldDataTag(reader => reader.ReadBoolean());
        public static WorldDataTag ascended = new WorldDataTag(reader => reader.ReadBoolean());
        public static WorldDataTag ascendedLevel = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag PlayerLevel = new WorldDataTag(reader => reader.ReadInt32());

        public Func<BinaryReader, object> read;

        public WorldDataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }
}
