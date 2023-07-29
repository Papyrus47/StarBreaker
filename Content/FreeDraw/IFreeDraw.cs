namespace StarBreaker.Content.FreeDraw
{
    public interface IFreeDraw
    {
        public void Draw();
        public FreeDrawEnum freeDrawEnum { get; }
        public bool ShouldRemove { get; set; }
    }
}
