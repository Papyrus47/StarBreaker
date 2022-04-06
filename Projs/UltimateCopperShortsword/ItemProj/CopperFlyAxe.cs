using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs.UltimateCopperShortsword.ItemProj
{
    public class CopperFlyAxe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("铜斧");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 500;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (Projectile.wet || Main.raining)
            {
                Projectile.ai[1] = 1;
            }
            Projectile.rotation += Math.Abs(0.05f + Projectile.velocity.Length()) * 0.1f;
            Projectile.velocity.Y += 0.1f - Projectile.ai[0];
            Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16];
            if (tile != null)
            {
                if (Main.tileAxe[tile.TileType])
                {
                    Projectile.ai[0] += 0.1f;
                    new Player().PickTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 110);
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool() || damage > 50 || crit)
            {
                Projectile.timeLeft += 30;
            }
            if (Projectile.ai[1] == 1)
            {
                target.buffImmune[BuffID.Poisoned] = false;
                target.AddBuff(BuffID.Poisoned, 120);
            }
            float maxDis = 800;
            NPC n = null;
            target.immune[Projectile.owner] = 0;
            foreach (NPC npc in Main.npc)
            {
                if (npc.whoAmI != target.whoAmI && npc.active && npc.CanBeChasedBy() && !npc.friendly && maxDis > npc.Distance(Projectile.Center))
                {
                    maxDis = npc.Distance(Projectile.Center);
                    n = npc;
                }
            }
            if (n != null)
            {
                Projectile.velocity = (n.position - Projectile.Center) * 0.1f;
                if (Projectile.ai[0] < 0.1f) Projectile.ai[0] += 0.01f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 1)
            {
                Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[Type].Value,
                    Projectile.Center - Main.screenPosition, null, Color.LightGreen, Projectile.rotation, Terraria.GameContent.TextureAssets.Projectile[Type].Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
    }
}
