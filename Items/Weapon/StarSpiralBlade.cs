using Microsoft.Xna.Framework;
using StarBreaker.Items.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class StarSpiralBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
            Tooltip.SetDefault("星辰旋刃,具有可怕的力量,和星辰鬼刀一样,根据主人的强度而改变自己的旋转速度,旋转速度决定其伤害\n" +
                "星辰之主的依靠武器,掌握神秘的回旋力量,速度达到一定时候就可以使用特殊技能");
        }
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.knockBack = 1.2f;
            Item.DamageType = ModContent.GetInstance<FourDamage>();
            Item.rare = ItemRarityID.Purple;
            Item.crit = 23;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 10;
            Item.channel = true;
            Item.width = Item.height = 80;
            Item.shoot = ModContent.ProjectileType<Projs.StarSpiralBladeProj>();
            Item.shootSpeed = 10;
            Item.noUseGraphic = true;

        }
        public override bool CanUseItem(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[Item.shoot] == 0)
            {
                int ai0;
                if (player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed)
                {
                    ai0 = 1;
                }
                else
                {
                    ai0 = 0;
                }
                Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, type, damage, knockback, player.whoAmI, ai0)].originalDamage = damage;
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
