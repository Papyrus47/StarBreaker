using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;

namespace StarBreaker.Projs.StarGhostKnife
{
    public class GhostFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("鬼炎");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 30;
            Projectile.width = 30;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.MediumPurple;
            return base.PreDraw(ref lightColor);
        }
    }
}
