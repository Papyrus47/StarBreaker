namespace StarBreaker.Items.StarOwner.StarBreakerWeapons
{
    public class StarBreakerWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star Breaker");
            //DisplayName.AddTranslation(7, "星辰击碎者");
            /* Tooltip.SetDefault("测试技能表\n"
                + "根据玩家朝向与移动方向是否一致,决定是否为向前运动\n"
                + "左键使用远程攻击,右键使用近战攻击\n"
                + "近战出招表:\n"
                + "右键+右键:使用两段挥舞\n"
                + "右键+停顿+右键:挥舞后接刺击\n"
                + "注意:方向键与玩家本人会影响出招"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 25;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<StarBreakerHeadProj>();
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center),
                    Item.shoot, player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
            }
        }
    }
}