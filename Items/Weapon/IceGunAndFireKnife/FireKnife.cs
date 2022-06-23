namespace StarBreaker.Items.Weapon.IceGunAndFireKnife
{
    public class FireKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "炎刀");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.Size = new(26);
            Projectile.hide = true;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if(player.HeldItem == null || player.HeldItem.type != ModContent.ItemType<IceGunAndFireKnife>())
            {
                Projectile.Kill();
                return;
            }
            else
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = 1;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<FireKnife_Blade>(),
                        Projectile.damage,Projectile.knockBack, player.whoAmI);
                    Main.projectile[proj].localAI[0] = 0;
                }
                IceGunAndFireKnife iceGunAndFireKnife = player.HeldItem.ModItem as IceGunAndFireKnife;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                Projectile.timeLeft = 2;
                player.itemRotation = Projectile.velocity.ToRotation();
                if(player.direction == -1)
                {
                    player.itemRotation += MathHelper.Pi;
                }
                if (player.itemAnimation > 0 && player.altFunctionUse != 2)
                {
                    if (Projectile.ai[0] <= 0)//判断攻击
                    {
                        Projectile.ai[0] = 200;
                        if(Projectile.ai[1] == 0)
                        {
                            Projectile.ai[1]++;
                        }
                        else
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0] = 0;
                            Projectile.localAI[1] = 0;
                            if(iceGunAndFireKnife.InMax)
                            {
                                iceGunAndFireKnife.ChannelTime = 0;
                            }
                            player.itemTime = player.itemAnimation = 0;
                        }
                    }
                    else//攻击
                    {
                        player.heldProj = Projectile.whoAmI;
                        player.itemTime = player.itemAnimation = 3;
                        if(iceGunAndFireKnife.InMax)
                        {
                            if (Projectile.ai[0] > 150)
                            {
                                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.RealSafeNormalize() * 20;
                                if (Projectile.localAI[1] < 0.3f)
                                {
                                    Projectile.localAI[1] += 0.05f;
                                    if(Main.myPlayer == player.whoAmI)
                                    {
                                        Projectile.velocity = (player.Center - Main.MouseWorld).RealSafeNormalize();
                                    }
                                }
                                else
                                {
                                    Projectile.velocity = Projectile.velocity.RotatedBy(0.5 * player.direction * (Projectile.ai[0] > 100).ToDirectionInt());
                                }
                            }
                            else
                            {
                                Projectile.position += Projectile.velocity.RealSafeNormalize() * (float)Math.Sqrt(Projectile.ai[0]);
                            }
                        }
                        else
                        {
                            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.RealSafeNormalize() * 20;
                            if (Projectile.ai[0] == 200 && Main.myPlayer == player.whoAmI)
                            {
                                Projectile.velocity = -Vector2.UnitY;
                            }
                            else
                            {
                                Projectile.velocity = Projectile.velocity.RotatedBy((0.05 + Math.Abs(100 - Projectile.ai[0]) * 0.005f) * player.direction * (Projectile.ai[0] > 100).ToDirectionInt());
                            }
                            Projectile.ai[0] -= 5;
                        }
                        Projectile.ai[0]--;
                    }
                }
                else
                {
                    if(player.controlUseTile)
                    {
                        player.itemTime = player.itemAnimation = 2;
                        player.velocity *= 0.8f;
                    }
                    Projectile.velocity = (Projectile.velocity * 5 + new Vector2(3 * player.direction, 8))/6;
                    Projectile.Center = Vector2.Lerp(Projectile.Center,player.Center + new Vector2(-40 * player.direction, -15),0.1f);
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void PostDraw(Color lightColor)
        {
            if (Main.LocalPlayer.HeldItem == null) return;

            IceGunAndFireKnife iceGunAndFireKnife = Main.LocalPlayer.HeldItem.ModItem as IceGunAndFireKnife;
            if (iceGunAndFireKnife == null) return;

            Texture2D texture = ModContent.Request<Texture2D>("StarBreaker/Items/Weapon/IceGunAndFireKnife/GunAndKnifeChannelUI").Value;
            Main.spriteBatch.Draw(texture, Main.LocalPlayer.Center + new Vector2(0, -50) - Main.screenPosition, null, Color.White,0,texture.Size() * 0.5f,1f,SpriteEffects.None,0);

            texture = ModContent.Request<Texture2D>("StarBreaker/Items/Weapon/IceGunAndFireKnife/GunAndKnifeChannelUI_Line").Value;
            Main.spriteBatch.Draw(texture, Main.LocalPlayer.Center + new Vector2(0, -50) - Main.screenPosition,new(0,0,(int)(texture.Width * (iceGunAndFireKnife.ChannelTime / 1800f)),texture.Height),iceGunAndFireKnife.InMax ? Color.Red : Color.Purple, 0, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
        }
        public override bool ShouldUpdatePosition() => false;
    }
}
