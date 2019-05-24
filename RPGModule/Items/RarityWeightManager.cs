 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            float totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
                totalWeight += weights[i].weight;
            float rn = Utils.Mathf.Random(0, totalWeight);
            float actualWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                if (rn < actualWeight + weights[i].weight)
                    return weights[i].rarity;
                actualWeight += weights[i].weight;
            }
            return weights[0].rarity;


        }
    }
}
