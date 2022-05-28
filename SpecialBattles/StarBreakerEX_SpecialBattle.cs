namespace StarBreaker.SpecialBattles
{
    public class StarBreakerEX_SpecialBattle : SpecialBattle
    {
        public StarBreakerEX_SpecialBattle(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            active = true;
        }
        public override void Update()
        {
            if(!Main.LocalPlayer.active)
            {
                active = false;
            }
        }
    }
}
