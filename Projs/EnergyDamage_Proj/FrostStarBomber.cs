using StarBreaker.Projs.Bullets;
using StarBreaker.Projs.IceGun;

namespace StarBreaker.Projs.EnergyDamage_Proj
{
    public class FrostStarBomber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Star Bomber");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "霜星轰击者");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 20;
            Projectile.width = 82;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage()
        {
            return false;//避免造成伤害
        }

        public override bool ShouldUpdatePosition()
        {
            return false;//避免更新位置
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            Projectile.timeLeft = 2;
            #region 手持
            if (Projectile.owner == Main.myPlayer)//单机检测,虽然星击不多人,但是怕星辰拳套搞乱
            {
                Projectile.velocity = (Projectile.Center - Main.MouseWorld).RealSafeNormalize() * 10f;
                Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
                Projectile.position = player.Top + new Vector2(0, -100);
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
                player.ChangeDir(-Projectile.direction);
                player.heldProj = Projectile.whoAmI;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                    Projectile.velocity.X * Projectile.direction);
            }
            #endregion
            #region 计时,如果小于180,那么就高速发射子弹,大于等于的时候发射冰锥
            if ((Projectile.ai[1] < 180 && !player.channel) || Projectile.ai[0] == 1)
            {
                Projectile.ai[0] = 1;//发射状态
                Projectile.ai[1]--;
                if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] % 3 == 0)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + (Main.rand.NextVector2Unit() * 10)
                        , -Projectile.velocity,
                        ModContent.ProjectileType<IceEnergyBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].friendly = true;
                    StarBreakerWay.PickAmmo_EnergyBulletItem(player, out int shootID, out int damage);
                    Projectile.damage += 2;
                    StarBreakerWay.Add_Hooks_ToProj(shootID, proj);

                }
                if (Projectile.ai[1] <= 0)
                {
                    Projectile.Kill();
                }
            }
            else if (!player.channel)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    StarBreakerWay.PickAmmo_EnergyBulletItem(player, out int damage, out int shootID);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + (Main.rand.NextVector2Unit() * 10)
                        , -Projectile.velocity,
                        ModContent.ProjectileType<IcePick>(), Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner);
                }
                Projectile.Kill();
            }
            else
            {
                Projectile.ai[1]++;
            }
            #endregion
        }
    }
}
