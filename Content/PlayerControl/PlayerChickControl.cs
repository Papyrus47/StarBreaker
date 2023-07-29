using StarBreaker.Content.Component.ComponentPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.PlayerControl
{
    public class PlayerChickControl : BasicPlayerComponent
    {
        public bool ChickLeft;
        public bool ChannelLeft;
        public int ChannelLeftTime;
        public bool ChickRigth;
        public bool ChannelRigth;
        public int ChannelRigthTime;

        public PlayerChickControl(StarBreakerPlayer starBreakerPlayer) : base(starBreakerPlayer)
        {
        }

        public override void SetControls()
        {
            ChickLeft = ChickRigth = false;

            if (ChannelLeft && ChannelLeftTime < 6) ChickLeft = true;
            if (player.controlUseItem)
            {
                ChannelLeftTime++;
                ChannelLeft = true;
            }
            else
            {
                ChannelLeftTime = 0;
                ChannelLeft = false;
            }


            if (ChannelRigth && ChannelRigthTime < 6) ChickRigth = true;
            if (player.controlUseTile)
            {
                ChannelRigthTime++;
                ChannelRigth = true;
            }
            else
            {
                ChannelRigth = false;
                ChannelRigthTime = 0;
            }
        }
    }
}
