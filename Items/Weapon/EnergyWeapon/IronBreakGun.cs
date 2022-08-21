using StarBreaker.Items.Type;

namespace StarBreaker.Items.Weapon.EnergyWeapon
{
    public class IronBreakGun : BaseEnergyRanged
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "碎铁枪");
            Tooltip.SetDefault("发射5连散弹\n" +
                "如果背包中有额外的能量子弹,那么消耗在背包所有的能量子弹(至多2种),换取发射出去的子弹的被动能力,同时增加发射的伤害\n" +
                "具有近战伤害");
        }
        public override void NewSetDef()
        {
            base.NewSetDef();
            Item.width = 126;
            Item.height = 32;
            Item.crit = 11;
            Item.damage = 11;
            Item.knockBack = 1.5f;
            Item.useTime = Item.useAnimation = 35;
            Item.noMelee = false;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 50).AddIngredient(ItemID.IllegalGunParts).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            List<EnergyBulletItem> bulletItems = new();
            foreach (Item item in player.inventory)
            {
                if (bulletItems.Count >= 2)
                {
                    break;
                }

                if (item.ammo == Item.useAmmo && item.shoot != type && item.ModItem is EnergyBulletItem bullet)
                {
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                    }

                    bulletItems.Add(bullet);
                }
            }
            for (float j = -2; j <= 2; j++)
            {
                Vector2 vel = (j.ToRotationVector2() * MathHelper.Pi / 2) + velocity;
                int proj = Projectile.NewProjectile(source, position + velocity, vel, type, damage, knockback, player.whoAmI, 0, -1);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
                if (bulletItems.Count > 0)
                {
                    foreach (EnergyBulletItem bullet in bulletItems)
                    {
                        StarBreakerUtils.Add_Hooks_ToProj(bullet, proj);
                    }
                }
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, 5);
        }
    }
}
