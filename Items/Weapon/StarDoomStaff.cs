using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 61;
            Item.mana = 2;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = 5130142;
            Item.UseSound = SoundID.Item101;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<Projs.StarDoomStaff.StarBoomCrystal>();
            Item.shootSpeed = 30;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Main.rand.NextVector2Unit() * Main.rand.NextFloat(60, 80);
            type = Main.rand.Next(new int[]{
            ModContent.ProjectileType<Projs.StarDoomStaff.StarBoomCrystal>(),
            ModContent.ProjectileType<Projs.StarDoomStaff.StarControlCrystal>()}
            );
            if (type == ModContent.ProjectileType<Projs.StarDoomStaff.StarControlCrystal>() && player.ownedProjectileCounts[ModContent.ProjectileType<Projs.StarDoomStaff.StarControlCrystal>()] > 5)
            {
                type = ModContent.ProjectileType<Projs.StarDoomStaff.StarCrystal>();
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasMinionAttackTargetNPC)
            {
                float max = 1200;
                foreach (NPC npc in Main.npc)
                {
                    float dis = Vector2.Distance(npc.position, player.position);
                    if (npc.active && dis < max && npc.CanBeChasedBy() && !npc.friendly)
                    {
                        max = dis;
                        player.MinionAttackTargetNPC = npc.whoAmI;
                    }
                }
            }
            else Main.npc[player.MinionAttackTargetNPC].GetGlobalNPC<NPCs.StarGlobalNPC>().StarDoomMark = true;
            Main.projectile[Projectile.NewProjectile(player.GetProjectileSource_Item(Item), position, (Main.MouseWorld - position).SafeNormalize(default) * Item.shootSpeed, type, damage, knockback, player.whoAmI)].originalDamage = damage;
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.mod == "Terraria")
                {
                    if (line.Name == "Tooltip0" || line.Name == "Tooltip3" || line.Name == "Tooltip8")
                    {
                        line.overrideColor = Color.Purple * 0.8f;
                    }
                    else if (line.Name == "Tooltip9")
                    {
                        line.overrideColor = Color.Red * 0.8f;
                    }
                    else if (line.Name == "ItemName")
                    {
                        line.overrideColor = new Color(100, 21, 203);
                    }
                }
            }
        }
    }
}
