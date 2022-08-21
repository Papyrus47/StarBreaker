using StarBreaker.Items.Bullet;
using StarBreaker.Items.DamageClasses;
using StarBreaker.Projs.IceGun;

namespace StarBreaker.Items.Weapon.IceGunAndFireKnife
{
    public class IceGun : Type.StarWeapon
    {
        public override string Favorability => "击败Boss:冰旋斩炎刀 即可使好感度变成100";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostStar Boom Gun");
            DisplayName.AddTranslation(7, "霜星轰击者");
            Tooltip.SetDefault("Shoot frost energy\n" +
                "Channel to shoot Ice");
            Tooltip.AddTranslation(7, "发射霜冻能量" +
                "蓄力以发射冰柱");
        }
        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.knockBack = 3;
            Item.value = 300000;
            Item.DamageType = ModContent.GetInstance<EnergyDamage>();
            Item.useTime = Item.useAnimation = 50;
            Item.channel = true;
            Item.Size = new(104, 36);
            Item.shoot = ModContent.ProjectileType<IceGun_Proj>();
            Item.shootSpeed = 10;
            Item.useAmmo = ModContent.ItemType<NebulaBulletItem>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Red;
            Item.crit = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height / 14);
            spriteBatch.Draw(texture, position, rectangle, drawColor, 0, rectangle.Size() * 0.5f, scale, Item.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height / 14);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, rectangle, lightColor, rotation, rectangle.Size() * 0.5f, scale, Item.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}
