using StarBreaker.Content;
using StarBreaker.Content.Appraise;
using StarBreaker.Content.Component;
using StarBreaker.Content.Component.ComponentPlayer;
using StarBreaker.Content.ControlPlayerSystem;
using StarBreaker.Content.Mission;
using StarBreaker.Content.PlayerControl;
using StarBreaker.Content.TheSkillProj;
using System.Linq;
using Terraria;
using Terraria.GameInput;

namespace StarBreaker
{
    public class StarBreakerPlayer : ModPlayer, IAppraiseEntity
    {
        public StarBreakerPlayer()
        {
            playerOldControlSystem = new();
            Component = new();
        }
        public const int Down = 0;
        public const int Up = 1;
        public const int Right = 2;
        public const int Left = 3;

        public BasicControlPlayerSystem playerSystem;
        public PlayerOldControlSystem playerOldControlSystem;

        public int ControlDir = -1;
        public int MoveDir = 0;
        public int DoubleControlDirTime;
        /// <summary>
        /// 这个是判定魔力需要多少才能进入Devil Trigger (我怎么天天打Devil Trigger了淦)
        /// </summary>
        public int MagicConsumptionCount;
        /// <summary>
        ///  没错,这是 "Devil Trigger"，当然可以用作别的
        /// </summary>
        public bool MagicConsumption;
        public bool InAttack;
        public bool OldInAttack;
        public bool InAir;
        public bool OldInAir;
        public bool LeftMouse;
        public bool RightMouse;
        /// <summary>
        /// 星击的预操作系统
        /// </summary>
        public bool PreControlAction;
        public bool Pre_LeftMouse;
        public bool Pre_RightMouse;
        /// <summary>
        /// 接触地面
        /// </summary>
        public bool ContactGround => InAir != OldInAir && OldInAir;
        /// <summary>
        /// 离开地面
        /// </summary>
        public bool LeaveGround => InAir != OldInAir && InAir;
        public bool StopVel;
        /// <summary>
        /// 后到前运动判断
        /// </summary>
        public bool Move_BackToFront => GetMoveDirection(playerOldControlSystem,Player.direction);
        /// <summary>
        /// 前到后运动判断
        /// </summary>
        public bool Move_FrontToBack => GetMoveDirection(playerOldControlSystem, -Player.direction);


        public object[] UseAttack { get => useAttack; set => useAttack = value; }

