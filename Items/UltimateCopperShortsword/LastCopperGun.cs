using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜枪");
            Tooltip.SetDefault("使所有射出的子弹都会使敌人200层流血\n" +
                "右键投掷出枪,命中方块或者击中敌人后消失(回到手中),如击中敌人施加300层流血(与上面效果叠加)");
        }
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.knockBack = 2.3f;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 32;
            Item.rare = ItemRarityID.Red;
            Item.width = 66;
            Item.height = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 40;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 10f;
            Item.UseSound = SoundID.Item101;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noUseGraphic = false;
            }
            return player.ownedProjectileCounts[ModContent.ProjectileType<LastCopperGunProj>()] == 0;
        }
        public override bool CanConsumeAmmo(Item weapon, Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, -2);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<LastCopperGunProj>();
            }
            Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.GetGlobalProjectile<Projs.StarBreakerGlobalProj>().Bloody = 200;
            return false;
        }
    }
}
