using StarBreaker.NPCs;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.UI;
using StarBreaker.SpecialBattles;

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
        }
        public override void PostUpdateEverything()
        {
            if (SpecialBattle != null)
            {
                if (SpecialBattle.active)
                {
                    SpecialBattle.Update();
                }
                else SpecialBattle = null;
            }
        }
        public override void OnWorldUnload()
        {
            downedStarBreakerNom = false;
            downedStarBreakerEX = false;
            downedStarSpiralBlade = false;
            downedStarFist = false;
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
            StarBreaker.Instantiate._userInterface?.Update(gameTime);
            StarBreaker.Instantiate.chargeUser?.Update(gameTime);
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
                        StarBreaker.Instantiate._userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },//委托
                    InterfaceScaleType.UI)
                );//添加UI
                layers.Insert(Index, new LegacyGameInterfaceLayer("StarBreaker:Charge",
                delegate
                {
                    StarBreaker.Instantiate.chargeUser.Draw(Main.spriteBatch, new GameTime());
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
