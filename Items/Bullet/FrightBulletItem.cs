using StarBreaker.Projs.Bullets;

namespace StarBreaker.Items.Bullet
{
    public class FrightBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("恐惧能量聚集器");
            Tooltip.SetDefault("使用恐惧之魂制作的稳定的聚集器,50%的概率不消耗\n" +
                "这种能量使人恐惧");
            Item.SacrificeCountNeededByItemId(100);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 21;
            Item.damage = 7;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<FrightBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Player player)
        {
            return Main.rand.Next(10) >= 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ItemID.SoulofFlight).AddTile(TileID.MythrilAnvil).Register();
        }
        public override void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life < target.lifeMax * 0.5f)
            {
                target.NPC_AddOnHitDamage(damage);
            }
        }
    }
}
