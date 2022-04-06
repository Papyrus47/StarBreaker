using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EnW.NPCs.TheBoss//填入你mod的命名空间
{
    /// <summary>
    /// 一个蠕虫基类
    /// </summary>
    public abstract class FSMWorm : ModNPC//抽象类不会被实例，做基类很好
    {
        /// <summary>
        /// npc的目标(不自动获取)
        /// </summary>
        protected Player Target => Main.player[NPC.target];
        /// <summary>
        /// 蠕虫的头部
        /// </summary>
        protected bool head = false;//protected的都是只能给继承的类用
        /// <summary>
        /// 蠕虫的身体
        /// </summary>
        protected bool body = false;
        /// <summary>
        /// 蠕虫的尾巴
        /// </summary>
        protected bool tila = false;
        /// <summary>
        /// 头部Type
        /// </summary>
        protected int headType = -1;
        /// <summary>
        /// 身体Type
        /// </summary>
        protected int bodyType = -1;
        /// <summary>
        /// 尾部Type
        /// </summary>
        protected int tilaType = -1;
        /// <summary>
        /// 召唤身体的长度
        /// </summary>
        protected int Num_Body//避免出现summonBody < 0 的情况
        {
            set
            {
                if (value > 0)
                {
                    summonBody = value;
                }
                else
                {
                    summonBody = 0;
                }
            }
        }
        /// <summary>
        /// 新的ai数组
        /// 在不同npc中可以用(npc.ModNPC as XXX).NewAI获取，前提是npc必须是XXX类里面的
        /// </summary>
        protected float[] NewAI = new float[4];
        private int summonBody = 0;//被封的召唤身体数量，通过Num_body修改值
        private bool _init = false;//注册用
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server)
            {
                for (int i = 0; i < NewAI.Length; i++)
                {
                    writer.Write(NewAI[i]);
                }
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < NewAI.Length; i++)
                {
                    NewAI[i] = reader.ReadUInt32();
                }
            }
        }
        public sealed override void AI()
        {
            if (!_init)
            {
                _init = true;
                Init();
            }//注册信息
            if (NPC.target <= 0 || NPC.target == 255 || !Target.active || Target.dead)
            {
                NPC.TargetClosest();//获取敌人
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tila && NPC.ai[0] == 0)//不让尾巴生成npc
                {
                    if (head)//如果是头
                    {
                        NPC.ai[3] = NPC.whoAmI;//锁定头的AI[3]
                        NPC.realLife = NPC.whoAmI;//让它realLife锁它自己
                        NPC.ai[2] = summonBody;//生成的身体的长度
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.position.X, (int)NPC.position.Y, bodyType, NPC.whoAmI);//生成npc
                    }
                    else if (NPC.ai[2] > 0)
                    {
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.position.X, (int)NPC.position.Y, Type, NPC.whoAmI);//体节生成npc
                    }
                    else
                    {
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.position.X, (int)NPC.position.Y, tilaType, NPC.whoAmI);//生成尾部
                    }
                    NPC theWorm = Main.npc[(int)NPC.ai[0]];
                    theWorm.ai[3] = NPC.ai[3];//让它知道它的头
                    theWorm.realLife = NPC.realLife;//锁这个生成的npc的realLife为头
                    theWorm.ai[1] = NPC.whoAmI;//生成的上一节体节
                    theWorm.ai[2] = NPC.ai[2] - 1f;//减少生成的体节的数量
                    NPC.netUpdate = true;
                }
            }
            if (body || tila)
            {
                if (NPC.ai[3] < Main.npc.Length)
                {
                    NPC wormOnwer = Main.npc[(int)NPC.ai[1]];//获取上一个npc
                    if (wormOnwer != null && wormOnwer.active)
                    {
                        Vector2 this_center = NPC.Center;//获取这个npc的位置
                        Vector2 toOnwer = wormOnwer.Center - this_center;//获取到上一个npc的向量
                        NPC.rotation = toOnwer.ToRotation();//旋转角度
                        float length = toOnwer.Length();//获取到上一个npc的距离
                        float dist = (length - NPC.width) / length;
                        Vector2 pos = toOnwer * dist;//位置
                        NPC.position = NPC.position + pos;//npc应该在的位置
                        NPC.velocity = Vector2.Zero;//避免出现蠕虫身体分离
                    }
                    else//如果上一个死了，那么就会让这个npc死亡，且没有掉落物
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                        if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1);
                        //发送npc死亡讯息
                    }
                }
            }
            CustomAI();
        }
        /// <summary>
        /// 注册所有信息
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// 自定义的ai
        /// </summary>
        public virtual void CustomAI() { }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return head ? null : false;
        }
    }
}
