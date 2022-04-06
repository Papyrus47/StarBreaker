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
            Tooltip.SetDefault("左键插矛，右键投矛，谁不喜欢?");
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(null, position, velocity * 5, ModContent.ProjectileType<FlySpearProj>(),
                    damage / 4, knockback, Main.myPlayer);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 5;
                Item.useAnimation = 16;
                Item.autoReuse = true;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.channel = false;
            }
            else
            {
                Item.reuseDelay = 0;
                Item.useTime = Item.useAnimation = 20;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.channel = true;
            }
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
