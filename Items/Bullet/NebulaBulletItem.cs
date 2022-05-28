using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class NebulaBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星云子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 11;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<NebulaBullet>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(50).AddIngredient(ItemID.FragmentNebula).AddTile(412).Register();
        }

        public override void ProjAI(Projectile Projectile)
        {
            float max = 1200;
            if (Projectile.ai[0] == 0)
            {
                foreach (NPC n in Main.npc)
                {
                    float dis = n.Distance(Projectile.position);
                    if (dis < max && n.active && n.CanBeChasedBy())
                    {
                        max = dis;
                        Projectile.ai[0] = n.whoAmI + 1;
                    }
                }
                if (Projectile.ai[0] - 1 >= 0 && Projectile.ai[0] - 1 <= 200)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0] - 1];
                    Projectile.tileCollide = false;
                    Projectile.position = npc.position + Projectile.velocity.SafeNormalize(default) * -300;
                    Projectile.ai[0] = -1;
                    Dust.NewDust(Projectile.position, 10, 10, DustID.PurpleCrystalShard);
                }
            }
        }
    }
}
