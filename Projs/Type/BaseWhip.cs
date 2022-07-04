namespace StarBreaker.Projs.Type
{
    public abstract class BaseWhip : ModProjectile
    {
        protected List<Vector2> ListVector2 = new();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            base.SetDefaults();
        }
        public override bool PreAI()
        {
            WhipAI();
            return base.PreAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            WhipDraw();
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
        public virtual void WhipAI() { }
        public virtual void WhipDraw()
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            Main.DrawWhip_WhipBland(Projectile, list);//调用原版鞭子绘制
        }
        protected void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }
    }
}
