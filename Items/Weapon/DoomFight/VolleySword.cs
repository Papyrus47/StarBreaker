using StarBreaker.NPCs;

namespace StarBreaker.Items.Weapon.DoomFight
{
    public class VolleySword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "凌空之剑");
            Tooltip.SetDefault("Shoot the line and Big Line");
            Tooltip.AddTranslation(7, "产生线和粗线");
        }
        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.knockBack = 1.2f;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = Item.height = 66;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VolleySwordProj>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.value = 805932;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage * 10, knockback, player.whoAmI);
            if (player.altFunctionUse == 2)
            {
                Main.projectile[proj].ai[0] = -1;
                Main.projectile[proj].extraUpdates = 1;
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
    public class VolleySwordProj : ModProjectile
    {
        internal Vector2[] oldVels;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volley sword");
            DisplayName.AddTranslation(7, "凌空之剑");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 5;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.usesLocalNPCImmunity = true;
            oldVels = new Vector2[10];
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 5;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                Projectile.velocity.X * Projectile.direction);
            player.ChangeDir(Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
            switch (Projectile.ai[0])
            {
                case < 3://挥舞
                    {
                        if (Math.Abs(Projectile.ai[1]) > 3f)
                        {
                            Projectile.ai[1] = 0;
                            if (Projectile.ai[0] == -1)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.ai[0]++;
                            oldVels = new Vector2[10];
                        }
                        else if (Projectile.ai[1] == 0)
                        {
                            NPC n = null;
                            if (!player.HasMinionAttackTargetNPC)
                            {
                                float max = 2000;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(npc.Center, Projectile.Center);
                                    if (dis < max && npc.active && npc.CanBeChasedBy() && !npc.friendly)
                                    {
                                        max = dis;
                                        n = npc;
                                    }
                                }
                            }
                            else
                            {
                                n = Projectile.OwnerMinionAttackTargetNPC;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient && n != null)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 center = n.Center + Main.rand.NextVector2Unit() * 300;
                                    Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), center, Vector2.Zero,
                                        ModContent.ProjectileType<VolleySword_SmallLine>(), Projectile.damage / 20, Projectile.knockBack, player.whoAmI, n.whoAmI);
                                }
                                SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
                            }
                            Projectile.ai[1] = MathHelper.PiOver2 * (Projectile.ai[0] % 2 == 0 ? 1 : -1);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                Projectile.localAI[0] = (Main.MouseWorld - Projectile.Center).ToRotation() + Main.rand.NextFloatDirection() * 0.5f;
                            }
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                        }
                        else
                        {
                            Projectile.ai[1] += (float)Math.Sin(-0.3f * (Projectile.ai[0] % 2 == 0 ? 1 : -1)) * (player.GetTotalAttackSpeed(DamageClass.Melee) + player.GetWeaponAttackSpeed(player.HeldItem) / 60);
                        }
                        Vector2 pos = Projectile.Center + Projectile.ai[1].ToRotationVector2() * Projectile.width * 2.5f;
                        Projectile.velocity = pos - Projectile.Center;
                        Projectile.velocity.Y *= 0.1f;
                        Projectile.velocity.X *= 0.3f;
                        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
                        break;
                    }
                case 3://举起
                    {
                        if (Main.myPlayer == player.whoAmI)
                        {
                            Projectile.velocity = (Projectile.velocity * 10 + (Main.MouseWorld - Projectile.Center).RealSafeNormalize() * 66) / 11;
                        }
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] > 60)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        else if (Projectile.ai[1] == 30)
                        {
                            NPC n = null;
                            if (!player.HasMinionAttackTargetNPC)
                            {
                                float max = 2000;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(npc.Center, Projectile.Center);
                                    if (dis < max && npc.active && npc.CanBeChasedBy() && !npc.friendly)
                                    {
                                        max = dis;
                                        n = npc;
                                    }
                                }
                            }
                            else
                            {
                                n = Projectile.OwnerMinionAttackTargetNPC;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient && n != null)
                            {
                                Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center - Projectile.velocity.RealSafeNormalize() * 100, Vector2.Zero,
                                    ModContent.ProjectileType<VolleySword_BigLine>(), Projectile.damage / 20, Projectile.knockBack, player.whoAmI, n.whoAmI);
                                SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
                            }
                        }
                        break;
                    }
                default:
                    {
                        Projectile.Kill();
                        break;
                    }
            }
            for (int i = oldVels.Length - 1; i > 0; i--)
            {
                oldVels[i] = oldVels[i - 1];
            }
            oldVels[0] = Projectile.velocity;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] >= 3)
            {
                return false;
            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 4, Projectile.width / 2, ref Projectile.localAI[1]);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            if (Projectile.ai[0] >= 3)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White,
                   Projectile.rotation, new Vector2(5, texture.Height - 10), 1.3f, 0, 0);
                return false;
            }
            bool flag = !Main.drawToScreen && Main.netMode != 2 && !Main.gameMenu && !Main.mapFullscreen && Lighting.NotRetro && Terraria.Graphics.Effects.Filters.Scene.CanCapture();
            if (!flag)
            {
                DrawVectrx(Projectile, oldVels);
            }
            Vector2 scale = Projectile.velocity.AbsVector2() / texture.Size() + Vector2.One;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White,
                Projectile.rotation, new Vector2(5, texture.Height - 10), scale, 0, 0);
            return false;
        }
        public static void DrawVectrx(Projectile Projectile, Vector2[] oldVels)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 1; i < oldVels.Length; i++)
            {
                if (oldVels[i] == Vector2.Zero)
                {
                    continue;
                }

                Vector2 vel = oldVels[i - 1] - oldVels[i];
                vel = vel.NormalVector();
                var factor = i / (float)oldVels.Length;
                var color = Color.Lerp(Color.AliceBlue * 0.4f, Color.Blue, 1 - factor);
                var w = MathHelper.Lerp(0.5f, 0.05f, factor);
                bars.Add(new(Projectile.Center + (oldVels[i] * 0.8f + vel.RealSafeNormalize()) * 5f, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
                bars.Add(new(Projectile.Center + (oldVels[i] * 0.8f + vel.RealSafeNormalize()).RealSafeNormalize() * 2f, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
            }
            if (bars.Count > 2)
            {
                List<CustomVertexInfo> triangleList = new();
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }

                if (StarBreakerWay.InBegin())
                {
                    Main.spriteBatch.End();
                }

                if (!StarBreakerWay.InBegin())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                }

                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("StarBreaker/Images/MyExtra_2").Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                StarBreaker.UseSwordShader.Parameters["uTransform"].SetValue(model * projection);
                StarBreaker.UseSwordShader.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                Main.graphics.GraphicsDevice.RasterizerState = originalState;

            }
            if (StarBreakerWay.InBegin())
            {
                Main.spriteBatch.End();
            }

            if (!StarBreakerWay.InBegin())
            {
                Main.spriteBatch.Begin();
            }
        }
    }
    public class VolleySword_BigLine : ModProjectile
    {
        private Vector2[] linePos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Line");
            DisplayName.AddTranslation(7, "线");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 5;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 15;
            Projectile.usesLocalNPCImmunity = true;
            linePos = null;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
            {
                Projectile.Kill();
            }
            if (linePos == null) linePos = new Vector2[15];
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.extraUpdates = 6;
                Projectile.timeLeft *= 3;
                Projectile.rotation = (Projectile.Center - npc.Center).ToRotation();
                Projectile.velocity = (npc.Center - Projectile.Center).RealSafeNormalize() * 70;
            }
            else
            {
                if (Projectile.ai[1] < linePos.Length)
                {
                    linePos[(int)Projectile.ai[1]] = Projectile.Center;
                    Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(Projectile.timeLeft) * 0.6f);
                    Projectile.ai[1]++;
                }
            }
            if (linePos != null && Projectile.ai[1] >= linePos.Length)
            {
                Projectile.Center -= Projectile.velocity;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Projectile.ai[0] || linePos == null || Projectile.ai[1] < linePos.Length)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<StarGlobalNPC>().VolleyAIStop = 50;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (linePos != null)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Main.spriteBatch.Draw(texture, linePos[1] - Main.screenPosition, null, new Color(50, 100, 255, 0), Projectile.rotation, texture.Size() * 0.5f,
                    new Vector2(0.3f, 0.8f) * 2.5f, SpriteEffects.None, 0);
                Color color = Color.LightBlue;
                List<CustomVertexInfo> customs = new();
                for (int i = linePos.Length - 1; i >= 1; i--)
                {
                    if (linePos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    float width = 9 * (1 - (i * 1f / linePos.Length));
                    Vector2 vel = (linePos[i] - linePos[i - 1]).NormalVector().RealSafeNormalize() * width;
                    customs.Add(new(linePos[i] + vel - Main.screenPosition, color, new Vector3(0.5f, 0.5f, 0)));
                    customs.Add(new(linePos[i] - vel - Main.screenPosition, color, new Vector3(0.5f, 0.5f, 0)));
                }
                if (customs.Count > 2)
                {
                    List<CustomVertexInfo> vertex = new();
                    vertex.Add(customs[0]);
                    vertex.Add(customs[1]);
                    vertex.Add(customs[2]);
                    for (int i = 0; i < customs.Count - 2; i += 2)
                    {
                        vertex.Add(customs[i]);
                        vertex.Add(customs[i + 2]);
                        vertex.Add(customs[i + 1]);

                        vertex.Add(customs[i + 1]);
                        vertex.Add(customs[i + 2]);
                        vertex.Add(customs[i + 3]);
                    }
                    Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.FishingLine.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        vertex.ToArray(), 0, vertex.Count / 3);
                }
            }
            return false;
        }
    }
    public class VolleySword_SmallLine : ModProjectile
    {
        private Vector2[] linePos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Line");
            DisplayName.AddTranslation(7, "线");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 40;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 5;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 15;
            Projectile.usesLocalNPCImmunity = true;
            linePos = null;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
            {
                Projectile.Kill();
            }
            if (linePos == null) linePos = new Vector2[15];
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.extraUpdates = 3;
                Projectile.timeLeft *= 3;
                Projectile.rotation = (Projectile.Center - npc.Center).ToRotation();
                Projectile.velocity = (npc.Center - Projectile.Center) / 15;
            }
            else
            {
                if (Projectile.ai[1] < linePos.Length)
                {
                    linePos[(int)Projectile.ai[1]] = Projectile.Center;
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(0.15);
                    Projectile.ai[1]++;
                }
            }
        }
        public override bool ShouldUpdatePosition()
        {
            if (linePos != null && Projectile.ai[1] >= linePos.Length)
            {
                return false;
            }
            return base.ShouldUpdatePosition();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Projectile.ai[0] || linePos == null || Projectile.ai[1] < linePos.Length)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.velocity *= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (linePos != null)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Main.spriteBatch.Draw(texture, linePos[0] - Main.screenPosition, null, new Color(50, 100, 255, 0), Projectile.rotation, texture.Size() * 0.5f,
                    new Vector2(0.3f, 0.8f), SpriteEffects.None, 0);
                CustomVertexInfo[] customs = new CustomVertexInfo[linePos.Length + 1];
                customs[0] = new(linePos[0] - Main.screenPosition, Color.LightBlue, new Vector3(0.5f, 0.5f, 0));
                for (int i = 1; i < linePos.Length; i++)
                {
                    if (linePos[i] == Vector2.Zero) break;
                    customs[i] = new(linePos[i] - Main.screenPosition, Color.LightBlue, new Vector3(0.5f, 0.5f, 0));
                }
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.FishingLine.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                    customs, 0, customs.Length - 2);

            }
            return false;
        }
    }
}
