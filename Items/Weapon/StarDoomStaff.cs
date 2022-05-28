using StarBreaker.Projs.StarDoomStaff;

namespace StarBreaker.Items.Weapon
{
    public class StarDoomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Doom Staff");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "终末之星杖");
            Tooltip.SetDefault("2%标记伤害\n" +
                "(标记伤害取玩家召唤伤害加成与玩家防御的与玩家的血量的和后,乘以2%)\n" +
                "使用时 如果没有敌人,则标记距离玩家最近的敌人\n" +
                "悬浮在玩家背后\n" +
                "随机使用以下水晶:\n" +
                "爆炸水晶:触碰方块发射爆炸\n" +
                "控制水晶:控制敌人\n" +
                "穿刺水晶:扎入npc体内,每扎入一根提升5点伤害\n" +
                "护卫水晶:保护玩家\n" +
                "使用时概率出现\"消除射线\"\n" +
                "这是最强的召唤师法杖,其因为吸收极大量的星辰之力,而变的无限疯狂\n" +
                "这把法杖现在在你的手上,但是它仍然没有完全听命于你,因为它实际可以给一个地区带来终末,但是它没有这么做");
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 210;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3.2f;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 61;
            Item.mana = 2;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.value = 5130142;
            Item.UseSound = SoundID.Item101;
            Item.rare = ItemRarityID.Red;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<StarDoomStaffProj>()] == 0)
            {
                Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<StarDoomStaffProj>(), Item.damage, Item.knockBack, player.whoAmI)].originalDamage = Item.damage;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria")
                {
                    if (line.Name == "Tooltip0" || line.Name == "Tooltip3" || line.Name == "Tooltip9")
                    {
                        line.OverrideColor = Color.Purple * 0.8f;
                    }
                    else if (line.Name == "Tooltip10")
                    {
                        line.OverrideColor = Color.Red * 0.8f;
                    }
                    else if (line.Name == "ItemName")
                    {
                        line.OverrideColor = new Color(100, 21, 203);
                    }
                }
            }
        }
    }
}
