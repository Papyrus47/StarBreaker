namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class LastCopperWhipProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜鞭");
            ProjectileID.Sets.IsAWhip[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.aiStyle = -1;
            Projectile.WhipSettings.Segments = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ai[0]++;

            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int segments, out float rangeMultiplier);

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * (Projectile.ai[0] - 1);//修改弹幕位置

            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) < 0f) ? -1 : 1;//修改鞭子贴图朝向
            if (Projectile.ai[0] >= timeToFlyOut || player.itemAnimation == 1)//如果玩家挥舞完成或者飞行时间过长
            {
                Projectile.Kill();//清除弹幕
                return;
            }
            player.heldProj = Projectile.whoAmI;//手持弹幕
            player.itemTime = player.itemAnimation = player.itemAnimationMax - (int)(Projectile.ai[0] / Projectile.MaxUpdates);
            if (Projectile.ai[0] == (float)((int)timeToFlyOut / 2f))//声音
            {
                SoundEngine.TryGetActiveSound(SoundEngine.PlaySound(SoundID.Item153, Projectile.Center), out var active);
                active.Sound.Pitch = Main.rand.NextFloat(-0.2f, -0.8f);

            }
            #region 粒子与弹幕
            List<Vector2> list = new();//new一个list
            Projectile.FillWhipControlPoints(Projectile, list);//为list填充点
            if (Projectile.ai[0] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient && player.ownedProjectileCounts[ModContent.ProjectileType<LastCopperWhipOnUseProj>()] < 5)
            {
                Vector2 pos = list[^1];
                Dust.NewDustDirect(pos, 2, 2, ModContent.DustType<Dusts.LastCopperWhipDust>());
                if (!Main.tile[(int)pos.X / 16, (int)pos.Y / 16].HasTile)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, Projectile.velocity,
                        ModContent.ProjectileType<LastCopperWhipOnUseProj>(), Projectile.damage / 2, Projectile.knockBack, player.whoAmI, 3);
                    projectile.friendly = true;
                    projectile.hostile = false;
                }
            }
            #endregion
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.immortal)
            {
                return;
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Projectile.damage;
            }

            Projectile.damage += (int)(Projectile.damage * 0.1f);
            if (Projectile.damage > Projectile.ai[1] * 10)
            {
                Projectile.damage = (int)Projectile.ai[1] * 10;
            }

            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Orange);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            texture = TextureAssets.Projectile[Type].Value;
            pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                frame = new Rectangle(0, 0, 14, 22);
                origin = new Vector2(6, 14);
                float scale = 1;

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;
                float rotation = diff.ToRotation() - MathHelper.PiOver2; //旋转
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                if (i == list.Count - 2)
                {
                    frame.Y = 96;
                    frame.Height = 24;
                    origin.Y -= 15;

                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Projectile.ai[1] / timeToFlyOut;
                    scale = MathHelper.Lerp(1.5f, 2f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    frame.Y = 24;
                    frame.Height = 14;
                }
                else if (i > 5)
                {
                    frame.Y = 48;
                    frame.Height = 14;
                }
                else if (i > 0)
                {
                    frame.Y = 72;
                    frame.Height = 14;
                }

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
}
