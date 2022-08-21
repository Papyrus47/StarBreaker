namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperSowrdSummonProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("小铜短剑");
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.Size = new(22);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 2;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NPC npc = Projectile.OwnerMinionAttackTargetNPC;
            Projectile.damage = Projectile.originalDamage + (int)(Projectile.originalDamage * (0.2f * (Projectile.numHits % 10)));
            if (!player.active)
            {
                Projectile.Kill();
                return;
            }
            if (player.HasBuff<Buffs.CopperSummonBuff>())
            {
                Projectile.timeLeft = 2;
            }
            if (npc == null)
            {
                float maxDis = 1200;
                foreach (NPC n in Main.npc)
                {
                    float dis = Vector2.Distance(n.Center, Projectile.Center);
                    if (dis < maxDis && n.active && !n.friendly && n.CanBeChasedBy())
                    {
                        maxDis = dis;
                        player.MinionAttackTargetNPC = n.whoAmI;
                    }
                }

                Vector2 center = player.Center + new Vector2((Projectile.minionPos * 20 * -player.direction) + (-100 * player.direction), -10);
                Projectile.velocity = (Projectile.velocity * 10f + (center - Projectile.Center).RealSafeNormalize() * 12f) / 11f;
                if (Projectile.Distance(player.Center) > 2000f)
                {
                    Projectile.Center = player.Center;
                }
                else if (Vector2.Distance(center, Projectile.Center) < 8f)
                {
                    Projectile.Center = center;
                    Projectile.velocity = Vector2.Zero;
                }
                Projectile.rotation = -MathHelper.PiOver4;
            }
            else if (npc != null && npc.active && !npc.friendly && npc.CanBeChasedBy())
            {
                Projectile.ai[1]--;
                Projectile.rotation += Projectile.velocity.Length() * 0.4f;
                if (Projectile.ai[1] < 0f)
                {
                    Projectile.ai[1] = 15f;
                    Projectile.velocity = (npc.Center - Projectile.Center).RealSafeNormalize() * 25f + npc.velocity * 0.3f;
                    Projectile.velocity.Y -= 2f;
                }
                else
                {
                    if (Projectile.velocity.Y < 3f)
                    {
                        Projectile.velocity.Y += 0.5f;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity *= 0.1f;
            Projectile.velocity.Y -= 18f;
            target.immune[Projectile.owner] = 0;

            Color color = Color.RosyBrown;
            color.A = 0;
            Particle.LightStar LightStar = new();
            LightStar.SetBasicInfo(StarBreakerAssetTexture.LightStar, null, Main.rand.NextVector2Unit() * 3f, Projectile.Center);
            LightStar.color = color;
            LightStar.Rotation = 0;
            LightStar.RotationVelocity = Main.rand.NextFloat(0.05f);
            LightStar.Scale = new(0.2f, 0.3f);
            LightStar.TimeLeft = 180;
            LightStar.ScaleVelocity = new(-0.02f);
            StarBreakerUtils.AddParticle(LightStar, false);
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override void PostDraw(Color lightColor)
        {
            StarBreakerUtils.ProjDrawTail(Projectile, lightColor * 0.5f, lightColor * 0.1f);
        }
    }
}
