namespace StarBreaker.Items.Type
{
    public abstract class StarWeapon : ModItem
    {
        public virtual string Favorability => null;
        public int FavorabilityCount = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips != null)
            {
                string FavorabilityLag = "Favorability:";
                string FavorabilityLag2 = "Ways to increase favorability:";
                if (Language.ActiveCulture.LegacyId == 7)
                {
                    FavorabilityLag = "好感度:";
                    FavorabilityLag2 = "增加好感度的方法";
                }
                tooltips.Add(new(Mod, "StarWeapon:Favorability_Count", FavorabilityLag + FavorabilityCount));
                tooltips.Add(new(Mod, "StarWeapon:Favorability", FavorabilityLag2 + Favorability));
            }
        }
    }
}
