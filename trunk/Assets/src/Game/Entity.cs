using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Hero = 1, // 英雄
    Monster = 2, // 怪物
    NPC = 3, // NPC
}

public class Entity
{
    public int id; // 实体ID
    public string name; // 实体名称

    public EntityAttribute Attrib; // 实体属性

    public Vector3 bron = new Vector3(0, 0, 0); // 实体出生位置
    // public Vector3 position = new Vector3(0, 5, 0); // 实体位置

    private Skill skill;
    public Entity(IEntityConfig config)
    {
        skill = new Skill(this);
        for(int i = 0; i < config.Skill.Length; i++)
        {
            skill.SkillLearn(config.Skill[i].Slot, config.Skill[i].SkillIds, 1);
        }
        Attrib = new EntityAttribute();
        Attrib.hp = Attrib.maxHp;
        Attrib.mp = Attrib.maxMp;
    }

    public float moveSpeed
    {
        get { return Attrib != null ? Attrib.moveSpeed : 5f; } // 假设 EntityAttribute 有 MoveSpeed 属性
    } // 移动速度

    public float rotationSpeed
    {
        get { return Attrib != null ? Attrib.rotationSpeed : 500f; } // 假设 EntityAttribute 有 RotationSpeed 属性
    } // 旋转速度

    public float attackCD
    {
        get { return Attrib != null ? Attrib.attackCD : 0.8f; } // 假设 EntityAttribute 有 AttackCD 属性
    } // 攻击冷却时间

    public float attackSpeed
    {
        get { return Attrib != null ? Attrib.attackSpeed : 1.0f; } // 假设 EntityAttribute 有 AttackSpeed 属性
    } // 攻击速度

    public bool TryCastSkill(int slot)
    { // 尝试施法
        return skill.TryCastSkill(slot);
    }

    public SkillSlot GetSkillSlot(int slot)
    {
        return skill.GetSkillSlot(slot);
    }

    public SkillSlot[] GetSkillSlots()
    {
        return skill.skillSlots;
    }

    public bool IsCasting
    {
        get { return skill.isCasting; } // 是否正在施法
    }

    public void Update(float deltaTime)
    {
        // 更新实体状态
        // 这里可以添加其他需要的更新逻辑
        skill.Update(deltaTime); // 更新技能状态
    }
}
