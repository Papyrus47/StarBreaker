using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items
{
    public class StarMounts : ModItem
    {
        public override string Texture => "StarBreaker/Projs/Type/EnergyProj";
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "星辰护盾");
            Tooltip.SetDefault("You sure you will fly sky by this shield?");
            Tooltip.AddTranslation(7, "你确定你会用这个护盾升天?");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.mountType = ModContent.MountType<Mounts.StarMountsMount>();
        }

    }
}