        public float DamageFactor;
        private object[] useAttack;
        public Dictionary<Type, BasicPlayerComponent> Component;
        /// <summary>
        /// 获取玩家对应朝向的移动
        /// </summary>
        /// <returns></returns>
        private static bool GetMoveDirection(PlayerOldControlSystem init, int dir)
        {
            StarBreakerPlayerControl frist = init.oldControls[0];
            StarBreakerPlayerControl next = init.oldControls[1];

            StarBreakerPlayerControl.ControlType FristType =
                dir == 1 ? StarBreakerPlayerControl.ControlType.Left : StarBreakerPlayerControl.ControlType.Right;
            StarBreakerPlayerControl.ControlType NextType =
                dir == -1 ? StarBreakerPlayerControl.ControlType.Left : StarBreakerPlayerControl.ControlType.Right;
            if (frist.Time < 8 && frist.Type == FristType)
            {
                if (next.Time < 10 && next.Type == NextType)
                {
                    return true;
                }
                else if (next.Type == StarBreakerPlayerControl.ControlType.None && next.Time < 10)
                {
                    next = init.oldControls[2];
                    if (next.Time < 10 && next.Type == NextType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnEnterWorld()
        {
            playerOldControlSystem = new(); // 加载一下
            _ = AppraiseSystem.Instance;
            _ = MissionSystem.Instance;
            AppraiseSystem.Instance.Load(Player, this);
            #region 添加各类组件
            Component.TryAdd(typeof(PlayerChickControl), new PlayerChickControl(this));
            #endregion
        }
        public override void ResetEffects()
        {
            DamageFactor = 0.5f; // 重置伤害加成
            Main.mapEnabled = false; // 取消地图启用
            Main.mapFullscreen = false; // 不让地图全屏
            Main.autoSave = false; // 禁止自动保存
            if(Main.myPlayer == Player.whoAmI)
            {
                playerSystem = StarBreakerSystem.playerSystem;
            }
            Player.statLifeMax2 = 1000;
            if (Player.statLife < Player.statLifeMax2) Player.statLife++;
            Player.statManaMax2 = 300;
            Player.jumpSpeed = 10;
            Player.maxFallSpeed = 8;

            Component.ForEachValue((x) => x.ResetEffects());
        }
        public override void PreUpdate()
        {
            List<BasicPlayerComponent> components = new(Component.Values);
            for(int i = 0; i < components.Count; i++)
            {
                (components[i] as IStarBreakerComponent).RemoveUpdate(Component);
            }
            Component.ForEachValue((x) => x.PreUpdate());
        }
        public override void Unload()
        {
            Type[] types = Mod.Code.GetTypes();
            foreach (Type type in types)
            {
                if (type.GetInterfaces().Contains(typeof(IUnLoad)) && type.IsClass)
                {
                    //IUnLoad unLoad = Activator.CreateInstance(type) as IUnLoad;
                    //unLoad.UnLoad();
                }
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {

        }
        public override void SetControls()
        {
            PlayerOldControlSave();
            ControlPlayerSystem();
            OldInAttack = InAttack; // 保存旧的攻击
            PlayerDoubleUseUpdate();
            PreControlActionUpdate(); 
            if (Component.TryGetValue(typeof(PlayerChickControl),out var p))
            {
                var playerChick = p as PlayerChickControl;
                LeftMouse = playerChick.ChickLeft || (OldInAttack && Pre_LeftMouse);
                RightMouse = playerChick.ChickRigth || (OldInAttack && Pre_RightMouse);
            }
            #region 操作控制
            if (ControlDir == Left)
            {
                MoveDir = -1;
            }
            else if (ControlDir == Right)
            {
                MoveDir = 1;
            }
            else
            {
                MoveDir = 0;
            }
            MoveDir *= Player.direction;
            #endregion
            Component.ForEachValue(x => x.SetControls());
        }
        /// <summary>
        ///  预操作判定函数
        /// </summary>
        private void PreControlActionUpdate()
        {
            if (InAttack)
            {
                PlayerChickControl playerChick = Component[typeof(PlayerChickControl)] as PlayerChickControl;
                if (PreControlAction) // 预定操作系统
                {
                    PreControlAction = false;
                    if (!Pre_LeftMouse)
                    {
                        Pre_LeftMouse = playerChick.ChickLeft;
                    }

                    if (!Pre_RightMouse)
                    {
                        Pre_RightMouse = playerChick.ChickRigth;
                    }
                }
                Player.controlDown = Player.controlLeft = Player.controlRight = Player.controlUp = false;
                Player.controlJump = false;
            }
            else if (!OldInAttack)
            {
                Pre_LeftMouse = Pre_RightMouse = false; // 取消预操作判定
            }
        }
        /// <summary>
        /// 玩家双击判断函数
        /// </summary>
        private void PlayerDoubleUseUpdate()
        {

            if (Player.controlDown && DoubleCheck(this, StarBreakerPlayerControl.ControlType.Down))
            {
                ControlDir = Down;
                DoubleControlDirTime = 0;
            }
            else if (Player.controlUp && DoubleCheck(this, StarBreakerPlayerControl.ControlType.Up))
            {
                ControlDir = Up;
                DoubleControlDirTime = 0;
            }
            else if (Player.controlRight && DoubleCheck(this, StarBreakerPlayerControl.ControlType.Right))
            {
                ControlDir = Right;
                DoubleControlDirTime = 0;
            }
            else if (Player.controlLeft && DoubleCheck(this, StarBreakerPlayerControl.ControlType.Left))
            {
                ControlDir = Left;
                DoubleControlDirTime = 0;
            }
            else
            {
                if (DoubleControlDirTime > 15)
                {
                    ControlDir = -1;
                }
                else
                {
                    DoubleControlDirTime++;
                }
            }

            static bool DoubleCheck(StarBreakerPlayer player, StarBreakerPlayerControl.ControlType type)
            {
                StarBreakerPlayerControl controlType = player.playerOldControlSystem.oldControls[0];
                StarBreakerPlayerControl controlType1 = player.playerOldControlSystem.oldControls[2];
                return controlType?.Type == type && controlType1?.Type == type && controlType.Time < 8 && controlType1.Time < 8;
            }
        }
        /// <summary>
        /// 保存玩家旧按键使用
        /// </summary>
        private void PlayerOldControlSave()
        {
            PlayerOldControlSystem playerControlSystem = playerOldControlSystem;
            if (Player.controlDown)
            {
                playerControlSystem.UpdateControl(StarBreakerPlayerControl.ControlType.Down);
            }
            else if (Player.controlUp)
            {
                playerControlSystem.UpdateControl(StarBreakerPlayerControl.ControlType.Up);
            }
            else if (Player.controlLeft)
            {
                playerControlSystem.UpdateControl(StarBreakerPlayerControl.ControlType.Left);
            }
            else if (Player.controlRight)
            {
                playerControlSystem.UpdateControl(StarBreakerPlayerControl.ControlType.Right);
            }
            else
            {
                playerControlSystem.UpdateControl(StarBreakerPlayerControl.ControlType.None);
            }
        }
        private void ControlPlayerSystem()
        {
            BasicControlPlayerSystem.ReleaseChangeRightWeapon =
                ControlPlayerSystem_KeySave(ref BasicControlPlayerSystem.ControlChangeRightWeapon, Player.controlHook); // 默认为钩子的使用

            BasicControlPlayerSystem.ReleaseChangeLeftWeapon =
                            ControlPlayerSystem_KeySave(ref BasicControlPlayerSystem.ControlChangeLeftWeapon, Player.controlMount); // 默认为坐骑

            BasicControlPlayerSystem.ReleaseChangeAuxiliary = ControlPlayerSystem_KeySave(ref BasicControlPlayerSystem.ControlChangeAuxiliary, Player.controlQuickHeal); // 默认为回血

            BasicControlPlayerSystem.ReleaseTaunt = ControlPlayerSystem_KeySave(ref BasicControlPlayerSystem.ControlTaunt,Player.controlMap); // 默认为打开地图
        }
        private static bool ControlPlayerSystem_KeySave(ref bool controlKey,bool vanillaKey)
        {
            bool flag = controlKey;
            controlKey = vanillaKey;
            return flag;
        }
        public override void PreUpdateMovement()
        {
            if (StopVel)
            {
                StopVel = false;
                Player.velocity *= 0;
            }
        }
        public override void PostUpdate()
        {
            OldInAir = InAir;
            Point center = (Player.Bottom / 16).ToPoint();
            for (int i = 0; i < 3; i++)
            {
                var pos = center;
                pos.Y += i;
                InAir = !Main.tile[pos].HasTile;
                if (!InAir)
                {
                    break;
                }
            }

            #region DevilTrigger的bool判定
            if(Player.statMana <= 0) // 如果魔力消耗殆尽
            {
                MagicConsumption = false; //则使这个为false,退出Devil Trigger(bushi)魔力消耗状态
            }
            else
            {
                bool flag = BasicControlPlayerSystem.ReleaseTransformation && BasicControlPlayerSystem.ControlTransformation;
                if (Player.statMana >= MagicConsumptionCount && flag)
                {
                    MagicConsumption = true;
                    playerSystem.OpenMagicConsumption();
                }
                if(MagicConsumption && flag)
                {
                    MagicConsumption = false;
                    playerSystem.CloseMagicConsumption();
                }
            }
            #endregion
            Component.ForEachValue(x => x.PostUpdate());
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach(var c in Component)
            {
                c.Value.ModifyHitNPCWithProj(proj, target, ref modifiers);
            }
        }
        #region 命中东西的判定
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            AddAttack(item);
            AppraiseSystem.Instance.OnHit(this, target, hit.Damage);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.ModProjectile is IBasicSkillProj skill)
            {
                AddAttack(skill.CurrentSkill);
            }
            if (proj.owner >= 0 && proj.owner != 255 && proj.friendly)
            {
                AppraiseSystem.Instance.OnHit(this, target, hit.Damage);
            }
            Component.ForEachValue(x => x.OnHitNPCWithProj(proj,target,hit,damageDone));
        }
        //public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        //{
        //    AddAttack(item);
        //    AppraiseSystem.Instance.OnHit(this, target, modifiers.GetDamage(Player.GetWeaponDamage(item),false));
        //}
        //public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        //{
        //    if (proj.ModProjectile is not SkillProj)
        //    {
        //        AddAttack(proj.Name);
        //    }
        //    if (proj.owner >= 0 && proj.owner != 255 && proj.friendly)
        //    {
        //        AppraiseSystem.Instance.OnHit(this, target, damage);
        //    }
        //}
        #endregion
        #region 被命中判定

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            AppraiseSystem.Instance.OnHurt(this, npc, hurtInfo.Damage);
            Component.ForEachValue(x => x.OnHitByNPC(npc,hurtInfo));
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            AppraiseSystem.Instance.OnHurt(this, proj, hurtInfo.Damage);
            Component.ForEachValue(x => x.OnHitByProjectile(proj, hurtInfo));
        }
        //public override void OnHitByNPC(NPC npc, int damage, bool crit)
        //{
        //    AppraiseSystem.Instance.OnHurt(this, npc, damage);
        //}
        //public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        //{
        //    AppraiseSystem.Instance.OnHurt(this, proj, damage);
        //}
        #endregion
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            foreach (var pair in Component.Values)
            {
                pair.ModifyDrawInfo(ref drawInfo);
            }
        }
        public void AddAttack(object attackID)
        {
            useAttack ??= new object[10];
            if (useAttack[0] != null && useAttack[0].Equals(attackID))
            {
                DamageFactor = 0f;
                return;
            } // 如果命中ID一致
            for (int i = useAttack.Length - 1; i > 0; i--)
            {
                useAttack[i] = useAttack[i - 1];
            }
            useAttack[0] = attackID; // 0存着现在命中的技能
        }
        public float OnHit(Entity target, int damage)
        {
            if (useAttack == null) return 0f;
            for (int i = 1; i < useAttack.Length; i++)
            {
                if (useAttack[0].Equals(useAttack[i]))
                {
                    return 0f;
                }
            }
            return damage * DamageFactor;
        }
        public float OnHurt(Entity target, int damage)
        {
            return damage;
        }

        public void Draw(float progress, AppraiseID id) // 评价的绘制
        {
            string drawFont = StarBreakerUtils.GetAppraiseDrawFont(id);
            Color color = StarBreakerUtils.GetAppraiseDrawColor(id);
            Rectangle drawRect = StarBreakerUtils.GetApprasieDrawRect(id);
            if (drawFont != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone,
                    null);

                Vector2 position = new(Main.screenWidth - 120, (Main.screenHeight / 2) - 180);
                Vector2 origin = drawRect.Size() * 0.5f;

                spriteBatch.Draw(StarBreakerAssetHelper.AppraiseTex.Value, position, drawRect, color,
                    0f, origin, 1.4f, SpriteEffects.None, 0f);
                int height = drawRect.Height;
                drawRect.Height = (int)(drawRect.Height * progress);
                //drawRect.Y = height - drawRect.Height;
                spriteBatch.Draw(StarBreakerAssetHelper.AppraiseTex.Value, position, drawRect, Color.Black * 0.8f,
                    0f, origin, 1.4f, SpriteEffects.None, 0f);

                spriteBatch.End();
            }
        }
    }
}
