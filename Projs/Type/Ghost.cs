namespace StarBreaker.Projs.Type
{
    public abstract class Ghost : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public sealed override void SetDefaults()
        {
            SetDef();
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hide = false;
        }
        public sealed override void AI()
        {
            Alive();
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.active = npc.active;
        }
        public NPC MyOwenr;
        protected Color LineColor;
        protected string TheColorTex;
        public abstract void SetDef();
        public abstract void Alive();


    }
}
