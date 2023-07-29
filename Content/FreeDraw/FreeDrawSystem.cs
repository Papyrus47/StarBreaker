namespace StarBreaker.Content.FreeDraw
{
    public class FreeDrawSystem
    {
        private FreeDrawSystem() { } // 单例模式
        private static FreeDrawSystem instance;

        public static FreeDrawSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new()
                    {
                        _freeDraw = new List<IFreeDraw>[5]
                    };
                    for (int i = 0; i < instance._freeDraw.Length; i++)
                    {
                        instance._freeDraw[i] = new();
                    }
                }
                return instance;
            }
        }
        private List<IFreeDraw>[] _freeDraw;
        public void Draw(FreeDrawEnum drawType)
        {
            List<IFreeDraw> draw = _freeDraw[(int)drawType];
            if (draw.Count == 0)
            {
                return;
            }

            draw.RemoveAll(x => x.ShouldRemove); // 总长度减去移除长度,就是实际绘制数量
            draw.ForEach(x => x.Draw());
        }
        public void Add(FreeDrawEnum drawType, IFreeDraw freeDraw)
        {
            List<IFreeDraw> draw = _freeDraw[(int)drawType];
            draw.Add(freeDraw);
        }
        public static void UnLoad()
        {
            instance = null;
        }
    }
}
