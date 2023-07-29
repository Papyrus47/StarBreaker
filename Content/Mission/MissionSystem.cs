using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace StarBreaker.Content.Mission
{
    public class MissionSystem : IUnLoad
    {
        private MissionSystem() 
        {
            InSummon = false;
        }
        private static MissionSystem _instance;
        public static MissionSystem Instance
        {
            get
            {
                _instance ??= new()
                {
                    MissionsGenPass = new()
                };
                return _instance;
            }
        }
        public Dictionary<string, BasicMission> MissionsGenPass;
        public bool InSummon;
        public float Progress;
        public void AddMission(string name, BasicMission genPass)
        {
            MissionsGenPass.Add(name, genPass);
        }

        public bool MissionLoad_Update(string ID)
        {
            if(MissionsGenPass.TryGetValue(ID, out BasicMission mission) && !InSummon)
            {
                Task.Factory.StartNew(() => mission.SummonMission());
                InSummon = true;
            }
            if (Progress >= 1f)
            {
                Progress = 0f;
                InSummon = false;
                return true;
            }
            return false;
        }
        public void MissionLoad_Draw(SpriteBatch sb)
        {
            string text = "加载进度:" + (Progress * 100f).ToString() + "%";
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            sb.DrawString(font, text,
                Main.ScreenSize.ToVector2() * 0.5f, Color.White,0f,font.MeasureString(text) * 0.5f,1f,SpriteEffects.None,0f);
        }
        public void UnLoad()
        {
            _instance = null;
        }
    }
}
