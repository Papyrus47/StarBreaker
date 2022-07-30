namespace StarBreaker.Items.Prefix
{
    public class EnergyEnhancement : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Ranged;
        public override bool CanRoll(Item item)
        {
            return true;
        }
        public override float RollChance(Item item)
        {
            return 2.3f;
        }
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            useTimeMult *= 0.8f;
            shootSpeedMult *= 1.2f;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 0.8f;
        }
        public override void Apply(Item item)
        {
            base.Apply(item);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("能源循环");
        }
    }
}
