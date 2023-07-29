using StarBreaker.Content.ElementClass;
using Terraria.GameInput;
using Terraria.UI;

namespace StarBreaker.Content.ControlPlayerSystem
{
    public abstract class BasicControlPlayerSystem
    {
        public static Player ControlPlayer;
        public static bool ControlSpecialKey;
        public static bool ReleaseSpecialKey;
        public static bool ControlChangeRightWeapon;
        public static bool ReleaseChangeRightWeapon;
        public static bool ControlChangeLeftWeapon;
        public static bool ReleaseChangeLeftWeapon;
        /// <summary>
        /// 嘲讽
        /// </summary>
        public static bool ControlTaunt;
        public static bool ReleaseTaunt;
        /// <summary>
        /// 魔力消耗状态
        /// </summary>
        public static bool ControlTransformation;
        public static bool ReleaseTransformation;
        /// <summary>
        /// 星辰之主更改辅助栏
        /// </summary>
        public static bool ControlChangeAuxiliary;
        public static bool ReleaseChangeAuxiliary;
        public static bool In_Auto_lock;
        public UIState PlayerSystemControlUI;
        public UserInterface userInterface;
        ///// <summary>
        ///// 加1次 给魔力增加100点
        ///// </summary>
        //public int ManaAdd;
        ///// <summary>
        ///// 加1次 给血条增加100点
        ///// </summary>
        //public int LifeAdd;
        public BasicControlPlayerSystem(Player player)
        {
            ControlPlayer = player;
            userInterface = new();
            Init();
        }
        public void InitUI(UIState ControlUI)
        {
            PlayerSystemControlUI = ControlUI;
            ControlUI.Initialize();
            userInterface.SetState(ControlUI);
        }
        public static void LoadStatic()
        {

        }
        public virtual void ModifyHitNPC(NPC target,List<BasicElementsClass> basicElementsClasses,ref NPC.HitModifiers hitModifiers)
        {

        }
        public virtual void OpenMagicConsumption()
        {

        }
        public virtual void CloseMagicConsumption()
        {

        }
        public virtual void Init() { }
        public virtual void PlayerUpdate() { }
        public virtual void PlayerOnHitNPC(NPC.HitInfo hitInfo,int damageDone) { }
        public virtual void UpdateUI(GameTime time) => userInterface.Update(time);
        public virtual void DrawUI(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name.Equals("Vanilla: Resource Bars"));
            if (index > -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("StarBreaker : Player Control Bars", () =>
                {
                    userInterface.Draw(Main.spriteBatch, new());
                    return true;
                },InterfaceScaleType.UI));
            }
        }
    }
}
