using StarBreaker.NPCs;
using StarBreaker.SpecialBattles;
using StarBreaker.StarUI;
using StarBreaker.StarUI.Research;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace StarBreaker
{
    public partial class StarBreakerSystem : ModSystem
    {
        public static bool downedStarBreakerNom;
        public static bool downedStarBreakerEX;
        public static bool downedStarSpiralBlade;
        public static bool downedStarFist;
        public static bool downedOnyxBlaster;
        public static SpecialBattle SpecialBattle = null;
        public static List<Particle.Particle> Particles = new();

        #region UI变量
        #region 星击子弹UI
        public static StarBreakerUIState starBreaker_UI;
        internal UserInterface _userInterface;
        #endregion
        #region 星击能量条显示UI
        public static StarChargeUIState chargeUIState;
        internal UserInterface chargeUser;
        #endregion
        #endregion
        #region 测试用 血魂任务书
        public static StarBook_UI book_UI;
        internal UserInterface starBook_UI;
        #endregion
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedStarBreakerNom)
            {
                tag["downedStarBrekerNom"] = downedStarBreakerNom;
            }

            if (downedStarBreakerEX)
            {
                tag["downedStarBreakerEX"] = downedStarBreakerEX;
            }

            if (downedStarSpiralBlade)
            {
                tag["downedStarSpiralBlade"] = downedStarSpiralBlade;
            }

            if (downedStarFist)
            {
                tag["downedStarFist"] = downedStarFist;
            }

            if (downedOnyxBlaster)
            {
                tag["downedOnyxBlaster"] = downedOnyxBlaster;
            }
        }
        public override void LoadWorldData(TagCompound tag)
        {
            downedStarBreakerNom = tag.ContainsKey("downedStarBrekerNom");
            downedStarBreakerEX = tag.ContainsKey("downedStarBreakerEX");
            downedStarSpiralBlade = tag.ContainsKey("downedStarSpiralBlade");
            downedStarFist = tag.ContainsKey("downedStarFist");
            downedOnyxBlaster = tag.ContainsKey("downedOnyxBlaster");
        }
        public override void OnWorldLoad()
        {
            downedStarBreakerNom = false;
            downedStarBreakerEX = false;
            downedStarSpiralBlade = false;
            downedStarFist = false;
            Particles = new();

            #region UI加载
            starBreaker_UI = new();
            starBreaker_UI.Activate();
            _userInterface = new();
            _userInterface.SetState(starBreaker_UI);

            chargeUIState = new();
            chargeUIState.Activate();
            chargeUser = new();
            chargeUser.SetState(chargeUIState);

            book_UI = new();
            book_UI.Activate();
            starBook_UI = new();
            starBook_UI.SetState(book_UI);
            #endregion
        }
        public override void Load()
        {
            #region UI加载
            starBreaker_UI = new();
            starBreaker_UI.Activate();
            _userInterface = new();
            _userInterface.SetState(starBreaker_UI);

            chargeUIState = new();
            chargeUIState.Activate();
            chargeUser = new();
            chargeUser.SetState(chargeUIState);

            book_UI = new();
            book_UI.Activate();
            starBook_UI = new();
            starBook_UI.SetState(book_UI);
            #endregion
        }
        public override void Unload()
        {
            SpecialBattle = null;
            Particles = null;
            starBreaker_UI = null;
            _userInterface = null;
            chargeUIState = null;
            chargeUser = null;
            starBook_UI = null;
            book_UI = null;
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
            foreach (var particle in Particles.ToArray()) particle.Update();
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            foreach (var particle in Particles.ToArray()) particle.Draw(spriteBatch);
        }
        public override void OnWorldUnload()
        {
            downedStarBreakerNom = false;
            downedStarBreakerEX = false;
            downedStarSpiralBlade = false;
            downedStarFist = false;
            starBook_UI = null;
            _userInterface = null;
            chargeUIState = null;
            chargeUser = null;
        }
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            BitsByte flags = new();//最多只有8个索引
            flags[0] = downedStarBreakerNom;//向多人服务器写入击败信息
            flags[1] = downedStarBreakerEX;
            flags[2] = downedStarSpiralBlade;
            flags[3] = downedStarFist;
            writer.Write(flags);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            BitsByte flags = reader.ReadByte();//与上面的对应
            downedStarBreakerNom = flags[0];
            downedStarBreakerEX = flags[1];
            downedStarSpiralBlade = flags[2];
            downedStarFist = flags[3];
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _userInterface?.Update(gameTime);
            chargeUser?.Update(gameTime);
            starBook_UI?.Update(gameTime);
            base.UpdateUI(gameTime);
        }
        public override void PostUpdateWorld()
        {
            if (Main.LocalPlayer.ZoneSkyHeight && Main.rand.NextBool(100) && !Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                if (NPC.downedAncientCultist && !downedStarBreakerNom)
                {
                    NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<StarBreakerN>());
                }
                else if (downedStarBreakerNom && !downedStarBreakerEX)
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
                layers.Insert(Index, new LegacyGameInterfaceLayer("StarBreaker:Charge",
                delegate
                {
                    chargeUser.Draw(Main.spriteBatch, new GameTime());
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(Index, new LegacyGameInterfaceLayer("BloodSoul:StarBook",
                delegate
                {
                    starBook_UI.Draw(Main.spriteBatch, new GameTime());
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
