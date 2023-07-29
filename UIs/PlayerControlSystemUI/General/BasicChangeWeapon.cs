using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.UI;

namespace StarBreaker.UIs.PlayerControlSystemUI.General
{
    public abstract class BasicChangeWeapon : UIElement
    {
        /// <summary>
        /// 绘制的贴图
        /// </summary>
        protected Asset<Texture2D> drawTex;
        /// <summary>
        /// 装备的物品
        /// </summary>
        public Item[] EquippedItems;
        /// <summary>
        /// 现在选择的物品
        /// </summary>
        public int NowItems;
        /// <summary>
        /// 透明度
        /// </summary>
        public byte Alpha;
        /// <summary>
        /// 时间
        /// </summary>
        public int ControlTime;
        /// <summary>
        /// 物品滚动需要时间
        /// </summary>
        public byte ItemRollTime;
        public Item ChooseItem => EquippedItems[NowItems];
        public Func<bool> ChooseControlFun;
        public bool IsSetRollItem;
        public bool RollItemEnd;
        private struct RollItem
        {
            public int ItemType;
            public Vector2 Pos,ToPos;
            public Vector2 Vel;
            public float Scale;
            public readonly void Draw(SpriteBatch spriteBatch,byte alpha)
            {
                if (Scale < 0f) return;
                Texture2D tex = TextureAssets.Item[ItemType].Value;
                Color color = Color.White;
                spriteBatch.Draw(tex, Pos, null, color * (alpha / 255f),0f,tex.Size() * 0.5f,Scale,SpriteEffects.None,0f);
            }
            public bool Update(bool ScaleShrink = false,bool nextRoll = false)
            {
                if ((nextRoll && Scale < 0.6f) || (!nextRoll && !ScaleShrink && Scale < 1)) Scale += 0.05f;
                else if (ScaleShrink && Scale > 0.6f) Scale -= 0.05f;
                if(Scale >= 0 && (ToPos - Pos).LengthSquared() >= 40)
                {
                    Pos += Vel;
                    return true;
                }
                return false;
            }
        }
        private RollItem nowRollItem, nextRollItem, oldRollItem;
        public BasicChangeWeapon(Item[] equippedItems, Func<bool> chooseControlFun)
        {
            EquippedItems = equippedItems;
            ChooseControlFun = chooseControlFun;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ControlTime > 0) ControlTime--;
            else if (Alpha > 0) Alpha -= 15; // 使透明度减少
            RollItemEnd = nowRollItem.Update(); // 为false的时候,可以转
            nextRollItem.Update(false, true);
            oldRollItem.Update(true);

            Main.LocalPlayer.active = true;
            Main.LocalPlayer.dead = false;
            //if(Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2) Main.LocalPlayer.statLife++;
            if (Main.LocalPlayer.TryGetModPlayer<StarBreakerPlayer>(out var starBreakerPlayer)
                && !starBreakerPlayer.InAttack) Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem] = ChooseItem;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // TODO : 对玩家按键的操作判断后,将按键控制时间增加到180
            if (ChooseControlFun.Invoke()) // 如果玩家按下对应的按键
            {
                ControlTime = 180;
                Alpha = 255; // 设置透明度为255,绘制非透明
                IsSetRollItem = false;
                NowItems = WrapIndex(NowItems + 1);
            }

            if (Alpha > 0)
            {
                Color color = Color.White * (Alpha / 255f);
                var rect = GetDimensions().ToRectangle();
                spriteBatch.Draw(drawTex.Value, rect, color); // 绘制
                if (!IsSetRollItem && !RollItemEnd) SetRollItem(rect.Center());
                DrawRollItem();
            }
        }
        private void SetRollItem(Vector2 Pos)
        {
            IsSetRollItem = true;
            Vector2 vel = Vector2.UnitX * -5f;

            Vector2 startPos = Pos + new Vector2(150, 0);

            nowRollItem.Pos = startPos;
            nowRollItem.ToPos = Pos;
            nowRollItem.Vel = vel;
            nowRollItem.Scale = 0f;
            nowRollItem.ItemType = EquippedItems[NowItems].type;

            int index = WrapIndex(NowItems - 1);
            oldRollItem.Pos = Pos;
            oldRollItem.ToPos = Pos - new Vector2(150, 0);
            oldRollItem.Vel = vel;
            oldRollItem.Scale = 1f;
            oldRollItem.ItemType = EquippedItems[index].type;

            index = WrapIndex(NowItems + 1);
            nextRollItem.Pos = startPos;
            nextRollItem.ToPos = startPos;
            nextRollItem.Vel = Vector2.Zero;
            nextRollItem.Scale = -0.5f;
            nextRollItem.ItemType = EquippedItems[index].type;
        }
        private int WrapIndex(int index)
        {
            if(index < 0)
            {
                index = EquippedItems.Length - 1;
            }
            else if(index >= EquippedItems.Length)
            {
                index = 0;
            }
            return index;
        }
        private void DrawRollItem()
        {
            var sb = Main.spriteBatch;
            oldRollItem.Draw(sb, Alpha);
            nextRollItem.Draw(sb, Alpha);
            nowRollItem.Draw(sb, Alpha);
        }
    }
}
