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
        public override string Texture => "StarBreaker/Items/Weapon/StarBreakerW";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
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
            Player player = Main.player[Projectile.owner];
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3());
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (!player.controlUseTile && !player.channel)//判断玩家有没有长按右键或者左键
            {
                Projectile.Kill();
            }
            Timer++;

            if (Timer > 40 - State && starPlayer.StarCharge < 10)
            {
                if ((starPlayer.Bullet1 is not null || starPlayer.Bullet2 is not null) && (!starPlayer.Bullet1.IsAir || !starPlayer.Bullet2.IsAir))
                {
                    StarBreakerWay.StarBrekaerUseBulletShoot(starPlayer, out int shootID, out int shootDamage, out Items.EnergyBulletItem bulletItem);//获取对应能量子弹
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);//声音
                    for (float i = -5; i <= 5; i++)
                    {
                        Vector2 vec = (i.ToRotationVector2() * MathHelper.Pi / 18) + Projectile.velocity.SafeNormalize(Vector2.Zero);//速度
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vec * 10, shootID, player.GetWeaponDamage(player.HeldItem) + shootDamage,
                            player.GetWeaponKnockback(player.HeldItem, 1), player.whoAmI, 0);//发射子弹
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].hostile = false;
                        StarBreakerWay.Add_Hooks_ToProj(bulletItem, proj);//添加被动钩子
                    }
                }
                if (State < 20)
                {
                    State++;
                }

                Timer = 0;
                Projectile.damage += (int)State;
                if (Projectile.damage > player.inventory[player.selectedItem].damage * State)
                {
                    Projectile.damage = (int)(player.inventory[player.selectedItem].damage * State);
                }
                starPlayer.StarCharge++;
            }
            else if(starPlayer.StarCharge >= 10)//在停火状态
            {
                if(Timer > 40)//冲刺
                {
                    if (Main.myPlayer == player.whoAmI) Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 30;
                    if (Projectile.localNPCHitCooldown != 1)
                    {
                        Projectile.localNPCHitCooldown = 1;
                        Timer = 0;
                    }
                    else
                    {
                        Timer = 0;
                        Projectile.localNPCHitCooldown = 0;
                        starPlayer.StarCharge = 0;
                    }
                }
                else if(Timer > 10)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            else
            {
                if (player.channel)//左键使用
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        float dis = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                        if (dis > 150)//速度快到类似传送
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * (dis - 140f);
                        }
                        else if (dis > 20)//改变速度
                        {
                            if(Projectile.velocity.Length() > 10)
                            {
                                Projectile.velocity = Projectile.velocity.RealSafeNormalize() * 10f;
                            }
                            Projectile.velocity = (Projectile.velocity * 8 + (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 10f) / 9f;
                        }
                        else//减速
                        {
                            Projectile.velocity *= 0.9f;
                        }
                    }
                }
                else//右键使用
                {

                }
            }
            #region 手持弹幕所需要的
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                Projectile.velocity.X * Projectile.direction);//改变旋转角度
            #endregion
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                damage += (int)Main.player[Projectile.owner].velocity.Length() * 5;
                if (damage > 700)
                {
                    damage = 700;
                }
                damage += (int)(Main.player[Projectile.owner].GetModPlayer<StarPlayer>().StarCharge * Main.rand.NextFloat(2));
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }

            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            if (Main.player[Projectile.owner].channel)
            {
                List<CustomVertexInfo> customs = new();
                int i = 0;
                for(float r = 0;r <= MathHelper.TwoPi;r += MathHelper.TwoPi / 50f)
                {
                    i++;
                    if (i % 3 == 0) continue;
                    customs.Add(new(Main.MouseWorld + ((r + Main.GlobalTimeWrappedHourly).ToRotationVector2() * 150) - Main.screenPosition,Color.Lerp(Color.Purple,Color.Red,0.4f), new Vector3(0.5f, 0.5f, 0)));
                }
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, customs.ToArray(), 0, customs.Count - 1);
            }
        }
    }
}
