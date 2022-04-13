using Microsoft.Xna.Framework;
using StarBreaker.Projs.StarGhostKnife;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker.Items.Weapon
{
    public class StarGhostKnife : ModItem
    {
        private int GetGhostAttack(Player player) => player.GetModPlayer<StarPlayer>().GhostSwordAttack;
        private void SetGhostAttack(Player player, int attack) => player.GetModPlayer<StarPlayer>().GhostSwordAttack = attack;
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.statLife -= 100;
            switch ((StarGhostKnifeAtk)GetGhostAttack(player))
            {
                case StarGhostKnifeAtk.Kalla://卡洛
                    {
                        Projectile.NewProjectile(player.GetProjectileSource_Item(Item), position, velocity, ModContent.ProjectileType<GhostFire>(),
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
                if (line.mod == "Terraria" && line.Name == "ItemName")
                {
                    line.overrideColor = new Color?(new Color((int)Math.Sqrt(Main.time), 1, (int)Math.Sqrt(Main.time)));
                }
                if(line.mod == "Terraria" && line.text =="Tooltip5")
                {
                    line.overrideColor = Color.Red;
                }
            }
        }
    }
}
