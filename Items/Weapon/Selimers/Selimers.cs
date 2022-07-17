using Terraria.ID;

namespace StarBreaker.Items.Weapon.Selimers
{
    public class Selimers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7,"自我");
            Tooltip.SetDefault("释放内心深处的怒火");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3,15));
        }
        public override void SetDefaults()
        {
            Item.damage = 180;
            Item.knockBack = 3.4f;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.Size = new(40,42);
            Item.useTurn = false;
            Item.mana = 10;
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SelimersProj>();
            Item.shootSpeed = 15f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void HoldItem(Player player)
        {
            Vector2 value = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if(player.direction != -1)//转向
            {
                value.X = player.bodyFrame.Width - value.X;
            }

            if(player.gravDir != 1f)//重力转向
            {
                value.Y = player.bodyFrame.Height - value.Y;
            }
            value -= new Vector2(player.bodyFrame.Width - player.width,player.bodyFrame.Height - 42) / 2f;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter - new Vector2(20f, 42f) / 2f + value) - player.velocity;
            for(int i = 0;i<4;i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center,0,0,DustID.BlueTorch);
                dust.position = pos;
                dust.velocity *= 0f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity += player.velocity; 
                if (Main.rand.NextBool(2))
                {
                    dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
                    dust.scale += Main.rand.NextFloat();
                    if (Main.rand.NextBool(2))
                        dust.customData = player;
                }
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2 && player.ownedProjectileCounts[type] == 0)//右键
            {
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[proj].alpha = 255;
            }
            else
            {
                for(int i = 0;i<2;i++)
                {
                    position -= velocity.RotatedByRandom(0.1);
                    velocity = (new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition - position).RealSafeNormalize() * 15f;
                    int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].alpha = 0;
                }
            }
            return false;
        }
    }
}
