using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class FrostFistProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("霜拳");
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 5000;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation -= MathHelper.PiOver2;
            }
            if (player.active && Projectile.localAI[0] == 0)//localAI[0]大于0表示为黑影
            {
                Projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != ModContent.ItemType<Items.Weapon.FrostFistW>())
            {
                Projectile.Kill();
            }
            if (Vector2.Distance(player.position, Projectile.position) > 300)
            {
                Projectile.position = player.position + (Projectile.position - player.position).SafeNormalize(default) * 300;
            }
            switch (Projectile.ai[0])
            {
                case -1://呆在玩家后背
                case 6://回血
                    {
                        Vector2 pos = player.Center + new Vector2(-80 * player.direction, -30);
                        Projectile.velocity = (pos - Projectile.position) * 0.2f;
                        if (Projectile.ai[0] == 6 && Projectile.localAI[0] == 0)
                        {
                            Projectile.ai[1]++;
                            if (Projectile.ai[1] > 60)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[0] = -1;
                                Vector2 center = default;
                                for (int i = 1; i < 20; i++)
                                {
                                    Tile tile = Main.tile[(int)(Projectile.position.X / 16), (int)((Projectile.position.Y / 16) + i)];
                                    center = Projectile.position + new Vector2(0, 16 * i);
                                    if (tile.HasTile) break;
                                }
                                foreach (Player player1 in Main.player)
                                {
                                    if (player1.active && Vector2.Distance(center, player1.position) < 200)
                                    {
                                        int heal = (int)(player1.statLifeMax2 * 0.2f);
                                        if (heal + player1.statLife > player1.statLifeMax2) heal = player1.statLifeMax2 - player1.statLife;
                                        player1.statLife += heal;
                                        player1.HealEffect(heal);
                                    }
                                }
                                foreach (NPC npc in Main.npc)
                                {
                                    if (npc.active && npc.friendly && Vector2.Distance(center, npc.position) < 200)
                                    {
                                        int heal = (int)(npc.lifeMax * 0.2f);
                                        if (heal + npc.life > npc.lifeMax) heal = npc.lifeMax - npc.life;
                                        npc.life += heal;
                                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 0://俯冲
                    {
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Projectile.ai[1]++;
                            if (Projectile.ai[1] % 30 == 0)
                            {
                                if (Projectile.ai[1] > 60)
                                {
                                    Projectile.ai[1] = 0;
                                    Projectile.ai[0]++;
                                }
                                Projectile.velocity = (Main.MouseWorld - Projectile.position).SafeNormalize(default) * 20;
                            }
                        }
                        break;
                    }
                case 1://俯冲直拳
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<OuLa>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner);
                            projectile.friendly = true;
                            projectile.hostile = false;
                            projectile.alpha = 255;
                            projectile.timeLeft = 20;
                        }
                        break;
                    }
                case 2://瞬拳
                    {
                        if (Projectile.velocity.Length() > 8) Projectile.velocity *= 0.7f;
                        else if (player.whoAmI == Main.myPlayer)
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.position).SafeNormalize(default) * 5;
                            Projectile.position -= Projectile.velocity;
                            Projectile.ai[1]++;
                            if (Projectile.ai[1] > 5 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Projectile.ai[1] > 60)
                                {
                                    Projectile.ai[1] = 0;
                                    Projectile.ai[0] = -1;
                                }
                                else if (Projectile.ai[1] % 10 == 0)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center, Projectile.velocity, ModContent.ProjectileType<OuLa>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.timeLeft = 30;
                                    projectile.alpha = 255;
                                }
                            }
                        }
                        break;
                    }
                case 3://圣拳连击
                    {
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] < 10)//直拳
                        {
                            if (Projectile.ai[1] == 1 && player.whoAmI == Main.myPlayer)
                            {
                                Projectile.position = player.position;
                                Projectile.velocity = (Main.MouseWorld - Projectile.position).SafeNormalize(default) * 20;
                                break;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<OuLa>(),
                                    Projectile.damage, Projectile.knockBack, Projectile.owner);
                                projectile.friendly = true;
                                projectile.hostile = false;
                                projectile.alpha = 255;
                            }
                        }
                        else if (Projectile.ai[1] < 40)//勾拳
                        {
                            Projectile.velocity = Projectile.velocity.RotatedBy(0.05);
                            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] % 3 == 0)
                            {
                                Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<OuLa>(),
                                    Projectile.damage, Projectile.knockBack, Projectile.owner);
                                projectile.friendly = true;
                                projectile.hostile = false;
                                projectile.timeLeft = 30;
                                projectile.alpha = 255;
                            }
                        }
                        else if (Projectile.ai[1] < 60)
                        {
                            if (Projectile.ai[1] == 40)//摆拳
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    Projectile.velocity = (Main.MouseWorld - Projectile.position).SafeNormalize(default) * 20;
                                    break;
                                }
                            }
                            else
                            {
                                Projectile.velocity = Projectile.velocity.RotatedBy(0.05);
                                if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] % 3 == 0)
                                {
                                    Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<OuLa>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    projectile.friendly = true;
                                    projectile.hostile = false;
                                    projectile.timeLeft = 30;
                                    projectile.alpha = 255;
                                }
                            }
                        }
                        else
                        {
                            Projectile.ai[0] = -1;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    }
                case 4://召唤一群黑影
                    {
                        if (Projectile.localAI[0] > 0)//如果是黑影
                        {
                            break;
                        }
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Projectile.ai[1] / 2 > 5)
                            {
                                Projectile.ai[0]++;
                                Projectile.ai[1] = 0;
                            }
                            else
                            {
                                Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, Type, Projectile.damage,
                                    Projectile.knockBack, Projectile.owner);
                                projectile.localAI[0] = Projectile.ai[1] / 2 + 1;
                                projectile.localAI[1] = Projectile.whoAmI;
                                projectile.alpha = (int)(Projectile.ai[1] * 10);
                                projectile.timeLeft = 5000;
                            }
                        }
                        else if (Projectile.ai[1] == 1)//通过遍历让黑影不生成
                        {
                            foreach (Projectile projectile in Main.projectile)
                            {
                                if (projectile.type == Type && projectile.localAI[0] > 0 && projectile.active)
                                {
                                    Projectile.ai[0]++;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case 5://刺拳猛击
                    {
                        if (Projectile.localAI[0] > 0)
                        {
                            break;
                        }//不要让影子使用这个技能
                        Projectile.ai[1]++;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.position) * 0.1f;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity, Projectile.velocity.RotateRandom(0.3), ModContent.ProjectileType<OuLa>(),
                                    Projectile.damage, Projectile.knockBack, Projectile.owner);
                                projectile.friendly = true;
                                projectile.hostile = false;
                                projectile.alpha = 255;
                            }
                        }
                        if (Projectile.ai[1] > 250)
                        {
                            Projectile.ai[0] = -1;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    }
                default:
                    {
                        Projectile.ai[0] = -1;
                        break;
                    }
            }
            if (Projectile.localAI[0] > 0)//幻影
            {
                Projectile projectile = Main.projectile[(int)Projectile.localAI[1]];
                Projectile.velocity = projectile.velocity;
                Projectile.ai[0] = projectile.ai[0];
                Projectile.ai[1] = projectile.ai[1];
                Projectile.rotation = projectile.rotation;
                Projectile.position = projectile.oldPos[(int)Projectile.localAI[0]];
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] > 0)
            {
                lightColor = Color.Black;
            }
            else if (Projectile.ai[0] == 6)
            {
                CustomVertexInfo[] customs = new CustomVertexInfo[6];

                Vector3[] Vector3Pos = new Vector3[4]//定义需要旋转的三维向量f
                {
                    new(60,180,180),
                    new(60,180,-180),
                    new(60,-180,180),
                    new(60,-180,-180)
                };
                Matrix matrix = Matrix.CreateRotationX(Projectile.ai[1] * 0.1f) * Matrix.CreateRotationY(-MathHelper.Pi / 12) *
                    Matrix.CreateRotationZ(MathHelper.PiOver2);

                for (int i = 0; i < 4; i++)
                {
                    Vector3Pos[i] = Vector3.Transform(Vector3Pos[i], matrix);
                }
                Vector2[] pos = new Vector2[4]
                {
                    1800 / (1800 - Vector3Pos[0].Z) * new Vector2(Vector3Pos[0].X,Vector3Pos[0].Y) + Projectile.Center,
                    1800 / (1800 - Vector3Pos[1].Z) * new Vector2(Vector3Pos[1].X,Vector3Pos[1].Y)+ Projectile.Center,
                    1800 / (1800 - Vector3Pos[2].Z) * new Vector2(Vector3Pos[2].X,Vector3Pos[2].Y)+ Projectile.Center,
                    1800 / (1800 - Vector3Pos[3].Z) * new Vector2(Vector3Pos[3].X,Vector3Pos[3].Y)+ Projectile.Center
                };
                customs[0] = customs[5] = new(pos[0], Color.White, new Vector3(0, 0, 1));
                customs[1] = new(pos[2], Color.White, new Vector3(0, 1, 1));
                customs[2] = customs[3] = new(pos[3], Color.White, new Vector3(1, 1, 1));
                customs[4] = new(pos[1], Color.White, new Vector3(1, 0, 1));

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

                StarBreaker.FrostFistHealMagic.Parameters["uTransform"].SetValue(model * projection);
                StarBreaker.FrostFistHealMagic.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap,
                    DepthStencilState.Default, RasterizerState.CullNone);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/Extra_34").Value;

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

                StarBreaker.FrostFistHealMagic.CurrentTechnique.Passes[0].Apply();

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    customs, 0, customs.Length / 3);

                //        triangleList.Count / 3 是三角形的个数

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }
            return base.PreDraw(ref lightColor);
        }
    }
}
