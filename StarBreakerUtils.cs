using ReLogic.Graphics;
using StarBreaker.Content.Appraise;
using System.Reflection;
using Terraria.Graphics.Renderers;

namespace StarBreaker
{
    public static class StarBreakerUtils
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
        public static void DrawTailTexInPos(Texture2D texture, Vector2[] oldPos, Color BeginColor, Color EndColor, float rot, int dir = 1, Vector2 origin = default, Rectangle? sourceRectangle = null, float[] oldRot = null, bool DrawScale = false)
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
                if (DrawScale)
                {
                    scale -= (float)i / oldPos.Length;
                }
                Main.spriteBatch.Draw(texture, oldPos[i] - Main.screenPosition, sourceRectangle, color, myRot, origin, scale, spriteEffects, 0f);
            }
        }
        /// <summary>
        /// 为你的对象绘制一个边界,但是不能是玩家,仅限于npc和弹幕
        /// 这个东西不能对非正常Draw的使用
        /// </summary>
        /// <param name="entity">对象</param>
        public static void EntityDrawLight(Entity entity, Color lightColor)
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
                if (npc.spriteDirection == -1)
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
                if (projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            else
            {
                return;//为了下面调用tex不报错
            }

            Rectangle rectangle = new(0, frame, texture.Width, hegiht);
            Vector2 origin = rectangle.Size() / 2f;

            Color floatingDaggerMinionGlowColor = color;
            floatingDaggerMinionGlowColor.A /= 4;
            for (int j = 0; j < 4; j++)
            {
                Vector2 DrawCenter = entity.Center - Main.screenPosition + new Vector2(0f, gfxOffY);
                Vector2 spinningpoint = rot.ToRotationVector2();
                double radians = (double)(Math.PI / 2 * j);
                Vector2 center = default;
                Main.EntitySpriteDraw(texture, DrawCenter + spinningpoint.RotatedBy(radians, center) * 2f, new Rectangle?(rectangle), floatingDaggerMinionGlowColor, rot, origin, scale, 0, 0);
            }
            if (entity is NPC)
            {
                rot += MathHelper.PiOver2;
            }
            Main.EntitySpriteDraw(texture, pos - Main.screenPosition + new Vector2(0f, gfxOffY), new Rectangle?(rectangle), color, rot, origin, scale, spriteEffects, 0);
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
        public static void NewDustByYouself(Vector2 center, int type, Func<bool> Draw, Vector2 vector, float dis = 200, int DustConst = 30, MyDust myDust = null)
        {
            if (Draw != null && Draw.Invoke())
            {
                for (int i = 0; i < DustConst; i++)
                {
                    Vector2 DustCenter = center + vector.RotatedBy(MathHelper.TwoPi * i / DustConst) * dis;
                    Dust dust = Main.dust[Dust.NewDust(DustCenter, 1, 1, type)];
                    myDust?.Invoke(dust);
                }
            }
        }
        public static float Vector2ToFloat_Atan2(Vector2 vector2, int dir = 1)
        {
            return (float)Math.Atan2(vector2.Y * dir, vector2.X * dir);
        }

        public static void DrawString(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, float rot, Vector2 origin, float scale, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.DrawString(FontAssets.MouseText.Value, text, pos, color, rot, origin, scale, spriteEffects, 0f);
        }

        public static void DrawString(SpriteBatch spriteBatch, DynamicSpriteFont dynamicSpriteFont, string text, Vector2 pos, Color color, float rot, Vector2 origin, float scale, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            spriteBatch.DrawString(dynamicSpriteFont, text, pos, color, rot, origin, scale, spriteEffects, 0f);
        }

        public static void AddParticle(IParticle particle, bool BehindPlayers = false)
        {
            if (BehindPlayers)
            {
                Main.ParticleSystem_World_BehindPlayers.Add(particle);
            }
            else
            {
                Main.ParticleSystem_World_OverPlayers.Add(particle);
            }
        }
        /// <summary>
        /// 生成三角形的函数
        /// </summary>
        /// <returns></returns>
        public static List<CustomVertexInfo> GenerateTriangle(List<CustomVertexInfo> customs)
        {
            List<CustomVertexInfo> triangleList = new();
            for (int i = 0; i < customs.Count - 2; i += 2)
            {
                triangleList.Add(customs[i]);
                triangleList.Add(customs[i + 2]);
                triangleList.Add(customs[i + 1]);

                triangleList.Add(customs[i + 1]);
                triangleList.Add(customs[i + 2]);
                triangleList.Add(customs[i + 3]);
            }
            return triangleList;
        }
        public static string GetAppraiseDrawFont(AppraiseID appraiseID)
        {
            string drawFont = null;
            switch (appraiseID)
            {
                case AppraiseID.Deadly: drawFont = "D"; break;
                case AppraiseID.Crazily: drawFont = "C"; break;
                case AppraiseID.Blast: drawFont = "B"; break;
                case AppraiseID.A: drawFont = "A"; break;
                case AppraiseID.S: drawFont = "S"; break;
                case AppraiseID.SS: drawFont = "SS"; break;
                case AppraiseID.SSS: drawFont = "SSS"; break;
                case AppraiseID.KillGod: drawFont = "KG!"; break;
            }
            return drawFont;
        }
        public static Color GetAppraiseDrawColor(AppraiseID appraiseID)
        {
            Color color = Color.Transparent;
            switch (appraiseID)
            {
                case AppraiseID.Deadly: color = Color.White * 0.5f; break;
                case AppraiseID.Crazily: color = Color.White * 0.8f; break;
                case AppraiseID.Blast: color = Color.SkyBlue * 1.3f; break;
                case AppraiseID.A: color = Color.OrangeRed; break;
                case AppraiseID.S: color = Color.Yellow; break;
                case AppraiseID.SS: color = Color.Lerp(Color.Yellow, Color.Gold, 0.5f); break;
                case AppraiseID.SSS: color = Color.Gold; break;
                case AppraiseID.KillGod: color = Color.BlueViolet; break;
            }
            return color;
        }
        public static Rectangle GetApprasieDrawRect(AppraiseID appraiseID)
        {
            Rectangle rect = default;
            rect.Height = 20;
            switch (appraiseID)
            {
                case AppraiseID.Deadly:
                    rect.Width = 22;
                    break;
                case AppraiseID.Crazily:
                    rect.X = 27;
                    rect.Width = 19;
                    break;
                case AppraiseID.Blast:
                    rect.X = 51;
                    rect.Width = 18;
                    break;
                case AppraiseID.A:
                    rect.X = 74;
                    rect.Width = 19;
                    break;
                case AppraiseID.S:
                    rect.X = 98;
                    rect.Width = 16;
                    break;
                case AppraiseID.SS:
                    rect.X = 119;
                    rect.Width = 31;
                    break;
                case AppraiseID.SSS:
                    rect.X = 155;
                    rect.Width = 46;
                    break;
                case AppraiseID.KillGod:
                    rect.X = 206;
                    rect.Width = 52;
                    break;
            }
            return rect;
        }


    }
}
