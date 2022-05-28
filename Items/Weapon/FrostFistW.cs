using StarBreaker.Projs;

namespace StarBreaker.Items.Weapon
{
    public class FrostFistW : ModItem
    {
        public int frostFistProjWhoAmI = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰拳套-霜拳");
            Tooltip.SetDefault("霜拳的气息让周围寒冷");
        }
        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 3.2f;
            Item.rare = ItemRarityID.Red;
            Item.channel = true;
            Item.width = 16;
            Item.height = 27;
            Item.holdStyle = ItemHoldStyleID.HoldHeavy;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 30;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 1;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            if (!player.GetModPlayer<StarPlayer>().InIdeaDriven)
            {
                Projectile.NewProjectile(null, player.position + new Vector2(0, -100), new Vector2(0, 5), ModContent.ProjectileType<IceMaker>(), 0, 0, player.whoAmI);
            }
            if (frostFistProjWhoAmI == -1 && player.ownedProjectileCounts[ModContent.ProjectileType<FrostFistProj>()] < 1)
            {
                frostFistProjWhoAmI = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position, Vector2.Zero, ModContent.ProjectileType<FrostFistProj>(),
                    Item.damage, Item.knockBack, player.whoAmI, -1);
            }
        }
        public override bool? UseItem(Player player)
        {
            if (frostFistProjWhoAmI >= 0)
            {
                Main.projectile[frostFistProjWhoAmI].ai[0]++;
                Main.projectile[frostFistProjWhoAmI].ai[1] = 0;
            }
            return base.UseItem(player);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria")
                {
                    if (line.Name == "Tooltip0")
                    {
                        line.OverrideColor = new Color(100, 100, 200, 0);
                    }
                    else if (line.Name == "ItemName")
                    {
                        line.Text = "霜拳";
                        line.OverrideColor = new Color(100, 100, 200, 0);
                    }
                }
            }
        }
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem != Item)
            {
                frostFistProjWhoAmI = -1;
                player.GetModPlayer<StarPlayer>().InIdeaDriven = false;
            }
        }
    }
}
