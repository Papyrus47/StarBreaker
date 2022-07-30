namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperKnifeProj : ModProjectile
    {
        private NPC target = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜投刀");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 120;
            Projectile.Size = new Vector2(10, 10);
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.velocity.Y += 0.01f;
            }
            else
            {
                Player player = Main.player[Projectile.owner];
                switch (Projectile.ai[1])
                {
                    case 0://飞出
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                            Projectile.velocity.Y += 0.01f;
                            break;
                        }
                    case 1://停留一段时间
                        {
                            Projectile.tileCollide = false;
                            if (Projectile.timeLeft < 500 || !target.active || !target.CanBeChasedBy() || target.friendly || Projectile.Distance(player.position) > 700)
                            {
                                Projectile.ai[1]++;
                            }
                            else
                            {
                                Vector2 proj_vel = target.Center - Projectile.Center;
                                if (proj_vel != Vector2.Zero)
                                {
                                    proj_vel.Normalize();
                                    proj_vel *= 14f;
                                }
                                Projectile.velocity = (Projectile.velocity * 4 + proj_vel) / 5;//速度渐变
                                if (target != null)
                                {
                                    if (target.active && target.CanBeChasedBy() && !target.friendly)
                                    {
                                        Projectile.Center = target.Center - Projectile.velocity * 2;
                                        Projectile.gfxOffY = target.gfxOffY;
                                    }
                                }
                            }
                            break;
                        }
                    case 2://飞回
                        {
                            if (target != null && target.active && target.CanBeChasedBy())
                            {
                                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Projectile.velocity = (player.position - Projectile.position).SafeNormalize(default) * 10;
                                    target.Center = Projectile.position;
                                    if (Projectile.Distance(player.position) < 200)
                                    {
                                        Projectile.Kill();
                                        target.velocity = (target.position - player.position).SafeNormalize(default) * 20f;
                                        if (!target.immortal)
                                        {
                                            for (int i = 0; i < 30; i++)
                                            {
                                                int NPC_Hit_damage = Main.rand.Next(300, 350);
                                                target.life -= NPC_Hit_damage;
                                                target.HitEffect(0, 10.0);
                                                Main.player[Projectile.owner].dpsDamage += NPC_Hit_damage;
                                                CombatText.NewText(target.Hitbox, CombatText.DamagedHostile, NPC_Hit_damage);
                                            }
                                            target.checkDead();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Projectile.Kill();
                            }
                            break;
                        }
                }
                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.direction);
                player.itemRotation = (player.position - Projectile.position).ToRotation();
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.damage > 20)
                {
                    Projectile.damage -= 5;
                }

                if (!target.immortal)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int NPC_Hit_damage = Projectile.damage + Main.rand.Next(-20, 20);
                        target.life -= NPC_Hit_damage;
                        target.HitEffect(0, 1.0);
                        Main.player[Projectile.owner].dpsDamage += NPC_Hit_damage;
                        CombatText.NewText(target.Hitbox, CombatText.DamagedHostile, NPC_Hit_damage);
                    }
                    target.checkDead();
                }
            }
            else
            {
                if (Projectile.ai[1] < 1)
                {
                    Projectile.timeLeft = 1000;
                    Projectile.ai[1] = 1;
                }
                if (target.realLife == -1)
                {
                    this.target = target;
                }
                else
                {
                    if (Main.npc[target.realLife].active && Main.npc[target.realLife].CanBeChasedBy())
                    {
                        this.target = Main.npc[target.realLife];
                    }
                    else
                    {
                        this.target = target;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            oldVelocity.Y = oldVelocity.Y > 0 ? 2 : -2;
            Projectile.velocity = -oldVelocity;
            return false;
        }
        //public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        //{
        //    Texture2D texture = Main.projectileTexture[Projectile.type];
        //    for(int i = 1;i< Projectile.oldPos.Length;i++)
        //    {
        //        Vector2 drawCenter = (Projectile.oldPos[i] + (new Vector2(Projectile.width,Projectile.height)/2)) - Main.screenPosition;
        //        Vector2 drawOrigir = texture.Size() * 0.5f;
        //        drawOrigir *= Projectile.scale;
        //        Color color = Color.Lerp(Color.Green, lightColor, 1 / (i - 0.2f));
        //        color *= 1 / (i - 0.2f);
        //        spriteBatch.Draw(texture, drawCenter, null,
        //            color,Projectile.rotation, drawOrigir,Projectile.scale, SpriteEffects.None, 0f);
        //    }
        //    if(Projectile.ai[0] == 1)
        //    {
        //        if (Main.myPlayer == Projectile.owner)
        //        {
        //            Utils.DrawLine(spriteBatch, Projectile.Center, Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter), Color.Green,Color.Green,2);
        //        }
        //    }
        //    return base.PreDraw(spriteBatch, lightColor);
        //}
    }
}
