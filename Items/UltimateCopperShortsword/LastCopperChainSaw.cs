using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperChainSaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜链锯");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(40, 40);
            Item.UseSound = SoundID.Item1;
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 64;
            Item.knockBack = 3.4f;
            Item.value = 99999;
            Item.useTurn = true;
            Item.useTime = Item.useAnimation = 10;
            Item.rare = ItemRarityID.Red;
            Item.width = 44;
            Item.height = 22;
            Item.mana = 0;
            Item.useStyle = 5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LastCopperChainSawProj>();
            Item.shootSpeed = 10;
            Item.axe = 22;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
