using StarBreaker.Items.DamageClasses;

namespace StarBreaker.Items.Weapon
{
    public class StarSpiralBlade : Type.StarWeapon
    {
        public override string Favorability => "方法1:击败星辰之主后,带着它到太空环境中待3分钟\n" +
            "方法2:击败星辰拳套之一:炎拳(人形态)";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星辰旋刃");
            Tooltip.SetDefault("星辰旋刃,具有可怕的力量,和星辰鬼刀一样,根据主人的强度而改变自己的旋转速度,旋转速度决定其伤害\n" +
                "星辰之主的依靠武器,掌握神秘的回旋力量,速度达到一定时候就可以使用特殊技能\n" +
                "根据旋转方向决定命中效果\n" +
                "当转速超过3000时,旋刃可以使用特殊技能\"无尽回旋\"\n" +
                "当转速超过10000时,旋刃会自动追踪一开始距离鼠标最近的npc\n" +
                "当转速超过30000时,旋刃可以使用特殊技能\"死亡回旋\"\n" +
                "当转速超过300000时,空间将被撕裂\n" +
                "可怜的是,除开星辰之主,没有人能将旋刃的转速提到15000转以上\n" +
                "长按左键为星辰旋刃积累旋转速度,松手后丢出\n" +
                "点击右键直接使用星辰旋刃上下劈砍");
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 1.2f;
            Item.DamageType = ModContent.GetInstance<FourDamage>();
            Item.rare = ItemRarityID.Purple;
            Item.crit = 23;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 10;
            Item.width = Item.height = 112;
            Item.shoot = ModContent.ProjectileType<Projs.StarSpiralBladeProj>();
            Item.shootSpeed = 30;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            proj.originalDamage = damage;
            if (player.altFunctionUse == 2)
            {
                proj.DamageType = DamageClass.Melee;
                proj.timeLeft = 500;
                proj.friendly = true;
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
