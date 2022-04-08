using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.DamageClasses
{
    public class FourDamage : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("Four Damgage");
            ClassName.AddTranslation((int)GameCulture.CultureName.Chinese, "混合伤害");
        }
        protected override float GetBenefitFrom(DamageClass damageClass) => 10f;
        public override bool CountsAs(DamageClass damageClass) => true;
    }
}
