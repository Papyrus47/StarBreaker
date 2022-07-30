namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperGunProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜枪");
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 68;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 900;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<NPCs.StarGlobalNPC>().BloodyBleed += 700;
        }
    }
}
