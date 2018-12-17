
namespace AnotherRpgMod.Items
{
    struct RarityWeight
    {
        public Rarity rarity;
        public float weight;

        public RarityWeight(Rarity rarity, float weight)
        {
            this.rarity = rarity;
            this.weight = weight;
        }
    }
}
