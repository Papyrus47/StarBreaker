using StarBreaker.Projs.Process.HardMode.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon.HradMode
{
    public class BloodyWhipW : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("血鞭");
            Tooltip.SetDefault("你的召唤物将集中攻击敌人\n" +
                "击中目标后,npc接下来被击中后会提升受到的伤害,身上对应\"流血\"层数就提升n点伤害\n" +
                "被击中后减少到剩下的1/3,鞭子每击中一次施加5层流血效果 \n" +
                "伤害不受到穿透影响\n" +
                "血液的味道");
            ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<BloodyWhip>(), 39, 0.5f, 3f, 30);
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
            CreateRecipe().AddIngredient(1332, 50).AddIngredient(1329, 30)
.AddIngredient(ItemID.SoulofNight, 15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
