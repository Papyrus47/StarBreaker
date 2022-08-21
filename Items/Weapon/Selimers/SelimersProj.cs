namespace StarBreaker.Items.Weapon.Selimers
{
    public class SelimersProj : ModProjectile
    {
        private Asset<Texture2D>[] asset = new Asset<Texture2D>[7];
        public override string Texture => base.Texture + "0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Selimers");
            DisplayName.AddTranslation(7, "自我");
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.Size = new(54);
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
        }
        public override void Load()
        {
            for (int i = 0; i < asset.Length; i++)
            {
                asset[i] = ModContent.Request<Texture2D>((GetType().Namespace + ".SelimersProj" + i.ToString()).Replace('.', '/'));
            }
        }
        public override void Unload()
        {
            if(asset != null)
            {
                for (int i = 0; i < asset.Length; i++)
                {
                    asset[i].Dispose();
                }
                asset = null;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            else if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.frame < 7)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, -1),
                        Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].alpha = 255;
                    Main.projectile[proj].frame++;
                }
            }

            switch (Projectile.frame)
            {
                case 6:
                    {
                        Projectile.rotation += 0.1f;
                        float max = 800f;
                        NPC n = null;
                        foreach (NPC npc in Main.npc)
                        {
                            float dis = Vector2.Distance(npc.Center, Projectile.Center);
                            if (npc.active && !npc.friendly && npc.CanBeChasedBy() && max > dis)
                            {
                                max = dis;
                                n = npc;
                            }
                        }
                        if (n != null)
                        {
                            Projectile.velocity = (Projectile.velocity * 5f + (n.Center - Projectile.Center).RealSafeNormalize() * 15f) / 6f;
                        }
                        break;
                    }
                case 3:
                    {
                        Projectile.rotation += 0.1f;
                        break;
                    }
                case 2:
                    {
                        Projectile.rotation += 0.1f;
                        break;
                    }
                default:
                    {
                        Projectile.penetrate = -1;
                        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
                        break;
                    }
            }
            if (Main.myPlayer == player.whoAmI && player.controlUseTile)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize();
                Projectile.Center = Vector2.UnitY.RotatedBy(MathHelper.Pi / (7f * Projectile.frame)) * 50f + player.Center;
            }

            if (!player.controlUseTile && Projectile.velocity.Length() < 15f)//逐渐加速
            {
                Projectile.velocity *= Projectile.velocity.Length() / 20f;
            }

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.frame == 3)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ProjectileID.DD2BetsyFlameBreath, Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
            }
            else if (Projectile.frame == 2)
            {
                target.AddBuff(BuffID.Ichor, 180);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = asset[Projectile.frame].Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition,
                null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
