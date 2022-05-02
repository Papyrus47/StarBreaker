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
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return new StatInheritanceData(10, 10, 0, 0, 0);
        }
        public override bool GetEffectInheritance(DamageClass damageClass) => true;
    }
}
