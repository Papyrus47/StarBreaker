using StarBreaker.Items.Bullet;

namespace StarBreaker.Items.Type
{
    public abstract class EnergyBulletItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.ammo = ModContent.ItemType<NebulaBulletItem>();
            Item.width = 40;
            Item.height = 20;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.DamageType = DamageClass.Ranged;
        }
        public virtual void ProjAI(Projectile projectile) { }
        public virtual void Kill(Projectile projectile) { }
        public virtual void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit) { }
        public virtual bool OnTileCollide(Projectile projectile, Vector2 oldVelocity) { return true; }
        public virtual bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                projectile.Center + projectile.rotation.ToRotationVector2() * 25,
                projectile.Center + projectile.rotation.ToRotationVector2() * -25,
                2, ref r);
        }
        public virtual bool ProjPreDraw(Projectile projectile, ref Color lightColor) { return true; }
    }
}
