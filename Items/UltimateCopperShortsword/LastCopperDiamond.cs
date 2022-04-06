using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperDiamond : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜钻");
            Tooltip.SetDefault("没错，铜可以做钻头");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(40, 40);
            Item.UseSound = SoundID.Item1;
            Item.damage = 130;
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
            Item.shoot = ModContent.ProjectileType<LastCopperDiamondProj>();
            Item.shootSpeed = 20;
            Item.pick = 110;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetProjectileSource_Item(Item), position, (Main.MouseWorld - player.position).SafeNormalize(default) * 20, type,
                Item.damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
