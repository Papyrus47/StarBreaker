using Terraria;
using Terraria.ModLoader;

namespace StarBreaker.Dusts
{
    public class LastCopperWhipDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
            dust.noGravity = true;
            dust.velocity *= 2;
            dust.frame = new(0, Main.rand.Next() * 6, 6, 6);
            dust.scale *= 2.5f;
        }
        public override bool Update(Dust dust)
        {
            base.Update(dust);
            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);
            return true;
        }
    }
}
