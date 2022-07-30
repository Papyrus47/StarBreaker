namespace StarBreaker.Items.Type
{
    public abstract class Drum : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;
            //Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Drum/Drum1");
        }
        public override void HoldItem(Player player)
        {
            player.GetModPlayer<StarPlayer>().DrumDraw = true;
        }
    }
}
