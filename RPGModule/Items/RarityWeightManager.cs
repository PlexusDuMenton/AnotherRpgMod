 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Utils;

namespace AnotherRpgMod.Items
{
    class RarityWeightManager
    {
        public RarityWeight[] weights;

        public RarityWeightManager(RarityWeight[] weights)
        {
            this.weights = weights;
        }

        public Rarity DrawRarity()
        {
            int bossKilled = WorldManager.BossDefeated;

                float totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
                totalWeight += weights[i].weight;
            float rn = Mathf.Random(0, totalWeight);

            rn += bossKilled * totalWeight * 0.02f;
            float actualWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                if (rn < actualWeight + weights[i].weight)
                    return weights[i].rarity;
                actualWeight += weights[i].weight;
            }
            return weights[weights.Length-1].rarity;


        }
    }
}
