namespace BloodSoul.Projectiles
{
    /// <summary>
    /// 弹幕的基类
    /// </summary>
    public abstract class BaseProj : ModProjectile
    {
        /// <summary>
        /// 激光通用
        /// </summary>
        public const string TheGrowLight = "BloodSoul/Projectiles/TheGrowLight";
        protected int TimeLeft;
        protected int width { get => Projectile.width; set => Projectile.width = value; }
        protected int height { get => Projectile.height; set => Projectile.height = value; }
        protected int aiStyle { get => Projectile.aiStyle; set => Projectile.aiStyle = value; }
        protected int penetrate { get => Projectile.penetrate; set => Projectile.penetrate = value; }
        protected float scale { get => Projectile.scale; set => Projectile.scale = value; }
        /// <summary>
        /// 这个召唤物的召唤栏位
        /// </summary>
        protected float minionSlots { get => Projectile.minionSlots; set => Projectile.minionSlots = value; }
        protected bool tileCollide { get => Projectile.tileCollide; set => Projectile.tileCollide = value; }
        protected bool hide { get => Projectile.hide; set => Projectile.hide = value; }
        /// <summary>
        /// 说明这个弹幕是不是召唤物
        /// </summary>
        protected bool minion { get => Projectile.minion; set => Projectile.minion = value; }
        /// <summary>
        /// 弹幕是否会掉落物品?
        /// </summary>
        protected bool noDropItem { get => Projectile.noDropItem; set => Projectile.noDropItem = value; }
        /// <summary>
        /// 召唤物联机同步解决？
        /// </summary>
        protected bool netImportant { get => Projectile.netImportant; set => Projectile.netImportant = value; }
        /// <summary>
        /// 提供类似useThisBaseProj的效果
        /// </summary>
        protected bool useThisBaseProj = false;
        /// <summary>
        /// 计时器
        /// </summary>
        public float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int State
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetDefaults()
        {
        }

        public virtual void SetDef() { }

        protected void SetName(string Name, string CName)
        {
            DisplayName.SetDefault(Name);
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, CName);
        }
        protected int ModifyHitDamage(int damage)
        {
            if (Main.expertMode || Main.masterMode)
            {
                damage = damage / 2;
            }

            return damage;
        }
        /// <summary>
        /// 追踪npc所用的(不是锁定某一目标，是锁定一个点
        /// </summary>
        /// <param name="speed">弹幕的速度</param>
        /// <param name="maxdis">距离中心的位置</param>
        /// <param name="center">寻找敌对npc的中心位置</param>
        protected virtual void TrackNPC(float speed, float maxdis, Vector2 center)
        {
            NPC target = null;//定义一个目标
            foreach (NPC npc in Main.npc)
            {
                float dis = Vector2.Distance(center, npc.position);//获取距离
                if (dis < maxdis && npc.active && npc.CanBeChasedBy() && !npc.friendly && npc.type != NPCID.TargetDummy)
                //小于范围 npc活着 可以追踪 不是友善的 不是假人
                {
                    target = npc;//让这个目标是这个npc
                    maxdis = dis;//在这个范围内，重新寻找npc
                }
            }
            if (target != null)
            {
                Projectile.velocity = (Projectile.velocity * 10 + (target.position - Projectile.position).SafeNormalize(Vector2.Zero) * speed) / 11;
            }
        }
    }
}
