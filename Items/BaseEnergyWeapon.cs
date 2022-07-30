using StarBreaker.Items.Bullet;
using StarBreaker.Items.DamageClasses;
using StarBreaker.Projs.Bullets;

namespace StarBreaker.Items
{
    public abstract class BaseEnergyWeapon : ModItem
    {
        public sealed override void SetDefaults()
        {
            Item.useAmmo = ModContent.ItemType<NebulaBulletItem>();
            Item.DamageType = ModContent.GetInstance<EnergyDamage>();
            Item.shoot = ModContent.ProjectileType<BrightBullet>();
            Item.shootSpeed = 10;
            NewSetDef();
        }
        public override void PostUpdate()
        {
            Item.velocity *= 0.3f;
        }
        public virtual void NewSetDef() { }
    }
    public abstract class BaseEnergyRanged : BaseEnergyWeapon
    {
        public override void NewSetDef()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item109;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position + velocity, velocity, type, damage, knockback, player.whoAmI, 0, -1);
            Main.projectile[proj].friendly = true;
            Main.projectile[proj].hostile = false;
            return false;
        }
    }
    public abstract class BaseEnergyMelee : BaseEnergyWeapon
    {
        public override void NewSetDef()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item109;
        }
    }
    public abstract class BaseEnergySummon : BaseEnergyWeapon
    {
        public override void NewSetDef()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item109;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, 0, -1);
            Main.projectile[proj].friendly = true;
            Main.projectile[proj].hostile = false;
            Main.projectile[proj].originalDamage = damage;
            StarBreakerWay.Add_Hooks_ToProj(source.AmmoItemIdUsed, proj);
            return false;
        }
    }
}
