namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperMagicStaffProj : ModProjectile
    {
        public override string Texture => StarBreakerWay.TransparentTex;
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
                    StarBreakerWay.NewParticle(new Particle.StarParticle(color,Projectile.Center));
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
        public override bool PreDraw(ref Color lightColor) => false;
    }
}