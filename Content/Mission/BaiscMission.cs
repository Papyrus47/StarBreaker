using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace StarBreaker.Content.Mission
{
    public abstract class BasicMission
    {
        public abstract string MissionID { get; }
        public static float Progress { set => MissionSystem.Instance.Progress = value; }
        public abstract void SummonMission();
    }
}
