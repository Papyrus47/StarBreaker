using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent;
using StarBreaker.Projs.Bosses.StarBreakerEX;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    public class SpikedCannon : EXGunNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("刺弹炮");
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
                            NPC.Center = Vector2.Lerp(NPC.Center, StarBreakerEX_NPC.Center + new Vector2(0, -200),0.1f);
                            if (StarBreakerEX_NPC.ai[2] == 4)//对应的状态
                            {
                                if (StarBreakerEX_NPC.ai[0] % 30 == 0)
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
                        NPC.Center = Vector2.Lerp(NPC.Center, StarBreakerEX_NPC.Center + new Vector2(0, -200), 0.1f);
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
                        NPC.Center = Target.Center + new Vector2(300, -300);
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
                Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RealSafeNormalize() * 10
                    , ModContent.ProjectileType<StarRocket_Hostile>(), 40, 2.3f, Main.myPlayer)];
                Timer1++;
                if(Timer1 > 3)
                {
                    Timer1 = 0;
                    projectile.scale = 2f;
                    projectile.ai[1] = Target.whoAmI;
                }
            }
        }
    }
}
