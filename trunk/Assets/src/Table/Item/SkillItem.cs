using System;

namespace TableItem {
    public enum SkillType {
        单体 = 1,
	    扇形,
	    圆形,
	    矩形
    }

    public enum DamageType {
        物理,
	    法术,
	    真伤
    }

    public enum SkillCostType {
        生命,
        魔法,
        体力,
        能量
    }

    public class SlotSkillItem {
        public int Slot;
        public int[] SkillIds;
    }

    public class Skill
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public SkillType SType { get; set; }  // 技能类型
        public DamageType DType { get; set; }  // 伤害类型
        public float CD { get; set; }
        public SkillCostType CostType { get; set; }  // 消耗类型
        public int Cost { get; set; }  // 消耗

        public float Range { get; set; }  // 范围
        public float Radius { get; set; }  // 半径
        public float Angle { get; set; }  // 角度
        public float CastTime { get; set; }  // 施法前摇时间
        public string CastAnimation { get; set; }  // 施法前摇动画名称

        public string CastEffect { get; set; }
        public string HitEffect { get; set; }
        public float BaseDamage { get; set; }  // 基础伤害
        public int AT { get; set; }  // 攻击
        public int DT { get; set; }  // 防御

        public int KT { get; set; }  // 斩杀

        public int MHPT { get; set; }  // 生命上限
        public int HPT { get; set; }  // 生命
    }
}