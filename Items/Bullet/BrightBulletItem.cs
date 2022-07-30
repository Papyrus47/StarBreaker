using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class BrightBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("光明能量聚集器");
            Tooltip.SetDefault("使用光明之魂制作的稳定的聚集器,50%的概率不消耗");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 5;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<BrightBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.Next(10) >= 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(15).AddIngredient(ItemID.SoulofLight).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(10))
            {
                Player player = Main.player[projectile.owner];
                player.statLife += damage / 10;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                player.HealEffect(damage / 10);
            }
        }
    }
}
