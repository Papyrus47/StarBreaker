using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.PlayerControl
{
    public class StarBreakerPlayerControl
    {
        public int Time;
        public ControlType Type;
        public enum ControlType : byte
        {
            Down = 0,
            Up = 1,
            Left = 2,
            Right = 3,
            None = 4,
        }
    }
}
