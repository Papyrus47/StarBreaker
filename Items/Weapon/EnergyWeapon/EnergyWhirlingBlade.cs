using StarBreaker.Items.Type;

namespace StarBreaker.Items.Weapon.EnergyWeapon
{
    public class EnergyWhirlingBlade : BaseEnergyMelee
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "能量回旋刃");
            Tooltip.SetDefault("消耗背包的能量子弹,扔出这把剑,最多存在10把");
        }
        public override void NewSetDef()
        {
            base.NewSetDef();
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 12;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.width = 80;
            Item.height = 94;
            Item.damage = 43;
            Item.knockBack = 2.1f;
            Item.crit = 29;
            Item.rare = ItemRarityID.Pink;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<Projs.EnergyDamage_Proj.EnergyWhirlingBlade>()] < 10;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<Projs.EnergyDamage_Proj.EnergyWhirlingBlade>();
            velocity = (Main.MouseWorld - position) * 0.05f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Item item = new(source.AmmoItemIdUsed);
            if (item.ModItem is EnergyBulletItem bullet)
            {
                StarBreakerUtils.Add_Hooks_ToProj(bullet, proj);
            }
            return false;
        }
    }
}
