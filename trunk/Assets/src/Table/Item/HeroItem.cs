using System;

namespace TableItem
{
    public class Hero : IEntityConfig
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public SlotSkillItem[] Skill { get; set; } // 普通技能ID
        public string Prefab { get; set; }
    }
}