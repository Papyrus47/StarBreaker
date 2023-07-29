using StarBreaker.Content.Component.ComponentNPC;
using StarBreaker.Content.ProjAIHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff.Skills
{
    public class SOS_ForwardSaltoSwing : StarOriginStaff_BasicSkill
    {
        private SwingHelper swingHelper;
        private float SwingLength;
        public SOS_ForwardSaltoSwing(ModProjectile projectile) : base(projectile)
        {
        }
        public override void AI()
        {
            Player.StarBreaker().InAttack = true;
            Player.StarBreaker().DamageFactor = 1.5f;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 渐变
                    {
                        Projectile.spriteDirection = Player.direction;
                        swingHelper.Change_Lerp(new Vector2(-1,1).RotatedBy(-0.3), 0.8f,new(1,0.9f), 0.8f, 0.1f, 0.8f);
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true);
                        swingHelper.SetNotSaveOldVel();

                        swingHelper.SwingAI(SwingLength, Player.direction, 0);
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] > 9)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                            ResetHit();
                            Projectile.extraUpdates = 1;
                        }
                        break;
                    }
                case 1: // 翻滚 + 挥舞
                    {
                        if (Projectile.ai[1] <= 250)
                        {
                            if ((int)Projectile.ai[1] == 9)
                            {
                                Player.velocity.Y = -3;
                            }
                            Projectile.ai[1] += 9;
                            float Timer = SwingTimerChange();
                            if ((int)Projectile.ai[1] == 144) // 音效
                            {

                            }
                            swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                            swingHelper.SwingAI(SwingLength, Player.direction, Timer);
                            if (Projectile.ai[1] > 190)
                            {
                                Player.StarBreaker().PreControlAction = true;
                                Projectile.extraUpdates = 0;
                            }
                            Player.fullRotation = Timer * 1.64f * Player.direction;
                            Player.fullRotationOrigin = Player.Size * 0.5f;
                            Player.velocity.X = Player.direction * 8.2f;
                        }
                        else
                        {
                            Player.fullRotation = 0;
                            Projectile.ai[2]++;
                            Player.velocity.X *= 0.5f;
                            float Timer = SOS_Swing.TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * 0.8f;
                            swingHelper.ProjFixedPlayerCenter(Player, 0, true);
                            swingHelper.SwingAI(SwingLength, Player.direction, Timer + Projectile.ai[2] * 0.002f);
                            if (Projectile.ai[2] > 24)
                            {
                                SkillTimeOut = true;
                            }
                        }
                        break;
                    }
            }
        }

        public virtual float SwingTimerChange()
        {
            return SOS_Swing.TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * 0.8f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddCompoent(new HitKnockCompoent(16,new Vector2(Player.direction * 10,1), 5));
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Effect effect = StarBreakerAssetHelper.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = StarBreakerAssetHelper.StarOriginStaffColor.Value;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, StarBreakerAssetHelper.SB_Extra[5].Value, (factor) => new Color(200, 50, 200, 0), effect, (f) => 0);
            return false;
        }
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            swingHelper = new(Projectile, 14);
            SwingLength = Projectile.Size.Length() * Projectile.scale;
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && StarOriginStaff.UseWaitAttack && Player.StarBreaker().LeftMouse;
        }
        public override bool SwitchCondition()
        {
            return !Player.StarBreaker().InAir && Projectile.ai[2] > 0;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            swingHelper = null;
            StarOriginStaff.UseWaitAttack = false;
            Player.StarBreaker().InAttack = false;
        }
    }
}
