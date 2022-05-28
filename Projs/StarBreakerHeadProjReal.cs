using StarBreaker.Items;
using System.IO;
using Terraria.Audio;

namespace StarBreaker.Projs
{
    internal class StarBreakerHeadProjReal : ModProjectile
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
        private float Timer2
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                writer.Write(Timer2);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                Timer2 = reader.ReadSingle();
            }
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
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<OmnipotentGun>()] < 2)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<OmnipotentGun>(), 1, 1, player.whoAmI);
                (Main.projectile[proj].ModProjectile as OmnipotentGun).States[4] = player.ownedProjectileCounts[ModContent.ProjectileType<OmnipotentGun>()];
            }
            else if (player.altFunctionUse == 2)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.position) * 0.1f;
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
            Item holdItem = player.inventory[player.selectedItem];
            int damage = player.GetWeaponDamage(holdItem);
            float knockBack = holdItem.knockBack;
            bool canUse = (player.controlUseTile || player.channel) && !player.noItems && !player.CCed;
            switch (State)
            {
                case 0://发射5发正常的四柱子弹
                    {
                        Timer++;
                        if (canUse && Timer > 30)
                        {
                            if ((starPlayer.Bullet1 is not null || starPlayer.Bullet2 is not null) && (!starPlayer.Bullet1.IsAir || !starPlayer.Bullet2.IsAir))
                            {
                                StarBreakerWay.StarBrekaerUseBulletShoot(starPlayer, out int shootID, out int shootDamage, out EnergyBulletItem bulletItem);
                                for (float i = -5; i <= 5; i++)
                                {
                                    Vector2 vec = (i.ToRotationVector2() * MathHelper.Pi / 18) + Projectile.velocity.SafeNormalize(Vector2.Zero);
                                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vec * 10,
                                        shootID, damage + shootDamage, knockBack, player.whoAmI, 0);
                                    Main.projectile[proj].friendly = true;
                                    Main.projectile[proj].hostile = false;
                                    StarBreakerWay.Add_Hooks_ToProj(bulletItem, proj);
                                }
                            }
                            Timer = 0;
                            Timer2++;
                            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                            if (Timer2 >= 5)
                            {
                                State++;
                                Timer2 = 0;
                                Timer = 0;
                            }
                        }
                        break;
                    }
                case 1://发射5次平行散弹
                    {
                        Timer++;
                        if (canUse && Timer > 20)
                        {
                            if ((starPlayer.Bullet1 is not null || starPlayer.Bullet2 is not null) && (!starPlayer.Bullet1.IsAir || !starPlayer.Bullet2.IsAir))
                            {
                                StarBreakerWay.StarBrekaerUseBulletShoot(starPlayer, out int shootID, out int shootDamage, out EnergyBulletItem bulletItem);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (float i = -5; i <= 5; i++)
                                    {
                                        Vector2 center = Projectile.Center + ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * i * 10);
                                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 10,
                                            shootID, damage + shootDamage, knockBack, player.whoAmI, 0);
                                        Main.projectile[proj].friendly = true;
                                        Main.projectile[proj].hostile = false;
                                        StarBreakerWay.Add_Hooks_ToProj(bulletItem, proj);
                                    }
                                }
                            }
                            Timer = 0;
                            Timer2++;
                            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                            if (Timer2 >= 5)
                            {
                                State++;
                                Timer2 = 0;
                                Timer = 0;
                            }
                        }
                        break;
                    }
                case 2://使用GiGA女王（？
                    {
                        Timer++;
                        float[] rad = new float[2]{
                            (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * 0.45f,
                            (float)Math.Cos(Main.GlobalTimeWrappedHourly * 5) * 0.45f
                        };
                        for (int s = 0; s < rad.Length; s++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                (Projectile.velocity.ToRotation() + rad[s]).ToRotationVector2(), ModContent.ProjectileType<StarLine>(), damage * 5, knockBack, player.whoAmI);
                        }
                        if (canUse && Timer > 30)
                        {
                            for (float i = -2; i <= 2; i++)
                            {
                                Vector2 vec = (i.ToRotationVector2() * MathHelper.Pi / 10) + Projectile.velocity.SafeNormalize(Vector2.Zero);
                                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 14, vec * 5, ModContent.ProjectileType<StarRocket>(), damage, knockBack, player.whoAmI);
                                Main.projectile[proj].friendly = true;
                                Main.projectile[proj].hostile = false;
                            }
                            Timer = 0;
                            Timer2++;
                            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                            if (Timer2 >= 3)
                            {
                                State++;
                                Timer2 = 0;
                                Timer = 0;
                            }
                        }
                        break;
                    }
                case 3://拼刺刀！
                    {
                        Timer++;
                        if (Timer2 == 0)
                        {
                            Timer2 = Projectile.damage;
                        }

                        Projectile.damage = 300 + starPlayer.StarCharge;
                        if (Timer > 200)
                        {
                            State++;
                            Projectile.damage = (int)Timer2;
                            Timer2 = 0;
                            Timer = 0;
                        }
                        break;
                    }
                default:
                    {
                        State = 0;
                        Timer = 0;
                        Timer2 = 0;
                        break;
                    }
            }
            #endregion
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }

            Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 1 : 60;
            if (crit)
            {
                target.velocity *= 2;
            }
            if (State == 3)
            {
                target.velocity *= 0.01f;
                target.immune[Projectile.owner] = 5;
                Main.player[Projectile.owner].GetModPlayer<StarPlayer>().SummonStarShieldTime -= target.active ? 10 : 360;
            }
            else
            {
                target.velocity += (Projectile.rotation - (Projectile.spriteDirection == -1 ? 0f : MathHelper.Pi)).ToRotationVector2() * -0.5f;
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
            #region 说话
            if (!target.active)
            {
                string[] sayText = new string[5]
                {
                    "就这?就这?",
                    "吃我一刀!",
                    "我才是拼刺刀的赢家",
                    "Good ~ Bye",
                    "也许我觉得你可以复活一下再被我打死"
                };
                int rand = Main.rand.Next(5);
                if (rand == 4)
                {
                    int npc = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)target.position.X, (int)target.position.Y, target.type);
                    NPC newNPC = Main.npc[npc];
                    newNPC.life = 1;
                }
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = sayText[rand],
                    DurationInFrames = 120,
                    Velocity = new Vector2(0, 4),
                    Color = Color.Purple
                }, Projectile.Center);
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
