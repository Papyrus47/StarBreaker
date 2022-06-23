using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria.ID;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperMagicStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜法杖");
            Tooltip.SetDefault("小心魔法四溅");
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(46);
            var sound = SoundID.Item4;
            sound.Pitch = -0.6f; ;
            Item.UseSound = sound;
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 64;
            Item.knockBack = 3.2f;
            Item.value = 180000;
            Item.useTurn = false;
            Item.useTime = Item.useAnimation = 18;
            Item.rare = ItemRarityID.Red;
            Item.mana = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LastCopperMagicStaffProj>();
            Item.shootSpeed = 15f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center;
            //velocity = velocity.RotatedByRandom(0.2);
        }
    }
}
