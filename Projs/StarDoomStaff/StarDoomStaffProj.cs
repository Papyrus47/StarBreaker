namespace StarBreaker.Projs.StarDoomStaff
{
    public class StarDoomStaffProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Doom Staff");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "终末之星杖");
        }
        public override void SetDefaults()
        {
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.width = 78;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HeldItem.type != ModContent.ItemType<Items.Weapon.StarDoomStaff>())
            {
                Projectile.Kill();
                return;
            }
            if (player.channel)//召唤水晶
            {
                player.itemTime = player.itemAnimation = 2;
                int mana = player.GetManaCost(player.HeldItem);
                player.statMana -= mana;
                player.CheckMana(mana);
                if (player.statMana <= 0)
                {
                    player.statMana = 0;
                }

                if (player.statMana >= mana)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                    {
                        int type = ModContent.ProjectileType<StarBoomCrystal>();
                        switch (Main.rand.Next(5))
                        {
                            case 0 when player.ownedProjectileCounts[ModContent.ProjectileType<StarControlCrystal>()] < 5: type = ModContent.ProjectileType<StarControlCrystal>(); break;
                        }
                        Vector2 center = player.Center + Main.rand.NextVector2Unit() * 50;
                        Vector2 vel = (Main.MouseWorld - center).RealSafeNormalize();
                        Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), center, vel * 20,
                            type, Projectile.damage, Projectile.knockBack, player.whoAmI)].originalDamage = Projectile.damage;
                        for (int i = 0; i < 20; i++)
                        {
                            Dust dust = Dust.NewDustDirect(center + (vel.RotatedBy(MathHelper.Pi / 10 * i) * 50), 3, 3, DustID.PurpleTorch);
                            dust.velocity = (center - dust.position).RealSafeNormalize() * 3;
                            dust.noGravity = true;
                        }
                    }
                    float max = 1200;
                    foreach (NPC npc in Main.npc)
                    {
                        float dis = Vector2.Distance(Projectile.Center, npc.Center);
                        if (npc.active && npc.CanBeChasedBy() && !npc.friendly && dis < max && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                        {
                            max = dis;
                            player.MinionAttackTargetNPC = npc.whoAmI;
                        }
                    }
                    if (player.HasMinionAttackTargetNPC)
                    {
                        Projectile.OwnerMinionAttackTargetNPC.GetGlobalNPC<NPCs.StarGlobalNPC>().StarDoomMark = true;
                    }
                }
            }
            Projectile.Center = player.Center + new Vector2(-50 * player.direction, -20);//改变位置
            Projectile.spriteDirection = Projectile.direction = player.direction;
            Projectile.rotation = -0.25f * Projectile.spriteDirection;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
