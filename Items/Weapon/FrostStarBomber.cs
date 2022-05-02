using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using System.Collections.Generic;
using StarBreaker.Items.Bullet;
using StarBreaker.Projs.Bullets;
using Terraria.DataStructures;
using StarBreaker.Items.DamageClasses;

namespace StarBreaker.Items.Weapon
{
    public class FrostStarBomber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Star Bomber");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "霜星轰击者");
            Tooltip.SetDefault("左键蓄力,蓄力时间越长,射击次数越多,如蓄力达到最高值,则发射冰锥\n" +
                "右键使它自动瞄准地方,一段时间后轰击\n" +
                "星辰与冰霜的结合体");
        }
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.knockBack = 1.2f;
            Item.crit = 19;
            Item.channel = true;
            Item.DamageType = ModContent.GetInstance<EnergyDamage>();
            Item.rare = ItemRarityID.Blue;
            Item.useTime = Item.useAnimation = 13;
            Item.noMelee = true;
            Item.useAmmo = ModContent.ItemType<NebulaBulletItem>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 82;
            Item.height = 34;
            Item.shoot = ModContent.ProjectileType<Projs.EnergyDamage_Proj.FrostStarBomber>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
        }
        public override bool CanConsumeAmmo(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool AltFunctionUse(Player player) => true;//可以右键使用
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach(TooltipLine line in tooltips)
            {
                if(line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    line.OverrideColor = Color.Blue;
                }
            }
        }
    }
}
