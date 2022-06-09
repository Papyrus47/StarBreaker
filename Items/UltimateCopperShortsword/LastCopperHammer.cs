using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜锤");
            Tooltip.SetDefault("\" 把墙砸了,然后我们出去 \"" +
                "\n丢出去后飞回,破坏路径上所有的墙");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.Size = new Vector2(32, 32);
            Item.hammer = 110;
            Item.useTime = Item.useAnimation = 4;
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
            Item.noUseGraphic = true;
        }
        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).RealSafeNormalize() * 20f, ModContent.ProjectileType<LastCopperHammerProj>(), player.GetWeaponDamage(Item),
                    player.GetWeaponKnockback(Item), player.whoAmI);
            }
            return true;
        }
    }
}
