using Mono.Cecil;
using StarBreaker.Items.DamageClasses;
using StarBreaker.Projs;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;

namespace StarBreaker.Items.Weapon
{
    public class StarBreakerW : ModItem
    {
        private bool _hasMe,_onSpwan;
        public List<int> UseAmmo; 
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Breaker");
            DisplayName.AddTranslation(7, "星辰击碎者");
            Tooltip.SetDefault("星辰之力,寄宿于之上，强度可以击碎星空\n" +
                "使用能量子弹/能量聚集器作为子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            UseAmmo = new(20);
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = ModContent.GetInstance<EnergyDamage>();
            Item.knockBack = 3.2f;
            Item.rare = ItemRarityID.Purple;
            Item.channel = true;
            Item.width = 66;
            Item.height = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 30;
            Item.autoReuse = true;
            Item.value = 2949204;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 40;
            Item.shoot = ModContent.ProjectileType<StarBreakerHeadProj>();
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            _hasMe = true;
            if (UseAmmo == null) UseAmmo = new(20);
        }
        public override void SaveData(TagCompound tag)
        {
            for(int i = 0;i< UseAmmo.Count;i++)
            {
                tag["StarBreakerW:UseAmmo" + i.ToString()] = UseAmmo[i];
            }
        }
        public override void LoadData(TagCompound tag)
        {
            UseAmmo ??= new(20);
            for (int i = 0; i < UseAmmo.Count; i++)
            {
                UseAmmo[i] = tag.GetInt("StarBreakerW:UseAmmo" + i.ToString());
            }
        }
        public override bool OnPickup(Player player)
        {
            const string Text = "Mods.StarBreaker.StarBreakerText.WeaponText.PickupText.";
            if (player.HasItem(Type))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = Language.GetTextValue(Text + "Text1"),
                    DurationInFrames = 120,
                    Velocity = new Vector2(0, -4),
                    Color = Color.Purple
                }, player.Center);
                Item.TurnToAir();
            }
            else
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = Main.rand.Next(new string[]
                    {
                    Language.GetTextValue(Text + "Text2"),
                    Language.GetTextValue(Text + "Text3"),
                    Language.GetTextValue(Text + "Text4")
                    }),
                    DurationInFrames = 120,
                    Velocity = new Vector2(0, -4),
                    Color = Color.Purple
                }, player.Center);

            }
            return base.OnPickup(player);
        }
        public override void HoldItem(Player player)
        {
            if(StarBreakerSystem.downed.downedStarBreakerEX)
            {
                Item.shoot = ModContent.ProjectileType<StarBreakerHeadProjReal>();
            }
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center),
                    Item.shoot,player.GetWeaponDamage(Item),Item.knockBack, player.whoAmI);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, 
            Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.useStyle = player.altFunctionUse == 2 ? ItemUseStyleID.Rapier : ItemUseStyleID.Shoot;
            return true;
        }

        public override void PostUpdate()
        {
            if (!_onSpwan)
            {
                _onSpwan = true;
                return;
            }
            if (_hasMe)//丢弃时的语句
            {
                const string Text = "Mods.StarBreaker.StarBreakerText.WeaponText.OnThrow.";
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = Main.rand.Next(new string[]
                    {
                    Language.GetTextValue(Text + "Text1"),
                    Language.GetTextValue(Text + "Text2"),
                    Language.GetTextValue(Text + "Text3")
                    }),
                    DurationInFrames = 120,
                    Velocity = new Vector2(0, -4),
                    Color = Color.Purple
                }, Item.Center);
                _hasMe = false;
            }
        }
        public override void UpdateInventory(Player player)
        {
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            if(starPlayer.Const++ > 1)
            {
                Item.TurnToAir();
                return;
            }
            if (StarBreakerSystem.downed.downedStarBreakerEX && Item.damage < 30)
            {
                Item.damage = 30;
            }

            _hasMe = true;
            if (starPlayer.SummonStarShieldTime == 1)
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = Language.GetTextValue("Mods.StarBreaker.StarBreakerText.WeaponText.ShieldTimeCooldown"),
                    DurationInFrames = 120,
                    Velocity = new Vector2(0, -4),
                    Color = Color.Purple
                }, player.Center);
            }
            if (player.HeldItem.type != ModContent.ItemType<StarBreakerW>())
            {
                starPlayer.SummonStarWenpon = true;
                int type = ModContent.ProjectileType<StarBreakerProj>();
                if (player.ownedProjectileCounts[type] < 1 && Item.favorited)
                {
                    int who = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, type, Item.damage * 2, Item.knockBack, player.whoAmI);
                    Main.projectile[who].originalDamage = Item.damage * 2;
                }
            }
            //else if (StarBreakerSystem.SpecialBattle == null)
            //{
            //    StarBreakerSystem.SpecialBattle = new SpecialBattles.StarBreakerEX_SpecialBattle();
            //}
        }
        public override bool? UseItem(Player player)
        {
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            if (starPlayer.SummonStarShieldTime <= 0)
            {
                starPlayer.SummonStarShieldTime = 3600;
                Projectile.NewProjectile(player.GetSource_ItemUse(Item),
                    player.position, Vector2.Zero, ModContent.ProjectileType<StarShieldPlayer>(), 1, 1, player.whoAmI);
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (StarBreakerSystem.downed.downedStarBreakerEX)
            {
                TooltipLine tooltip1 = new(Mod, "StarBreaker", "封印解除:");
                tooltip1.OverrideColor = Color.MediumPurple;
                tooltips.Add(tooltip1);
                TooltipLine[] starBreaker = new TooltipLine[]{
                    new(Mod, "StarBreaker1", "攻击方法更改"),
                    new(Mod, "StarBreaker2", "可以召唤\"星辰击碎者\"所用的四把特殊武器(右键)"),
                    new(Mod, "StarBreaker3", "上升伤害至30"),
                };
                for (int i = 0; i < starBreaker.Length; i++)
                {
                    starBreaker[i].OverrideColor = Color.MediumPurple;
                }
                tooltips.AddRange(starBreaker);
            }
        }
    }

}
