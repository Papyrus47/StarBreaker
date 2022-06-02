using StarBreaker.Buffs;
using StarBreaker.Projs.Bullets;
using Terraria.GameContent.Creative;

namespace StarBreaker.Items.Bullet
{
    public class MightBulletItem : EnergyBulletItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("力量能量聚集器");
            Tooltip.SetDefault("使用力量之魂制作的稳定的聚集器,30%的概率不消耗\n" +
                "你真的确定这种可怕的力量可用?");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.crit = 12;
            Item.damage = 7;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<MightBullet>();
        }
        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.Next(10) >= 3;
        }
        public override void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            base.ProjOnHitNPC(projectile, target, damage, knockback, crit);
            target.AddBuff(ModContent.BuffType<EnergySmash>(), 120);
        }
        public override void AddRecipes()
        {
            CreateRecipe(5).AddIngredient(ItemID.SoulofMight).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
