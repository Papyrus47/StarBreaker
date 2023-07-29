namespace StarBreaker.Items.StarOwner.StarsPierceWeapon
{
    public class StarsPierce : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("繁星刺破");
            /* 
             * Tooltip.SetDefault("以星辰之力凝聚而形成的一把刺剑,隶属于\"劈刺剑\"类型\n" +
                "攻击时,将运用少量魔力,形成一层很薄的膜覆盖在剑身上,以增加锋利程度且保护剑身(剑身自带定量魔力,无需额外注入)\n" +
                "Combo1:攻击-停顿-攻击(可蓄力):第一击将普通的刺出,随后注入魔力,提升第二击时,剑的锋利以及刺出速度\n" +
                "Combo2:攻击-攻击-攻击-攻击:刺出后,将命中的目标上挑,随后下砍,最后以一记强而有力的刺击贯穿目标\n" +
                "Combo3:攻击-攻击-停顿-攻击(可连续点击):上挑后,稍微停顿,随后接着暴风雨般的刺击,将目标刺的面目全非!\n" +
                "Combo4:攻击-攻击-攻击-停顿-攻击-攻击(可连续点击):下砍后,再次将目标上挑,随后接着暴风雨般的刺击,将目标刺的面目全非!\n" +
                "前冲刺击:连续向前移动(点击两次玩家朝向方向键)时,按下攻击键:向前前冲大段距离的同时," +
                "举起刺剑面对着前方,其威力之大可以刺穿盔甲\n" + //使用咿呀剑法(bushi)
                "刺星:连续向后移动时,按下攻击键(可蓄力):向后撤一段距离,随后为刺剑注入魔力,随后前冲,连续刺击目标,最后一击稍微蓄力\n" +
                "乱舞:在连续刺击时,疯狂按攻击键:360度的向前挥舞一周后,再360度向后挥舞一周," +
                "最后疯狂向前刺击,最后一击将稍微蓄力,以提升威力(未完成)\n" + //鬼泣3的crazy combo
                "Air Combo1:在空中时,攻击-攻击(可蓄力):向目标刺去,然后使用盔甲向下喷气,悬浮在空中,对刺剑注入魔力," +
                "随后用盔甲展现手臂部分,向朝着目标反方向喷气,以极其恐怖的力量刺穿目标\n" +
                "Air Combo2:在空中时,攻击-停顿-攻击:向目标刺去,然后上挑\n" +
                //"繁星刺破:盔甲展现时,装备在右手,长按特殊攻击键:" +
                //"使用魔力固定住一个目标(仅限中小型目标),随后盔甲喷气,以高速飞到目标前面,对所有在飞行时接触到玩家的敌人造成伤害\n" +
                //"随后以极高速刺击指定目标,最后一击将让目标体验什么是恐怖到祖宗十八代一起上天堂!\n" +
                "特殊状态:贯穿敌人:当目标被贯穿时,目标将被钉在剑上,这时保持拿着剑,而不回收,直到目标自行挣脱或者进行使用刺剑其他动作(未完成)");
            */
        }
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 2;
            Item.Size = new(104, 102);
            Item.knockBack = 1f;
            Item.value = 0;
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<StarsPierceProj>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] == 0)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, player.velocity,
                    Item.shoot, player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
            }
        }
    }
}
