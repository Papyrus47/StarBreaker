using StarBreaker.Items.Weapon;

namespace StarBreaker.Projs
{
    public class StarSpiralBladeProj : ModProjectile
    {
        private List<Vector2> pointList = new();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.width = Projectile.height = 112;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            pointList = new();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.damage = Projectile.originalDamage + (Projectile.timeLeft < 1500 ? Projectile.timeLeft : 1500);
            Projectile.rotation += Projectile.timeLeft * 0.1f;//旋转
            if (Projectile.rotation > 1000)//避免角度炸了,虽然没什么可能
            {
                Projectile.rotation -= 1000;
            }

            if (player.channel && !Projectile.friendly)
            {
                if (Projectile.ai[1] < 15000)
                {
                    Projectile.ai[1] += 20;
                }
                Projectile.timeLeft = (int)Projectile.ai[1] + 1;
                if (Projectile.timeLeft < 5) Projectile.timeLeft = 5;

                Projectile.Center = player.Center;
                player.itemTime = player.itemAnimation = 2;

                pointList.Clear();//清空所有点,避免重复

                if (player.whoAmI == Main.myPlayer)
                {
                    pointList.Add(player.Center);
                    pointList.Add(Main.MouseWorld);
                }
                float max = 800;
                bool AddPoint_MouseToNPC = Projectile.ai[1] > 10000;
                foreach (NPC npc in Main.npc)//获取所有范围内npc,添加他们的点
                {
                    float dis = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dis < 1500 && npc.active && npc.CanBeChasedBy() && !npc.friendly)
                    {
                        pointList.Add(npc.Center);//添加npc位置

                        dis = Vector2.Distance(npc.Center, Main.MouseWorld);//计算鼠标到npc的距离
                        if (AddPoint_MouseToNPC && dis < max && Main.myPlayer == player.whoAmI)
                        {
                            max = dis;
                            player.MinionAttackTargetNPC = npc.whoAmI;
                        }
                    }
                }
                if (AddPoint_MouseToNPC && player.HasMinionAttackTargetNPC)
                {
                    Vector2 center = pointList[^2];
                    Vector2 targetCenter = Projectile.OwnerMinionAttackTargetNPC.Center;
                    Vector2 vel = Projectile.velocity.RealSafeNormalize() * 30;
                    for (int i = 0; i < 100; i++)
                    {
                        vel = (vel * 10 + (targetCenter - center).RealSafeNormalize() * 60) / 11;
                        center += vel;
                        if(i % 2 == 0)vel = vel.RotatedBy(0.1);
                        pointList.Add(center);
                    }
                }
            }
            else if (Projectile.friendly)
            {
                if (Projectile.ai[0] < pointList.Count && Projectile.timeLeft > 500)
                {
                    Vector2 center = pointList[(int)Projectile.ai[0]];
                    if (Vector2.Distance(Projectile.Center, center) < 50)
                    {
                        Projectile.ai[0]++;
                        Projectile.alpha = 150;
                    }
                    else if (Projectile.alpha > 100)
                    {
                        Projectile.alpha = 0;
                        Projectile.velocity = (center - Projectile.Center) / 5f;
                    }
                }
                else
                {
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha--;
                    }
                    if (Vector2.Distance(player.Center, Projectile.Center) < 30)
                    {
                        Projectile.Kill();
                    }
                    Projectile.velocity = (Projectile.velocity + (player.Center - Projectile.Center) * 0.5f) / 2f;
                }
            }
            else
            {
                Projectile.alpha = 150;//透明
                Projectile.friendly = true;
            }
            Projectile.timeLeft -= 2;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.timeLeft -= 5;
            if(Projectile.timeLeft > 2000)
            {
                target.GetGlobalNPC<NPCs.StarGlobalNPC>().StarSpiralBladeProj = Projectile.whoAmI;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.timeLeft > 400)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 origin = texture.Size() * 0.5f;
                int conut = Projectile.timeLeft / 400;
                if(conut > 6)
                {
                    conut = 6;
                }
                for (int i = 0; i <= conut; i++)
                {
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.6f, Projectile.rotation + (i * 0.9f),
                            origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            Player player = Main.player[Projectile.owner];
            if (pointList != null && player.channel && !Projectile.friendly)
            {
                List<CustomVertexInfo> customVertexInfos = new();
                for (int i = 0; i < pointList.Count; i++)
                {
                    customVertexInfos.Add(new(pointList[i] - Main.screenPosition, Color.Purple, new(0.5f, 0.5f, 0)));
                }
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, customVertexInfos.ToArray(), 0, customVertexInfos.Count - 1);
            }
        }
    }
}
