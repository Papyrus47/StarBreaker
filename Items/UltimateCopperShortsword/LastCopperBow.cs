using StarBreaker.Projs.UltimateCopperShortsword;
using Terraria.ID;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜弓");
            Tooltip.SetDefault("不管怎么说,不要让它生锈\n" +
                "将射弹转换为铜箭,使用铜箭时提升10%的伤害");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0.2f;
            Item.shoot = ModContent.ProjectileType<PatinaArrow>();
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 30;
            Item.Size = new(24,42);
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            var sound = SoundID.Item5;
            sound.Volume += 1.9f;
            sound.Pitch = -0.7f;
            Item.UseSound = sound;
            Item.value = 100000;
            Item.crit = 36;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (type == Item.shoot)
                {
                    damage += (int)(damage * 0.1f);
                }
                velocity *= i * 0.7f;
                if (velocity.Length() > 20f) velocity = velocity.RealSafeNormalize() * 20f;
                Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.1),Item.shoot, damage, knockback, player.whoAmI);
                projectile.GetGlobalProjectile<Projs.StarBreakerGlobalProj>().ProjectileForLastBow = true;
                projectile.penetrate = 5;
            }
            return false;
        }
    }
}
