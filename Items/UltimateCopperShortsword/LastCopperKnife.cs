using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜投刀");
            Tooltip.SetDefault("你管飞刀是投刀?");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 0.14f;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.value = 24300;
            Item.Size = new Vector2(14, 44);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.damage = 73;
            Item.crit = 21;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<LastCopperKnifeProj>();
            Item.shootSpeed = 6;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.active && projectile.type == type && projectile.ai[0] == 1)
                    {
                        projectile.ai[1] = 2;
                        projectile.velocity *= 1.3f;
                        return false;
                    }
                }
                Projectile.NewProjectile(null, position, velocity, type, damage, knockback, player.whoAmI, 1);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
