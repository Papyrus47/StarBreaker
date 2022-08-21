using StarBreaker.Items.Type;
using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    internal class DarkBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("黑暗能量聚集器");
            Tooltip.SetDefault("使用暗影之魂制作的稳定的聚集器,50%的概率不消耗");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 10;
            Item.damage = 5;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<DarkBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.Next(10) >= 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(15).AddIngredient(ItemID.SoulofNight).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            target.life -= damage / 5;
            target.HitEffect(damage / 5, 10.0);
            CombatText.NewText(target.Hitbox, Microsoft.Xna.Framework.Color.MediumPurple, damage / 5);
            target.checkDead();
        }
    }
}
