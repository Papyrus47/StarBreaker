namespace StarBreaker.Content
{
    public abstract partial class SkillProj : ModProjectile
    {
        public delegate void Skill();
        public delegate bool Skill_Draw(Color lightColor);
        public delegate void Skill_OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone);
        public delegate void Skill_ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers);
        private List<SkillProj_SkillInstance> _skills;
        private Dictionary<string, int> _skillsDict;
        private bool _init;
        /// <summary>
        /// 用于记录之前所有的攻击ID
        /// </summary>
        public List<string> oldSkillsID;
        public int State { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float Timer { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        /// <summary>
        /// 处于等待-输入指令,用于切换到部分等待Combo
        /// </summary>
        public bool WaitControl;
        /// <summary>
        /// 用于判断是否处于攻击状态
        /// </summary>
        public bool WeaponInAttack;
        /// <summary>
        /// 用于判断玩家是否操控
        /// </summary>
        public bool ControlAttack;
        public sealed override void AI()
        {
            if (!_init)
            {
                _init = true;
                _skills = new();
                _skillsDict = new();
                Init();
                oldSkillsID = new(10);
                _skills.TrimExcess();
                _skillsDict.TrimExcess();
                Init_SkillChange();
            }
            PreSkillAI();
            _skills[State].Invoke(); // 调用AI

            #region 技能表的自动切换部分
            string str = _skills[State].TryChangeSkill(ControlAttack, WeaponInAttack, WaitControl);
            if (str != null)
            {
                //Main.NewText(_skills[State].ID);
                ChangeSkill(str);
                Timer = 0;
                //Main.NewText(_skills[State].ID);
            }
            #endregion

            PostSkillAI();
            if (WaitControl)
            {
                WaitControl = false; // 更新它
            }

            if (ControlAttack)
            {
                ControlAttack = false;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return SkillDraw(lightColor);
        }
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            _skills[State]?.Invoke(target,hit,damageDone);
            PostOnHitNPC();
        }
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            _skills[State]?.Invoke(target, ref modifiers);
            PostModifyHitNPC();
        }

        public virtual void PreSkillAI() { }
        public virtual void PostSkillAI() { }
        public virtual void PostOnHitNPC() { }
        public virtual void PostModifyHitNPC() { }
        public bool InSkill(params string[] SkillID)
        {
            for (int i = 0; i < SkillID.Length; i++)
            {
                if (_skillsDict.TryGetValue(SkillID[i], out int j) && j == State)
                {
                    return true;
                }
            }
            return false;
        }
        public bool SkillDraw(Color lightColor)
        {
            bool? flag = _skills[State].Invoke(lightColor);
            return !flag.HasValue || flag.Value;
        }
        /// <summary>
        /// 切换ai
        /// </summary>
        /// <param name="skill">技能ID</param>
        /// <param name="reset_oldSkillID">重置保存的技能表</param>
        public void ChangeSkill(string skill, bool reset_oldSkillID = false)
        {
            if (reset_oldSkillID)//删除经历过的技能表
            {
                oldSkillsID.TrimExcess();
                oldSkillsID.Clear();
            }
            else //添加旧技能表
            {
                oldSkillsID.Add(skill);
            }

            if (_skillsDict.TryGetValue(skill, out int i))
            {
                State = i;
            }
            OnChangeSkill();
        }
        /// <summary>
        /// 技能发生改变时调用这个函数
        /// </summary>
        public virtual void OnChangeSkill() { }
        public void AddSkill(string skillID, SkillProj_SkillInstance skillProj_Skill)
        {
            _skillsDict.Add(skillID, _skills.Count);
            skillProj_Skill.ID = skillID; // 添加技能的ID
            _skills.Add(skillProj_Skill);
        }
        public SkillProj_SkillInstance GetSkill(string skillID)
        {
            if (_skillsDict.TryGetValue(skillID, out int i))
            {
                return _skills[i];
            }

            return null;
        }
        public abstract void Init();
        public abstract void Init_SkillChange();
    }
}
