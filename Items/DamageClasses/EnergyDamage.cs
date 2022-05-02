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
        public override bool GetEffectInheritance(DamageClass damageClass) => true;
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
            {
                return StatInheritanceData.Full;
            }
            return StatInheritanceData.None;
        }
    }
}
