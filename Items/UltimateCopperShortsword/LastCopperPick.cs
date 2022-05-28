using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperPick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜稿");
            Tooltip.SetDefault("没有它，你也不会有现在的成就");
        }
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 32;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.pick = 110;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 2;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlyPick>()] > 0)
            {
                return false;
            }
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
                Projectile.NewProjectile(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(default) * 8, ModContent.ProjectileType<FlyPick>(), Item.damage, Item.knockBack, player.whoAmI);
            }
            else
            {
                Item.noUseGraphic = false;
            }
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
