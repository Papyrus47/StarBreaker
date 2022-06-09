namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperShortSowrdProj : Type.BaseMeleeItemProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate copper short sword");
            DisplayName.AddTranslation(7, "最终铜短剑");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 5;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.usesLocalNPCImmunity = true;
            oldVels = new Vector2[10];
            LerpColor = Color.Green;
            LerpColor2 = Color.DarkGreen;
            DrawLength = 3.3f;
        }
        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                case -1://丢出
                    {
                        if (Timer == 0)
                        {
                            Timer++;
                            Projectile.timeLeft = 500;
                        }
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                        Projectile.velocity.Y += 0.01f;
                        Projectile.position += Projectile.velocity;
                        break;
                    }
                case < 2://挥舞
                    {
                        base.AI();
                        if (Timer == 0)
                        {
                            Projectile.localAI[0] = UseRot(Player);//储存旋转角度
                        }
                        else
                        {
                            UseAI(100, 0.8f, 0.3f, Projectile.localAI[0], -MathHelper.PiOver2 * (Projectile.ai[0] % 2 == 0).ToDirectionInt());
                        }
                        if (Math.Abs(Timer) > MathHelper.ToRadians(200))
                        {
                            Projectile.ai[0]++;
                            Timer = 0;
                            oldVels = new Vector2[10];
                        }
                        Timer += UseSpeed(Player, 0.35f) * (Projectile.ai[0] == 1).ToDirectionInt();
                        if (Player.GetModPlayer<StarPlayer>().EGO)
                        {
                            Timer -= 0.5f;
                        }
                        break;
                    }
                case 2://突刺
                    {
                        break;
                    }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 2 || Projectile.ai[0] <= -1)
            {
                return true;
            }
            return base.PreDraw(ref lightColor);
        }
        public override bool CanDraw()
        {
            if (Projectile.ai[0] >= 2 || Projectile.ai[0] <= -1)
            {
                return false;
            }
            return base.CanDraw();
        }
    }
}
