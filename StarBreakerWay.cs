using StarBreaker.Items;
using StarBreaker.Projs.Type;
using System.Reflection;
using Terraria.GameContent.Creative;

namespace StarBreaker
{
    public class StarBreakerWay
    {
        public const string TransparentTex = "Terraria/Images/Item_0";
        public delegate void MyDust(Dust dust);
        public static void NPCDrawTail(NPC npc, Color drawColor, Color TailColor)
        {
            for (int j = npc.oldRot.Length - 1; j > 0; j--)
            {
                npc.oldRot[j] = npc.oldRot[j - 1];
            }
            npc.oldRot[0] = npc.rotation;
            Texture2D NPCTexture = TextureAssets.Npc[npc.type].Value;
            SpriteEffects spriteEffects = 0;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[npc.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2(TextureAssets.Npc[npc.type].Width() / 2, TextureAssets.Npc[npc.type].Height() / frameCount / 2);

            for (int i = 1; i < npc.oldPos.Length; i += 2)
            {
                Color color = Color.Lerp(drawColor, TailColor, 0.5f);
                color = npc.GetAlpha(color);
                color *= (npc.oldPos.Length - i) / 15f;
                Vector2 DrawPosition = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * npc.scale / 2f;
                DrawPosition += DrawOrigin * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(npc.frame), color, npc.oldRot[i], DrawOrigin, npc.scale, spriteEffects, 0f);
            }
        }
        /// <summary>
        /// 无帧图的绘制
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="drawColor"></param>
        /// <param name="TailColor"></param>
        public static void ProjDrawTail(Projectile projectile, Color drawColor, Color TailColor)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                Vector2 pos = projectile.oldPos[i] + (new Vector2(projectile.width, projectile.height) / 2) - Main.screenPosition;
                Vector2 origin = texture.Size() * 0.5f;
                Main.spriteBatch.Draw(texture, pos, null, Color.Lerp(drawColor, TailColor, i / projectile.oldPos.Length), projectile.rotation,
                    origin, projectile.scale * (1f - (float)i / projectile.oldPos.Length), SpriteEffects.None, 0);
            }
        }
        /// <summary>
        /// 绘制图片拖尾到旧位置上
        /// </summary>
        /// <param name="texture">图片</param>
        /// <param name="oldPos">旧位置</param>
        /// <param name="BeginColor">最开始的颜色</param>
        /// <param name="EndColor">最后的颜色</param>
        /// <param name="oldRot">旧的旋转</param>
        /// <param name="origin"></param>
        /// <param name="sourceRectangle">帧图切割</param>
        /// <param name="dir">朝向,-1为反,1为正,-2为"FlipVertically"</param>
        /// <param name="rot">旋转</param>
        /// <param name="DrawScale">绘制时会不会随着draw拖尾的长度缩小</param>
        public static void DrawTailTexInPos(Texture2D texture, Vector2[] oldPos, Color BeginColor, Color EndColor, float rot, int dir = 1, Vector2 origin = default, Rectangle? sourceRectangle = null, float[] oldRot = null,bool DrawScale = false)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (dir == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else if (dir == -2)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            float myRot = rot;
            for (int i = 0; i < oldPos.Length; i++)
            {
                if (oldRot != null)
                {
                    myRot = oldRot[i];
                }
                Color color = Color.Lerp(BeginColor, EndColor, (float)i / oldPos.Length);
                float scale = 1f;
                if(DrawScale)
                {
                    scale -= (float)i / oldPos.Length;
                }
                Main.spriteBatch.Draw(texture, oldPos[i] - Main.screenPosition, sourceRectangle, color, myRot, origin,scale, spriteEffects, 0f);
            }
        }
        /// <summary>
        /// 为你的对象绘制一个边界,但是不能是玩家,仅限于npc和弹幕
        /// 这个东西不能对非正常Draw的使用
        /// </summary>
        /// <param name="entity">对象</param>
        public static void EntityDrawLight(Entity entity,Color lightColor)
        {
            if (entity is not Projectile && entity is not NPC)
            {
                return;
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            Texture2D texture;
            Vector2 pos;
            Color color;
            float rot;
            float scale;
            float gfxOffY;
            int frame;
            int hegiht;
            if (entity is NPC npc)
            {
                texture = TextureAssets.Npc[npc.type].Value;
                hegiht = texture.Height / Main.npcFrameCount[npc.type];
                frame = npc.frame.Y / hegiht;
                pos = npc.Center;
                gfxOffY = npc.gfxOffY;
                scale = npc.scale;
                rot = npc.rotation;
                color = npc.GetAlpha(lightColor);
                if(npc.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            else if (entity is Projectile projectile)
            {
                texture = TextureAssets.Projectile[projectile.type].Value;
                hegiht = texture.Height / Main.projFrames[projectile.type];
                frame = projectile.frame;
                pos = projectile.Center;
                gfxOffY = projectile.gfxOffY;
                scale = projectile.scale;
                rot = projectile.rotation;
                color = projectile.GetAlpha(lightColor);
                if(projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            else return;//为了下面调用tex不报错
            Rectangle rectangle = new(0, frame, texture.Width, hegiht);
            Vector2 origin = rectangle.Size() / 2f;

            Color floatingDaggerMinionGlowColor = color;
            floatingDaggerMinionGlowColor.A /= 4;
            for (int j = 0; j < 4; j++)
            {
                Vector2 DrawCenter = entity.Center - Main.screenPosition + new Vector2(0f, gfxOffY);
                Vector2 spinningpoint = rot.ToRotationVector2();
                double radians = (double)(Math.PI / 2 * (float)j);
                Vector2 center = default(Vector2);
                Main.EntitySpriteDraw(texture, DrawCenter + spinningpoint.RotatedBy(radians, center) * 2f, new Rectangle?(rectangle), floatingDaggerMinionGlowColor, rot, origin,scale, 0, 0);
            }
            if(entity is NPC)
            {
                rot += MathHelper.PiOver2;
            }
            Main.EntitySpriteDraw(texture, pos - Main.screenPosition + new Vector2(0f, gfxOffY), new Rectangle?(rectangle),color,rot, origin, scale, spriteEffects, 0);
        }
        public static void StarBrekaerUseBulletShoot(StarPlayer starPlayer, out int shootID, out int shootDamage, out EnergyBulletItem BulletPassive)
        {
            shootID = 0;
            shootDamage = 0;
            BulletPassive = null;

            if (starPlayer.Bullet1 != null && !starPlayer.Bullet1.IsAir)
            {
                shootID = starPlayer.Bullet1.shoot;
                shootDamage = starPlayer.Bullet1.damage;
                starPlayer.Bullet1.stack--;
                if (starPlayer.Bullet1.stack <= 0)
                {
                    starPlayer.Bullet1.TurnToAir();
                }

                if (starPlayer.Bullet2 != null && !starPlayer.Bullet2.IsAir)//被动子弹
                {
                    Item item = new(starPlayer.Bullet2.type, 1);
                    if (item.ModItem is EnergyBulletItem bullet)
                    {
                        BulletPassive = bullet;
                    }
                }
            }
            else if (starPlayer.Bullet2 != null && !starPlayer.Bullet2.IsAir)
            {
                shootID = starPlayer.Bullet2.shoot;
                shootDamage = starPlayer.Bullet1.damage;
                BulletPassive = null;
                starPlayer.Bullet2.stack--;
                if (starPlayer.Bullet2.stack <= 0)
                {
                    starPlayer.Bullet2.TurnToAir();
                }
            }
        }
        public static void PickAmmo_EnergyBulletItem(Player player, out int ShootItemID, out int shootDamage)
        {
            ShootItemID = -1;
            shootDamage = 0;
            if (player.HasAmmo(player.HeldItem))
            {
                for (int i = 0; i < player.inventory.Length; i++)//遍历 背包
                {
                    Item item = player.inventory[i];
                    if (item.active && item.ammo == ModContent.ItemType<Items.Bullet.NebulaBulletItem>() && item.consumable && item.ModItem is EnergyBulletItem)
                    {
                        item.stack--;
                        if (item.stack <= 0)
                        {
                            item.TurnToAir();
                        }

                        ShootItemID = item.type;
                        shootDamage = item.damage;
                    }
                }
            }
        }
        public static void NewParticle(Color color, Texture2D texture, int timeLeft, Vector2 position, Vector2? velocity = null, float rotation = 0f, float scale = 1f)
        {
            var particle = new Particle.Particle(color, texture, position, velocity)
            {
                Maincolor = color,
                timeLeft = timeLeft,
                rotation = rotation,
                scale = scale
            };
            NewParticle(particle);
        }

        public static void NewParticle(Particle.Particle particle)
        {
            if (Main.dedServ) return;

            StarBreakerSystem.Particles.Add(particle);
            particle.OnSpawn();
        }
        public static void Add_Hooks_ToProj(EnergyBulletItem bulletItem, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && bulletItem != null)
            {
                AddHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        /// <summary>
        /// 获取弹幕对应基础了能量类型弹幕基类
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>返回实例或者null值</returns>
        public static EnergyProj GetEnergyProj(Projectile projectile) => projectile.ModProjectile as EnergyProj;
        /// <summary>
        /// 尝试获取弹幕对应基础了能量类型弹幕基类
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="energyProj"></param>
        /// <returns></returns>
        public static bool TryGetEnergyProj(Projectile projectile,out EnergyProj energyProj)
        {
            energyProj = null;
            if(projectile.ModProjectile is EnergyProj energy)
            {
                energyProj = energy;
                return true;
            }
            return false;
        }
        public static void Add_Hooks_ToProj(int useBulletID, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && new Item(useBulletID).ModItem is EnergyBulletItem bulletItem)
            {
                AddHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        public static void Del_Hooks_ToProj(EnergyBulletItem bulletItem, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && bulletItem != null)
            {
                DelHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        public static void Del_Hooks_ToProj(int useBulletID, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && new Item(useBulletID).ModItem is EnergyBulletItem bulletItem)
            {
                DelHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        public static bool InBegin()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            FieldInfo field = spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null && field.GetValue(spriteBatch) is bool canDraw)
            {
                return canDraw;
            }
            return false;
        }
        /// <summary>
        /// 多功能自定义粒子生成器
        /// </summary>
        /// <param name="center">位置</param>
        /// <param name="type">类型</param>
        /// <param name="dis">距离</param>
        /// <param name="DustConst">粒子数量</param>
        /// <param name="Draw">可以绘制粒子条件,与修改其他事物</param>
        /// <param name="myDust">修改粒子</param>
        public static void NewDustByYouself(Vector2 center,int type,Func<bool> Draw,float dis = 200,int DustConst = 30,MyDust myDust = null)
        {
            if (Draw != null && Draw.Invoke())
            {
                for (int i = 0; i < DustConst; i++)
                {
                    Vector2 DustCenter = center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * (float)i / DustConst) * dis;
                    Dust dust = Main.dust[Dust.NewDust(DustCenter, 1, 1, type)];
                    myDust?.Invoke(dust);
                }
            }
        }
        private static void AddHook(EnergyBulletItem bulletItem, EnergyProj BulletProj, int projWhoAmI)
        {
            BulletProj.Proj_AI += bulletItem.ProjAI;
            BulletProj.Proj_Colliding += bulletItem.Colliding;
            BulletProj.Proj_Kill += bulletItem.Kill;
            BulletProj.Proj_OnHitNPC += bulletItem.ProjOnHitNPC;
            BulletProj.Proj_OnTileCollide += bulletItem.OnTileCollide;
            BulletProj.Proj_Draw += bulletItem.ProjPreDraw;
            Main.projectile[projWhoAmI].damage += bulletItem.Item.damage;
        }
        private static void DelHook(EnergyBulletItem bulletItem, EnergyProj BulletProj, int projWhoAmI)
        {
            BulletProj.Proj_AI -= bulletItem.ProjAI;
            BulletProj.Proj_Colliding -= bulletItem.Colliding;
            BulletProj.Proj_Kill -= bulletItem.Kill;
            BulletProj.Proj_OnHitNPC -= bulletItem.ProjOnHitNPC;
            BulletProj.Proj_OnTileCollide -= bulletItem.OnTileCollide;
            BulletProj.Proj_Draw -= bulletItem.ProjPreDraw;
            Main.projectile[projWhoAmI].damage -= bulletItem.Item.damage;

        }
    }
    public static class StarBreakerWay_ByExtension
    {
        public static void NPC_AddOnHitDamage(this NPC npc,Player player, int damage, bool ByDefense = false)
        {
            int LastDamage = damage;
            if (ByDefense)
            {
                LastDamage -= npc.defense;
            }

            npc.StrikeNPC(LastDamage, 0, npc.direction);
            npc.checkDead();
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, damage);
            }
        }
        public static void SacrificeCountNeededByItemId(this Item item, int num) => CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[item.type] = num;

        public static Vector2 RealSafeNormalize(this Vector2 vector2)
        {
            Vector2 vector = Vector2.Normalize(vector2);
            if (vector.HasNaNs())
            {
                vector = Vector2.Zero;
            }

            return vector;
        }
        public static Vector2 AbsVector2(this Vector2 vector2)
        {
            float x = Math.Abs(vector2.X);
            float y = Math.Abs(vector2.Y);
            return new Vector2(x, y);
        }
        public static NPC FindTargetNPC(this Player player, float maxDis = 800)
        {
            foreach (NPC npc in Main.npc)
            {
                float dis = Vector2.Distance(player.Center, npc.Center);
                if (npc.CanBeChasedBy() && npc.active && !npc.friendly && maxDis > dis)
                {
                    maxDis = dis;
                    player.MinionAttackTargetNPC = npc.whoAmI;
                }
            }
            if (player.HasMinionAttackTargetNPC)
            {
                return Main.npc[player.MinionAttackTargetNPC];
            }
            return null;
        }
        public static Vector2 NormalVector(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }
    }
}
