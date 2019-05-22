
using AnotherRpgMod.Utils;

namespace AnotherRpgMod.RPGModule.Entities
{
    class StatData
    {
        private int natural;
        private int level;
        private int xp;

        public int AddLevel { get { return level; } }
        public int NaturalLevel { get { return natural; } }
        public int GetLevel { get { return level + natural; } }
        public int GetXP { get { return xp; } }

        public int XpForLevel()
        {
            return Mathf.CeilInt(Mathf.Pow(level * 0.04f,0.75f)) + 1;
        }
        public void AddXp(int _xp)
        {
            xp += _xp;
            while (xp >= XpForLevel())
            {
                xp -= XpForLevel();
                level = level + 1;
            }
        }
        public StatData(int _natural, int _level = 0, int _xp = 0)
        {
            natural = _natural;
            xp = _xp;
            level = _level;
        }
        public void LevelUp()
        {
            natural++;
        }

    }
}
