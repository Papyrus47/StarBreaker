using StarBreaker.Content;

namespace StarBreaker.Items.StarOwner.CrushTheStarsWeapon
{
    public class CrushTheStarsProj : SkillProj
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.Size = new(88, 86);
            Projectile.penetrate = -1;
        }
        public override void Init()
        {
        }
        public void Attack1()
        {

        }
        /// <summary>
        /// 用于挥舞的函数
        /// </summary>
        /// <param name="y_scale">单独缩放Y轴,这是椭圆的核心</param>
        /// <param name="scale">用于同时缩放X与Y用</param>
        public void SlashAttack(float y_scale, float scale = 1f)
        {

        }

        public override void Init_SkillChange()
        {
        }
    }
}
