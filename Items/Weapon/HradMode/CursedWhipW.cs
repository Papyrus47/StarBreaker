using StarBreaker.Projs.Process.HardMode.Summon;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.HradMode
{
    public class CursedWhipW : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Whip");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "诅咒之鞭");
            Tooltip.SetDefault("你的召唤物将集中攻击敌人\n" +
                "累计击中敌人5次会触发\" 咒火入体 \"的效果\n" +
                "\" 咒火入体 \":持续对敌人造成10点伤害\n" +
                "腐肉正在蠕动");
            ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<CursedWhip>(), 39, 0.5f, 3f, 30);
            Item.useTurn = false;
            Item.value = 24300;
            Item.rare = ItemRarityID.LightRed;
            Item.crit = 32;
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return player.autoReuseGlove;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SoulofNight, 15)
.AddIngredient(522, 20).AddIngredient(68, 20)
.AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
