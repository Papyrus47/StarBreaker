using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Projs
{
    public class StarBreakerGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool ProjectileForSporadicBow = false;
        public bool ProjectileForLastBow = false;
        public int Bloody = 0;
        public override void SetDefaults(Projectile projectile)
        {
            base.SetDefaults(projectile);
            switch (projectile.type)
            {
                case 387:
                case 388:
                    {
                        projectile.usesLocalNPCImmunity = true;
                        break;
                    }
                case 407://鲨鱼龙卷
                case 759://小鸟
                    {
                        projectile.extraUpdates = 1;
                        break;
                    }
            }
        }
        public override bool PreAI(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.Hornet://黄蜂
                    {
                        Player player = Main.player[projectile.owner];
                        projectile.spriteDirection = -projectile.direction;
                        projectile.rotation = projectile.velocity.X * 0.05f;//改变旋转角度
                        float slotsPos = player.slotsMinions * projectile.height;

                        if (player.HasBuff(BuffID.HornetMinion))
                        {
                            projectile.timeLeft = 2;
                            player.hornet = true;
                        }
                        if (player.dead) player.hornet = false;
                        #region 帧图
                        if (projectile.frameCounter > 1)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                            if (projectile.frame > 2) projectile.frame = 0;
                        }
                        projectile.frameCounter++;
                        #endregion

                        #region 待机
                        if (!player.HasMinionAttackTargetNPC)
                        {
                            float max = 800;
                            foreach (NPC npc in Main.npc)
                            {
                                float dis = Vector2.Distance(npc.position, projectile.position);
                                if (dis < max && npc.CanBeChasedBy() && npc.active && !npc.friendly)
                                {
                                    max = dis;
                                    player.MinionAttackTargetNPC = npc.whoAmI;
                                }
                            }
                            Vector2 pos = player.Center + new Vector2(projectile.velocity.X * 10, -20 - slotsPos);
                            projectile.velocity = (projectile.velocity * 20 + Vector2.Normalize(pos - projectile.position) * 8) / 21;
                        }
                        #endregion
                        #region 发射
                        else if ((projectile.position - projectile.OwnerMinionAttackTargetNPC.Center).Length() - slotsPos < 400f && projectile.ai[1] == 0f)
                        {
                            projectile.ai[1] = 60f;
                            if (Main.myPlayer == projectile.owner)
                            {
                                for (int i = -2; i <= 2; i++)
                                {
                                    Vector2 toTarget = projectile.OwnerMinionAttackTargetNPC.Center - projectile.position;
                                    toTarget.Normalize();
                                    toTarget = toTarget.RotatedBy(MathHelper.Pi / 32 * i);
                                    toTarget *= 10f;
                                    Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, toTarget, ProjectileID.HornetStinger,
                                        projectile.damage, 0f, Main.myPlayer);
                                }
                                projectile.netUpdate = true;
                            }
                        }
                        #endregion
                        #region 飞行
                        else
                        {
                            if (projectile.ai[1] > 0) projectile.ai[1]--;
                            NPC npc = projectile.OwnerMinionAttackTargetNPC;
                            projectile.velocity = (projectile.velocity * 20 + (npc.position - projectile.position).SafeNormalize(default) * 8f) / 21;
                            if (projectile.position.Y > npc.position.Y - (50 + slotsPos))
                            {
                                projectile.velocity.Y -= 1f;
                            }
                        }
                        #endregion
                        return false;
                    }
                case ProjectileID.FlyingImp://小鬼,8帧,射出376
                    {
                        Player player = Main.player[projectile.owner];
                        projectile.rotation = projectile.velocity.X * 0.05f;//改变旋转角度
                        if (player.HasBuff(BuffID.ImpMinion))
                        {
                            projectile.timeLeft = 2;
                            player.impMinion = true;
                        }
                        if (player.dead) player.impMinion = false;
                        #region 帧图
                        if (projectile.frameCounter >= 16)
                        {
                            projectile.frameCounter = 0;
                        }
                        projectile.frame = projectile.frameCounter / 4;
                        if (projectile.ai[1] > 0f && projectile.ai[1] < 16f)
                        {
                            projectile.frame += 4;
                        }
                        projectile.frameCounter++;
                        #endregion
                        #region 挂机
                        if (!player.HasMinionAttackTargetNPC)
                        {
                            float max = 800;
                            foreach (NPC npc in Main.npc)
                            {
                                float dis = Vector2.Distance(npc.position, projectile.position);
                                if (dis < max && npc.CanBeChasedBy() && npc.active && !npc.friendly)
                                {
                                    max = dis;
                                    player.MinionAttackTargetNPC = npc.whoAmI;
                                }
                            }
                            #region 待机移动
                            Vector2 vel = player.Center - projectile.Center - new Vector2(0f, 60f);
                            float velSpeed = 6f;
                            vel.X -= 40f * player.slotsMinions * player.direction;
                            vel.Y -= 10f;
                            float velLength = vel.Length();
                            if (velLength > 2000f)
                            {
                                projectile.position = player.position;
                            }//传送
                            else if (velLength > 500f) velSpeed = 10f;//改变速度
                            if (velLength > 200f && velSpeed < 9f)
                            {
                                velSpeed = 9f;//加速
                            }

                            if (velLength > 10f)
                            {
                                vel.Normalize();
                                if (velLength < 50f)
                                {
                                    velSpeed /= 2f;
                                }
                                vel *= velSpeed;
                                projectile.velocity = (projectile.velocity * 20 + vel) / 21;
                            }
                            else
                            {
                                projectile.direction = player.direction;
                                projectile.velocity *= 0.9f;
                            }
                            #endregion
                        }
                        #endregion
                        #region 攻击,16帧一攻击,发射三串火球,攻击3次后瞬移
                        else if (projectile.ai[1] <= 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            projectile.ai[0]++;
                            projectile.ai[1] = 16f;
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 vel = (projectile.OwnerMinionAttackTargetNPC.Center - projectile.Center).SafeNormalize(default) * 10;
                                vel *= (i + 1) / 1.1f;
                                Projectile fireProj = Main.projectile[Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(),
                                    projectile.Center, vel,
                                    376, projectile.damage, 0f, projectile.owner)];
                                fireProj.extraUpdates = 2;
                            }
                        }
                        #endregion
                        #region 传送
                        else
                        {
                            projectile.velocity *= 0.9f;
                            if (projectile.velocity.Length() < 1f)
                            {
                                projectile.ai[1]--;
                                if (projectile.ai[1] <= 0f && projectile.ai[0] >= 3f)
                                {
                                    projectile.ai[0] = 0f;
                                    while (true)
                                    {
                                        projectile.Center = projectile.OwnerMinionAttackTargetNPC.Center + Main.rand.NextVector2Unit() * 200;
                                        Tile tile = Main.tile[(int)projectile.Center.X / 16, (int)projectile.Center.Y / 16];
                                        if (tile == default(Tile) || !tile.HasTile)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion                        
                        projectile.spriteDirection = -projectile.direction;
                        return false;
                    }
            }
            if(ProjectileForLastBow)
            {
                if(projectile.tileCollide) projectile.penetrate = 5;//如果是非穿墙箭使它穿透5次
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 0;
                projectile.tileCollide = false;
                if(projectile.velocity.Length() > 2f && projectile.timeLeft % 5 == 0)
                {
                    projectile.velocity *= 0.9f;
                }
            }
            return base.PreAI(projectile);
        }
        public override void AI(Projectile projectile)
        {
            base.AI(projectile);
            if (ProjectileForSporadicBow)
            {
                //ai[0]指向目标
                //ai[1]是旋转的弧度
                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc != null && npc.Center.Distance(projectile.position) < 400 && projectile.ai[1] < 10)
                {
                    projectile.velocity = projectile.velocity.RotatedBy(MathHelper.PiOver2 / 5);
                    projectile.ai[1]++;
                }
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.OwnerMinionAttackTargetNPC != null && projectile.OwnerMinionAttackTargetNPC.GetGlobalNPC<NPCs.StarGlobalNPC>().DrumHitDamage > 0)
            {
                int addDa = damage / 5;
                if(addDa > 20)addDa = 20;
                damage += addDa;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(projectile, target, damage, knockback, crit);
            switch (projectile.type)
            {
                case ProjectileID.VampireFrog:
                    {
                        if (Main.rand.Next(10) == 0)
                        {
                            Player player = Main.player[projectile.owner];
                            int heal = damage / 2;
                            if (heal < 1) heal = 1;
                            if (player.statLife + heal > player.statLifeMax2) heal = player.statLifeMax2 - player.statLife;
                            player.statLife += heal;
                            player.HealEffect(heal);
                        }
                        break;
                    }
            }
            if(Bloody > 0)
            {
                target.GetGlobalNPC<NPCs.StarGlobalNPC>().BloodyBleed += Bloody;
            }
        }
    }
}
