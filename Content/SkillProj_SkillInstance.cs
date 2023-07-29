using static StarBreaker.Content.SkillProj;

namespace StarBreaker.Content
{
    /// <summary>
    /// 用于保存所有的技能函数实例
    /// </summary>
    public class SkillProj_SkillInstance
    {
        public string ID;
        public List<SkillProj_SkillListElement> SonSkills = null;
        public List<Func<bool, bool, bool, string>> ChangeConditions = null;
        public Skill ai_skill;
        public Skill_Draw draw_skill;
        public Skill_OnHitNPC onHitNPC_skill;
        public Skill_ModifyHitNPC modifyHitNPC_skill;

        public SkillProj_SkillInstance(Skill ai_skill, Skill_Draw draw_skill = null, Skill_OnHitNPC onHitNPC_skill = null,
            Skill_ModifyHitNPC modifyHitNPC_skill = null)
        {
            this.ai_skill = ai_skill;
            this.draw_skill = draw_skill;
            this.onHitNPC_skill = onHitNPC_skill;
            this.modifyHitNPC_skill = modifyHitNPC_skill;
        }

        public void Invoke()
        {
            ai_skill?.Invoke();
        }

        public bool? Invoke(Color color)
        {
            return draw_skill?.Invoke(color);
        }

        public void Invoke(NPC target, NPC.HitInfo hit, int damageDone)
        {
            onHitNPC_skill?.Invoke(target,hit,damageDone);
        }

        public void Invoke(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifyHitNPC_skill?.Invoke(target, ref modifiers);
        }

        public SkillProj_SkillInstance To(SkillProj_SkillInstance skill, Func<bool> func = null)
        {
            SonSkills ??= new();
            ChangeConditions ??= new();
            if (SonSkills.Find(x => x.skillProj_Skill.Equals(skill) && !x.isWait) != null)
            {
                return skill;
            }

            SonSkills.Add(new SkillProj_SkillListElement()
            {
                skillProj_Skill = skill,
                isWait = false
            }); // 添加已经添加过的技能,避免重复出现

            ChangeConditions.Add((isControl, inAttack, isWait) =>
            {
                if (isControl && !inAttack && !isWait && (func == null || func.Invoke()))
                {
                    return skill.ID;
                }
                return null;
            }); // 添加委托
            return skill;
        }
        public SkillProj_SkillInstance Wait(SkillProj_SkillInstance skill, Func<bool> func = null)
        {
            SonSkills ??= new();
            ChangeConditions ??= new();
            if (SonSkills.Find(x => x.skillProj_Skill.Equals(skill) && x.isWait) != null)
            {
                return skill;
            }

            SonSkills.Add(new SkillProj_SkillListElement()
            {
                skillProj_Skill = skill,
                isWait = true
            }); // 添加已经添加过的技能,避免重复出现

            ChangeConditions.Add((isControl, inAttack, isWait) =>
            {
                if (isControl && !inAttack && isWait && (func == null || func.Invoke()))
                {
                    return skill.ID;
                }
                return null;
            }); // 添加委托
            return skill;
        }
        public string TryChangeSkill(bool isControl, bool inAttack, bool isWait)
        {
            string ID = null;
            if (ChangeConditions != null)
            {
                foreach (var func in ChangeConditions)
                {
                    if (func != null)
                    {
                        ID = func.Invoke(isControl, inAttack, isWait);
                        if (ID != null)
                        {
                            break;
                        }
                    }
                }
            }
            return ID;
        }
    }
}

