using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items
{
    public class PlayerSwitch : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("角色切换器");
            Tooltip.SetDefault("切换操控角色");
        }
        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 10;
            Item.width = Item.height = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.autoReuse = false;
        }
    }
}
