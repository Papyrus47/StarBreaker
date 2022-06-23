using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastCopperSowrdSummonStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜召唤杖");
            Tooltip.SetDefault("召唤一只小铜短剑为你战斗\n" +
                "小铜短剑将会在敌人头上跳跃,每跳跃一次伤害提升20%,十次后恢复原本伤害");
            Item.SacrificeCountNeededByItemId(1);
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.Size = new Vector2(48);
            Item.damage = 30;
            Item.knockBack = 1.1f;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.value = 114510;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<LastCopperSowrdSummonProj>();
            Item.shootSpeed = 10;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<Buffs.CopperSummonBuff>(), 114514);
            Main.projectile[Projectile.NewProjectile(source,position,velocity,type,damage,knockback,player.whoAmI)].originalDamage = damage;
            return false;
        }
    }
}
