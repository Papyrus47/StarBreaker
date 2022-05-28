using StarBreaker.Items.Weapon;

namespace StarBreaker.NPCs
{
    public class StarGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;//必须加上,且返回值为true,否则GlobalNPC不能创建实例字段
        public bool EnergySmash = false;//声明并赋值一个实例字段
        public bool StarDoomMark = false;
        public int BloodyBleed = 0;//流血效果
        public int CursedWhipHit = 0;//诅咒鞭
        public int StarSpiralBladeProj = -1;//星辰旋刃弹幕
        public int DrumHitDamage = 0;//鼓的标记伤害
        public int XuanYuSlowTime = 0;//宣雨减速
        public static int StarBreaker = -1;
        public static int StarGhostKnife = -1;
        public static int StarFrostFist = -1;
        public override bool PreAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<StarBreakerN>() || npc.type == ModContent.NPCType<StarBreakerEX>())
            {
                StarBreaker = -1;
            }

            if (npc.type == ModContent.NPCType<StarGhostKnife>())
            {
                StarGhostKnife = -1;
            }

            if (npc.type == ModContent.NPCType<FrostFist>())
            {
                StarFrostFist = -1;
            }

            return true;
        }
        public override void PostAI(NPC npc)
        {
            if (StarSpiralBladeProj > -1 && StarSpiralBladeProj <= 200)
            {
                if (!Main.projectile[StarSpiralBladeProj].active || npc.type == ModContent.NPCType<StarSpiralBladeN>())
                {
                    StarSpiralBladeProj = -1;
                    return;
                }
                npc.velocity += (Main.projectile[StarSpiralBladeProj].position - npc.position).SafeNormalize(default);
            }
            if (XuanYuSlowTime > 0)
            {
                XuanYuSlowTime--;
                if (npc.velocity.Length() > 10f)
                {
                    npc.velocity = npc.velocity.RealSafeNormalize() * 10;
                }
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (StarDoomMark && projectile.friendly && !projectile.hostile && projectile.minion)
            {
                Player player = Main.player[projectile.owner];
                StarDoomMark = false;
                damage += (int)((player.GetDamage(DamageClass.Summon).Additive * 100f + player.statDefense + player.statLife) * 0.02f);
            }
            if (DrumHitDamage > 0 && projectile.minion)
            {
                damage += DrumHitDamage;
                DrumHitDamage = 0;
            }
            if (StarBreakerSystem.downedStarBreakerNom && !Main.player[projectile.owner].HasItem(ModContent.ItemType<StarBreakerW>()))
            {
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ModContent.ItemType<StarBreakerW>());
            }
            base.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        }
        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            //在这个重写函数进行判断
            if (EnergySmash)//如果buff存在了
            {
                damage = Main.CalculateDamageNPCsTake((int)damage, defense - 20);//调用原版伤害计算,然后转回伤害
                return false;
            }
            if (BloodyBleed > 0 && !npc.immortal)//血鞭
            {
                npc.life -= BloodyBleed;
                CombatText.NewText(npc.Hitbox, Color.Red, BloodyBleed);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, BloodyBleed);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < BloodyBleed; i++)
                    {
                        if (i > 20)
                        {
                            break;
                        }

                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                    }
                }
                if (!crit)
                {
                    BloodyBleed -= BloodyBleed / 3;
                }
            }
            if (CursedWhipHit >= 5)//诅咒鞭
            {
                CursedWhipHit = 0;
                npc.AddBuff(ModContent.BuffType<Buffs.IncantationFireInBody>(), 60);
            }
            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (EnergySmash)
            {
                drawColor = new(100, 100, 255);
            }
            if (XuanYuSlowTime > 0)
            {
                drawColor = Color.Blue;
            }
            base.DrawEffects(npc, ref drawColor);
        }
        public override void ResetEffects(NPC npc)//重置东西用的重写函数
        {
            EnergySmash = false;//这样可以避免buff不存在时,效果依然存在
            base.ResetEffects(npc);
        }
    }
}
