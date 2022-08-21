namespace StarBreaker.Items.Weapon.IceGunAndFireKnife
{
    public class IceGunAndFireKnife : ModItem
    {
        public bool InMax => ChannelTime >= 1800;
        public int ChannelTime = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Gun and Fire Knife");
            Tooltip.SetDefault("A combination of \"Frost Star Bombarder\" and \"Fire Spin Blunt Knife\"\n" +
                "");
            DisplayName.AddTranslation(7, "冰旋斩炎刀");
            Tooltip.AddTranslation(7, "\"霜星轰击者\" 和 \"炎旋钝刀\"的结合体\n" +
                "星辰之主研发的最为可怕的蓄力武器\n" +
                "长按左键为激光炮蓄力,点击左键挥舞分裂的刀\n" +
                "长按右键为整个武器蓄力\n" +
                "当右键蓄力满后,攻击状态将会发生改变");
        }
        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Generic;
            Item.knockBack = 3.2f;
            Item.rare = ItemRarityID.Red;
            Item.channel = true;
            Item.width = 16;
            Item.height = 27;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.crit = 30;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 2;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FireKnife>()] == 0)
            {
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<FireKnife>(),
                    player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
            }
            if (player.altFunctionUse == 2 && ChannelTime < 3600)
            {
                ChannelTime++;
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Item.playerIndexTheItemIsReservedFor];
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Damage")
                {
                    line.Text = player.GetWeaponDamage(Item).ToString() + "近战伤害";
                }
            }
            tooltips.Insert(tooltips.FindIndex(x => x.Name == "Damage"), new(Mod, "IceGunAndFireKnife:Damage",
                (player.GetWeaponDamage(Item) * 5).ToString() + "蓄力远程伤害"));
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;

            Rectangle frame = new(0, 0, texture.Width, texture.Height / 2);//分为2帧
            if (InMax)
            {
                frame.Y = frame.Height;
            }
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center, frame, lightColor, 0, origin, scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            frame.Height = texture.Height / 2;
            if (InMax)
            {
                frame.Y = frame.Height;
            }
            origin = frame.Size() * 0.5f;
            position.Y += 15;
            position.X += frame.Width / 10;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0, origin, scale * 2.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}
