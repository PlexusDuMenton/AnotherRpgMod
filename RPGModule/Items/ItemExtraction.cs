using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherRpgMod.Items
{
    static class ItemExtraction
    {
        static public long GetTotalEarnedXp(ItemUpdate item)
        {
            long xp = 0;
            int level = item.Level;
            int ascLevel = item.Ascention;


            for (int j = 0; j < item.Level; j++)
            {
                xp += item.GetExpToNextLevel(j, ascLevel);
            }

            return xp;
        }

        static public float GetExtractedXp(bool Destroy, ItemUpdate item)
        {
            float exp = 0;

            if (Destroy)
            {
                exp = GetTotalEarnedXp(item) * 0.5f;
            }
            else
            {
                exp = GetTotalEarnedXp(item) * 0.25f;
            }
            return exp;
        }
    }
}
