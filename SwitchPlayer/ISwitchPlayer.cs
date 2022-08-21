using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.SwitchPlayer
{
    public interface ISwitchPlayer
    {
        Player Player { get; set; }
        void Draw();
        void Update();
        void SetAppearance();
    }
}
