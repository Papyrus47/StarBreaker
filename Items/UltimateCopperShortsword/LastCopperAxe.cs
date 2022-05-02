using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜斧");
            Tooltip.SetDefault("带铜绿之斧,遇水就锈");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 0.2f;
            Item.damage = 30;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
            Item.UseSound = SoundID.Item1;
            Item.axe = 22;
            Item.crit = 19;
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player)
        {
            Item.useTime = 10;
            for (int i = -1; i <= 1; i++)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(default).RotatedBy(i * MathHelper.Pi / 18) * 10, ModContent.ProjectileType<CopperFlyAxe>(), Item.damage, Item.knockBack, player.whoAmI);
            }
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return false;
        }
    }
}
