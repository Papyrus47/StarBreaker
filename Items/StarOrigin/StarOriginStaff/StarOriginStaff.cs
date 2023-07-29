using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarBreaker.Content.ProjAIHelper;
using StarBreaker.Content.TheSkillProj;
using StarBreaker.Items.StarOrigin.StarOriginStaff.Skills;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff
{
    public class StarOriginStaff : ModProjectile,IBasicSkillProj
    {
        public Player player;
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public bool UseWaitAttack;
        public SwingHelper swingHelper;
        public void Init()
        {
            swingHelper = new(Projectile, 16);
            swingHelper.Change(Vector2.One * Projectile.Size.Length() * Projectile.scale, Vector2.One, 0);
            SOS_Swing swing1 = SOS_Swing.Slash(this,swingHelper, new(-1, 1), 1,MathHelper.PiOver4 * 0.25f);
            SOS_Swing swing2 = SOS_Swing.Swept(this, swingHelper, new(-1, 1), -1);
            SOS_Swing swing3 = SOS_Swing.Raise(this, swingHelper, (-Vector2.UnitY).RotatedBy(-0.7), 1, 0);
            swing3.HitKnockDir = new Vector2(0, 4);
            SOS_Swing swing4 = SOS_Swing.Raise(this, swingHelper, Vector2.UnitY.RotatedBy(0.7), -1, 0);
            swing4.IsWait = true;
            swing4.HitKnockDir = new Vector2(0, -10);
            SOS_Swing swing5 = new SOS_Swing(this, new(1f,0.9f),(-Vector2.UnitY).RotatedBy(-0.3),0,1, swingHelper);
            SOS_CircleSwing circleSwing = new(this,swingHelper);
            SOS_Swing swing6 = SOS_Swing.Slash(this, swingHelper, new(-1, 1), -1, -MathHelper.PiOver4 * 0.25f);
            SOS_ForwardSaltoSwing forwardSaltoSwing = new(this);
            SOS_Swing swing7 = SOS_Swing.Swept(this, swingHelper, new(-1, 1), -1);
            forwardSaltoSwing.IsWait = true;
            SOS_Hit hit = new(this,swingHelper)
            {
                IsWait = true
            };

            SOS_Swing air_Swing1 = SOS_Swing.Swept(this, swingHelper, (-Vector2.UnitY).RotatedBy(0.4),1,MathHelper.PiOver4 * 0.5f);
            air_Swing1.SkyAttack = true;
            SOS_Swing air_Swing2 = SOS_Swing.Swept(this, swingHelper, (Vector2.UnitY).RotatedBy(0.4), -1, MathHelper.PiOver4 * 0.5f);
            air_Swing2.SkyAttack = true;
            SOS_Swing air_Swing3 = SOS_Swing.Raise(this, swingHelper, (-Vector2.UnitY).RotatedBy(0.4), 1, MathHelper.PiOver4 * 0.25f);
            air_Swing3.SkyAttack = true;
            SOS_CircleSwing air_circleSwing = new(this,swingHelper);
            air_circleSwing.SkyAttack = true;
            air_circleSwing.IsWait = true;

            SOS_NotUse notUse = new(this);
            notUse.AddSkill(swing1);
            notUse.AddSkill(air_Swing1);
            #region 空中

            #region Air Combo2

            air_Swing2.AddSkill(air_circleSwing);

            #endregion

            #region Air Combo1

            air_Swing1.AddSkill(air_Swing2);
            air_Swing2.AddSkill(air_Swing3);

            #endregion

            #endregion

            #region Combo4
            swing1.AddSkill(hit);
            #endregion

            #region Combo3
            swing4.AddSkill(forwardSaltoSwing);
            forwardSaltoSwing.AddSkill(swing7);
            #endregion

            #region Combo2
            swing2.AddSkill(swing4);
            swing4.AddSkill(swing5);
            swing5.AddSkill(circleSwing);
            circleSwing.AddSkill(swing6);
            #endregion

            #region Combo1
            swing1.AddSkill(swing2);
            swing2.AddSkill(swing3);
            #endregion
            CurrentSkill = notUse;
        }
        public override void OnSpawn(IEntitySource source)
        {
            player = Main.player[Projectile.owner];
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(102);
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool ShouldUpdatePosition() => false;
    }
}
