namespace StarBreaker.Projs.Waste
{
    public class SporadicBowProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("零星弓");
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.noDropItem = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 44;
            Projectile.height = 77;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            #region 手持部分
            if (!player.channel)
            {
                Projectile.Kill();
                return;
            }
            Vector2 ves1 = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size * 0.5f;
            Projectile.position = ves1;//改变位置

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.damage = 0;
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
            #region 增加伤害部分
            if (Timer > 300)
            {
                Projectile.Kill();
            }
            else
            {
                Timer++;
            }
            #endregion
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.HasAmmo(player.HeldItem))
            {
                int damage = player.GetWeaponDamage(player.HeldItem);
                player.PickAmmo(player.HeldItem,out int ID,out float speed, out damage,out float kn, out ID);
                for (int i = 0; i < 2; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int proj = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center, Projectile.rotation.ToRotationVector2() * Projectile.spriteDirection * speed, ID, Timer > 180 ? (int)Timer * 2 * damage : damage, kn, Projectile.owner);
                        Main.projectile[proj].extraUpdates = 5;
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].hostile = false;
                        if (i == 1)
                        {
                            Main.projectile[proj].velocity = Main.projectile[proj].velocity.RotatedByRandom(0.1);
                        }

                        if (Timer > 180)
                        {
                            Main.projectile[proj].GetGlobalProjectile<StarBreakerGlobalProj>().ProjectileForSporadicBow = true;
                            float max = 1200;
                            foreach (NPC npc in Main.npc)
                            {
                                float dis = npc.Center.Distance(Projectile.position);
                                if (npc.active && npc.CanBeChasedBy() && !npc.friendly && dis < max)
                                {
                                    max = dis;
                                    Main.projectile[proj].ai[0] = npc.whoAmI;//ai[0]指向目标
                                }
                            }
                        }
                    }
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item102, Projectile.position);
            }
        }
    }
}
