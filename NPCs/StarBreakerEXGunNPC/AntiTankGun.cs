using StarBreaker.Projs.Bosses.StarBreakerEX;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    public class AntiTankGun : EXGunNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("反坦克炮");
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Hide = true
            };//隐藏
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);//让它绘制
        }
        public override void GunAI()
        {
            switch (StarBreakerEX_NPC.ai[3])
            {
                case 1:
                case 2:
                    {
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
                        NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 8;
                        if (StarBreakerEX_NPC.ai[3] == 1)
                        {
                            NPC.Center = Vector2.Lerp(NPC.Center, StarBreakerEX_NPC.Center + new Vector2(200, 0), 0.1f);
                            if (StarBreakerEX_NPC.ai[2] == 2)//对应的状态
                            {
                                if (StarBreakerEX_NPC.ai[0] % 20 == 0)
                                {
                                    Shoot();
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
                        NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize() * 5;
                        NPC.Center = Vector2.Lerp(NPC.Center, StarBreakerEX_NPC.Center + new Vector2(200, 0), 0.1f);
                        if (StarBreakerEX_NPC.ai[0] > 20 && StarBreakerEX_NPC.ai[0] % 20 == 0)//可以发射
                        {
                            Shoot();
                        }
                        break;
                    }
                case 4:
                    {
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;//npc朝向
                        NPC.velocity = (Target.Center - NPC.Center).RealSafeNormalize();
                        NPC.Center = Target.Center + new Vector2(300);
                        if (StarBreakerEX_NPC.ai[2] == 2 && StarBreakerEX_NPC.ai[0] % 20 == 0)
                        {
                            Shoot();
                        }
                        break;
                    }
                case 5:
                    {
                        float rot = (Target.Center - NPC.Center).ToRotation();
                        bool X = Target.Center.X > NPC.Center.X;
                        NPC.rotation = rot + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);//NPC旋转部分
                        NPC.spriteDirection = NPC.direction = X ? 1 : -1;//npc朝向
                        NPC.velocity *= 0.9f;
                        break;
                    }
            }
        }
        private void Shoot()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                _ = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RealSafeNormalize() * 20
                    , ModContent.ProjectileType<AntiTankBoom>(), 40, 2.3f, Main.myPlayer);
            }
        }
    }
}
