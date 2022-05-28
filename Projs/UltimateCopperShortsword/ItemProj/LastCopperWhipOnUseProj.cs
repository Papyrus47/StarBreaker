namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperWhipOnUseProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Copper thorn ball");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铜刺球");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 14;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.OwnerMinionAttackTargetNPC != null)
            {
                Projectile.velocity = (Projectile.velocity * 5 + (Projectile.OwnerMinionAttackTargetNPC.Center - Projectile.position).SafeNormalize(default) * (Projectile.OwnerMinionAttackTargetNPC.velocity.Length() + 1)) / 6;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = -oldVelocity.X;
            return false;
        }
    }
}
