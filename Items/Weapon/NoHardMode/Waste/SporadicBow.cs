namespace StarBreaker.Items.Weapon.NoHardMode.Waste
{
    public class SporadicBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("零星弓");
            Tooltip.SetDefault("用特殊材料做的复合弓");
        }
        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 12;
            Item.knockBack = 2.1f;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.width = 44;
            Item.height = 74;
            Item.value = 54000;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<Projs.Waste.SporadicBowProj>();
            Item.shootSpeed = 1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override bool CanConsumeAmmo(Item weapon, Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.FallenStar, 30).AddIngredient(RecipeGroupID.IronBar, 20).AddRecipeGroup(RecipeGroupID.GoldenCritter, 20)
.AddTile(TileID.Anvils).Register();
        }
    }
}
