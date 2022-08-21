namespace StarBreaker.MyGraphics.RenderTargetProjDraws
{
    internal abstract class RenderTargetProjDraws : IRenderTargetProjDraws
    {
        public SpriteBatch sb;
        public GraphicsDevice gd;
        public RenderTargetProjDraws()
        {
            sb = Main.spriteBatch;
            gd = Main.instance.GraphicsDevice;
        }
        public bool Remove(int projWhoAmI)
        {
            Projectile projectile = Main.projectile[projWhoAmI];
            return projectile.active && CanRemove(projectile);
        }
        public virtual bool CanDraw(int projWhoAmI)
        {
            return true;
        }

        public abstract void Draw(int projwhoAmI);
        public virtual bool CanRemove(Projectile projectile)
        {
            return false;
        }
    }
}
