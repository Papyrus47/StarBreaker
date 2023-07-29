using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.UIs.PlayerControlSystemUI.General
{
    public class HealthUIHelper
    {
        public int OldLife;
        public int OldMana;
        public int Time;
        public byte Alpha;
        public void Update(int life,int Mana)
        {
            if (OldLife != life || OldMana != Mana)
            {
                Time = 0;
                Alpha = 255;
            }
            else if (Time++ > 180)
            {
                if (Alpha >= 15) Alpha -= 15;
                else Alpha = 0;
            }
            OldLife = life;
            OldMana = Mana;
        }
    }
}
