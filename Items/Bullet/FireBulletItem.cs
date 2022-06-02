using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class FireBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("火焰能量聚集器");
            Tooltip.SetDefault("使用火焰制作的稳定的聚集器,20%的概率不消耗");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 5;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<FireBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.Next(10) >= 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemID.LivingFireBlock).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool ProjPreDraw(Projectile projectile, ref Color lightColor)
        {
            return projectile.ai[0] == 0;
        }

        public override void ProjAI(Projectile projectile)
        {
            float gry = 1.55f;
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position, 1, 1, DustID.Torch);
            }

            projectile.velocity.Y += gry;
        }
        public override void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.minion)
            {
                target.AddBuff(BuffID.OnFire3, 200, true);
            }
        }
        public override bool OnTileCollide(Projectile Projectile, Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                int proj = Projectile.NewProjectile(null, Projectile.Center, oldVelocity * -1,
                                Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1, Projectile.ai[1]);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                return false;
            }
            return base.OnTileCollide(Projectile, oldVelocity);
        }
    }
}
