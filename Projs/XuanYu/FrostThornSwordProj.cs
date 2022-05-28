using Terraria.Audio;

namespace StarBreaker.Projs.XuanYu
{
    public class FrostThornSwordProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Thorn Sword");
            DisplayName.AddTranslation(7, "寒霜刺剑");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Size = new Vector2(40);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 50f;
            }
            player.itemRotation = Projectile.velocity.ToRotation();
            player.RotateRelativePoint(ref Projectile.velocity.X, ref Projectile.velocity.Y);
            player.itemTime = player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(15) * (1 - (int)(Projectile.ai[0] % 3)));//旋转15度
            Projectile.ai[0] += 0.8f;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            if (Projectile.ai[0] > 6)
            {
                Projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<NPCs.StarGlobalNPC>().XuanYuSlowTime = 180;
            for (int i = 0; i < 2; i++)
            {
                Main.player[Projectile.owner].dpsDamage += (int)target.StrikeNPC((int)(damage * 0.2f), knockback, 10);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int j = -1; j <= 5; j++)
            {
                Rectangle val6 = projHitbox;
                Vector2 val7 = Projectile.velocity.RealSafeNormalize() * Projectile.width * j;
                val6.Offset((int)val7.X, (int)val7.Y);
                if (val6.Intersects(targetHitbox))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Color.Lerp(Color.Blue, lightColor, 0.5f);//获取绘制颜色
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center + Projectile.velocity - Main.screenPosition, null, color,
                Projectile.velocity.ToRotation() - MathHelper.PiOver2, texture.Size() * 0.5f, new Vector2(1, 6f * Projectile.scale),
                SpriteEffects.None, 0f);
            texture = TextureAssets.Item[Main.player[Projectile.owner].HeldItem.type].Value;
            Main.spriteBatch.Draw(texture, Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter) + Projectile.velocity.RealSafeNormalize() * (texture.Width - 5) - Main.screenPosition, null, lightColor,
                Projectile.velocity.ToRotation() + MathHelper.PiOver4, texture.Size() * 0.5f, 1f,
                SpriteEffects.None, 0f);
            return false;
        }
    }
}
