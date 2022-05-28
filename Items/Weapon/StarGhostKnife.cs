using StarBreaker.Projs.StarGhostKnife;
using Terraria.ModLoader.IO;

namespace StarBreaker.Items.Weapon
{
    public class StarGhostKnife : ModItem
    {
        private int _MyOwner = -1;
        private static StarGhostKnifeAtk GetGhostAttack(Player player)
        {
            return (StarGhostKnifeAtk)player.GetModPlayer<StarPlayer>().GhostSwordAttack;
        }

        private static void SetGhostAttack(Player player, int attack)
        {
            player.GetModPlayer<StarPlayer>().GhostSwordAttack = attack;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰鬼刀");
            Tooltip.SetDefault("绝对的力量\n" +
                "星辰之主用了这把刀和星辰击碎者守护星辰\n" +
                "即便敌人再恐怖,它也有可怕的力量撕裂敌人\n" +
                "通过召唤鬼神战斗\n" +
                "如果论起强度,它是鬼神之主" +
                "现在，它是你的了\n" +
                "切换状态:使用被绑定的按钮切换\n" +
                " \" 痛苦,绝望,都凝聚之上,只是被注入了星辰的力量才压制了那份真实的实力 \" \n" +
                "鬼神在嚎叫");
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<GhostFireHit>();
            Item.shootSpeed = 9f;
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 38;
            Item.rare = ItemRarityID.Red;
            Item.value = 13100232;
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }
        public override bool OnPickup(Player player)
        {
            _MyOwner = player.whoAmI;
            return base.OnPickup(player);
        }
        public override void SaveData(TagCompound tag)
        {
            tag["myOnwer"] = _MyOwner;
        }
        public override void LoadData(TagCompound tag)
        {
            _MyOwner = tag.GetInt("myOnwer");
        }
        public override void PostUpdate()
        {
            try
            {
                if (_MyOwner >= 0)
                {
                    Player player = Main.player[_MyOwner];
                    for (int i = 0; i < 58; i++)
                    {
                        if (player.inventory[i].stack == 0 || !player.inventory[i].active)
                        {
                            player.inventory[i] = Item.Clone();
                            break;
                        }
                        else if (i == 57 && player.inventory[i].active)
                        {
                            player.inventory[i] = Item.Clone();
                        }
                    }
                    Item.TurnToAir();
                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Text = "你无法丢弃它,除开销毁",
                        DurationInFrames = 120,
                        Velocity = new Vector2(0, -4),
                        Color = Color.White
                    }, player.Center);
                }
            }
            catch { }
            base.PostUpdate();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.statLife -= 100;
            switch (GetGhostAttack(player))
            {
                case StarGhostKnifeAtk.Kalla://卡洛
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, ModContent.ProjectileType<GhostFire>(),
                            damage * 50, knockback, player.whoAmI);
                        return false;
                    }
                case StarGhostKnifeAtk.GhostFireHit://鬼影斩
                    {
                        //SetGhostAttack(player,2);
                        break;
                    }
                case StarGhostKnifeAtk.StoneShower:
                    {
                        SetGhostAttack(player, 0);
                        break;
                    }
                case StarGhostKnifeAtk.LunarSlash://满月斩
                    {
                        break;
                    }

                case StarGhostKnifeAtk.KeiGa:
                    break;
                case StarGhostKnifeAtk.Puchumeng:
                    break;
                case StarGhostKnifeAtk.SaYa:
                    break;
                case StarGhostKnifeAtk.GhostSlash:
                    break;
                case StarGhostKnifeAtk.Rhasa:
                    break;
                case StarGhostKnifeAtk.Kazan:
                    break;
                case StarGhostKnifeAtk.Buraxiu:
                    break;
                default:
                    break;
            }
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "鬼神之力的过度使用"), 10, player.direction);
            }
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && (line.Name == "ItemName" || line.Name == "Damage"))
                {
                    line.OverrideColor = new Color?(new Color((int)Math.Sqrt(Main.time), 1, (int)Math.Sqrt(Main.time)));
                }
                if (line.Mod == "Terraria" && line.Name == "Tooltip3")
                {
                    line.OverrideColor = Color.Red;
                }
            }
        }
    }
}
