using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace StarBreaker.Items.Weapon.EnergyWeapon
{
    public class ToDarkFoxSword : BaseEnergySummon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("至暗遗狐");
            Tooltip.SetDefault("召唤攻击中敌人一次就消失的\"至暗遗狐\"\n" +
                "召唤物受到能量充能器的影响");
        }
        public override void NewSetDef()
        {
            base.NewSetDef();
            Item.damage = 43;
            Item.crit = 0;
            Item.knockBack = 0;
            Item.useTime = 10;
            Item.useAnimation = 12;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<Projs.EnergyDamage_Proj.Summon.ToDarkFoxSword>();
            Item.width = Item.height = 62;
        }
    }
}
