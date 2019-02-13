
using System.Collections.Generic;
using AnotherRpgMod.RPGModule;

namespace AnotherRpgMod.Items
{
    public struct ItemStat
    {
        public Stat stat;
        public float value;

        public ItemStat(Stat stat, float value)
        {
            this.stat = stat;
            this.value = value;
        }
    }

    public struct ItemStats
    {
        public List<ItemStat> Stats;

        public void CreateStat(ItemStat itemStat)
        {
            Stats.Add(itemStat);
        }
        public float GetStat(int key)
        {
            return Stats[key].value;
        }
    }
}
