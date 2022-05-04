using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace StarBreaker.Projs.XuanYu
{
    public class FrostSpearProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Spear");
            DisplayName.AddTranslation(7, "寒霜刺枪");
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.Size = new Vector2(66);
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];//获取玩家
            if(!player.active)
            {
                Projectile.Kill();
                return;
            }
            for (int j = Projectile.oldRot.Length - 1; j > 0; j--)
            {
                Projectile.oldRot[j] = Projectile.oldRot[j - 1];
            }
            Projectile.oldRot[0] = Projectile.rotation;
            player.itemTime = player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            switch(Projectile.ai[0])
            {
                case 0://旋转
                    {
                        Projectile.localNPCHitCooldown = 5;
                        Projectile.rotation += 0.35f;
                        if(Projectile.timeLeft % 5 == 0)
                        {
                            Projectile.velocity *= 0.9f;
                        }
                        if(Projectile.velocity.Length() < 5f)
                        {
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 1://回归
                    {
                        Projectile.timeLeft = 300;
                        Projectile.rotation += 0.35f;
                        Projectile.velocity = (Projectile.velocity * 6 + (player.Center - Projectile.Center).RealSafeNormalize() * 20) / 7;
                        Projectile.ai[1]++;
                        if(Projectile.Center.Distance(player.Center) < 33f || Projectile.ai[1] > 240)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                        }
                        break;
                    }
                case 2://抓住冲刺
                    {
                        Projectile.extraUpdates = 1;
                        Projectile.tileCollide = false;
                        Projectile.localNPCHitCooldown = -2;
                        Projectile.timeLeft = 300;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                        Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.RealSafeNormalize() * 33f;
                        if (Main.myPlayer == player.whoAmI)
                        {
                            if (Projectile.ai[1] == 0)
                            {
                                Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 30f;
                                for(int i =0;i<Projectile.localNPCImmunity.Length;i++)
                                {
                                    Projectile.localNPCImmunity[i] = 0;
                                }
                                if(Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Projectile.velocity * 5,
                                        Projectile.velocity.RealSafeNormalize(), ModContent.ProjectileType<FrostSpearPiercing>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                                }
                            }
                            else
                            {
                                if(Projectile.ai[1] % 5 == 0)Projectile.velocity *= 0.9f;
                                if (Projectile.velocity.Length() < 5)
                                {
                                    Projectile.Kill();
                                    return;
                                }
                            }
                            Projectile.ai[1]++;
                            player.velocity = Projectile.velocity;
                            player.ChangeDir((player.velocity.X > 0).ToDirectionInt());
                            Projectile.Center -= Projectile.velocity;
                            player.immune = true;
                            player.immuneTime = 5;
                            player.immuneAlpha = 200;
                        }
                        break;
                    }
                default:
                    {
                        Projectile.ai[0] = 0f;
                        break;
                    }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] < 2)
            {
                target.GetGlobalNPC<NPCs.StarGlobalNPC>().XuanYuSlowTime = 300;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 33f,
                Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -33f,
                8,ref r);
        }
    }
}
