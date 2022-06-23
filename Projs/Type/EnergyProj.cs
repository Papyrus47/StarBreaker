using StarBreaker.Items.DamageClasses;

namespace StarBreaker.Projs.Type
{
    public abstract class EnergyProj : ModProjectile
    {
        protected Color projColor = Color.White;
        protected bool? DrawBulletBody = true;//true表示正常绘制,返回false相反,null就会只绘制弹幕本体
        public bool ShouldUpdatePositionByProjVel = true;//根据速度改变位置
        #region 委托
        public delegate void ProjAI(Projectile projectile);
        public delegate void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit);
        public delegate void ProjKill(Projectile projectile);
        public delegate bool ProjOnTileCollide(Projectile projectile, Vector2 oldVelocity);
        public delegate bool? ProjColliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox);
        public delegate bool ProjDraw(Projectile projectile, ref Color lightColor);
        #endregion
        #region 事件
        public event ProjAI Proj_AI;
        public event ProjOnHitNPC Proj_OnHitNPC;
        public event ProjKill Proj_Kill;
        public event ProjOnTileCollide Proj_OnTileCollide;
        public event ProjColliding Proj_Colliding;
        public event ProjDraw Proj_Draw;
        #endregion
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public sealed override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = Projectile.height = 1;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<EnergyDamage>();
            NewSetDef();
        }
        public sealed override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            StateAI();
            if (Projectile.friendly && !Projectile.hostile && Proj_AI != null)
            {
                Proj_AI(Projectile);
            }

        }
        public override bool ShouldUpdatePosition() => ShouldUpdatePositionByProjVel;

        public override void Kill(int timeLeft)
        {
            if (Projectile.friendly && !Projectile.hostile && Proj_Kill != null)
            {
                Proj_Kill(Projectile);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (Projectile.friendly && !Projectile.hostile && Proj_OnHitNPC != null)
            {
                Proj_OnHitNPC(Projectile, target, damage, knockback, crit);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.friendly && !Projectile.hostile && Proj_OnTileCollide != null)
            {
                return Proj_OnTileCollide(Projectile, oldVelocity);
            }

            return base.OnTileCollide(oldVelocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Proj_Draw != null)
            {
                if (!Proj_Draw(Projectile, ref lightColor))
                {
                    return false;
                }
            }
            if (DrawBulletBody == true)
            {
                Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 DrawOrigin = new Vector2(tex.Width / 2, (float)tex.Height / 2);
                Vector2 center = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition;
                center -= new Vector2(tex.Width, tex.Height) * Projectile.scale / 2f;
                center += DrawOrigin * Projectile.scale + new Vector2(0f, 4f + Projectile.gfxOffY);
                Main.spriteBatch.Draw(tex,
                    center,
                    null,
                    projColor,
                    Projectile.rotation,
                    DrawOrigin,
                    new Vector2(5, 1),
                    Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0f);
            }
            else if (DrawBulletBody == null)
            {
                return base.PreDraw(ref lightColor);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Proj_Colliding == null)
            {
                float r = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 25,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * -25,
                2, ref r);
            }
            return Proj_Colliding(Projectile, projHitbox, targetHitbox);
        }
        public abstract void StateAI();
        public abstract void NewSetDef();

    }
}