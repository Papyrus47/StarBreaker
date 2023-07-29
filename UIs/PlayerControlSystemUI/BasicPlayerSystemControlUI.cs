using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace StarBreaker.UIs.PlayerControlSystemUI
{
    public abstract class BasicPlayerSystemControlUI : UIState
    {
        protected UIElement healthUI;
        public override void OnInitialize()
        {
            base.OnInitialize();
            healthUI.Left.Set(50, 0);
            healthUI.Top.Set(50, 0);
            Append(healthUI);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (StarBreakerSystem.InWorld && StarBreakerSystem.WorldLoad) base.Draw(spriteBatch);
        }
    }
}
