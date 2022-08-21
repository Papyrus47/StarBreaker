using StarBreaker.NPCs;
using StarBreaker.SpecialBattles;
using StarBreaker.StarUI;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace StarBreaker
{
    public partial class StarBreakerSystem : ModSystem
    {
        public class DownedNPC
        {
            public bool downedStarBreakerNom;
            public bool downedStarBreakerEX;
            public bool downedStarSpiralBlade;
            public bool downedStarFist;
            public bool downedOnyxBlaster;
        }
        public static DownedNPC downed;
        public static SpecialBattle SpecialBattle = null;
        public static Item[] saveStarBreakerUIStates;

        #region UI变量
        #region 星击子弹UI
        public static StarBreakerUIState starBreaker_UI;
        internal UserInterface _userInterface;
        #endregion
        #endregion
        public override void SaveWorldData(TagCompound tag)
        {
            if (downed.downedStarBreakerNom)
            {
                tag["downedStarBrekerNom"] = downed.downedStarBreakerNom;
            }

            if (downed.downedStarBreakerEX)
            {
                tag["downedStarBreakerEX"] = downed.downedStarBreakerEX;
            }
             if (saveStarBreakerUIStates != null)
            {
                tag["StarBreaker:Bullet1"] = saveStarBreakerUIStates[0];
                tag["StarBreaker:Bullet2"] = saveStarBreakerUIStates[1];
            }

        }
        public override void LoadWorldData(TagCompound tag)
        {
            downed.downedStarBreakerNom = tag.ContainsKey("downedStarBrekerNom");
            downed.downedStarBreakerEX = tag.ContainsKey("downedStarBreakerEX");
            if (saveStarBreakerUIStates == null) saveStarBreakerUIStates = new Item[2];
            else
            {
                starBreaker_UI.element1.Item = tag.Get<Item>("StarBreaker:Bullet1");
                starBreaker_UI.element2.Item = tag.Get<Item>("StarBreaker:Bullet2");
            }
        }
        public override void OnWorldLoad()
        {
            downed = new();

            #region UI加载
            starBreaker_UI = new();
            starBreaker_UI.Activate();
            _userInterface = new();
            _userInterface.SetState(starBreaker_UI);

            #endregion
        }
        public override void Load()
        {
            #region UI加载
            starBreaker_UI = new();
            starBreaker_UI.Activate();
            _userInterface = new();
            _userInterface.SetState(starBreaker_UI);
            #endregion
            downed = new();
            saveStarBreakerUIStates = new Item[2];
        }
        public override void Unload()
        {
            SpecialBattle = null;
            starBreaker_UI = null;
            _userInterface = null;
            downed = null;
            SpecialBattle = null;
            saveStarBreakerUIStates = null;
        }
        public override void PostUpdateEverything()
        {
            if (SpecialBattle != null)
            {
                if (SpecialBattle.active)
                {
                    SpecialBattle.Update();
                }
                else
                {
                    SpecialBattle = null;
                }
            }
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
        }
        public override void OnWorldUnload()
        {
            downed = null;
            _userInterface = null;
        }
        //public override void NetSend(BinaryWriter writer)
        //{
        //    base.NetSend(writer);
        //    BitsByte flags = new();//最多只有8个索引
        //    flags[0] = downed.downedStarBreakerNom;//向多人服务器写入击败信息
        //    flags[1] = downed.downedStarBreakerEX;
        //    writer.Write(flags);
        //}
        //public override void NetReceive(BinaryReader reader)
        //{
        //    base.NetReceive(reader);
        //    BitsByte flags = reader.ReadByte();//与上面的对应
        //    downed.downedStarBreakerNom = flags[0];
        //    downed.downedStarBreakerEX = flags[1];
        //    downedStarSpiralBlade = flags[2];
        //    downedStarFist = flags[3];
        //}
        public override void UpdateUI(GameTime gameTime)
        {
            _userInterface?.Update(gameTime);
            if(saveStarBreakerUIStates != null)
            {
                saveStarBreakerUIStates[0] = starBreaker_UI.element1.Item;
                saveStarBreakerUIStates[1] = starBreaker_UI.element2.Item;
            }
            base.UpdateUI(gameTime);
        }
        public override void PostUpdateWorld()
        {
            if (Main.LocalPlayer.ZoneSkyHeight && Main.rand.NextBool(100) && !Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                if (NPC.downedAncientCultist && !downed.downedStarBreakerNom)
                {
                    NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<StarBreakerN>());
                }
                else if (downed.downedStarBreakerNom && !downed.downedStarBreakerEX)
                {
                    NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<StarBreakerN>());
                }
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int Index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));//获得在鼠标下的索引
            if (Index != -1)//如果找到绘制层
            {
                layers.Insert(Index, new LegacyGameInterfaceLayer(
                    "StarBreaker:Bullet",//名字
                    delegate
                    {
                        _userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },//委托
                    InterfaceScaleType.UI)
                );//添加UI
            }
        }
    }
}
