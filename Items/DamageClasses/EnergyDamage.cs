using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.DamageClasses
{
    public class EnergyDamage : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("Energy Damage");
            ClassName.AddTranslation((int)GameCulture.CultureName.Chinese, "能量伤害");
        }
        protected override float GetBenefitFrom(DamageClass damageClass)
        {
            if (damageClass == Generic)
            {
                return 1f;
            }
            if (damageClass == Ranged)
            {
                return 0.8f;
            }
            return 0f;
        }
        public override bool CountsAs(DamageClass damageClass)
        {
            return false;
        }
    }
}
