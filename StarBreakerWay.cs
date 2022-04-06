using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarBreaker.Items;
using StarBreaker.Projs.Type;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarBreaker
{
    public class StarBreakerWay
    {
        public static void NPCDrawTail(NPC npc, Color drawColor, Color TailColor)
        {
            for (int j = npc.oldRot.Length - 1; j > 0; j--)
            {
                npc.oldRot[j] = npc.oldRot[j - 1];
            }
            npc.oldRot[0] = npc.rotation;
            Texture2D NPCTexture = TextureAssets.Npc[npc.type].Value;
            SpriteEffects spriteEffects = 0;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[npc.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2(TextureAssets.Npc[npc.type].Width() / 2, TextureAssets.Npc[npc.type].Height() / frameCount / 2);

            for (int i = 1; i < npc.oldPos.Length; i += 2)
            {
                Color color = Color.Lerp(drawColor, TailColor, 0.5f);
                color = npc.GetAlpha(color);
                color *= (npc.oldPos.Length - i) / 15f;
                Vector2 DrawPosition = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * npc.scale / 2f;
                DrawPosition += DrawOrigin * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(npc.frame), color, npc.oldRot[i], DrawOrigin, npc.scale, spriteEffects, 0f);
            }
        }
        /// <summary>
        /// 无帧图的绘制
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="drawColor"></param>
        /// <param name="TailColor"></param>
        public static void ProjDrawTail(Projectile projectile, Color drawColor, Color TailColor)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                Vector2 pos = projectile.oldPos[i] + (new Vector2(projectile.width, projectile.height) / 2) - Main.screenPosition;
                Vector2 origin = texture.Size() * 0.5f;
                Main.spriteBatch.Draw(texture, pos, null, Color.Lerp(drawColor, TailColor, i / projectile.oldPos.Length), projectile.rotation,
                    origin, projectile.scale, SpriteEffects.None, 0);
            }
        }
        public static void StarBrekaerUseBulletShoot(StarPlayer starPlayer, out int shootID, out int shootDamage, out EnergyBulletItem BulletPassive)
        {
            shootID = 0;
            shootDamage = 0;
            BulletPassive = null;

            if (starPlayer.Bullet1 != null && !starPlayer.Bullet1.IsAir)
            {
                shootID = starPlayer.Bullet1.shoot;
                shootDamage = starPlayer.Bullet1.damage;
                starPlayer.Bullet1.stack--;
                if (starPlayer.Bullet1.stack <= 0)
                {
                    starPlayer.Bullet1.TurnToAir();
                }

                if (starPlayer.Bullet2 != null && !starPlayer.Bullet2.IsAir)//被动子弹
                {
                    Item item = new(starPlayer.Bullet2.type, 1);
                    if (item.ModItem is EnergyBulletItem bullet)
                    {
                        BulletPassive = bullet;
                    }
                }
            }
            else if (starPlayer.Bullet2 != null && !starPlayer.Bullet2.IsAir)
            {
                shootID = starPlayer.Bullet2.shoot;
                shootDamage = starPlayer.Bullet1.damage;
                BulletPassive = null;
                starPlayer.Bullet2.stack--;
                if (starPlayer.Bullet2.stack <= 0)
                {
                    starPlayer.Bullet2.TurnToAir();
                }
            }
        }
        public static void PickAmmo_EnergyBulletItem(Player player,out int ShootItemID, out int shootDamage)
        {
            if(player.HasAmmo(player.HeldItem,true))
            {
                for(int i = 0;i<player.inventory.Length;i++)//遍历 背包
                {
                    Item item = player.inventory[i];
                    if(item.active && item.ammo == ModContent.ItemType<Items.Bullet.NebulaBulletItem>() && item.consumable && item.ModItem is EnergyBulletItem)
                    {
                        item.stack--;
                        if (item.stack <= 0) item.TurnToAir();
                        ShootItemID = item.type;
                        shootDamage = item.damage;
                        return;
                    }
                }
            }            
            ShootItemID = -1;
            shootDamage = 0;
        }
        public static void Add_Hooks_ToProj(EnergyBulletItem bulletItem, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && bulletItem != null)
            {
                AddHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        public static void Add_Hooks_ToProj(int useBulletID, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && new Item(useBulletID).ModItem is EnergyBulletItem bulletItem)
            {
                AddHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        public static void Del_Hooks_ToProj(EnergyBulletItem bulletItem, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && bulletItem != null)
            {
                DelHook(bulletItem,BulletProj,projWhoAmI);
            }
        }
        public static void Del_Hooks_ToProj(int useBulletID, int projWhoAmI)
        {
            if (Main.projectile[projWhoAmI].ModProjectile is EnergyProj BulletProj && new Item(useBulletID).ModItem is EnergyBulletItem bulletItem)
            {
                DelHook(bulletItem, BulletProj, projWhoAmI);
            }
        }
        private static void AddHook(EnergyBulletItem bulletItem,EnergyProj BulletProj,int projWhoAmI)
        {
            BulletProj.Proj_AI += bulletItem.ProjAI;
            BulletProj.Proj_Colliding += bulletItem.Colliding;
            BulletProj.Proj_Kill += bulletItem.Kill;
            BulletProj.Proj_OnHitNPC += bulletItem.ProjOnHitNPC;
            BulletProj.Proj_OnTileCollide += bulletItem.OnTileCollide;
            BulletProj.Proj_Draw += bulletItem.ProjPreDraw;
            BulletProj.Proj_AI_Minion += bulletItem.ProjAI_Minion;
            Main.projectile[projWhoAmI].damage += bulletItem.Item.damage;
        }
        private static void DelHook(EnergyBulletItem bulletItem, EnergyProj BulletProj, int projWhoAmI)
        {
            BulletProj.Proj_AI -= bulletItem.ProjAI;
            BulletProj.Proj_Colliding -= bulletItem.Colliding;
            BulletProj.Proj_Kill -= bulletItem.Kill;
            BulletProj.Proj_OnHitNPC -= bulletItem.ProjOnHitNPC;
            BulletProj.Proj_OnTileCollide -= bulletItem.OnTileCollide;
            BulletProj.Proj_Draw -= bulletItem.ProjPreDraw;
            BulletProj.Proj_AI_Minion -= bulletItem.ProjAI_Minion;
            Main.projectile[projWhoAmI].damage -= bulletItem.Item.damage;

        }
    }
    public static class StarBreakerWay_ByExtension
    {
        public static void NPC_AddOnHitDamage(this NPC npc,int damage,bool ByDefense = false)
        {
            int LastDamage = damage;
            if (ByDefense) LastDamage -= npc.defense;
            npc.life -= Math.Abs(LastDamage);
            npc.checkDead();
            if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, damage);
        }
        public static void SacrificeCountNeededByItemId(this Item item, int num) => CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[item.type] = num;
        public static Vector2 RealSafeNormalize(this Vector2 vector2)
        {
            Vector2 vector = Vector2.Normalize(vector2);
            if (vector.HasNaNs()) vector = Vector2.Zero;
            return vector;
        }
        public static NPC FindTargetNPC(this Player player,float maxDis = 800)
        {
            foreach(NPC npc in Main.npc)
            {
                float dis = Vector2.Distance(player.Center, npc.Center);
                if(npc.CanBeChasedBy() && npc.active && !npc.friendly && maxDis > dis)
                {
                    maxDis = dis;
                    player.MinionAttackTargetNPC = npc.whoAmI;
                }
            }
            if(player.HasMinionAttackTargetNPC)
            {
                return Main.npc[player.MinionAttackTargetNPC];
            }
            return null;
        }
    }
}
