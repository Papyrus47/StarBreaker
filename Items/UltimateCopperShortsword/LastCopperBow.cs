using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜弓");
            Tooltip.SetDefault("不管怎么说,不要让它生锈");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0.2f;
            Item.shoot = ModContent.ProjectileType<PatinaArrow>();
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 30;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
            Item.UseSound = SoundID.Item101;
            Item.crit = 36;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (type == Item.shoot)
                {
                    damage += (int)(damage * 0.5f);
                }
                velocity *= 1 + i / 5;
                Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                projectile.GetGlobalProjectile<Projs.StarBreakerGlobalProj>().ProjectileForLastBow = true;
                projectile.penetrate = 5;
            }
            return false;
        }
    }
}
