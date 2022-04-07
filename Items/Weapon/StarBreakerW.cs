using Microsoft.Xna.Framework;
using StarBreaker.Items.DamageClasses;
using StarBreaker.Projs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class StarBreakerW : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰击碎者");
            Tooltip.SetDefault("星辰之力,寄宿于之上，强度可以击碎星空\n" +
                "使用能量子弹/能量聚集器作为子弹");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 40;
            Item.shoot = ModContent.ProjectileType<StarBreakerHeadProj>();
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
        public override bool OnPickup(Player player)
        {
            PopupText.NewText(new AdvancedPopupRequest()
            {
                Text = Main.rand.Next<string>(new string[]
                {
                    "额...你好?",
                    "YaHo~",
                    "我来了~"
                }),
                DurationInFrames = 120,
                Velocity = new Vector2(0, -4),
                Color = Color.Purple
            }, player.Center);
            return base.OnPickup(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (StarBreakerSystem.downedStarBreakerEX)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Normalize(Main.MouseWorld - player.Center),
                    ModContent.ProjectileType<StarBreakerHeadProjReal>(), damage, knockback, player.whoAmI);
                //召唤枪
                if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<OmnipotentGun>()] < 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int proj = Projectile.NewProjectile(source, position, velocity,
                            ModContent.ProjectileType<OmnipotentGun>(), 1, 1, player.whoAmI);

                        if (Main.projectile[proj].ModProjectile is OmnipotentGun gun)
                        {
                            gun.States[4] = i;
                        }
                        #region 获取对应武器贴图
                        foreach (Item item in player.inventory)
                        {
                            if (item.DamageType == ModContent.GetInstance<StarBreaker.Items.DamageClasses.EnergyDamage>())
                            {
                                (Main.projectile[proj].ModProjectile as OmnipotentGun).ItemGun = item;
                                Main.projectile[proj].damage = item.damage;
                                continue;
                            }
                        }
                    }
                }
                #endregion
                return false;
            }
            Projectile.NewProjectile(source, player.Center, Vector2.Normalize(Main.MouseWorld - player.Center),
                ModContent.ProjectileType<StarBreakerHeadProj>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override void UpdateInventory(Player player)
        {
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            if (StarBreakerSystem.downedStarBreakerEX && Item.damage < 30) Item.damage = 30;
            if (starPlayer.SummonStarShieldTime == 1)
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Text = "我觉得护盾快好了",
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
                    int who = Projectile.NewProjectile(player.GetProjectileSource_Item(Item), player.Center, Vector2.Zero, type, Item.damage * 2, Item.knockBack, player.whoAmI);
                    Main.projectile[who].originalDamage = Item.damage * 2;
                }
            }
        }
        public override bool? UseItem(Player player)
        {
            StarPlayer starPlayer = player.GetModPlayer<StarPlayer>();
            if (starPlayer.SummonStarShieldTime <= 0)
            {
                starPlayer.SummonStarShieldTime = 3600;
                Projectile.NewProjectile(player.GetProjectileSource_Item(Item),
                    player.position, Vector2.Zero, ModContent.ProjectileType<StarShieldPlayer>(), 1, 1, player.whoAmI);
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (StarBreakerSystem.downedStarBreakerEX)
            {
                TooltipLine tooltip1 = new(Mod, "StarBreaker", "星辰之力 得到解放:");
                tooltip1.overrideColor = Color.MediumPurple;
                tooltips.Add(tooltip1);
                TooltipLine tooltip2 = new(Mod, "StarBreaker1", "星辰击碎者真正的信任你,获得控制使用能量的枪的能力");
                tooltip2.overrideColor = Color.MediumPurple;
                tooltips.Add(tooltip2);
            }
        }
    }

}
