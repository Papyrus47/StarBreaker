using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.Projs.Bosses;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.NPCs
{
    [AutoloadBossHead]
    public class StarSpiralBladeN : FSMNPC
    {
        private Vector2 targetOldPos;
        public override string BossHeadTexture => Texture;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 600000;
            NPC.knockBackResist = 0;
            NPC.defense = 12;
            NPC.damage = 90;
            NPC.noGravity = NPC.noTileCollide = true;
            NPC.width = NPC.height = 80;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.friendly = false;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Bloodtower2");
                SceneEffectPriority = SceneEffectPriority.BossMedium;//曲子优先度
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange( //添加信息
               new IBestiaryInfoElement[]//创建一个数组
               {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,//说明本npc为星空
                    new FlavorTextBestiaryInfoElement("他的主人在发疯的时候,他一直在身边,现在他被他的主人叫来寻找其他星辰武器,最后发现全在你身上后,不得不开始战斗")//信息
               }
               );
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapon.StarSpiralBlade>()));
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target > 200 || !Target.active || Target.dead)
            {
                NPC.TargetClosest(true);
            }
            NPC.rotation += Main.GlobalTimeWrappedHourly;
            if (Target.immuneTime > 5)
            {
                Target.immuneTime = 5;
            }
            if (NPC.rotation > 31415)
            {
                NPC.rotation = 0;
            }
            if (Target.dead)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y > 80)
                {
                    NPC.active = false;
                }
                return;
            }
            int damage = NPC.damage;
            if (Main.expertMode) damage /= 2;
            else if (Main.masterMode) damage /= 3;
            if (NPC.life < NPC.lifeMax * 0.75f && State < 7)
            {
                State = 7;
                Main.NewText("准备好迎接无尽回旋了吗", Color.Purple);
            }
            else if (NPC.life < NPC.lifeMax * 0.35f && State < 14)
            {
                State = 14;
                Main.NewText("一起来跳最后一支舞吧", Color.Purple);
            }
            switch (State)
            {
                case 0://开幕
                    {
                        Timer1++;
                        NPC.rotation = -MathHelper.PiOver4;
                        if (Timer1 > 50)
                        {
                            Timer1 = 0;
                            string sayText = "";
                            switch (Timer2)
                            {
                                case 0:
                                    {
                                        sayText = "看起来你拿到了他们";
                                        break;
                                    }
                                case 1:
                                    {
                                        sayText = "不得不开打了...";
                                        break;
                                    }
                                case 2:
                                    {
                                        if (Target.name == "paparyus") sayText = "我会试图打败你的";
                                        else sayText = "等等,我可以投靠你啊";
                                        break;
                                    }
                                case 3:
                                    {
                                        if (Target.name == "paparyus") sayText = "废话不多说了,来吧";
                                        else sayText = "星 辰 旋 刃 加 入 了 队 伍 !";
                                        break;
                                    }
                            }
                            if (Timer2 >= 4)
                            {
                                Timer2 = 0;
                                if (Target.name != "paparyus")
                                {
                                    NPC.life = 0;
                                    NPC.checkDead();
                                    break;
                                }
                                State++;
                                #region 沙雕部分
                                #endregion
                                break;
                            }
                            Main.NewText(sayText, Color.Purple);

                            Timer2++;
                        }
                        break;
                    }
                case 1://冲刺
                case 11://冲刺并散发
                case 13://死亡绞杀
                case 14://最后的共舞
                case 17://无尽绞杀(原左右横杀)
                case 23://投掷血牙运动弹幕
                    {
                        if (State == 11)
                        {
                            if (Timer2 < 2) Timer2 = 2;
                        }
                        else if (State == 14 && NPC.life >= NPC.lifeMax * 0.35f)
                        {
                            State = 7;
                        }
                        if (Timer1 <= 0)
                        {
                            Timer1 = 60;
                            if (State == 13)
                            {
                                targetOldPos = Target.position + ((Target.position - NPC.position).SafeNormalize(default).RotatedBy(0.1) * 200);
                            }
                            else
                            {
                                targetOldPos = Target.position;
                            }
                            Timer2++;
                            if (Timer2 > 5)
                            {
                                Timer2 = 0;
                                Timer1 = 0;
                                targetOldPos = Vector2.Zero;
                                State++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            Timer1--;
                            if (State == 11 && Timer1 == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 8 * i) * 10;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                }
                            }
                            else if (State == 14 && Timer1 % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 pos = NPC.Center + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 4 * i - MathHelper.PiOver4) * 800);
                                    Vector2 vel = (pos - NPC.position).SafeNormalize(default).RotatedBy(MathHelper.PiOver4) * -10;
                                    Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer)].extraUpdates = 3;
                                }
                            }
                            else if (State == 23 && Timer1 == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    Vector2 vel = (Target.position - NPC.position).SafeNormalize(default).RotatedBy(0.2 * i) * 10;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer, 3, i);
                                }
                            }
                            if (State == 17 && Timer1 > 50)
                            {
                                NPC.position = Target.position + new Vector2(600 * (Timer2 % 2 == 0).ToDirectionInt(), 0);
                                targetOldPos = Target.position + ((Target.position - NPC.position).SafeNormalize(default) * 600);
                            }
                            NPC.velocity = (targetOldPos - NPC.Center) * 0.2f;
                            if (State == 13)
                            {
                                NPC.velocity *= 0.4f;
                            }
                            else if (State == 17) NPC.velocity = (targetOldPos - NPC.Center) * 0.06f;
                        }
                        break;
                    }
                case 2://回旋冲刺拦路
                    {
                        switch (Timer3)
                        {
                            case 0://瞬移到上或者下后，开始拦路
                                //瞬移上下根据玩家瞬移前速度决定
                                {
                                    if (NPC.alpha < 255) NPC.alpha += 10;
                                    else if (NPC.alpha > 255) NPC.alpha = 255;
                                    else
                                    {
                                        Vector2 pos;
                                        if (Target.velocity.X > 0)
                                        {
                                            pos = Target.position + new Vector2(0, 800);
                                            Timer2 = 1;
                                        }
                                        else
                                        {
                                            pos = Target.position + new Vector2(0, -800);
                                            Timer2 = -1;
                                        }
                                        NPC.position = pos;
                                        targetOldPos = Target.position;
                                        Timer3++;
                                        NPC.alpha = 0;
                                    }
                                    break;
                                }
                            case 1://开始回旋
                                {
                                    Timer1++;
                                    if (NPC.velocity.Length() < 1) NPC.velocity = Vector2.UnitX * 5;
                                    NPC.position = targetOldPos + ((targetOldPos - NPC.position).SafeNormalize(default).RotatedBy(MathHelper.ToRadians(1.5f) * Timer2) * 800);
                                    int findProj = Timer2 == -1 ? 0 : 1;
                                    if (Timer1 > 90)
                                    {
                                        Timer3++;
                                        Timer1 = 0;
                                    }
                                    else if (Timer1 % 6 == findProj && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, Vector2.Zero, ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                    }
                                    break;
                                }
                            default:
                                State++;
                                Timer1 = 0;
                                Timer2 = 0;
                                Timer3 = 0;
                                break;
                        }
                        break;
                    }
                case 3://四角冲刺
                    {
                        switch (Timer1)
                        {
                            case 0://归零
                                {
                                    Timer1++;
                                    return;
                                }
                            case 1://左下角
                            case 5:
                                {
                                    targetOldPos = Target.position + new Vector2(500, 100);
                                    break;
                                }
                            case 2://右下角
                                {
                                    targetOldPos = Target.position + new Vector2(-500, 100);
                                    break;
                                }
                            case 3://右上角
                                {
                                    targetOldPos = Target.position + new Vector2(-500, -100);
                                    break;
                                }
                            case 4://左上角
                                {
                                    targetOldPos = Target.position + new Vector2(500, -100);
                                    break;
                                }
                        }
                        if (Vector2.Distance(NPC.position, targetOldPos) > NPC.width)
                        {
                            NPC.velocity = (targetOldPos - NPC.Center) * 0.15f;

                        }
                        else if (Timer1 == 5)
                        {
                            Timer1 = 0;
                            State++;
                        }
                        else
                        {
                            Timer1++;
                        }
                        break;
                    }
                case 4://经典回旋
                case 22://绕一圈冲刺
                    {
                        float oldPosDis = 800;
                        if (State == 22) oldPosDis -= 400;
                        if (Timer1 == 0)
                        {
                            targetOldPos = Target.position;
                        }
                        else if (Timer1 < 200)
                        {
                            #region 限制圈
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 dustCenter = targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 200 * i) * oldPosDis);
                                Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.PurpleCrystalShard);
                                dust.noGravity = true;
                                dust.velocity *= 0;
                            }
                            if (Vector2.Distance(Target.position, targetOldPos) > oldPosDis)
                            {
                                Target.position = targetOldPos + (((Target.position - targetOldPos).SafeNormalize(default) * (oldPosDis - 1)));
                            }
                            #endregion
                            Vector2 pos = targetOldPos + ((NPC.position - targetOldPos).SafeNormalize(default).RotatedBy(0.1) * oldPosDis);
                            NPC.velocity = (pos - NPC.position) * 0.3f;
                            if (Timer1 % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (State == 22)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.32f, Main.myPlayer);
                                }
                                else
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (targetOldPos - NPC.position).SafeNormalize(default) * 20,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.32f, Main.myPlayer);
                                }
                            }
                        }
                        else
                        {
                            if (State == 22)
                            {
                                if (Timer2 == 1) NPC.velocity = (Target.position - NPC.position).SafeNormalize(default) * 20;
                                else if (Timer2 > 30)
                                {
                                    Timer1 = Timer2 = 0;
                                    State++;
                                }
                                Timer2++;
                            }
                            else
                            {
                                Timer1 = 0;
                                State++;
                                break;
                            }
                        }
                        Timer1++;
                        break;
                    }
                case 5://隐身，向下冲刺
                case 19://向上冲刺
                    {
                        switch (Timer3)
                        {
                            case 0://隐身
                                {
                                    NPC.velocity *= .9f;
                                    if (NPC.alpha < 255) NPC.alpha += 10;
                                    else if (NPC.alpha > 255) NPC.alpha = 255;
                                    else
                                    {
                                        NPC.alpha = 0;
                                        Timer3++;
                                        Vector2 vector2 = new Vector2(0, -500);
                                        if (State == 19) vector2.Y = 500;
                                        NPC.position = Target.position + vector2;
                                    }
                                    break;
                                }
                            case 1://向下/上冲刺
                                {
                                    NPC.velocity.X = 0;
                                    NPC.velocity.Y += State == 19 ? -1 : 1;
                                    Timer1++;
                                    if (Timer1 > 35)
                                    {
                                        Timer1 = 0;
                                        Timer3++;
                                    }
                                    break;
                                }
                            case 2://散发并跳出
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        if (State == 19)
                                        {
                                            for (int i = 0; i < 10; i++)
                                            {
                                                Vector2 vel = new(i - 5, -5);
                                                vel.X *= 2.3f;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                                                    ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 0; i < 10; i++)
                                            {
                                                Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 10 * i) * 10;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                                                    ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                            }
                                        }
                                    }
                                    Timer3 = 0;
                                    State++;
                                    break;
                                }
                            default:
                                Timer3 = 0;
                                break;
                        }
                        break;
                    }
                case 6://拦截
                    {
                        switch (Timer1)//追踪
                        {
                            case < 120:
                                NPC.velocity = (Target.position - NPC.position) * 0.03f;
                                break;
                            case 120:
                                NPC.velocity = (Target.position - NPC.position).SafeNormalize(default) * 30;
                                break;
                            case 140:
                                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(-60));
                                break;
                            case > 140 when Timer1 % 5 == 0:
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                                ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                    }
                                    break;
                                }
                            case >= 200:
                                Timer1 = 0;
                                State++;
                                break;
                        }
                        Timer1++;
                        break;
                    }
                case 7://无尽回旋
                    {
                        if (NPC.life > NPC.lifeMax * 0.75f)
                        {
                            State = 1;
                            break;
                        }
                        const float DIS_OLD_POS = 800;
                        if (Timer1 == 0)
                        {
                            targetOldPos = Target.position;
                        }
                        else if (Timer1 < 200)
                        {
                            #region 限制圈
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 dustCenter = targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 200 * i) * DIS_OLD_POS);
                                Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.PurpleCrystalShard);
                                dust.noGravity = true;
                                dust.velocity *= 0;
                            }
                            if (Vector2.Distance(Target.position, targetOldPos) > DIS_OLD_POS)
                            {
                                Target.position = targetOldPos + (((Target.position - targetOldPos).SafeNormalize(default) * (DIS_OLD_POS - 1)));
                            }
                            #endregion
                            Vector2 pos = targetOldPos + ((NPC.position - targetOldPos).SafeNormalize(default).RotatedBy(0.05) * DIS_OLD_POS);
                            NPC.velocity = (pos - NPC.position) * 0.3f;
                            if (Timer1 % 35 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2[] vectors = new Vector2[3]
                                {
                                    targetOldPos - (NPC.Center - targetOldPos),
                                    new Vector2(NPC.Center.X, (targetOldPos - (NPC.Center - targetOldPos)).Y),
                                     new Vector2((targetOldPos - (NPC.Center - targetOldPos)).X, NPC.Center.Y)
                                };
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 center;
                                    if (i < 3)
                                    {
                                        center = vectors[i];
                                    }
                                    else
                                    {
                                        center = NPC.Center;
                                    }
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, (targetOldPos - center).SafeNormalize(default) * 10,
                                    ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.32f, Main.myPlayer);
                                }
                            }
                        }
                        else
                        {
                            Timer1 = 0;
                            State++;
                            break;
                        }
                        Timer1++;
                        break;
                    }
                case 8://三角形拦截
                    {
                        Timer1++;
                        switch (Timer1)//追踪
                        {
                            case < 120:
                                NPC.velocity = (Target.position - NPC.position) * 0.03f;
                                break;
                            case 120:
                                NPC.velocity = (Target.position - NPC.position).SafeNormalize(default) * 30;
                                break;
                            case 160:
                            case 200:
                            case 240:
                                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(120));
                                break;
                            case > 140 when Timer1 % 2 == 0:
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                                ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                                    }
                                    break;
                                }
                            case >= 280:
                                Timer1 = 0;
                                State++;
                                break;
                        }
                        break;
                    }
                case 9://高速自旋
                    {
                        if (NPC.velocity.Length() < 5)
                        {
                            NPC.velocity = Vector2.UnitX * 5;
                        }
                        else
                        {
                            NPC.velocity = NPC.velocity.RotatedBy(0.2);
                            Timer1++;
                            if (Timer1 > 10 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer1 = 0;
                                if (Timer2 > 20)//散发20次后
                                {
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                Timer2++;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity,
                                    ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                            }
                        }
                        break;
                    }
                case 10://旋刃瞬间停止,并朝四周朝玩家投掷弹幕
                case 18://投掷会回旋的弹幕
                    {
                        NPC.velocity *= 0f;
                        const float DIS_OLD_POS = 800;
                        #region 限制圈
                        for (int i = 0; i < 200; i++)
                        {
                            Vector2 dustCenter = NPC.position + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 200 * i) * DIS_OLD_POS);
                            Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.PurpleCrystalShard);
                            dust.noGravity = true;
                            dust.velocity *= 0;
                        }
                        if (Vector2.Distance(Target.position, NPC.position) > DIS_OLD_POS)
                        {
                            Target.position = NPC.position + ((Target.position - NPC.position).SafeNormalize(default) * (DIS_OLD_POS - 1));
                        }
                        #endregion
                        Timer1++;
                        if (State == 10)
                        {
                            if (Timer1 > 10 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer1 = 0;
                                if (Timer2 > 20)
                                {
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                Timer2++;
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 pos = NPC.position + Vector2.UnitX.RotatedBy((MathHelper.TwoPi / 4 * i) + (Timer2 / 10)) * DIS_OLD_POS;
                                    Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, (NPC.Center - pos).SafeNormalize(default) * 10,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer)].timeLeft = 80;
                                }
                            }
                        }
                        else
                        {
                            if (Timer1 > 10 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Timer1 = 0;
                                if (Timer2 > 3)
                                {
                                    Timer2 = 0;
                                    State++;
                                    break;
                                }
                                Timer2++;
                                for (int i = 0; i < 4 + (Timer2 * 2); i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (4 + (Timer2 * 2)) * i) * 10,
                                        ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer, 1);
                                }
                            }
                        }
                        break;
                    }
                case 12://二次函数运动
                    {
                        if (Timer1 == 0)
                        {
                            Timer1 = -600;
                        }
                        else if (Timer1 > 600)
                        {
                            Timer1 = 0;
                            State++;
                            break;
                        }
                        else if (Timer1 % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * 5,
                                ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                        }
                        Timer1 += 7;
                        float Y = Target.position.Y + ((Timer1 - 600) * (Timer1 + 600) / 600);
                        NPC.position = new(Target.position.X + Timer1, Y);
                        break;
                    }
                case 15://缓慢接近玩家
                    {
                        const float DIS_OLD_POS = 130;
                        if (Timer1 == 0)
                        {
                            targetOldPos = Target.position;
                        }
                        else if (Timer1 < 800)
                        {
                            #region 限制圈
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 dustCenter = targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 200 * i) * DIS_OLD_POS);
                                Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.PurpleCrystalShard);
                                dust.noGravity = true;
                                dust.velocity *= 0;
                            }
                            if (Vector2.Distance(Target.position, targetOldPos) > DIS_OLD_POS)
                            {
                                Target.position = targetOldPos + (((Target.position - targetOldPos).SafeNormalize(default) * (DIS_OLD_POS - 1)));
                            }
                            #endregion
                            Vector2 pos = targetOldPos + ((NPC.position - targetOldPos).SafeNormalize(default).RotatedBy(0.1) * (800 - Timer1));
                            NPC.velocity = (pos - NPC.position) * 0.3f;
                        }
                        else
                        {
                            Timer1 = 0;
                            State++;
                            break;
                        }
                        Timer1 += 3;
                        break;
                    }
                case 16://五角星冲刺
                case 20://迷幻阵
                    {
                        float oldPosDis = 800;
                        if (State == 20) oldPosDis -= 400;
                        if (Timer1 == 0)
                        {
                            targetOldPos = Target.position;
                            Timer1++;
                        }
                        #region 限制圈
                        for (int i = 0; i < 200; i++)
                        {
                            Vector2 dustCenter = targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 200 * i) * oldPosDis);
                            Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.PurpleCrystalShard);
                            dust.noGravity = true;
                            dust.velocity *= 0;
                        }
                        if (Vector2.Distance(Target.position, targetOldPos) > oldPosDis)
                        {
                            Target.position = targetOldPos + (((Target.position - targetOldPos).SafeNormalize(default) * (oldPosDis - 1)));
                        }
                        #endregion
                        #region 冲刺
                        List<Vector2> pos = new();
                        Vector2 endPos;
                        if (State == 16)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                pos.Add(targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 5 * i) * oldPosDis));
                            }
                            switch (Timer2)
                            {
                                case 0://到达起点
                                case 5://回归
                                    {
                                        endPos = pos[2];
                                        break;
                                    }
                                case 1:
                                    {
                                        endPos = pos[4];
                                        break;
                                    }
                                case 2:
                                    {
                                        endPos = pos[1];
                                        break;
                                    }
                                case 3:
                                    {
                                        endPos = pos[3];
                                        break;
                                    }
                                case 4:
                                    {
                                        endPos = pos[0];
                                        break;
                                    }
                                default:
                                    {
                                        Timer2 = 0;
                                        Timer1 = 0;
                                        State++;
                                        return;
                                    }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                pos.Add(targetOldPos + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 8 * i) * oldPosDis));
                            }
                            switch (Timer2)
                            {
                                case 0://到达起点
                                case 7:
                                    {
                                        endPos = pos[6];
                                        break;
                                    }
                                case 1:
                                    {
                                        endPos = pos[3];
                                        break;
                                    }
                                case 2:
                                    {
                                        endPos = pos[5];
                                        break;
                                    }
                                case 3:
                                    {
                                        endPos = pos[2];
                                        break;
                                    }
                                case 4:
                                    {
                                        endPos = pos[7];
                                        break;
                                    }
                                case 5:
                                    {
                                        endPos = pos[4];
                                        break;
                                    }
                                case 6:
                                    {
                                        endPos = pos[1];
                                        break;
                                    }
                                default:
                                    {
                                        Timer2 = 0;
                                        Timer1 = 0;
                                        State++;
                                        return;
                                    }
                            }
                        }
                        NPC.velocity = (endPos - NPC.position) * 0.05f;

                        if (Timer2 > 0 && Timer1 % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient && State == 16)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<StarSpiralBladeProj_Hostile>(), damage, 1.2f, Main.myPlayer);
                        }
                        if (Vector2.Distance(endPos, NPC.position) < 20)
                        {
                            Timer2++;
                        }
                        Timer1++;
                        #endregion
                        break;
                    }
                case 21://死亡回旋
                    {
                        switch (Timer3)
                        {
                            case 0:
                                {
                                    switch (Timer2)
                                    {
                                        case 0://旋转
                                            {
                                                Vector2 pos = Target.position + ((NPC.position - Target.position).SafeNormalize(default).RotatedBy(MathHelper.ToRadians(0.1f) * Timer1 * 0.3f) * 800);
                                                NPC.velocity = (pos - NPC.position) * 0.3f;
                                                Timer1++;
                                                if (Timer1 > 300)
                                                {
                                                    Timer2++;
                                                    Timer1 = 0;
                                                }
                                                else if (Timer1 == 1)
                                                {
                                                    Main.NewText("来吧...死亡回旋", Color.Purple);
                                                }
                                                break;
                                            }
                                        case 1://冲刺
                                            {
                                                if (Timer1 == 0)
                                                {
                                                    NPC.velocity = ((Target.position - NPC.position) + Target.velocity * 0.3f).SafeNormalize(default) * 30;
                                                }
                                                else if (Timer1 > 40)
                                                {
                                                    Timer1 = 0;
                                                    Timer2 = 0;
                                                    Timer3++;
                                                }
                                                Timer1++;
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 1://迷惑玩家
                                {
                                    if (Timer1 == 0) targetOldPos = Target.position;
                                    Vector2 pos = targetOldPos + ((NPC.position - targetOldPos).SafeNormalize(default).RotatedBy(0.3f) * (400 - Timer1));
                                    NPC.velocity = (pos - NPC.position) * 0.3f;
                                    Timer1++;
                                    if (Timer1 > 300)
                                    {
                                        Timer1 = 0;
                                        Timer3 = 0;
                                        State++;
                                        NPC.life = NPC.lifeMax = 60000;
                                        Main.NewText("可恶...没有力气了", Color.Purple);
                                    }
                                    else if (Timer1 == 2)
                                    {
                                        Main.NewText("你能看清我的位置?", Color.Purple);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default://其他模式
                    {
                        if (State >= 21)
                        {
                            State = 22;
                            break;
                        }
                        State = 1;
                        if (NPC.life < NPC.lifeMax * 0.75f)
                        {
                            State = 7;
                        }
                        else if (NPC.life < NPC.lifeMax * 0.35f)
                        {
                            State = 15;
                        }
                        break;
                    }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            if (State <= 21 && State > 0)
            {
                NPC.life = 1;
                return false;
            }
            return base.CheckDead();
        }
        public override void BossHeadSlot(ref int index)
        {
            base.BossHeadSlot(ref index);
            if (State == 20 || (State == 21 && Timer3 == 1))
            {
                index = -1;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (State == 20 || (State == 21 && Timer3 == 1))
            {
                return false;
            }
            return base.DrawHealthBar(hbPosition, ref scale, ref position);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            StarBreakerWay.NPCDrawTail(NPC, Color.White, drawColor);
            switch (State)
            {
                case 3://幻影
                    {
                        Texture2D texture = TextureAssets.Npc[Type].Value;
                        Vector2 origin = texture.Size() * 0.5f;
                        Main.spriteBatch.Draw(texture, Target.Center - (NPC.Center - Target.Center) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2(NPC.Center.X, (Target.Center - (NPC.Center - Target.Center)).Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2((Target.Center - (NPC.Center - Target.Center)).X, NPC.Center.Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        break;
                    }
                case 7://无尽回旋
                    {
                        Texture2D texture = TextureAssets.Npc[Type].Value;
                        Vector2 origin = texture.Size() * 0.5f;
                        Main.spriteBatch.Draw(texture, targetOldPos - (NPC.Center - targetOldPos) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2(NPC.Center.X, (targetOldPos - (NPC.Center - targetOldPos)).Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texture, new Vector2((targetOldPos - (NPC.Center - targetOldPos)).X, NPC.Center.Y) - screenPos, null, Color.White * 0.7f, NPC.rotation,
                            origin, 1f, SpriteEffects.None, 0);
                        break;
                    }
                case 20://迷幻阵
                    {
                        Texture2D texture = TextureAssets.Npc[Type].Value;
                        Vector2 origin = texture.Size() * 0.5f;
                        for (int i = 1; i < 8; i++)
                        {
                            Vector2 pos = targetOldPos + (NPC.Center - targetOldPos).RotatedBy(MathHelper.PiOver4 * i);
                            Main.spriteBatch.Draw(texture, pos - screenPos, null, drawColor, NPC.rotation,
                           origin, 1f, SpriteEffects.None, 0);
                        }
                        break;
                    }
                case 21://死亡回旋
                    {
                        Texture2D texture = TextureAssets.Npc[Type].Value;
                        Vector2 origin = texture.Size() * 0.5f;
                        for (int i = 1; i < 16; i++)
                        {
                            Vector2 pos = targetOldPos + (NPC.Center - targetOldPos).RotatedBy(MathHelper.PiOver4 / 2 * i);
                            Main.spriteBatch.Draw(texture, pos - screenPos, null, drawColor, NPC.rotation,
                           origin, 1f, SpriteEffects.None, 0);
                        }
                        break;
                    }
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
