namespace StarBreaker.Projs
{
    public class StarRocket : ModProjectile
    {
        private int Target
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰火箭");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 1;
            Projectile.width = 1;
            //Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Target < 0 || Target == 200)
            {
                float max = 1000;
                foreach (NPC npc in Main.npc)
                {
                    float dis = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dis < max && npc.active && !npc.friendly && npc.CanBeChasedBy())
                    {
                        max = dis;
                        Target = npc.whoAmI;
                    }
                }
            }
            else
            {
                NPC npc = Main.npc[Target];
                if (!npc.active || !npc.CanBeChasedBy())
                {
                    Target = -1;
                }
                Projectile.velocity = (Projectile.velocity * 30 + Vector2.Normalize(npc.Center - Projectile.Center) * 20) / 31;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2((float)Math.Sqrt(oldVelocity.X), (float)Math.Sqrt(oldVelocity.Y));
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return (targetHitbox.Center.ToVector2() - projHitbox.Center.ToVector2()).Length() < 50;
        }
    }
}
