using StarBreaker.Projs.XuanYu.Boss;

namespace StarBreaker.NPCs.OtherTimeLine
{
    public class XuanYu : FSMNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xuan Yu");
            DisplayName.AddTranslation(7, "宣雨");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 6300;
            NPC.damage = 57;
            NPC.defense = 3;
            NPC.boss = true;
            NPC.width = 26;
            NPC.height = 48;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.value = 0;//245时间线经典的出门不带钱
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.npcSlots = 10;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void AI()
        {
            if(NPC.lifeMax != 6300)
            {
                NPC.lifeMax = 6300;
            }
            NPC.velocity = Vector2.Zero;
            switch(State)
            {
                case 0://开幕对话 因为没有对话UI 所以用NewText做
                    {
                        Timer1++;
                        if(Timer1 > 50)
                        {
                            Timer1 = 0;
                            string sayText = "";
                            switch(Timer2++)
                            {
                                case 0:sayText = "啊...妈的...我这是到哪了";break;
                                case 1:sayText = "欸欸欸要站不住了";break;
                                case 2:sayText = "独自一个人乱跑的代价就是这样吗...被人偷袭,然后还断了条手和脚";break;
                                case 3:sayText = "好晕啊...";break;
                                case 4:sayText = "我能撑到那个笨蛋发现我吗...";break;
                                case 5:sayText = "暂时用魔法堵一下伤...";break;
                                case 6:sayText = "好了,我觉得应该没什么问题了";break;
                                case 7:sayText = "就这一点血...用魔法...应该可以让我几天内死不掉吧...";break;
                                case 8:sayText = "好像有人...";break;
                                case 9:sayText = "完了,这个人穿的这么花里胡哨的,不会是刚刚那群人有能用穿越时间线的技术来抓我的吧";break;
                                case 10:sayText = "冷静一下...我要冷静一下";break;
                                case 11:sayText = "不行了,这雪地好软,我要睡一下";break;
                                case 12:sayText = "不对,现在不能睡,睡了不知道会不会被面前的这个家伙抓走";break;
                                case 13:sayText = "但是我没学过断个手脚的时候战斗...";break;
                                case 14:sayText = "等下,我是不是可以直接用魔法来攻击";break;
                                case 15:sayText = "太久没用冰魔法主动攻击了,准头可能...有点不好了";break;
                                case 16:sayText = "管它那么多,保命要紧";break;
                                case 17:sayText = "呜...那么来吧";break;
                            }
                            Main.NewText(sayText, Color.LightBlue);
                        }
                        if(Timer2 == 1 && NPC.rotation > -MathHelper.PiOver2)
                        {
                            NPC.rotation -= MathHelper.PiOver2 / 20f;
                        }
                        break;
                    }
            }
        }
    }
}
