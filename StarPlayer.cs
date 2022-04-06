﻿using Microsoft.Xna.Framework;
using StarBreaker.Items.UltimateCopperShortsword;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;

namespace StarBreaker
{
    public class StarPlayer : ModPlayer
    {
        public bool SummonStarWenpon;
        public bool InIdeaDriven;
        public int FrostFistModScr = -1;
        public int SummonStarShieldTime;
        public int GhostSwordAttack;
        public Item Bullet1;
        public Item Bullet2;
        public int StarCharge;
        public bool DrumDraw;//鼓的绘制
        #region 最终铜短剑
        public bool SwordSum = false;
        public bool EGO = false;
        public bool SwordTurret = false;
        public int PlayerEmotion = 0;//情感
        public int PlayerVectorZero = 0;//速度为0的时间
        #endregion
        public override void SaveData(TagCompound tag)
        {
            tag["Bullet1"] = Bullet1;
            tag["Bullet2"] = Bullet2;
            tag["StarBreker:StarCharge"] = StarCharge;
            base.SaveData(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            Bullet1 = tag.Get<Item>("Bullet1");
            Bullet2 = tag.Get<Item>("Bullet2");
            StarCharge = tag.GetInt("StarBreker:StarCharge");
            base.LoadData(tag);
        }
        public override void ResetEffects()
        {
            SummonStarWenpon = false;
            SwordSum = false;
            DrumDraw = false;
            FrostFistModScr = -1;
            if (SummonStarShieldTime > 0) SummonStarShieldTime--;
            if (PlayerVectorZero > 0)
            {
                PlayerVectorZero--;
            }
            if (EGO)
            {
                Player.whipRangeMultiplier += 2f;
            }
            EGO = false;
            if (PlayerEmotion < 0)
            {
                PlayerEmotion = 0;
            }
            if (PlayerEmotion < 50)
            {
                Player.ClearBuff(ModContent.BuffType<Buffs.CopperBuff>());
            }
            if (PlayerEmotion > 150)
            {
                PlayerEmotion = 150;
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (mediumCoreDeath)
            {
                return null;
            }
            return new[]
            {
                new Item(ModContent.ItemType<Items.Weapon.NoHardMode.StoneSword>())//塞入石剑
            };
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (DrumDraw)
            {
                Vector2 pos = Player.Center + new Vector2(30 * Player.direction, 0) - Main.screenPosition;
                if(Player.direction == 1)
                {
                    pos.Y -= 10;
                    pos.X -= 20;
                }
                else
                {
                    pos.Y -= 8;
                    pos.X -= 7;
                }
                Main.spriteBatch.Draw(TextureAssets.Item[Player.HeldItem.type].Value,
                    pos,
                    null,
                    Color.White);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            PlayerEmotion++;
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(3) == 0)
            {
                PlayerEmotion++;
            }
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            PlayerEmotion--;
            if (EGO) PlayerEmotion -= 20;
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (Main.rand.Next(3) == 0)
            {
                PlayerEmotion--;
                if (EGO) PlayerEmotion -= 20;
            }
        }
        public override void SetControls()
        {
            if (EGO)
            {
                PlayerVectorZero = 300;
                Player.moveSpeed += 5;
                Player.accRunSpeed += 5;
                Player.maxRunSpeed += 5;
                Player.meleeSpeed += 1.2f;
                Player.noFallDmg = true;
                Player.fallStart += 10;
                Player.maxFallSpeed += 10;
                Player.GetDamage(DamageClass.Melee) += 0.2f;
                if (Player.HeldItem.type == ModContent.ItemType<LastShortSowrd>()) Player.maxRunSpeed += 5;
            }

            else if (PlayerVectorZero > 0 && !EGO)
            {
                PlayerVectorZero--;
                Player.controlUp = false;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUseItem = false;
                Player.gravControl = false;
                Player.gravControl2 = false;
                Player.controlSmart = false;
                Player.controlThrow = false;
                Player.controlTorch = false;
                Player.controlQuickHeal = false;
                Player.controlQuickMana = false;
                Player.controlMount = false;
                Player.controlMap = false;
                Player.controlInv = false;
                Player.controlUseTile = false;
                Player.controlHook = false;
                Player.statDefense = 0;
                PlayerEmotion = 0;
                Main.mapFullscreen = false;
                Main.playerInventory = false;
            }
        }
        public override void PreUpdateMovement()
        {
            if (PlayerVectorZero > 0 && !EGO)
            {
                Player.velocity = Vector2.Zero;
                //Player.direction = Math.Sign(Main.screenPosition.X - Player.Center.X);
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (StarBreaker.ToggleGhostSwordAttack.JustPressed)
            {
                GhostSwordAttack++;
                if (GhostSwordAttack > 2) GhostSwordAttack = 0;
                else if (GhostSwordAttack < 0) GhostSwordAttack = 0;
            }
        }
        public override void PreUpdate()
        {
        }
        public override void PostUpdate()
        {

        }
        public override void OnEnterWorld(Player Player)
        {
            if (Bullet1 is null)
            {
                Bullet1 = new();
                Bullet1.SetDefaults(0);
            }
            if (Bullet2 is null)
            {
                Bullet2 = new();
                Bullet2.SetDefaults(0);
            }
            StarBreaker.Instantiate.starBreaker_UI.element1.Item = Bullet1;
            StarBreaker.Instantiate.starBreaker_UI.element2.Item = Bullet2;
            base.OnEnterWorld(Player);
        }
        public override void ModifyScreenPosition()
        {
            if (FrostFistModScr != -1)
            {
                Main.screenPosition = Main.npc[FrostFistModScr].position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            }
        }
    }
}
