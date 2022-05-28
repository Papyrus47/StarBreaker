namespace StarBreaker.Items.Weapon.HradMode.Waste
{
    public class StarsPierce : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Broad Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "繁星刺破");
            Tooltip.SetDefault("刺击\n" +
                "命中敌人时,施加50层流血");
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 1200;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3.2f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 61;
            Item.mana = 2;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = 5130142;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<Projs.Waste.StarsPierceProj>();
            Item.shootSpeed = 10;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage / 2, knockback, player.whoAmI, 0, 0);
                projectile.extraUpdates = 1;
                projectile.localAI[0] = 1f;
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
