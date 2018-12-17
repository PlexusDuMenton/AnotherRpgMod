
using System.Collections.Generic;

namespace AnotherRpgMod.RPGModule.Entities
{
    class RPGStats
    {

        readonly int Default = 4;
        private Dictionary<Stat, StatData> ActualStat;

        public RPGStats()
        {
            ActualStat = new Dictionary<Stat, StatData>(8);
            for (int i = 0; i <= 7; i++)
            {
                ActualStat.Add((Stat)i, new StatData(Default));
            }
        }

        public void SetStats(Stat _stat, int _natural, int _level, int _xp)
        {
            ActualStat[_stat] = new StatData(_natural, _level, _xp);
        }

        public int GetLevelStat(Stat a)
        {
            return ActualStat[a].AddLevel;
        }
        public int GetStat(Stat a)
        {
            return ActualStat[a].GetLevel;
        }

        public int GetNaturalStat(Stat a)
        {
            return ActualStat[a].NaturalLevel;
        }

        public void UpgradeStat(Stat statname, int value = 1)
        {
            ActualStat[statname].AddXp(value);
        }
        public int GetStatXP(Stat statname)
        {
            return ActualStat[statname].GetXP;
        }
        public int GetStatXPMax(Stat statname)
        {
            return ActualStat[statname].XpForLevel();
        }

        public void Reset(int level)
        {
            for (int i = 0; i <= 7; i++)
            {
                ActualStat[(Stat)i] = new StatData(level + Default - 1);
            }
        }

        public void OnLevelUp()
        {
            for (int i = 0; i <= 7; i++)
            {
                ActualStat[(Stat)i].LevelUp();
            }
        }
    }
}
