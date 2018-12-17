using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.IO;

namespace AnotherRpgMod.Utils
{
    class WorldManager : ModWorld
    {
        

        public static int BossDefeated = 0;
        public static int Day = 1;
        private static bool lastDayTime = true;

        
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
            return Mathf.FloorInt(baseLevelMult * Level);
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
            AnotherRpgMod.PlayerLevel = 0;
        }

    }
}
