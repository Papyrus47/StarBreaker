namespace StarBreaker.Projs.StarDoomStaff
{
    public abstract class StarCrystal : ModProjectile
    {
        public override string Texture => (GetType().Namespace + ".StarCrystal").Replace('.', '/');
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.width = 26;
            Projectile.height = 24;
            Projectile.minion = true;
            Projectile.minionSlots = 0.01f;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.player[Projectile.owner].active)
            {
                Projectile.timeLeft = 2;
            }
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                Projectile.extraUpdates = 2;
                Projectile.velocity = (Projectile.velocity * 10 + (Projectile.OwnerMinionAttackTargetNPC.position - Projectile.position) * 0.1f) / 11;
            }
        }
    }
}
