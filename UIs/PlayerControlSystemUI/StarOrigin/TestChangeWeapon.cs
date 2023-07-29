using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.UIs.PlayerControlSystemUI.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.UIs.PlayerControlSystemUI.StarOrigin
{
    public class TestChangeWeapon : BasicChangeWeapon
    {
        public TestChangeWeapon(Item[] equippedItems) : base(equippedItems, () => BasicControlPlayerSystem.ControlChangeRightWeapon && !BasicControlPlayerSystem.ReleaseChangeRightWeapon)
        {
            drawTex = ModContent.Request<Texture2D>("StarBreaker/UIs/PlayerControlSystemUI/StarOrigin/ChooseItem");
        }
    }
}
