using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    internal class ProjDrawStar : ModProjectile
    {
        private Vector2 EnCenter => new(Projectile.ai[0], Projectile.ai[1]);
        private List<Vector2> starOldPos = new();
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰绘制");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 10;
            //Projectile.hide = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == 2)
            {
                writer.Write(Projectile.localAI[0]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode != 1)
            {
                Projectile.localAI[0] = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            if (starOldPos == null)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.localAI[0] > 0) starOldPos.Add(Projectile.position);
            const float rot = 0.314159f;
            Vector2[] starPos =
            {
                EnCenter + (((MathHelper.TwoPi / 5) - rot).ToRotationVector2() * 800),
                EnCenter + (((MathHelper.TwoPi / 5 * 2)- rot).ToRotationVector2() * 800),
                EnCenter + (((MathHelper.TwoPi / 5 * 3)- rot).ToRotationVector2() * 800),
                EnCenter + (((MathHelper.TwoPi / 5 * 4)- rot).ToRotationVector2() * 800),
                EnCenter + ((MathHelper.TwoPi- rot).ToRotationVector2() * 800),
            };
            Projectile.timeLeft = 2;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.velocity = (starPos[3] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[3], Projectile.position) < 5)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 1:
                    Projectile.velocity = (starPos[1] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[1], Projectile.position) < 5)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 2:
                    Projectile.velocity = (starPos[4] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[4], Projectile.position) < 5)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 3:
                    Projectile.velocity = (starPos[2] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[2], Projectile.position) < 5)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 4:
                    Projectile.velocity = (starPos[0] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[0], Projectile.position) < 5)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 5:
                    Projectile.velocity = (starPos[3] - Projectile.position).SafeNormalize(default);
                    if (Vector2.Distance(starPos[3], Projectile.position) < 5)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
            Projectile.velocity *= 2f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            for (int i = 0; i < starOldPos.Count; i++)
            {
                if (i - 1 <= 0) continue;
                Vector2 drawCenter = starOldPos[i] - Main.screenPosition;
                Main.spriteBatch.Draw(tex, drawCenter, null, Color.Purple, (starOldPos[i - 1] - starOldPos[i]).ToRotation(), Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            //for (float r = 0;r<=MathHelper.TwoPi;r+= MathHelper.TwoPi/5)
            //{
            //    Vector2 center = EnCenter + (r.ToRotationVector2() * 100) - Main.screenPosition;
            //    Main.spriteBatch.Draw(tex,center, null, Color.Purple, 0f, Vector2.Zero, new Vector2(r, 1), SpriteEffects.None, 0f);
            //}
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool flag = false;
            for (int i = 0; i < starOldPos.Count; i++)
            {
                if (starOldPos.Count < 20) continue;
                flag = Vector2.Distance(targetHitbox.TopLeft(),
                    starOldPos[i]) < 5;
            }
            return flag;
        }
    }
}
