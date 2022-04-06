using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperSickle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜镰刀");
            Tooltip.SetDefault("据说它被隔壁的旋刃影响了");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.Size = new Vector2(52, 42);
            Item.damage = 150;
            Item.knockBack = 1.1f;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.value = 114510;
            Item.rare = ItemRarityID.Purple;
            Item.crit = 21;
            Item.shoot = ModContent.ProjectileType<LastCopperSickleProj>();
            Item.shootSpeed = 10;
        }
        public override bool CanShoot(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] >= 3)
                return false;
            return base.CanShoot(player);
        }
    }
}
