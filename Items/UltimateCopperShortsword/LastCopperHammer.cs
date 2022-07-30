using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜锤");
            Tooltip.SetDefault("\" 把墙砸了,然后我们出去 \"");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.Size = new Vector2(32, 32);
            Item.hammer = 110;
            Item.useTime = Item.useAnimation = 30;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.value = 9650;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.tileBoost = -1;
            Item.damage = 20;
            Item.knockBack = 1.5f;
            Item.crit = 2;
        }
        public override bool? UseItem(Player player)
        {
            Projectile.NewProjectile(null, player.position, Vector2.Zero, ModContent.ProjectileType<CopperHammerGas>(),
                Item.damage * 10, Item.knockBack, player.whoAmI, 0, (Main.MouseWorld - player.Center).SafeNormalize(default).X);
            return base.UseItem(player);
        }
    }
}
