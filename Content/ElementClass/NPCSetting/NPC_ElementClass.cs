using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.ElementClass.NPCSetting
{
    public class NPC_ElementClass
    {
        public List<BasicElementsClass> sourceElementDamage;
        /// <summary>
        /// 注意:负值加伤,正值减伤
        /// </summary>
        public Dictionary<BasicElementsClass, float> ElementDamageDef;
        public NPC_ElementClass()
        {
            sourceElementDamage = new();
            ElementDamageDef = new();
        }
        public void ModifyHit(ref NPC.HitModifiers modifiers)
        {
            if (sourceElementDamage == null) return;
            foreach(BasicElementsClass element in sourceElementDamage)
            {
                modifiers.SourceDamage += element.ElementAddDamage.Additive - 1f;
                if (ElementDamageDef.TryGetValue(element, out var value))
                {
                    modifiers.FinalDamage -= value;
                }
            }
        }
        public void OnHit(NPC npc)
        {
            foreach(BasicElementsClass element in sourceElementDamage)
            {
                element.OnHurt(npc);
            }
        }
    }
}
