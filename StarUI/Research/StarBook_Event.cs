namespace StarBreaker.StarUI.Research
{
    public abstract class StarBook_Event
    {
        private bool new_Event = true;//新的任务
        /// <summary>
        /// 事件名字
        /// </summary>
        public virtual string Event => null;
        /// <summary>
        /// 事件主人
        /// </summary>
        public virtual string Event_Name => null;
        /// <summary>
        /// 事件的介绍
        /// </summary>
        public virtual string Event_Show => null;
        /// <summary>
        /// 事件的贴图
        /// </summary>
        public virtual Texture2D Texture => null;

        public bool New_Event { get => new_Event; private set => new_Event = value; }

        /// <summary>
        /// 任务完成
        /// </summary>
        public virtual void OnFinish() { }
        /// <summary>
        /// 看到任务内容后
        /// </summary>
        public virtual void OnBegin()
        {
            New_Event = false;
        }
        public override bool Equals(object obj)
        {
            if (obj is StarBook_Event)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
