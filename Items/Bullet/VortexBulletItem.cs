using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class VortexBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星璇子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 11;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<StarSwirlBullet>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).AddIngredient(ItemID.FragmentVortex).AddTile(412).Register();
        }

        public override void ProjAI(Projectile Projectile)
        {
            if (Projectile.timeLeft < 250)
            {
                NPC npc = null;
                float max = 800;
                foreach (NPC n in Main.npc)
                {
                    float dis = n.Distance(Projectile.position);
                    if (dis < max && n.active && n.CanBeChasedBy() && Collision.CanHit(n.position, 1, 1, Projectile.position, 1, 1))
                    {
                        max = dis;
                        npc = n;
                    }
                }
                if (npc != null)
                {
                    Projectile.velocity = (Projectile.velocity * 20 + (npc.position - Projectile.position).SafeNormalize(default) * 50) / 21;
                }
            }
        }
    }
}
