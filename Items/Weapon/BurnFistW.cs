using Microsoft.Xna.Framework;
using StarBreaker.Projs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class BurnFistW : ModItem
    {
        public int burnFistProjWhoAmI = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰拳套-炎拳");
            Tooltip.SetDefault("炎拳正在灼烧附近的事物");
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
            if (burnFistProjWhoAmI == -1 && player.ownedProjectileCounts[ModContent.ProjectileType<BurnFistProj>()] == 0)
            {
                burnFistProjWhoAmI = Projectile.NewProjectile(player.GetProjectileSource_Item(Item), player.position, Vector2.Zero, ModContent.ProjectileType<BurnFistProj>(),
                    Item.damage, Item.knockBack, player.whoAmI, -1);
            }
        }
        public override bool? UseItem(Player player)
        {
            Main.projectile[burnFistProjWhoAmI].ai[0]++;
            Main.projectile[burnFistProjWhoAmI].ai[1] = 0;
            return base.UseItem(player);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria")
                {
                    if (line.Name == "Tooltip0")
                    {
                        line.overrideColor = new Color(200, 100, 100, 0);
                    }
                    else if (line.Name == "ItemName")
                    {
                        line.text = "炎拳";
                        line.overrideColor = new Color(200, 100, 100, 0);
                    }
                }
            }
        }
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem != Item && player.HeldItem.type != ModContent.ItemType<FrostFistW>())
            {
                burnFistProjWhoAmI = -1;
            }
            else if (player.HeldItem.type == ModContent.ItemType<FrostFistW>() && player.ownedProjectileCounts[ModContent.ProjectileType<BurnFistProj>()] == 0)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Item(Item), player.position, Vector2.Zero, ModContent.ProjectileType<BurnFistProj>(),
                    Item.damage, Item.knockBack, player.whoAmI, -1);
            }
        }
    }
}
