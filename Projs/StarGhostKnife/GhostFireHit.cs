using Terraria.Audio;

namespace StarBreaker.Projs.StarGhostKnife
{
    public class GhostFireHit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghost Fire");
            DisplayName.AddTranslation(7, "鬼炎斩(冥炎斩)");
            Main.projFrames[Type] = 18;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 358;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 17)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.friendly && !Projectile.hostile)
            {
                Player player = Main.player[Projectile.owner];
                if (Projectile.frame % 6 == 0)
                {
                    player.statLife -= (int)(player.statLifeMax2 * 0.05f);
                    if (player.statLife <= 0)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "鬼神之力的过度使用"), 10, player.direction);
                    }
                    player.velocity.X = Projectile.velocity.X > 0 ? 6 : -6;
                    if (Projectile.frame < 16)
                    {
                        var sound = SoundEngine.PlaySound(SoundID.Item1,player.Center);
                        SoundEngine.TryGetActiveSound(sound, out var activeSound);
                        activeSound.Volume = 4.5f;
                        activeSound.Sound.Pitch = -0.5f;
                    }
                }
                if (player.dead)
                {
                    Projectile.Kill();
                }

                player.heldProj = Projectile.whoAmI;
                Projectile.direction = Projectile.spriteDirection = player.direction;
                player.itemAnimation = player.itemTime = 2;
                player.ChangeDir(Projectile.direction);
                Projectile.position = player.MountedCenter - Projectile.Size / 2f + new Vector2(0, 50);
                Projectile.timeLeft = 2;
                Projectile.damage = player.GetWeaponDamage(player.HeldItem) * 1500;
                PlayerHeadFrame(player);
            }
            else
            {
                Projectile.Kill();
            }
        }
        private void PlayerHeadFrame(Player player)
        {
            int frame = 0;
            switch (Projectile.frame)
            {
                case 1:
                case 10:
                case 11:
                case 12:
                    {
                        frame = 1;
                        break;
                    }
                case 2:
                case 9:
                case 13:
                    {
                        frame = 2;
                        break;
                    }
                case 3:
                case 8:
                case 14:
                    {
                        frame = 3;
                        break;
                    }
                case 4:
                case 5:
                case 6:
                case 7:
                case 15:
                    {
                        frame = 4;
                        break;
                    }
            }
            player.bodyFrame.Y = player.bodyFrame.Height * frame;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.Distance(target.Center) < (Projectile.width / 2 + Projectile.height / 2);
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, 70) - Main.screenPosition, new Rectangle(0, texture.Height / 18 * (Projectile.frame - 1), texture.Width, texture.Height / 18)
                , Color.Purple * 0.45f, 0f, new Vector2(texture.Width, texture.Height / 18) * 0.5f,
                1.2f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
