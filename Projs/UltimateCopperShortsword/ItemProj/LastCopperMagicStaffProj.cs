namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperMagicStaffProj : ModProjectile
    {
        public override string Texture => StarBreakerUtils.TransparentTex;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("紫魔法");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 6;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.Size = new(5);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.extraUpdates = Projectile.timeLeft;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 2 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Color color = Main.rand.NextBool() ? Color.MediumPurple : Color.RosyBrown;
                    color.A = 0;
                    Particle.LightStar LightStar = new();
                    LightStar.SetBasicInfo(StarBreakerAssetTexture.LightStar, null, Vector2.Zero, Projectile.Center);
                    LightStar.color = color;
                    LightStar.Rotation = Main.rand.NextFloat(6.28f);
                    LightStar.RotationVelocity = Main.rand.NextFloat(0.1f);
                    LightStar.Scale = new(Main.rand.NextFloat(0.2f, 0.4f));
                    LightStar.TimeLeft = 120;
                    LightStar.ScaleVelocity = new(-0.02f);
                    StarBreakerUtils.AddParticle(LightStar, false);
                }
            }
            if (Projectile.timeLeft < 260)
            {
                NPC npc = null;
                float maxDis = 800;
                foreach (NPC n in Main.npc)
                {
                    float dis = Vector2.Distance(n.Center, Projectile.Center);
                    if (n.active && dis < maxDis && n.CanBeChasedBy() && !n.friendly)
                    {
                        npc = n;
                        maxDis = dis;
                    }
                }
                if (npc != null)
                {
                    Projectile.velocity = (Projectile.velocity * 60 + (npc.Center - Projectile.Center).RealSafeNormalize() * 15f) / 61;
                }
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(9));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}