using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarBreakerHeadProj : ModProjectile
    {
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override string Texture => "StarBreaker/Items/Weapon/StarBreakerW";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = 40;
            Projectile.width = 142;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            #region 杂鱼
            Player player = Main.player[Projectile.owner];
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            Vector2 toMouse = Main.MouseWorld - player.Center;
            Lighting.AddLight(Projectile.Center, new Vector3(1, 1, 1));
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (!player.controlUseTile && !player.channel)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.velocity = toMouse.SafeNormalize(Vector2.UnitX);
            }
            Timer++;
            #endregion
            #region 关键
            if (player.altFunctionUse != 2)
            {
                Vector2 ves = player.RotatedRelativePoint(player.MountedCenter, true)
                - Projectile.Size * 0.5f + new Vector2(toMouse.X, toMouse.Y);
                Projectile.position = ves;
            }
            else
            {
                Vector2 ves1 = player.RotatedRelativePoint(player.MountedCenter, true)
                    - Projectile.Size * 0.5f + new Vector2(Projectile.velocity.X * 500, Projectile.velocity.Y * 250);
                Projectile.position = ves1;
                Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
            }
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction,
                Projectile.velocity.X * Projectile.direction);
            #endregion
            #region 射弹幕
            if (Timer > 40 - State)
            {
                if ((starPlayer.Bullet1 is not null || starPlayer.Bullet2 is not null) && (!starPlayer.Bullet1.IsAir || !starPlayer.Bullet2.IsAir))
                {
                    int shootID, shootDamage;
                    Items.EnergyBulletItem bulletItem;
                    StarBreakerWay.StarBrekaerUseBulletShoot(starPlayer, out shootID, out shootDamage, out bulletItem);
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                    for (float i = -5; i <= 5; i++)
                    {
                        Vector2 vec = (i.ToRotationVector2() * MathHelper.Pi / 18) + Projectile.velocity.SafeNormalize(Vector2.Zero);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vec * 10, shootID, player.GetWeaponDamage(player.HeldItem) + shootDamage,
                            player.GetWeaponKnockback(player.HeldItem, 1), player.whoAmI, 0);
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].hostile = false;
                        StarBreakerWay.Add_Hooks_ToProj(bulletItem, proj);
                    }
                }
                if (State < 20) State++;
                Timer = 0;
                Projectile.damage += (int)State;
                if (Projectile.damage > player.inventory[player.selectedItem].damage * State) Projectile.damage = (int)(player.inventory[player.selectedItem].damage * State);
            }
            #endregion
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                damage += (int)Main.player[Projectile.owner].velocity.Length() * 5;
                if (damage > 700)
                {
                    damage = 700;
                }
                damage += (int)(Main.player[Projectile.owner].GetModPlayer<StarPlayer>().StarCharge * Main.rand.NextFloat(2));
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy) return;
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
            #region 充能
            Player player = Main.player[Projectile.owner];
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            if (starPlayer.StarCharge < 100)
            {
                starPlayer.StarCharge += 15;
            }
            if (starPlayer.StarCharge > 100)
            {
                starPlayer.StarCharge = 100;
            }
            else if (starPlayer.StarCharge == 100)
            {
                int da = 0;
                for (int i = 0; i < 5; i++)
                {
                    int AddDamage = damage + 100;
                    target.life -= AddDamage;
                    player.dpsDamage += AddDamage;
                    target.checkDead();
                    da += AddDamage;
                    CombatText.NewText(target.Hitbox, CombatText.OthersDamagedHostile, AddDamage);
                }
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, da, target.whoAmI);
                starPlayer.StarCharge = 0;
            }
            #endregion
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -1.5f;
            if (crit)
            {
                target.velocity *= 2;
            }
        }
    }
}
