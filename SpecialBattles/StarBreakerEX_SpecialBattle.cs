namespace StarBreaker.SpecialBattles
{
    public class StarBreakerEX_SpecialBattle : SpecialBattle
    {
        public BattleTile[] battleTiles;
        public struct BattleTile
        {
            public Vector2 Pos;
            public Texture2D Texture;
            public float Scale;
            public float Alpha;
            public float Rot;
            public bool Active;
            public BattleTile(Vector2 pos,Texture2D texture,float scale = 1f,float alpha = 0f)
            {
                Pos = pos;
                Texture = texture;
                Scale = scale;
                Alpha = alpha;
                Rot = 0f;
                Active = true;
            }
            public bool CheckActive()
            {
                Vector2 playerCenter = Main.LocalPlayer.Center;
                if(Vector2.Distance(playerCenter,Pos) > 1500)
                {
                    Active = false;
                }
                return Active;
            }
            public void Update_BattleTile(Player player)
            {
                Vector2 playerCenter = player.Center;
                if(Math.Abs(playerCenter.X - Pos.X) < Texture.Height * Scale && playerCenter.Y - Texture.Width * Scale < Pos.Y && playerCenter.Y > Pos.Y && !player.controlDown && !player.controlUp && !player.controlJump)
                {
                    player.GetModPlayer<StarPlayer>().CanFall = false;
                }
            }
        }
        public StarBreakerEX_SpecialBattle(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            active = true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (active)
            {
                for (int i = 0; i < battleTiles.Length; i++)
                {
                    if (battleTiles[i].Active && battleTiles[i].Texture != null)
                    {
                        spriteBatch.Draw(battleTiles[i].Texture, battleTiles[i].Pos - Main.screenPosition,
                            null, Color.White, battleTiles[i].Rot,new Vector2(0, battleTiles[i].Texture.Height / 2),
                            new Vector2(1,1.5f) * battleTiles[i].Scale, SpriteEffects.None, 0);
                    }
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<Items.Weapon.StarBreakerW>())
            {
                active = false;
            }
            if (battleTiles == null)//实例化战斗平台
            {
                battleTiles = new BattleTile[15];
            }
            else
            {
                for (int i = 0; i < battleTiles.Length; i++)
                {
                    if(battleTiles[i].Active)
                    {
                        if (battleTiles[i].CheckActive())
                        {
                            battleTiles[i].Update_BattleTile(Main.LocalPlayer);
                        }
                    }
                    else
                    {
                        battleTiles[i].Rot = MathHelper.PiOver2;
                        battleTiles[i].Active = true;
                        battleTiles[i].Scale = 5;
                        battleTiles[i].Texture = TextureAssets.FishingLine.Value;
                        battleTiles[i].Pos = Main.LocalPlayer.Center + Main.rand.NextVector2Unit() * 1000f;

                    }
                }

            }
        }
    }
}
