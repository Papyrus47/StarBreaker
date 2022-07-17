namespace StarBreaker.Items
{
    public class Tester : ModItem
    {
        public override string Texture => "StarBreaker/Projs/Star";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("本Mod测试用翅膀");
            Tooltip.SetDefault("开发者所用，用于Hero无法使用时，直接芜湖起飞的boss测试器(拥有极高血量与防御)\n" +
                "可空手合成");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 66;
            Item.height = 25;
            Item.useAnimation = Item.useTime = 40;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxRunSpeed = 100;
            player.moveSpeed += 2f;
            player.statDefense = player.statLife = player.statLifeMax2 = 1000;
            if (!player.controlDown && !player.controlJump)
            {
                player.gravDir = 0;
                player.gravity = 0;
                player.velocity.Y = 0;
            }
            else if (player.controlDown)
            {
                player.gravDir = 10;
                player.grapCount = 1;
                player.gravity = 10;
                player.velocity.X *= 0.9f;
            }
            else if (player.controlJump)
            {
                player.velocity.Y = -5;
            }
        }
        //public override void AddRecipes()
        //{
        //    CreateRecipe().Register();
        //}
    }
}
