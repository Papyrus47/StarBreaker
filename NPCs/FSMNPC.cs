namespace StarBreaker.NPCs
{
    public abstract class FSMNPC : ModNPC
    {
        protected float Timer1
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        protected float Timer2
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        protected float Timer3
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        protected float State
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        /// <summary>
        /// NPC的目标
        /// </summary>
        protected Player Target => Main.player[NPC.target];
        public int Damage//这一个是属性,获取伤害用的
        {
            get
            {
                int damage = NPC.damage;
                if (Main.masterMode || Main.expertMode)//大师或者专家模式
                {
                    damage /= 2;
                }
                return damage;//一套普通与其他模式的伤害切换
            }
        }
    }
}
