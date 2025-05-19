using System;

public class EntityAttribute
{
    public float moveSpeed = 20f; // 移动速度
    public float rotationSpeed = 1000f; // 旋转速度

    public float attackCD = 0.8f; // 攻击冷却时间

    public float attackSpeed = 1.0f; // 攻击速度

    public float skillCDSpeed = 1.0f; // 技能冷却时间

    public float hp = 100f; // 生命值
    public float maxHp = 100f; // 最大生命值

    public float mp = 100f; // 魔法值
    public float maxMp = 100f; // 最大魔法值

    public float armor = 0f; // 护甲

    public float atk = 10f; // 攻击力

    public float def = 0f; // 防御力

    public float defBreak = 0f; // 减防
    public float defBreakRate = 0f; // 减防率

    public float penetration = 0f; // 穿透
    public float penetrationRate = 0f; // 穿透率
}
