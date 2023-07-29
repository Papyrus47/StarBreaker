using StarBreaker.Content.Component.ComponentNPC;
using StarBreaker.Content.ProjAIHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff.Skills
{
    public class SOS_Swing : StarOriginStaff_BasicSkill
    {
        public Vector2 HitKnockDir;
        protected SwingHelper swingHelper;
        protected readonly Vector2 VelScale;
        protected readonly Vector2 SwingStartVel;
        protected readonly float VisualRotation;
        protected readonly float RotVel;
        protected float SwingLength;
        protected readonly int SwingRotDir;
        public SOS_Swing(ModProjectile projectile, Vector2 velScale, Vector2 swingStartVel, float visualRotation, int swingDir, SwingHelper swingHelper, float rotVel = 0f) : base(projectile)
        {
            VelScale = velScale;
            SwingStartVel = swingStartVel;
            VisualRotation = visualRotation;
            SwingRotDir = swingDir;
            RotVel = rotVel;
            this.swingHelper = swingHelper;
        }

        public override void AI()
        {
            Player.StarBreaker().InAttack = true;
            if (SkyAttack)
            {
                Player.velocity.Y *= 0.8f;
            }
            switch ((int)Projectile.ai[0])
            {
                case 0: // 渐变
                    {
                        swingHelper.Change_Lerp(SwingStartVel, 0.7f, VelScale, 0.1f,VisualRotation,0.8f);
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
                case 1: // 挥舞
                    {
                        if (Projectile.ai[1] <= 250)
                        {
                            Projectile.ai[1] += 9;
                            float Timer = SwingTimerChange();
                            swingHelper.SetRotVel(RotVel);
                            swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                            swingHelper.SwingAI(SwingLength, Player.direction, Timer);
                            if (Projectile.ai[1] > 190)
                            {
                                Player.StarBreaker().PreControlAction = true;
                                if (Projectile.extraUpdates != 0)
                                {
                                    // 可以添加音效
                                    Projectile.extraUpdates = 0;
                                }
                            }
                            Player.velocity.X = Player.direction * 2.2f;
                        }
                        else
                        {
                            Projectile.ai[2]++;
                            Player.velocity.X *= 0.9f;
                            float Timer = TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * SwingRotDir;
                            swingHelper.ProjFixedPlayerCenter(Player, 0, true);
                            swingHelper.SwingAI(SwingLength, Player.direction, Timer + Projectile.ai[2] * 0.002f);
                            if (Projectile.ai[2] > 24)
                            {
                                SkillTimeOut = true;
                            }
                            if (Projectile.ai[2] > 8)
                            {
                                StarOriginStaff.UseWaitAttack = true;
                            }
                        }
                        break;
                    }
            }
        }

        public virtual float SwingTimerChange()
        {
            return TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * SwingRotDir;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 0 && Projectile.ai[1] < 250;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Effect effect = StarBreakerAssetHelper.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = StarBreakerAssetHelper.StarOriginStaffColor.Value;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, StarBreakerAssetHelper.SB_Extra[5].Value,(factor) => new Color(200,50,200,0), effect,(f) => 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Player.StarBreaker().AddAttack(this);
            if(HitKnockDir != default)
            {
                target.AddCompoent(new HitKnockCompoent(15,HitKnockDir,50));
            }
            else
            {
                target.AddCompoent(new NPCByHit_StunLocked(8));
            }
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Projectile.velocity * 0.01f, 3, 2, 1,-1,"SOS_S"));
        }
        public override string ToString()
        {
            return nameof(SOS_Swing) + VelScale.X.ToString() + VelScale.Y.ToString() + SwingStartVel.X.ToString() + SwingStartVel.Y.ToString();
        }
        public override void OnSkillActive()
        {
            Projectile.ai[0] = 0;
            SwingLength = Projectile.Size.Length() * Projectile.scale;
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.extraUpdates = 0;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Player.StarBreaker().InAttack = false;
            StarOriginStaff.UseWaitAttack = false;
        }
        public override bool ActivationCondition()
        {
            if (IsWait)
            {
                return base.ActivationCondition() && StarOriginStaff.UseWaitAttack && Player.StarBreaker().LeftMouse;
            }
            return base.ActivationCondition() && Player.StarBreaker().LeftMouse;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[2] > 0;
        }
        public static float TimerChange(float Timer)
        {
            return MathF.Pow(Timer, 2.5f);
        }
        public static SOS_Swing Slash(ModProjectile modProjectile,SwingHelper swingHelper, Vector2 StartVel, int SwingRotDir = 1,float RotVel = 0) =>
            new(modProjectile, new(1, 0.7f), StartVel, 0.1f, SwingRotDir,swingHelper, RotVel);
        public static SOS_Swing Swept(ModProjectile modProjectile, SwingHelper swingHelper, Vector2 StartVel, int SwingRotDir = 1, float RotVel = 0) =>
       new(modProjectile, new(1, 0.4f), StartVel, 0.6f, SwingRotDir, swingHelper, RotVel);
        public static SOS_Swing Raise(ModProjectile modProjectile, SwingHelper swingHelper, Vector2 StartVel, int SwingRotDir = 1, float RotVel = 0) =>
           new(modProjectile,Vector2.One, StartVel, 0f, SwingRotDir, swingHelper, RotVel);
    }
}
