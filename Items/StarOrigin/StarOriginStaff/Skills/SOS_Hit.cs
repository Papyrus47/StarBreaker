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
    public class SOS_Hit : SOS_Swing
    {
        public int HitConst;
        public int Const;
        public int MouseChickConst;
        public SOS_Hit(ModProjectile projectile,SwingHelper swingHelper) : base(projectile, Vector2.One,(-Vector2.UnitX), 0,1,swingHelper,-0.3f)
        {
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
                        if (Projectile.ai[1] < -5)
                        {
                            SkillTimeOut = true;
                        }
                        Projectile.spriteDirection = Player.direction;
                        swingHelper.Change_Lerp(SwingStartVel, 1f, VelScale, 0.1f, VisualRotation, 0.8f);
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
                        if (Projectile.ai[1] <= 250) // 每一段挥舞
                        {
                            Projectile.ai[1] += 6 + MathF.Log(HitConst + 1,2) * 1.5f;
                            float Timer = SwingTimerChange();
                            swingHelper.SetRotVel(RotVel);
                            swingHelper.ProjFixedPlayerCenter(Player, -7, true, true);
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
                        }
                        else // 挥舞结束后
                        {
                            if (Projectile.ai[2] > 6 && HitConst <= 2 + Const) // 如果攻击次数小于 2 + 需攻击次数
                            {
                                Projectile.ai[0] = 2;
                                Projectile.ai[2] = 0;
                                Projectile.extraUpdates = 2;
                                HitConst++; // 增加一次攻击次数,继续攻击
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Projectile.velocity * 0.01f, 3, 2, 1, -1, "SOS_H"));
                                break;
                            }
                            Projectile.ai[2]++; // 增加退出计时器
                            Player.velocity.X *= 0.9f;
                            //swingHelper.SetNotSaveOldVel();
                            float Timer = TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * 0.75f * SwingRotDir;
                            swingHelper.ProjFixedPlayerCenter(Player, -7, true);
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
                case 2: // 回归性挥舞
                    {
                        if (Projectile.ai[1] >= 0) // 回归性挥舞时期
                        {
                            Projectile.ai[1] -= 6 + MathF.Log(HitConst + 1, 2) * 1.5f;
                            float Timer = SwingTimerChange();
                            swingHelper.SetRotVel(RotVel);
                            swingHelper.ProjFixedPlayerCenter(Player, -7, true, true);
                            swingHelper.SwingAI(SwingLength, Player.direction, Timer);
                        }
                        else // 回归挥舞结束
                        {
                            Projectile.ai[0] = 1;
                            Projectile.extraUpdates = 1;
                            ResetHit();
                        }
                        break;
                    }
            }

            if (Const < 6 && Player.StarBreaker().LeftMouse && MouseChickConst++ > 9)
            {
                Const++;
                MouseChickConst = 0;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player.StarBreaker().AddAttack(this);
            target.AddCompoent(new HitKnockCompoent(15, new(Player.direction * 0.2f,1), 50));
            target.velocity.X = Player.direction * 1.5f;
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Projectile.velocity * 0.01f, 3, 2, 1, -1, "SOS_S"));
        }
        public override float SwingTimerChange()
        {
            return TimerChange(Projectile.ai[1] / 270f) * MathHelper.TwoPi * 0.8f;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            HitConst = Const = MouseChickConst = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            HitConst = Const = MouseChickConst = 0;
        }
    }
}
