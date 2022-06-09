using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class SightBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("视域能量聚集器");
            Tooltip.SetDefault("使用视域之魂制作的稳定的聚集器,50%的概率不消耗\n" +
                "你感觉有这种能量在盯着你");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 32;
            Item.damage = 7;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SightBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.Next(10) >= 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ItemID.SoulofSight).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void ProjAI(Projectile Projectile)
        {
            if (Projectile.extraUpdates != 2)
            {
                Projectile.penetrate = 5;
                Projectile.extraUpdates = 2;
            }
        }
    }
}
