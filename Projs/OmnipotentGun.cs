using StarBreaker.Items.Bullet;
using System.IO;
using Terraria.Audio;

namespace StarBreaker.Projs
{
    public class OmnipotentGun : ModProjectile
    {
        public Item ItemGun;
        /// <summary>
        /// 表示状态
        /// </summary>
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        /// <summary>
        /// 表示主人星辰击碎者
        /// </summary>
        private int TheOwenrProj
        {
            get => (int)Projectile.ai[1];
            set
            {
                if (value >= 0 && value < 200)
                {
                    Projectile.ai[1] = value;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }
        }
        public int[] States = new int[5];
        private NPC npc;
        private Projectile StarBreakerProj = null;
        public override string Texture => "StarBreaker/Projs/Star";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("被控制的枪");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                for (int i = 0; i < States.Length; i++)
                {
                    writer.Write(States[i]);
                }
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                for (int i = 0; i < States.Length; i++)
                {
                    States[i] = reader.ReadInt32();
                }
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Player player = Main.player[Projectile.owner];
            Projectile projectile = Main.projectile[TheOwenrProj];
            Projectile.spriteDirection = Projectile.direction;
            #region 弹幕消失条件
            if (!player.active)
            //玩家死亡
            {
                Projectile.Kill();
            }
            #endregion
            #region 获取星辰击碎者武器弹幕
            if (!projectile.active || projectile.type != ModContent.ProjectileType<StarBreakerHeadProjReal>())
            {
                foreach (Projectile projectile1 in Main.projectile)
                {
                    if (projectile1.active && projectile1.type == ModContent.ProjectileType<StarBreakerHeadProjReal>())
                    {
                        TheOwenrProj = projectile1.whoAmI;
                        break;
                    }

                }
            }
            if (StarBreakerProj == null || !StarBreakerProj.active)
            {
                foreach (Projectile projectile1 in Main.projectile)
                {
                    if (projectile1.active && projectile1.type == ModContent.ProjectileType<StarBreakerProj>())
                    {
                        StarBreakerProj = projectile1;
                        break;
                    }

                }
            }
            #endregion
            #region 获取敌对npc
            if (npc == null)
            {
                float max = 1000;
                foreach (NPC n in Main.npc)
                {
                    float dis = Vector2.Distance(n.Center, Projectile.Center);
                    if (dis < max && n.active && !n.friendly && n.CanBeChasedBy() && Collision.CanHit(n.position, 1, 1, Projectile.position, 1, 1))
                    {
                        max = dis;
                        npc = n;
                    }
                }
                Projectile.Center = player.Center + new Vector2((States[4] == 1 ? 1 : -1) * 100, 0);
                Projectile.velocity = player.velocity;

            }
            #endregion
            else if (ItemGun != null && npc != null)
            {
                if (!npc.active || !npc.CanBeChasedBy() || !Collision.CanHit(npc.position, 1, 1, Projectile.position, 1, 1))
                {
                    npc = null;
                    return;
                }
                if (StarBreakerProj != null)//判断召唤物活着
                {
                    if (!StarBreakerProj.active)
                    {
                        StarBreakerProj = null;
                        State = 0;
                        States[1] = 0;
                    }
                    else
                    {
                        State = 3;
                    }
                }

                switch (State)
                {
                    case 0://闲置在玩家边
                        {
                            if (TheOwenrProj >= 0 && projectile.type == ModContent.ProjectileType<StarBreakerHeadProjReal>())
                            {
                                State = 1;
                                break;
                            }
                            Projectile.Center = player.Center + new Vector2((States[4] == 1 ? 1 : -1) * 100, 0);
                            Projectile.velocity = player.velocity;
                            Projectile.rotation = (npc.Center - Projectile.Center).ToRotation();
                            Projectile.spriteDirection = Projectile.direction = ((npc.Center - Projectile.Center).X > 0).ToDirectionInt();
                            States[0]++;
                            ShootBullet(player);
                            break;
                        }
                    case 1://进行移动
                        {
                            if (TheOwenrProj >= 0 && projectile.type == ModContent.ProjectileType<StarBreakerHeadProjReal>())
                            {
                                if (player.altFunctionUse == 2)
                                {
                                    State = 2;
                                    break;
                                }
                                Projectile.Center = projectile.Center + new Vector2((States[4] == 1 ? 1 : -1) * 100, 0);
                                Projectile.velocity = Vector2.Zero;
                                Projectile.rotation = (npc.Center - Projectile.Center).ToRotation();
                                States[0]++;
                                ShootBullet(player);
                            }
                            else
                            {
                                State = 0;
                            }

                            break;
                        }
                    case 2://瞄准鼠标
                        {
                            if (player.altFunctionUse != 2)
                            {
                                State = 1;
                                break;
                            }
                            Projectile.Center = player.Center + new Vector2((States[4] == 1 ? 1 : -1) * 100, 0);
                            Projectile.velocity = player.velocity;
                            Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                            States[0]++;
                            ShootBullet(player);
                            break;
                        }
                    case 3://与星辰击碎者召唤物一起打npc
                        {
                            float speed = (float)Math.Sqrt((Projectile.Center - npc.Center).Length());
                            Projectile.velocity = Vector2.Normalize(npc.Center - Projectile.Center) * speed * 0.1f;
                            Projectile.rotation = (npc.Center - Projectile.Center).ToRotation();
                            States[0]++;
                            ShootBullet(player);
                            break;
                        }
                    default:
                        {
                            State = 0;
                            break;
                        }
                }
            }
            else if (ItemGun == null)
            {
                Projectile.Kill();
            }
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            if (ItemGun != null)
            {
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    Projectile.Center - Main.screenPosition,
                    null,
                    new Color(0.5f, 0f, 1f, 0f),
                    MathHelper.Pi,
                    Vector2.Zero,
                    new Vector2(1f, 1000),
                    SpriteEffects.None,
                    0f);
                Texture2D tex = TextureAssets.Item[ItemGun.type].Value;
                Projectile.width = tex.Width;
                Projectile.height = tex.Height;
                Main.spriteBatch.Draw(tex,
                   Projectile.Center - Main.screenPosition,
                   null,
                   Color.White,
                   Projectile.rotation,
                   tex.Size() * 0.5f,
                   1,
                   Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                   0f);
            }
        }
        private void ShootBullet(Player player)
        {
            if (ItemGun.channel)
            {
                States[0] = 0;
                return;
            }
            if (States[0] > ItemGun.useTime && Main.netMode != NetmodeID.MultiplayerClient && ItemGun.ModItem is Items.BaseEnergyWeapon gun)
            {
                int shootID = ModContent.ItemType<NebulaBulletItem>();
                #region 手动PickAmmo
                Item item = new();
                for (int i = 0; i < 58; i++)
                {
                    if (player.inventory[i].ammo == shootID && player.inventory[i].stack > 0)
                    {
                        item = player.inventory[i];
                    }
                }
                #endregion
                if (item.consumable && gun.CanConsumeAmmo(player))
                {
                    item.stack--;
                }

                if (item.stack <= 0)
                {
                    item.active = false;
                    item.TurnToAir();
                }
                else
                {
                    gun.Shoot(player, null, Projectile.Center + Projectile.rotation.ToRotationVector2() / (Vector2)gun.HoldoutOffset(), Projectile.rotation.ToRotationVector2() * ItemGun.shootSpeed, item.shoot, Projectile.damage, Projectile.knockBack);
                    SoundEngine.PlaySound(ItemGun.UseSound, Projectile.position);
                }
                States[0] = 0;
            }
        }
    }
}
