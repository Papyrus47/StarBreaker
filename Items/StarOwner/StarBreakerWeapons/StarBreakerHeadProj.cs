using StarBreaker.Content;

namespace StarBreaker.Items.StarOwner.StarBreakerWeapons
{
    public class StarBreakerHeadProj : SkillProj
    {
        public override string Texture => (GetType().Namespace + ".StarBreakerWeapon").Replace('.','/');

        private Player player;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("星辰击碎者");
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 40;
            Projectile.width = 142;
            Projectile.penetrate = -1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void PreSkillAI()
        {
            player = Main.player[Projectile.owner];
            if (player.dead || player.HeldItem.ModItem is not StarBreakerWeapon)
            {
                Projectile.Kill();
            }

            if (player.active)
            {
                Projectile.timeLeft = 2;
            }

            float rot = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
            {
                rot += MathHelper.Pi;
            }

            Projectile.rotation = rot;
        }
        public override void Init()
        {
            AddSkill(nameof(NotUse), new(NotUse));
            AddSkill(nameof(Attack1), new(Attack1, Draw));
            AddSkill(nameof(Attack2), new(Attack2));
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!InSkill(nameof(NotUse)))//近战
            {
                float r = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    Projectile.Center - Projectile.velocity.SafeNormalize(default) * Projectile.width / 2,
                    Projectile.Center + Projectile.velocity.SafeNormalize(default) * Projectile.width / 2,
                    Projectile.height, ref r);
            }
            return false;
        }
        public bool Draw(Color lightColor)
        {
            Vector2 origin;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            origin = texture.Size() * 0.5f;
            int length = Projectile.oldPos.Length;
            for (int i = 0; i < length; i++)
            {
                Color color = lightColor * ((length - i) / (float)length);
                if (i > 0)
                {
                    color = Color.Lerp(color, Color.Transparent, 0.7f);
                }

                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, color, Projectile.oldRot[i],
                    origin, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }
        public void NotUse()
        {
            Timer = 0;
            Projectile.spriteDirection = player.direction;
            Projectile.Center = player.Center + new Vector2((-player.width / 2 - Projectile.height / 2) * player.direction, player.gfxOffY);
            Projectile.velocity = Vector2.UnitY;
            if (player.controlUseTile)
            {
                ChangeSkill(nameof(Attack1));
            }
        }
        public void Attack1()
        {
            Timer++;
            float rot;
            if (Timer < 20)
            {
                Projectile.spriteDirection = player.direction;
                Projectile.Center = player.Center + new Vector2((-player.width / 2 - Projectile.height / 2) * player.direction, player.gfxOffY - Timer * 2);
            }
            else
            {
                if (Timer == 50)
                {
                    var sound = SoundID.Item1.WithPitchOffset(-0.8f);
                    sound.MaxInstances = 5;
                    sound.PitchVariance = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        SoundEngine.PlaySound(sound, player.Center);
                    }
                }
                rot = MathHelper.ToRadians((Timer - 40f) * 5f);

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.spriteDirection);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
                player.itemTime = player.itemAnimation = 2;
                player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

                if (rot > MathHelper.ToRadians(170))
                {
                    Projectile.extraUpdates = 0;
                    Projectile.velocity = (Projectile.velocity * 5 - (player.direction * Vector2.UnitX).RotatedBy(-0.9 * player.direction) * (Projectile.width / 2 - 14)) / 6;
                    if (Timer > 100)
                    {
                        if (Timer < 150 && player.controlUseTile)
                        {
                            Timer = 0;
                            ChangeSkill(nameof(Attack2));
                        }
                        else if (Timer >= 170)
                        {
                            ChangeSkill(nameof(NotUse));
                        }

                    }
                    else if (player.controlUseTile)
                    {
                        Timer = 0;
                        ChangeSkill(nameof(Attack1));
                    }
                    return;
                }

                Projectile.extraUpdates = 3;
                Projectile.velocity = (-Vector2.UnitY).RotatedBy(rot * Projectile.spriteDirection) * (Projectile.width / 2 - 14);

            }
        }

        public void Attack2()
        {
            Timer++;//蓄力
            player.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Projectile.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            if (Timer < 40)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.velocity = (Projectile.velocity * 5 + (Main.MouseWorld - player.Center).SafeNormalize(default)) / 6;
                    Projectile.velocity.Normalize();
                    player.itemRotation = (-Projectile.velocity * Projectile.direction).ToRotation();
                    Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * (Projectile.width / 2 - 14);
                    Projectile.Center -= Projectile.velocity;
                    Projectile.position.Y += player.gfxOffY;
                }
            }
            else if (Timer < 45)//刺出
            {
                player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * (Projectile.width / 2 - 14) * (Timer - 40);
                Projectile.Center -= Projectile.velocity;
                Projectile.position.Y += player.gfxOffY;
                Main.instance.CameraModifiers.Add(new Terraria.Graphics.CameraModifiers.PunchCameraModifier(player.Center, Projectile.velocity, 10, 5, 3));//屏幕震动
            }
            else if (Timer < 70)//保持不动
            {
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * (Projectile.width / 2 - 14);
                Projectile.position.Y += player.gfxOffY;
                if (Timer == 45)//产生粒子
                {
                    var sound = SoundID.Item1.WithPitchOffset(2f);
                    sound.PitchVariance = 0;
                    sound.MaxInstances = 5;
                    for (int i = 0; i < 60; i++)
                    {
                        if (i < 5)
                        {
                            SoundEngine.PlaySound(sound, Projectile.Center);
                        }
                        float dustRot = MathHelper.TwoPi / 60 * i;
                        Vector2 center = Projectile.Center + Projectile.velocity * 30 + dustRot.ToRotationVector2() * 60;
                        //TheBall ball = new()
                        //{
                        //    TimeLeft = 80,
                        //    color = Color.Purple,
                        //    Scale = Vector2.One * 2f,
                        //    ScaleVelocity = Vector2.One * -0.1f
                        //};
                        //ball.SetBasicInfo(StarBreakerAssetTexture.TheBall, null, (Projectile.Center - center) * 0.25f, Projectile.Center);
                        //ball.Velocity.Y *= 0.05f;
                        //if (dustRot > MathHelper.PiOver2 && dustRot < MathHelper.PiOver2 + MathHelper.Pi) ball.Velocity.X *= 0.2f;
                        //else if (Projectile.direction == -1) ball.Velocity.X *= 2f;
                        //ball.Velocity = ball.Velocity.RotatedBy((-Projectile.velocity).ToRotation());
                        //ball.Velocity += player.velocity;
                        //StarBreakerUtils.AddParticle(ball);
                        Dust dust = Dust.NewDustDirect(Projectile.Center, 5, 5, DustID.RedTorch);
                        dust.scale *= 2.3f;
                        dust.velocity = (dust.position - center) * 0.25f;
                        dust.velocity.Y *= 0.15f;
                        if (dustRot > MathHelper.PiOver2 && dustRot < MathHelper.PiOver2 + MathHelper.Pi)
                        {
                            dust.velocity.X *= 0.2f;
                        }
                        else if (Projectile.direction == -1)
                        {
                            dust.velocity.X *= 2f;
                        }

                        dust.velocity = dust.velocity.RotatedBy((-Projectile.velocity).ToRotation());
                        dust.velocity += player.velocity;
                        dust.noGravity = true;
                    }
                }
            }
            else
            {
                Timer = 0;
                ChangeSkill(nameof(NotUse));
            }
        }

        public override void Init_SkillChange()
        {
        }
    }
}