using StarBreaker.Projs.Process.HardMode.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.HradMode
{
    public class CrystalWhipW : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("水晶鞭");
            Tooltip.SetDefault("你的召唤物将集中攻击敌人\n" +
                "命中目标时,分裂水晶碎块\n" +
                "易碎品,小心保管");
            ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<CrystalWhip>(), 39, 0.5f, 5f, 30);
            Item.useTurn = false;
            Item.value = 24300;
            Item.rare = ItemRarityID.LightRed;
            Item.crit = 21;
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return player.autoReuseGlove;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(502, 50).AddIngredient(501, 30)
.AddIngredient(ItemID.SoulofLight, 15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
