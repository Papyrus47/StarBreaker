using StarBreaker.Items.Type;
using StarBreaker.Items.Weapon;
using StarBreaker.Particle;
using StarBreaker.Projs.UltimateCopperShortsword;

namespace StarBreaker.Projs
{
    public class StarBreakerHeadProj : ModProjectile
    {
        private int State
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public Player player;
        public Asset<Texture2D> texture;
        public override string Texture => "StarBreaker/Items/Weapon/StarBreakerW";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
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
        }
        public override void AI()
        {
            player ??= Main.player[Projectile.owner];
            Item item = player.HeldItem;
            float rot = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1) rot += MathHelper.Pi;
            Projectile.rotation = rot;
            if (item.ModItem is not StarBreakerW)
            {
                Projectile.Kill();
                return;
            }
            if (player.channel && State == 0)//左键持续使用
            {
                Timer++;
                player.itemTime = player.itemAnimation = 2;
                player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                if(Timer < 20)//飞出
                {
                    Projectile.position.Y -= 0.9f * Timer;
                }
                else //开火/正常移动
                {
                    Projectile.spriteDirection = Projectile.direction;
                    if (Timer > 50)//开火
                    {
                        Timer = 20;
                        if(StarBreakerUtils.StarBrekaerUseBulletShoot(player,out int shootID,out int shootDamage,out EnergyBulletItem bulletItem))//发射弹药
                        {
                            for (float i = -5; i <= 5; i++)
                            {
                                Vector2 vel = (i.ToRotationVector2() * MathHelper.Pi / 18) + Projectile.velocity.RealSafeNormalize();
                                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel * 10,shootID,Projectile.damage + shootDamage, 1.2f, Main.myPlayer);
                                StarBreakerUtils.Add_Hooks_ToProj(bulletItem, proj);
                                Main.projectile[proj].hostile = false;
                                Main.projectile[proj].friendly = true;
                            }
                            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                        }
                        else
                        {
                            Timer = 0;
                            State = -1;//装弹
                        }
                    }
                    if (Main.myPlayer == player.whoAmI)
                    {
                        if (Projectile.ai[0] == 0f)//靠近鼠标
                        {
                            Projectile.velocity = (Projectile.velocity * 10 + (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 5f) / 11f;
                            if (Projectile.velocity.Length() < 5f)
                            {
                                Projectile.position -= Projectile.velocity;//固定位置
                            }
                            if (Vector2.Distance(Main.MouseWorld, Projectile.Center) > 400f)
                            {
                                Projectile.ai[0] = 0.5f;//重新移动
                            }
                        }
                        else if (Projectile.ai[0] == 0.5f)//追随鼠标
                        {
                            Projectile.velocity = (Projectile.velocity * 10 + (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 20f)/11f;
                            if(Vector2.Distance(Main.MouseWorld,Projectile.Center) < 100f)
                            {
                                Projectile.ai[0] = 0f;//确定点,取消移动
                            }
                        }
                    }
                }
            }
            else//右键给你一刀,或者待机(
            {
                switch(State)
                {
                    case 0://待机
                        {
                            if(player.controlUseTile)//使用了右键
                            {
                                State++;
                            }
                            Timer = 0;
                            Projectile.spriteDirection = player.direction;
                            Projectile.Center = player.Center + new Vector2((-player.width / 2 - Projectile.height / 2) * player.direction, player.gfxOffY);
                            Projectile.velocity = Vector2.UnitY;
                            break;
                        }
                    case 1://近战
                        {
                            Timer++; 
                            if (Timer < 20)
                            {
                                Projectile.spriteDirection = player.direction;
                                Projectile.Center = player.Center + new Vector2((-player.width / 2 - Projectile.height / 2) * player.direction, player.gfxOffY - Timer * 2);
                            }
                            else
                            {
                                if(Timer == 50)
                                {
                                    var sound = SoundID.Item1.WithPitchOffset(-0.8f);
                                    sound.MaxInstances = 5;
                                    sound.PitchVariance = 0;
                                    for (int i = 0; i < 5; i++)
                                    {
                                        SoundEngine.PlaySound(sound, player.Center);
                                    }
                                }
                                player.heldProj = Projectile.whoAmI;
                                player.ChangeDir(Projectile.spriteDirection);
                                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
                                player.itemTime = player.itemAnimation = 2;
                                player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);
                                rot = MathHelper.ToRadians((Timer - 40f) * 5f);
                                Projectile.extraUpdates = 3;
                                Projectile.velocity = (-Vector2.UnitY).RotatedBy(rot * Projectile.spriteDirection) * (Projectile.width / 2 - 14);
                                if(rot > MathHelper.ToRadians(170) && rot < MathHelper.Pi)
                                {
                                    Projectile.extraUpdates = 0;
                                    Timer = 0;
                                    State = player.controlUseTile ? 2 : 0;
                                }
                            }
                            break;
                        }
                    case 2://刺击
                        {
                            Timer++;//蓄力
                            player.ChangeDir(Projectile.direction);
                            Projectile.spriteDirection = Projectile.direction;
                            player.heldProj = Projectile.whoAmI;
                            player.itemTime = player.itemAnimation = 2;
                            if (Timer < 40)
                            {
                                if (Timer % 10 == 0)
                                {
                                    StarBreakerUtils.NewDustByYouself(Projectile.Center, DustID.YellowStarDust, () => true, Vector2.UnitX,
                                    5, 30, dust =>
                                    {
                                        dust.scale *= 0.4f;
                                        dust.fadeIn = 0.05f;
                                    });
                                }
                                if (Main.myPlayer == player.whoAmI)
                                {
                                    Projectile.velocity = (Projectile.velocity * 5 + (Main.MouseWorld - player.Center).RealSafeNormalize())/6;
                                    Projectile.velocity.Normalize();
                                    player.itemRotation = (-Projectile.velocity).ToRotation();
                                    Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + (Projectile.velocity * (Projectile.width / 2 - 14));
                                    Projectile.Center -= Projectile.velocity;
                                    Projectile.position.Y += player.gfxOffY;
                                }
                            }
                            else if (Timer < 45)//刺出
                            {
                                player.itemRotation = Projectile.velocity.ToRotation();
                                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + (Projectile.velocity * (Projectile.width / 2 - 14) * (Timer - 40));
                                Projectile.Center -= Projectile.velocity;
                                Projectile.position.Y += player.gfxOffY;
                                Main.instance.CameraModifiers.Add(new Terraria.Graphics.CameraModifiers.PunchCameraModifier(player.Center,Projectile.velocity,10,5,3));//屏幕震动
                            }
                            else if(Timer < 70)//保持不动
                            {
                                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + (Projectile.velocity * (Projectile.width / 2 - 14));
                                Projectile.position.Y += player.gfxOffY;
                                if (Timer == 45)//产生粒子
                                {
                                    var sound = SoundID.Item1.WithPitchOffset(2f);
                                    sound.PitchVariance = 0;
                                    sound.MaxInstances = 5; 
                                    for (int i =0;i < 60;i++)
                                    {
                                        if(i < 5)
                                        {
                                            SoundEngine.PlaySound(sound, Projectile.Center);
                                        }
                                        float dustRot = MathHelper.TwoPi/ 60 * i;
                                        Vector2 center = Projectile.Center + (Projectile.velocity * 30) + (dustRot.ToRotationVector2() * 60);
                                        TheBall ball = new()
                                        {
                                            TimeLeft = 80,
                                            color = Color.Purple,
                                            Scale = Vector2.One * 2f,
                                            ScaleVelocity = Vector2.One * -0.1f
                                        };
                                        ball.SetBasicInfo(StarBreakerAssetTexture.TheBall, null, (Projectile.Center - center) * 0.25f, Projectile.Center);
                                        ball.Velocity.Y *= 0.05f;
                                        if (dustRot > MathHelper.PiOver2 && dustRot < MathHelper.PiOver2 + MathHelper.Pi) ball.Velocity.X *= 0.2f;
                                        else if (Projectile.direction == -1) ball.Velocity.X *= 2f;
                                        ball.Velocity = ball.Velocity.RotatedBy((-Projectile.velocity).ToRotation());
                                        ball.Velocity += player.velocity;
                                        StarBreakerUtils.AddParticle(ball);
                                        //Dust dust = Dust.NewDustDirect(Projectile.Center, 5, 5, DustID.RedTorch);
                                        //dust.scale *= 2.3f;
                                        //dust.velocity = (dust.position - center) * 0.25f;
                                        //dust.velocity.Y *= 0.15f;
                                        //if (dustRot > MathHelper.PiOver2 && dustRot < MathHelper.PiOver2 + MathHelper.Pi) dust.velocity.X *= 0.2f;
                                        //else if (Projectile.direction == -1) dust.velocity.X *= 2f;
                                        //dust.velocity = dust.velocity.RotatedBy((-Projectile.velocity).ToRotation());
                                        //dust.velocity += player.velocity;
                                        //dust.noGravity = true;
                                    }
                                }
                            }
                            else
                            {
                                Timer = 0;
                                State = 0;
                            }
                            break;
                        }
                    case -1://装填
                        {
                            Timer++;
                            if(Timer == 5)
                            {
                                StarBreakerUtils.NewDustByYouself(Projectile.Center, DustID.YellowStarDust, () => true, Vector2.UnitX,
                                    5, 30, dust =>
                                    {
                                        dust.scale *= 1.1f;
                                    });
                                Projectile.velocity = Vector2.Zero;
                            }
                            else if(Timer > 5)
                            {
                                Projectile.rotation = 0.2f * Projectile.spriteDirection;
                                Projectile.Center = player.Center;
                                player.heldProj = Projectile.whoAmI;
                                if (Timer == 50)//自动装弹
                                {
                                    while (true)
                                    {
                                        if (player.GetHasItem<StarBreakerW>().ModItem is StarBreakerW starBreaker && starBreaker.UseAmmo.Count < 20)
                                        {
                                            Item[] useAmmo = player.GetHasItem<EnergyBulletItem>(2);
                                            if (useAmmo == null)
                                            {
                                                break;
                                            }
                                            if (useAmmo[0] != null)
                                            {
                                                starBreaker.UseAmmo.Add(useAmmo[0].type);
                                                useAmmo[0].ItemStackDeduct();
                                            }
                                            if (starBreaker.UseAmmo.Count < 20)
                                            {
                                                if (useAmmo[1] != null)
                                                {
                                                    starBreaker.UseAmmo.Add(useAmmo[1].type);
                                                    useAmmo[1].ItemStackDeduct();
                                                }
                                                else
                                                {
                                                    starBreaker.UseAmmo.Add(0);
                                                }
                                            }//避免装多子弹
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    var sound = SoundID.Item149.WithPitchOffset(-0.66f);
                                    sound.MaxInstances = 3;
                                    sound.PitchVariance = 0;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        SoundEngine.PlaySound(sound, player.Center);
                                    }
                                }
                                else if(Timer == 70)
                                {
                                    State = 0;
                                    Timer++;
                                }
                            }
                            break;
                        }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == 1 || State == 2)//近战
            {
                float r = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),
                    Projectile.Center - (Projectile.velocity.RealSafeNormalize() * Projectile.width / 2),
                    Projectile.Center + (Projectile.velocity.RealSafeNormalize() * Projectile.width / 2),
                    Projectile.height,ref r);
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
            target.velocity -= target.velocity.RealSafeNormalize() * hitDirection * knockback;
            if(State == 2)//刺击
            {
                damage = (int)(damage * 1.5f);
                target.velocity -= target.velocity.RealSafeNormalize() * hitDirection * knockback;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }

            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            texture ??= TextureAssets.Projectile[Type];
            origin = texture.Size() * 0.5f;
            int length = Projectile.oldPos.Length;
            for (int i = 0; i < length; i++)
            {
                Color color = lightColor * ((length - i) / (float)length);
                if (i > 0) color = Color.Lerp(color, Color.Transparent, 0.8f);
                Main.spriteBatch.Draw(texture.Value, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, color, Projectile.oldRot[i],
                    origin, 1f, Projectile.SpriteDirToEffects(), 0f);
            }
            return false;
        }
    }
}
