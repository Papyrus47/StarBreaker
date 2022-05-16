using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.HradMode
{
    public class OnyxBlasterGun : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OnyxBlaster;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Blaster");
            DisplayName.AddTranslation(7, "玛瑙爆破枪");
            Tooltip.SetDefault("Stronger than ever");
            Tooltip.AddTranslation(7, "比过去更强");
        }
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1.2f;
            Item.crit = 30;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.notAmmo = true;
            Item.rare = ItemRarityID.Purple;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<OnyxBlasterGunProj>();
            Item.shootSpeed = 1;
            Item.width = 60;
            Item.height = 23;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
        public override bool CanConsumeAmmo(Player player)
        {
            if(player.heldProj < 0)
            {
                return false;
            }
            return true;//避免消耗子弹,让手持弹幕自己消耗
        }
    }
    public class OnyxBlasterGunProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OnyxBlaster;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Blaster");
            DisplayName.AddTranslation(7, "玛瑙爆破枪");
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 23;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.glowMask = 220;
        }
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            if (!player.channel)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            if (Projectile.spriteDirection == -1) Projectile.rotation += MathHelper.Pi;
            player.ChangeDir(Projectile.direction);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).RealSafeNormalize();
                player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                    Projectile.velocity.X * Projectile.direction);
            }
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * 12;
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && player.HasAmmo(player.HeldItem, true))
                {
                    int damage = 0, ammoID = AmmoID.Bullet;
                    float speed = 0, kn = 0f;
                    bool canUse = true;
                    player.PickAmmo(player.HeldItem, ref ammoID, ref speed, ref canUse, ref damage, ref kn, out ammoID);
                    if (canUse)
                    {
                        if (Projectile.ai[1] > 8)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Projectile.Center, Projectile.velocity * 15,
                                    661, Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                            if (Main.myPlayer == player.whoAmI)
                            {
                                Vector2 center = Main.MouseWorld;
                                float maxDis = 800;
                                NPC n = null;
                                foreach (NPC npc in Main.npc)
                                {
                                    float dis = Vector2.Distance(npc.Center, center);
                                    if (!npc.townNPC && npc.CanBeChasedBy() && npc.active && dis < maxDis)
                                    {
                                        n = npc;
                                        maxDis = dis;
                                    }
                                }
                                if (n != null)
                                {
                                    center = n.Center;
                                }
                                for (int i = -3; i <= 3; i++)
                                {
                                    Vector2 Realcenter = center;
                                    Realcenter += new Vector2(i * 60, -600);
                                    int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Realcenter, (Main.MouseWorld - Realcenter).RealSafeNormalize() * 30,
                                                                            661, Projectile.damage, Projectile.knockBack, player.whoAmI);
                                    Main.projectile[proj].tileCollide = false;
                                    Main.projectile[proj].extraUpdates = 5;
                                    Main.projectile[proj].timeLeft = 300;

                                    Realcenter = Projectile.Center;
                                    Realcenter -= Projectile.velocity.RotatedBy(i * 0.2) * 200;
                                    proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Realcenter, (Main.MouseWorld - Realcenter).RealSafeNormalize() * 30,
                                                                            661, Projectile.damage, Projectile.knockBack, player.whoAmI);
                                    Main.projectile[proj].tileCollide = false;
                                    Main.projectile[proj].extraUpdates = 1;
                                    Main.projectile[proj].timeLeft = 300;
                                }
                            }
                            Projectile.ai[1] = 0;
                        }
                        else
                        {
                            for (int i = 0; i < Projectile.ai[1] + 5; i++)
                            {
                                if (i > 10) break;
                                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Projectile.Center, Projectile.velocity.RotatedByRandom(0.2) * Main.rand.NextFloat(15,20),
                                    ammoID, Projectile.damage + damage, Projectile.knockBack, player.whoAmI);
                            }
                            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Projectile.Center, Projectile.velocity * 20,
                                661, Projectile.damage, Projectile.knockBack, player.whoAmI);
                        }
                    }
                }
                Projectile.ai[1]++;
                Projectile.ai[0] = 0;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //texture = TextureAssets.GlowMask[220].Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.2f, Projectile.rotation,
            //    texture.Size() * 0.5f, Projectile.scale, (SpriteEffects)Projectile.spriteDirection, 0);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin();
            return true;
        }
    }
}
