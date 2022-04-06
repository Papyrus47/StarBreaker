using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            Projectile.timeLeft = 200;
            Projectile.height = Projectile.width = 18;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ai[0]++;

            float timeToFlyOut, rangeMultiplier;
            int segments;
            Projectile.GetWhipSettings(Projectile, out timeToFlyOut, out segments, out rangeMultiplier);

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
                SoundEngine.PlaySound(SoundID.Item153.SoundId, (int)Projectile.position.X, (int)Projectile.position.Y, SoundID.Item153.Style, 1f, Main.rand.NextFloat(-0.8f, -0.2f));
            }
            #region 粒子与弹幕
            List<Vector2> list = new();//new一个list
            Projectile.FillWhipControlPoints(Projectile, list);//为list填充点
            if (Projectile.ai[0] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 pos = list[^1];
                Dust.NewDustDirect(pos, 2, 2, ModContent.DustType<Dusts.LastCopperWhipDust>());
                if (!Main.tile[(int)pos.X / 16, (int)pos.Y / 16].HasTile)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), pos, Projectile.velocity,
                        ModContent.ProjectileType<LastCopperWhipOnUseProj>(), Projectile.damage / 2, Projectile.knockBack, player.whoAmI, 3);
                    projectile.friendly = true;
                    projectile.hostile = false;
                }
            }
            #endregion
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.immortal) return;
            if (Projectile.ai[1] == 0) Projectile.ai[1] = Projectile.damage;
            Projectile.damage += (int)(Projectile.damage * 0.1f);
            if (Projectile.damage > Projectile.ai[1] * 10) Projectile.damage = (int)Projectile.ai[1] * 10;
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            #region 绘制线
            List<Vector2> list = new();//new一个list
            Projectile.FillWhipControlPoints(Projectile, list);//为list填充点
            Texture2D texture2D = TextureAssets.FishingLine.Value;//获取鱼线贴图
            Rectangle rectangle = texture2D.Frame();
            Color color = Color.White;//颜色
            Vector2 pos = list[0];//位置
            Vector2 origin = new Vector2(rectangle.Width / 2, 2f);//绘制偏移

            for (int i = 0; i < list.Count - 1; i++)//for循环绘制线
            {
                //剩下的无法理解（bushi
                Vector2 vector2 = list[i + 1] - list[i];
                float rot = vector2.ToRotation() - MathHelper.PiOver2;
                color = Lighting.GetColor(list[i].ToTileCoordinates(), color);
                if (list.Count > 0) color = Color.Lerp(Color.Orange, color, i / list.Count);
                Vector2 scale = new Vector2(1f, (vector2.Length() + 2f) / rectangle.Height);
                Main.spriteBatch.Draw(texture2D, pos - Main.screenPosition, new Rectangle?(rectangle), color, rot, origin, scale, SpriteEffects.None, 0);
                pos += vector2;
            }
            #endregion
            #region 绘制弹幕
            texture2D = TextureAssets.Projectile[Type].Value;//获取弹幕贴图
            rectangle = texture2D.Frame(1, 5);//切割帧图
            int height = rectangle.Height;
            rectangle.Height -= 2;//减少帧图高度
            origin = rectangle.Size() * 0.5f;
            pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)//绘制鞭子身子
            {
                bool canDraw = true;
                if (i != 0)
                {
                    switch (i)
                    {
                        case 3:
                        case 5:
                        case 7:
                            rectangle.Y = height;//鞭子图1
                            break;
                        case 9:
                        case 11:
                        case 13:
                            rectangle.Y = height * 2;//鞭子图2
                            break;
                        case 15:
                        case 17:
                            rectangle.Y = height * 3;//鞭子图3
                            break;
                        case 19:
                            rectangle.Y = height * 4;//鞭子图4
                            break;
                        default://其他情况不绘制
                            canDraw = false;
                            break;
                    }
                }
                else
                {
                    origin.Y -= 4;
                }
                Vector2 vector = list[i + 1] - list[i];
                if (canDraw)
                {
                    float rot = vector.ToRotation() - MathHelper.PiOver2;
                    Color alpha = Projectile.GetAlpha(Lighting.GetColor(list[i].ToTileCoordinates()));
                    Main.spriteBatch.Draw(texture2D, pos - Main.screenPosition, new Rectangle?(rectangle), alpha, rot, origin, 1f, SpriteEffects.None, 0f); ;
                }
                pos += vector;
            }
            #endregion
            return false;
        }
    }
}
