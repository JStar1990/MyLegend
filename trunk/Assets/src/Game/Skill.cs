using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    Entity entity;
    public Skill(Entity entity) {
        this.entity = entity; 
    }
    public SkillSlot[] skillSlots = new SkillSlot[10]; // 技能槽列表
    // Start is called before the first frame update

    private CDParams communalCd = new CDParams();

    private float castTime = 0f; // 施法时间
    // Update is called once per frame
    public void Update(float deltaTime)
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            SkillSlot skillSlot = skillSlots[i];
            if (skillSlot != null)
            {
                skillSlot.Update(deltaTime);
            }
        }
        // if(communalCd > 0) {
        //     communalCd -= deltaTime;
        //     if(communalCd <= 0) {
        //         communalCd = 0;
        //     }
        // }
        if(castTime > 0) {
            castTime -= deltaTime;
            if(castTime <= 0) {
                castTime = 0;
            }
        }
    }

    public bool isCasting {
        get { return castTime > 0; }
    }

    public void SkillLearn(int slot, int[] skillId, int level) {
        if (slot < 0 || slot >= skillSlots.Length)
        {
            Debug.LogError("技能槽索引超出范围");
            return;
        }

        SkillSlot skillSlot = skillSlots[slot];
        if (skillSlot == null)
        {
            skillSlot = new SkillSlot(slot);
            skillSlots[slot] = skillSlot;
        }

        // 假设 SkillSlot 有一个方法可以学习技能
        skillSlot.SkillLearn(skillId, level);
    }

    public bool TryCastSkill (int slot) {
        if (slot < 0 || slot >= skillSlots.Length) {
            Debug.LogError("技能槽索引超出范围");
            return false; 
        }
        SkillSlot skillSlot = skillSlots[slot];
        if (skillSlot == null) {
            Debug.LogError("该技能槽未学习任何技能");
            return false;
        }

        if(communalCd.isCD) {
            Debug.Log("技能公共冷却中，无法施放");
            return false;
        }

        if(!checkCost(skillSlot)){
            Debug.Log("技能能量不足，无法施放");
            return false;
        }

        if(skillSlot.isCD) {
            Debug.Log("技能冷却中，无法施放");
            return false;
        }
    
        CastSkill(skillSlot);
        return true;
    }

    private bool checkCost (SkillSlot skillSlot) {
        // 检查技能槽的能量是否足够
        if (skillSlot.skill != null ) {
            if(skillSlot.skill.CostType == TableItem.SkillCostType.魔法) {
                return entity.Attrib.mp >= skillSlot.skill.Cost;
            } else {
                return true;
            }
        }
        return false;
    }

    public void CastSkill(SkillSlot skillSlot)
    {
        float cd = skillSlot.cd;

        skillSlot.SetSkillIndex();

        SkillCost(skillSlot);

        skillSlot.pushCD(cd);
        communalCd.set(0.5f);
        castTime = skillSlot.skill.CastTime;
    }

    public void SkillCost (SkillSlot skillSlot) {

    }

    public SkillSlot GetSkillSlot (int slot) {
        if (slot < 0 || slot >= skillSlots.Length) {
            Debug.LogError("技能槽索引超出范围");
            return null;
        }
        SkillSlot skillSlot = skillSlots[slot];
        if (skillSlot == null) {
            Debug.LogError("该技能槽未学习任何技能");
            return null;
        }
        // 返回技能槽
        return skillSlot;
    }
}
