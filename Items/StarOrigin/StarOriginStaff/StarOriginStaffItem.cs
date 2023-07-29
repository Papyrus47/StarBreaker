using StarBreaker.Items.StarOwner.StarsPierceWeapon;

namespace StarBreaker.Items.StarOrigin.StarOriginStaff
{
    public class StarOriginStaffItem : ModItem,IHeldProjItem
    {
        public override string Texture => (GetType().Namespace + ".StarOriginStaff").Replace('.','/');
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 2;
            Item.Size = new(102);
            Item.knockBack = 1f;
            Item.value = 0;
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<StarOriginStaff>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
        }
    }
}
