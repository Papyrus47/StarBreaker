namespace StarBreaker.Projs.Bosses.StarBreakerEX
{
    public class AntiTankBoom : ModProjectile
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti Tank");
            DisplayName.AddTranslation(7, "反坦克炮");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 3;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[1] == 1)
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                Projectile.Center = player.Center - Projectile.velocity;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.ai[0] = target.whoAmI;
            Projectile.ai[1] = 1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.Center, 3, 3, DustID.Smoke);
            }
            if (Projectile.ai[1] == 1)
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                player.statLife -= Projectile.damage * 2;
                if (player.statLife <= 0)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "被反坦克炮炸死"), 10, player.direction);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[Type].Value,
                Projectile.Center - Main.screenPosition, null, Color.White,
                Projectile.rotation, Terraria.GameContent.TextureAssets.Projectile[Type].Size() * 0.5f,
                new Vector2(1.5f, 1), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            return false;
        }
    }
}
