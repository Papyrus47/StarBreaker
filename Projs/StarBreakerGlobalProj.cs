namespace StarBreaker.Projs
{
    public class StarBreakerGlobalProj : GlobalProjectile//这就是一个继承了GlobalProj的类
    {
        public override bool InstancePerEntity => true;//就这个
        //这一个东西是允许Global系列创建实例字段
        public bool ProjectileForSporadicBow = false;
        public bool ProjectileForLastBow = false;
        public int Bloody = 0;
        public int ProjOwner_NPC;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ProjOwner_NPC = -1;
            if (source is EntitySource_Parent entity)//就像这样
            {
                if (entity.Entity is NPC npc)
                {
                    ProjOwner_NPC = npc.whoAmI;
                }
            }
        }
        public override void SetDefaults(Projectile projectile)//那么这里的这一个proj,是对所有存活着的弹幕都有效的
                                                               //大部分时候,Global和Mod系列不会差在哪里
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
            if (StarBreakerSystem.SpecialBattle != null)
            {
                projectile.tileCollide = false;
            }
        }
        public override bool PreAI(Projectile projectile)
        //这里是修改弹幕ai的地方
        //用了PreAI
        //知道为什么每个mod要是修改 同一个东西就会出现冲突吗?
        //因为tml会遍历所有mod的Global系列对应部分
        //然后调用
        //也就是说
        //mod安装优先度越后,mod就会被越后调用(也就是覆盖了前者的ai,但是又没有完全覆盖,因为ai[0]之类的东西)
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
                        if (player.dead)
                        {
                            player.hornet = false;
                        }
                        #region 帧图
                        if (projectile.frameCounter > 1)
                        {
                            projectile.frame++;
                            projectile.frameCounter = 0;
                            if (projectile.frame > 2)
                            {
                                projectile.frame = 0;
                            }
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
                            FlyingProj_Move(player, projectile, 4.5f, 9f);
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
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, toTarget, ProjectileID.HornetStinger,
                                        projectile.damage, 0f, Main.myPlayer);
                                }
                                projectile.netUpdate = true;
                            }
                        }
                        #endregion
                        #region 飞行
                        else
                        {
                            if (projectile.ai[1] > 0)
                            {
                                projectile.ai[1]--;
                            }

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
                        if (player.dead)
                        {
                            player.impMinion = false;
                        }
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
                            FlyingProj_Move(player, projectile);
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
                                Projectile fireProj = Main.projectile[Projectile.NewProjectile(projectile.GetSource_FromThis(),
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
                case ProjectileID.FlinxMinion://小雪怪,12帧
                    {
                        Player player = Main.player[projectile.owner];
                        int AttackNPC = -1;
                        projectile.Minion_FindTargetInRange(800, ref AttackNPC, true);//寻找敌人
                        projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0f).ToDirectionInt();//改变弹幕朝向
                        projectile.shouldFallThrough = player.position.Y + player.height - 12f > projectile.position.Y + projectile.height;//可以坠落平台
                        if (player.HasBuff(BuffID.FlinxMinion))//让弹幕存活
                        {
                            projectile.timeLeft = 2;
                            player.flinxMinion = true;
                        }

                        if (++projectile.frameCounter > 10 - ((int)Math.Abs(projectile.velocity.X) / 10))//修改帧图
                        {
                            projectile.frameCounter = 0;
                            if (++projectile.frame >= Main.projFrames[projectile.type])
                            {
                                projectile.frame = 0;
                            }
                        }
                        if (projectile.velocity.Y > 0.5f || projectile.velocity.Y < -0.5f)
                        {
                            projectile.frame = 2;
                        }//跳跃时候固定为第二帧
                        #region 待机
                        if (AttackNPC == -1)
                        {
                            Vector2 pos = player.Center;
                            pos.X -= 35 * player.width / 2f * player.direction;//改变位置
                            pos.X -= projectile.minionPos * 5f * player.direction;//改变最终位置

                            bool canHit = Collision.CanHit(projectile.Center, 1, 1, pos, 1, 1);
                            if (Vector2.Distance(pos, projectile.Center) < 20f)//速度变的很小时候
                            {
                                projectile.frame = 0;//停下脚步
                            }
                            if (projectile.velocity.Length() < 6f)
                            {
                                projectile.velocity.X += 0.5f * (pos.X - projectile.Center.X > 0f).ToDirectionInt();
                            }
                            else
                            {
                                projectile.velocity.X = (projectile.velocity.X > 0f).ToDirectionInt() * 5.5f;
                            }
                            if (projectile.velocity.Y == 0 && !canHit)
                            {
                                projectile.velocity.Y = -4.5f;
                            }
                        }
                        #endregion
                        #region 攻击ai
                        else
                        {
                            NPC npc = Main.npc[AttackNPC];//获取目标npc
                            float dis = Vector2.Distance(projectile.Center, npc.Center);
                            bool canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                            if (projectile.ai[0] == 0)//不处于攻击状态
                            {
                                if (projectile.velocity.Length() < 4f)
                                {
                                    projectile.velocity.X += (npc.Center.X - projectile.Center.X > 0f).ToDirectionInt() * 0.3f;//提升速度 
                                }
                                else
                                {
                                    projectile.velocity.X = (npc.Center.X - projectile.Center.X > 0f).ToDirectionInt() * 3.5f;
                                }
                            }

                            if (projectile.ai[0] == 0 && dis + npc.width < 200 && canHit && projectile.velocity.Y != 0f)//当未起跳,同时距离靠近的时候
                            {
                                projectile.ai[0] = 1;//起跳
                                projectile.velocity.Y = (npc.height * -0.1f) - 6f;//根据npc高度,控制跳跃高度
                            }
                            else if (!canHit && projectile.velocity.Y == 0f)
                            {
                                projectile.velocity.Y -= 6f;
                            }
                            if (projectile.ai[0] != 0 && projectile.velocity.Y == 0)//落地状态
                            {
                                projectile.ai[0] = 0;
                            }

                            if (!npc.active)
                            {
                                player.MinionAttackTargetNPC = -1;
                            }
                        }
                        #endregion
                        if (projectile.velocity.Y < 5f)
                        {
                            projectile.velocity.Y += 0.5f;
                        }

                        return false;
                    }
            }
            if (ProjectileForLastBow)
            {
                if (projectile.tileCollide)
                {
                    projectile.penetrate = 5;//如果是非穿墙箭使它穿透5次
                }

                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 0;
                projectile.tileCollide = false;
                if (projectile.velocity.Length() > 2f && projectile.timeLeft % 2 == 0)
                {
                    projectile.damage++;
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
                if (addDa > 20)
                {
                    addDa = 20;
                }

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
                        if (Main.rand.NextBool(10))//按下Alt + 回车使用小灯泡
                        {
                            Player player = Main.player[projectile.owner];
                            int heal = damage / 2;
                            if (heal < 1)
                            {
                                heal = 1;
                            }

                            if (player.statLife + heal > player.statLifeMax2)
                            {
                                heal = player.statLifeMax2 - player.statLife;
                            }

                            player.statLife += heal;
                            player.HealEffect(heal);
                        }
                        break;
                    }
                case ProjectileID.FlinxMinion:
                    {
                        if (projectile.ai[0] != 0)//回复到普通模式
                        {
                            projectile.ai[0] = 0;
                        }
                        break;
                    }
            }
            if (Bloody > 0)
            {
                target.GetGlobalNPC<NPCs.StarGlobalNPC>().BloodyBleed += Bloody;
            }
        }
        private void FlyingProj_Move(Player player, Projectile projectile, float velSpeed = 6f, float velSpeed_MoveFast = 10f)
        {
            #region 待机移动
            Vector2 vel = player.Center - projectile.Center - new Vector2(0f, 60f);
            vel.X -= 40f * player.slotsMinions * player.direction;
            vel.Y -= 10f;
            float velLength = vel.Length();
            if (velLength > 2000f)
            {
                projectile.position = player.position;
            }//传送
            else if (velLength > 500f)
            {
                velSpeed = velSpeed_MoveFast;//改变速度
            }

            if (velLength > 200f && velSpeed < 9f)
            {
                velSpeed = velSpeed_MoveFast / 2f;//加速
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
    }
}
