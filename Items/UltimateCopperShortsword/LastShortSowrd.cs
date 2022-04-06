using Microsoft.Xna.Framework;
using StarBreaker.Projs.UltimateCopperShortsword;
using StarBreaker.Projs.UltimateCopperShortsword.ItemProj;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.UltimateCopperShortsword
{
    public class LastShortSowrd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
            Tooltip.SetDefault("真正的力量" +
                "\n右击物品以切换 跟随/炮塔 模式");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(40, 40);
            Item.UseSound = SoundID.Item1;
            Item.damage = 230;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 64;
            Item.knockBack = 3.4f;
            Item.value = 99999;
            Item.useTurn = true;
            Item.useTime = Item.useAnimation = 10;
            Item.rare = ItemRarityID.Red;
            Item.mana = 0;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LostSword2>();
            Item.shootSpeed = 21;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                var proj = Projectile.NewProjectileDirect(player.GetProjectileSource_Item(Item), player.Center, velocity,
                    ModContent.ProjectileType<LostSword2>(), damage, knockback, player.whoAmI);
                proj.friendly = true;
                proj.hostile = false;
                proj.ai[0] = 3;
            }
            if (player.altFunctionUse == 2)
            {
                player.AddBuff(ModContent.BuffType<Buffs.CopperBuff>(), 60);
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            if (shortSword.PlayerEmotion > 40 && player.altFunctionUse == 2)
            {
                return true;
            }
            else if (shortSword.PlayerEmotion < 40 && player.altFunctionUse == 2)
            {
                return false;
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            return base.UseItem(player);
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void RightClick(Player player)
        {
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            shortSword.SwordTurret = !shortSword.SwordTurret;
            Item.stack++;
        }
        public override void UpdateInventory(Player player)
        {
            StarPlayer shortSword = player.GetModPlayer<StarPlayer>();
            Item.useTime = Item.useAnimation;
            int lastSowrd = ModContent.ProjectileType<LastSowrd>();
            Item.damage = 230 + shortSword.PlayerEmotion;
            if (shortSword.EGO)
            {
                Item.damage += shortSword.PlayerEmotion * 5;
            }
            if (Item.favorited)
            {
                shortSword.SwordSum = true;
                if (player.ownedProjectileCounts[lastSowrd] < 1 && player.HeldItem.type != ModContent.ItemType<LastShortSowrd>())
                {
                    int proj = Projectile.NewProjectile(null, player.Center, Vector2.Zero, lastSowrd, Item.damage, 3f, player.whoAmI);
                    Main.projectile[proj].originalDamage = Item.damage;
                }
                if (player.HeldItem.type == ModContent.ItemType<LastShortSowrd>())
                {
                    shortSword.SwordSum = false;
                }
            }
        }
    }
}
