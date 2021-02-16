using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.IO;
using System.IO;

namespace AnotherRpgMod.Utils
{
    class WorldManager : ModWorld
    {

        public static Dictionary<Message, List<WorldDataTag>> worldDataTag = new Dictionary<Message, List<WorldDataTag>>()
        {
            { Message.syncWorld, new List<WorldDataTag>(){
                WorldDataTag.Day,WorldDataTag.BossDefeated,WorldDataTag.lastDayTime
            } },
        };

        public static int BossDefeated = 0;
        public static int Day = 1;
        private static bool lastDayTime = true;

        public static bool ascended = false;
        public static int ascendedLevelBonus = 0;
        
        public static List<int> BossDefeatedList;

        public static void OnBossDefeated(NPC npc)
        {
            if (BossDefeatedList.Exists(x => x == npc.type))
            {
                return;
            }
            BossDefeatedList.Add(npc.type);
            BossDefeated++;
            Main.NewText("The world grow stronger..", 144, 32, 185);
        }

        public override void Initialize()
        {
            BossDefeated = 0;
            BossDefeatedList = new List<int>();
        }

        public static int GetWorldLevelMultiplier(int Level)
        {
            float baseLevelMult = 0.35f;
            if (Main.hardMode)
                baseLevelMult = 1;
            baseLevelMult += Day * 0.1f;
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

            limit = (BossDefeated * Config.gpConfig.AscendLimitPerBoss);

            if (Main.hardMode && limit < 5)
            {
                limit = 5;
                if (NPC.downedPlantBoss && limit < 15)
                    limit = 15;
                if (NPC.downedMoonlord)
                    return 999;
            }
            AnotherRpgMod.Instance.Logger.Info(limit);



            return Mathf.FloorInt(limit);
        }









        public override void PostUpdate()
        {
            if (Main.dayTime != lastDayTime) {
                lastDayTime = Main.dayTime;
                if (Main.dayTime)
                    Day++;
            }
            base.PostUpdate();
        }

        public static int GetWorldAdditionalLevel()
        {
            int bonuslevel = 0;

            bonuslevel = BossDefeated * 10;

            if (Main.hardMode)
            {
                bonuslevel = Mathf.CeilInt(bonuslevel * 1.1f);
                bonuslevel += 20;
            }
            
            return bonuslevel;
        }

        public override TagCompound Save()
        {
            if (BossDefeatedList == null)
            {
                BossDefeatedList = new List<int>();
            }
            return new TagCompound {
                    {"BossDefeated", BossDefeated},
                    {"BossDefeatedList", ConvertToInt(BossDefeatedList)},
                    {"day", Day},
                    {"ascended", ascended},
                    {"ascendedLevel", ascendedLevelBonus},
                };
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

        public override void Load(TagCompound tag)
        {
            BossDefeatedList = new List<int>();
            BossDefeated = tag.GetInt("BossDefeated");
            Day = tag.GetInt("day");
            ConvertToList(tag.GetIntArray("BossDefeatedList"));
            ascended = tag.GetBool("ascended");
            ascendedLevelBonus = tag.GetInt("ascendedLevel");
            AnotherRpgMod.PlayerLevel = 0;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)Message.syncWorld);
            writer.Write(Day);
            writer.Write(BossDefeated);
            writer.Write(lastDayTime);
            writer.Write(ascended);
            writer.Write(ascendedLevelBonus);
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {

            Message msg;
            msg = (Message)reader.ReadByte();

            Dictionary<WorldDataTag, object> tags = new Dictionary<WorldDataTag, object>();
            foreach (WorldDataTag tag in worldDataTag[msg])
            {
                tags.Add(tag, tag.read(reader));
            }

            Day = (int)tags[WorldDataTag.Day];
            BossDefeated = (int)tags[WorldDataTag.BossDefeated];
            lastDayTime = (bool)tags[WorldDataTag.lastDayTime];
            
            ascended = (bool)tags[WorldDataTag.ascended];
            BossDefeated = (int)tags[WorldDataTag.ascendedLevel];

            base.NetReceive(reader);
        }

    }

    

    public class WorldDataTag
    {
        public static WorldDataTag Day = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag BossDefeated = new WorldDataTag(reader => reader.ReadInt32());
        public static WorldDataTag lastDayTime = new WorldDataTag(reader => reader.ReadBoolean());
        public static WorldDataTag ascended = new WorldDataTag(reader => reader.ReadBoolean());
        public static WorldDataTag ascendedLevel = new WorldDataTag(reader => reader.ReadInt32());

        public Func<BinaryReader, object> read;

        public WorldDataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }
}
