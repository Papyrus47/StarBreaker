using StarBreaker.Content;

namespace StarBreaker.Items.StarOwner.CrushTheStarsWeapon
{
    /// <summary>
    /// 这个就一个例子
    /// </summary>
    public abstract class BasicMeleeWeaponSwingProj : SkillProj
    {
        public MeleeProj meleeProj;
        public Vector2[] oldVels;
        public override void PreSkillAI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 5;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * player.direction,
            Projectile.velocity.X * player.direction);
            player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.extraUpdates = 2;
        }
        public override void PostSkillAI()
        {
            oldVels ??= new Vector2[5];
            for (int i = oldVels.Length - 1; i > 0; i--)
            {
                oldVels[i] = oldVels[i - 1];
            }
            oldVels[0] = Projectile.velocity;
        }
        public override void Init()
        {
            AddSkill(nameof(ChannelSword), new(ChannelSword));
            AddSkill(nameof(NormalAttack), new(NormalAttack, NormalAttack_Draw));
        }
        public virtual void ChannelSword() // TODO : 蓄力
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = (MathHelper.PiOver2 * (Projectile.ai[0] % 2 == 0 ? 1 : -1) + MathHelper.PiOver4 * (Projectile.ai[0] % 2 == 0 ? 1 : -1)).ToRotationVector2() * Projectile.Size.Length();
            Projectile.velocity = Projectile.velocity.RotatedBy((Main.MouseWorld - Projectile.Center).ToRotation());
            if (Timer < meleeProj.MaxChannelTime) //增加蓄力时间
            {
                Timer++;
            }
            if (!player.channel)
            {
                ChangeSkill(nameof(NormalAttack));//切换到攻击ai
                meleeProj.AttackRot = (Main.MouseWorld - Projectile.Center).ToRotation();
                meleeProj.NowRot = Projectile.ai[1] = MathHelper.PiOver2 * (Projectile.ai[0] % 2 == 0 ? 1 : -1)
                    + MathHelper.PiOver4 * (Projectile.ai[0] % 2 == 0 ? 1 : -1);//控制挥舞起点
            }
            Projectile.direction = -Projectile.direction;
        }
        /// <summary>
        /// 竖着挥舞,如果要在外面修改挥舞弧度请再写方法
        /// </summary>
        public virtual void NormalAttack()
        {
            Vector2 pos = Projectile.Center + meleeProj.NowRot.ToRotationVector2() * Projectile.Size.Length();
            Projectile.velocity = pos - Projectile.Center;
            Projectile.velocity = Projectile.velocity.RotatedBy(meleeProj.AttackRot);

            meleeProj.NowRot += meleeProj.RotSpeed;// 更新旋转
            if (Math.Abs(meleeProj.NowRot) > 3f)
            {
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity, Projectile.width / 2, ref Projectile.localAI[1]);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            #region 绘制剑
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            CustomVertexInfo[] custom = new CustomVertexInfo[6];

            Vector2 Size = Projectile.Size;
            Vector2 heldPos = Projectile.Center + Projectile.velocity.RealSafeNormalize() * Size * 0.5f;//获取绘制的中心点

            gd.Textures[0] = TextureAssets.Projectile[Type].Value;
            custom[0] = custom[5] = new(Projectile.Center - Main.screenPosition, lightColor, new Vector3(0, 1, 0));//把柄/左下角
            custom[1] = new(heldPos - Projectile.velocity.NormalVector().RealSafeNormalize() * Size.Length() * 0.5f - Main.screenPosition, lightColor, Vector3.Zero);//左上角
            custom[2] = custom[3] = new(Projectile.Center + Projectile.velocity - Main.screenPosition, lightColor, new Vector3(1, 0, 0));//右上角
            custom[4] = new(heldPos + Projectile.velocity.NormalVector().RealSafeNormalize() * Size.Length() * 0.5f - Main.screenPosition, lightColor, new Vector3(1, 1, 0));//右下角
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, custom, 0, 2);
            #endregion
            SkillDraw(lightColor);
            return false;
        }
        public virtual bool NormalAttack_Draw(Color color)
        {
            return true;
        }
        public struct MeleeProj
        {
            public int MaxChannelTime;//最大蓄力时间
            public float AttackRot;//攻击时旋转弧度
            public float NowRot;//现在的旋转弧度
            public float RotSpeed;//攻击旋转速度
        }
    }
}
