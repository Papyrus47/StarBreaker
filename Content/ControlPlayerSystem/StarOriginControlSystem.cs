using StarBreaker.Content.Component.ComponentPlayer.StarOrigin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.ControlPlayerSystem
{
    internal class StarOriginControlSystem : BasicControlPlayerSystem
    {
        public float ECh_Max;
        public float ECh_Now;
        public int ElementID;
        public StarOriginControlSystem(Player player) : base(player)
        {
            ControlPlayer.StarBreaker().Component.TryAdd(typeof(SOS_AttackElementAdd_Component),new SOS_AttackElementAdd_Component(player));
        }
        public override void OpenMagicConsumption()
        {
            base.OpenMagicConsumption();
        }
        public override void PlayerUpdate()
        {
            ECh_Max = ControlPlayer.statMana;
            if (ControlPlayer.StarBreaker().MagicConsumption)
            {
                ECh_Now = ECh_Max;
            }
            if(ECh_Now < ControlPlayer.statManaMax2 * 0.3f)
            {
                ECh_Max = 0;
            }
        }
        public override void CloseMagicConsumption()
        {
            base.CloseMagicConsumption();
        }
    }
}
