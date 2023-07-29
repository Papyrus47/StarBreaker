using StarBreaker.Content.Component;
using StarBreaker.Content.Component.ComponentNPC;
using StarBreaker.Content.ElementClass;
using StarBreaker.NPCs;
using System.Reflection;

namespace StarBreaker
{
    public static class StarBreakerUtils_ByExtension
    {
        public static Vector2 RealSafeNormalize(this Vector2 vector2, Vector2 SafeVector = default)
        {
            if (vector2.Length() == 0)
            {
                if (SafeVector == default)
                {
                    SafeVector = Vector2.UnitX;
                }

                vector2 = SafeVector;
            }
            vector2 *= 1 / vector2.Length();

            return vector2;
        }
        public static Vector2 AbsVector2(this Vector2 vector2)
        {
            float x = Math.Abs(vector2.X);
            float y = Math.Abs(vector2.Y);
            return new Vector2(x, y);
        }
        public static NPC FindTargetNPC(this Player player, float maxDis = 800)
        {
            foreach (NPC npc in Main.npc)
            {
                float dis = Vector2.Distance(player.Center, npc.Center);
                if (npc.CanBeChasedBy() && npc.active && !npc.friendly && maxDis > dis)
                {
                    maxDis = dis;
                    player.MinionAttackTargetNPC = npc.whoAmI;
                }
            }
            if (player.HasMinionAttackTargetNPC)
            {
                return Main.npc[player.MinionAttackTargetNPC];
            }
            return null;
        }
        public static Vector2 NormalVector(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        /// <summary>
        /// 通过Z轴,将三维空间的点投影到二维空间之上
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="ProjectionZ">一个巨大的缩放率</param>
        /// <returns></returns>
        public static Vector2 Vector3ProjectionToVectoer2(this Vector3 vector3, float ProjectionZ)
        {
            return ProjectionZ / (vector3.Z - ProjectionZ) * new Vector2(vector3.X, vector3.Y);
        }
        public static T DeepClone<T>(this T obj)
        {
            if (obj is string || obj is null || obj.GetType().IsValueType)
            {
                return obj;
            }

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(retval, DeepClone(field.GetValue(obj)));
                }
                catch { }
            }

            return (T)retval;
        }
        /// <summary>
        /// 获取玩家身上有的物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Item GetHasItem(this Player player, int type)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == type)
                {
                    return player.inventory[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 获取玩家身上的物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <param name="GetCount">数量</param>
        /// <returns></returns>
        public static Item[] GetHasItem(this Player player, int type, int GetCount)
        {
            Item[] items = new Item[GetCount];
            int j = 0;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == type)
                {
                    if (j++ < GetCount)
                    {
                        items[j] = player.inventory[i];
                    }
                }
            }
            if (j != 0)
            {
                return items;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取玩家身上的物品
        /// </summary>
        /// <typeparam name="T">ModItem类型</typeparam>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Item GetHasItem<T>(this Player player) where T : ModItem
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is T)
                {
                    return player.inventory[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 获取玩家身上的物品
        /// </summary>
        /// <typeparam name="T">ModItem类型</typeparam>
        /// <param name="player"></param>
        /// <param name="GetCount">数量</param>
        /// <returns></returns>
        public static Item[] GetHasItem<T>(this Player player, int GetCount) where T : ModItem
        {
            Item[] items = new Item[GetCount];
            int j = 0;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is T)
                {
                    if (j++ < GetCount)
                    {
                        items[j - 1] = player.inventory[i];
                    }
                }
            }
            if (j != 0)
            {
                return items;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 减少物品数量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="num">数量</param>
        public static void ItemStackDeduct(this Item item, int num = 1)
        {
            item.stack -= num;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }
        }
        public static SpriteEffects SpriteDirToEffects(this Projectile projectile)
        {
            return projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public static SpriteEffects SpriteDirToEffects(this NPC npc)
        {
            return npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        public static void ForEachValue<T,T1>(this Dictionary<T,T1> pairs,Action<T1> action)
        {
            foreach (var pair in pairs)
            {
                action(pair.Value);
            }
        }
        public static StarBreakerPlayer StarBreaker(this Player player) => player.GetModPlayer<StarBreakerPlayer>();
        public static StarBreaker_GlobalProj StarBreaker(this Projectile projectile) => projectile.GetGlobalProjectile<StarBreaker_GlobalProj>();
        public static StarBreakerGlobalNPC StarBreaker(this NPC npc) => npc.GetGlobalNPC<StarBreakerGlobalNPC>();
        public static void AddCompoent(this NPC npc,BasicComponentNPC component)
        {
            if(!npc.StarBreaker().Compontent.TryAdd(component.GetType(), component))
            {
                npc.StarBreaker().Compontent[component.GetType()] = component;
            }
        }
        public static void AddElementClass(this NPC npc, BasicElementsClass basicElementsClass) => npc.StarBreaker().elementClass.sourceElementDamage.Add(basicElementsClass);
    }
}
