using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.PlayerControl
{
    public class PlayerOldControlSystem
    {
        public PlayerOldControlSystem() 
        {
            oldControls = new StarBreakerPlayerControl[12];
        }
        public StarBreakerPlayerControl[] oldControls;
        public void UpdateControl(StarBreakerPlayerControl.ControlType controlType)
        {
            if (oldControls[0] == null || oldControls[0].Type != controlType)
            {
                for (int i = oldControls.Length - 1; i > 0; i--)
                {
                    oldControls[i] = oldControls[i - 1]; // 保存玩家操作
                }
                oldControls[0] = new()
                {
                    Type = controlType,
                    Time = 0
                };
            }
            oldControls[0].Time++; // 更新时间
        }
    }
}
