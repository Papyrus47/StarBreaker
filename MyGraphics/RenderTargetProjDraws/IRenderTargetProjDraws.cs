namespace StarBreaker.MyGraphics.RenderTargetProjDraws
{
    internal interface IRenderTargetProjDraws
    {
        public bool Remove(int projWhoAmI);
        public void Draw(int projWhoAmI);
        public bool CanDraw(int projWhoAmI);
    }
}
