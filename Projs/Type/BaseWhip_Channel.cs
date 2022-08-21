namespace StarBreaker.Projs.Type
{
    public abstract class BaseWhip_Channel : ModProjectile
    {
        public ChannelWhipSet ChannelWhip;
        protected List<Vector2> ListVector2 = new();
        protected float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        protected float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public sealed override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            ChannelWhip = new();
            SetDefault();
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.channel || ChargeTime >= ChannelWhip.MaxChargeTime)
            {
                return true;
            }

            ChannelAI();
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Main.player[Projectile.owner].channel;
        public override bool? CanHitNPC(NPC target) => Main.player[Projectile.owner].channel;
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
        public virtual void SetDefault() { }
        public virtual void ChannelAI()
        {
            Player player = Main.player[Projectile.owner];
            if (++ChargeTime % ChannelWhip.SegmentsTime == 0)
            {
                Projectile.WhipSettings.Segments++;
            }
            Projectile.WhipSettings.RangeMultiplier += ChannelWhip.RangeMultiplier;
            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemTimeMax;
        }
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
        public struct ChannelWhipSet
        {
            /// <summary>
            /// 最大蓄力时间
            /// </summary>
            public int MaxChargeTime;
            /// <summary>
            /// 随着时间而增加的长度
            /// </summary>
            public float RangeMultiplier;
            /// <summary>
            /// 蓄力加一段长度的时间
            /// </summary>
            public int SegmentsTime;
            public ChannelWhipSet(int maxChargeTime, int segmentsTime, float Multiplier)
            {
                MaxChargeTime = maxChargeTime;
                RangeMultiplier = Multiplier;
                SegmentsTime = segmentsTime;
            }

        }
    }
}
