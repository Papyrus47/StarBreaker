using StarBreaker.Items.StarOrigin.StarOriginStaff;
using StarBreaker.Items.StarOwner.StarsPierceWeapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.UIs.PlayerControlSystemUI.StarOrigin
{
    public class StarOriginControlUI : BasicPlayerSystemControlUI
    {
        private TestChangeWeapon changeWeapon;
        public StarOriginControlUI() 
        {
            healthUI = new StarOriginHealthUI();
            changeWeapon = new(new Item[]
            {
                new Item(ItemID.Zenith),
                new Item(ModContent.ItemType<StarsPierce>()),
                new Item(ModContent.ItemType<StarOriginStaffItem>()),
                new Item(ItemID.AdamantiteSword)
            });
            changeWeapon.Width.Set(42, 0f);
            changeWeapon.Height.Set(42, 0f);
            changeWeapon.Top.Set(Main.screenHeight - 150, 0f);
            changeWeapon.Left.Set(Main.screenWidth - 400, 0f);
            changeWeapon.Initialize();
            Append(changeWeapon);
        }
    }
}
