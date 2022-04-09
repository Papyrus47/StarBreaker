using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword;
using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜矛");
            Tooltip.SetDefault("使用时额外投出铜矛,谁不喜欢?");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 0.3f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.value = 24300;
            Item.Size = new Vector2(54, 54);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.damage = 30;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<LastCopperSpearProj>();
            Item.shootSpeed = 2;
        }
    }
}
