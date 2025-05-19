using System;
using UnityEngine;

public class DamageInstance
{
    private EntityAttribute _src;
    private EntityAttribute _dst;

    public DamageInstance(EntityAttribute src, EntityAttribute dst)
    {
        _src = src;
        _dst = dst;
    }

    // 源属性
    public float 攻击 => 10;//_src.GetAttr("攻击").Value;
    public float 减防 => 0;//_src.GetAttr("减防").Value;
    public float 穿透 => 0;//_src.GetAttr("穿透").Value;
    public float 暴击 => 5;//_src.GetAttr("暴击").Value;
    public float 暴击伤害 => 50;//_src.GetAttr("暴击伤害").Value;
    public float 增伤 => 0;//_src.GetAttr("增伤").Value;

    // 目标属性
    public float 防御 => 2;//_dst.GetAttr("防御").Value;
    public float 减伤 => 0;//_dst.GetAttr("减伤").Value;

    // 源和目标的访问器
    public EntityAttribute Src => _src;
    public EntityAttribute Dst => _dst;

    private float _baseDamage = 0;
    /// <summary>
    /// 基础伤害
    /// </summary>
    public float BaseDamage
    {
        get => _baseDamage;
        set => _baseDamage = value;
    }

    private float _finalDamage = 0;
    /// <summary>
    /// 最终结算伤害
    /// </summary>
    public float FinalDamage
    {
        get => _finalDamage;
        set => _finalDamage = value;
    }

    private float _defReduction = 0;
    /// <summary>
    /// 防御免伤后的伤害千分比
    /// </summary>
    public float DefReduction
    {
        get => _defReduction;
        set => _defReduction = value;
    }

    private int _isCrit = 0;
    /// <summary>
    /// 是否暴击
    /// </summary>
    public int IsCrit
    {
        get => _isCrit;
        set => _isCrit = value;
    }

    private float _baseCrit = 0;
    /// <summary>
    /// 暴击伤害
    /// </summary>
    public float BaseCrit
    {
        get => _baseCrit;
        set => _baseCrit = value;
    }

    private float _baseIncreased = 0;
    /// <summary>
    /// 增伤/减伤
    /// </summary>
    public float BaseIncreased
    {
        get => _baseIncreased;
        set => _baseIncreased = value;
    }

    private float _killDamage = 0;
    /// <summary>
    /// 斩杀伤害
    /// </summary>
    public float KillDamage
    {
        get => _killDamage;
        set => _killDamage = value;
    }
}

public class Formula
{
    public static DamageInstance ApplyDamage(TableItem.Skill skill, EntityAttribute src, EntityAttribute dst)
    {
        var damageInstance = new DamageInstance(src, dst);

        // 基础伤害值，由攻击力系数，防御转模系数，生命转模系数，技能基础伤害获得
        damageInstance.BaseDamage = 0;
        if (skill.DType == TableItem.DamageType.物理)
        {
            damageInstance.BaseDamage = damageInstance.攻击 * skill.AT * 0.001f;
        }
        else if (skill.DType == TableItem.DamageType.法术)
        {
            // 法术伤害计算逻辑
        }

        // 自身防御转模
        if (skill.DT != 0)
        {
            damageInstance.BaseDamage += damageInstance.防御 * skill.DT * 0.001f;
        }

        // 自身生命上限转模
        if (skill.MHPT != 0)
        {
            damageInstance.BaseDamage += src.maxHp * skill.MHPT * 0.001f;
        }

        // 附加目标当前生命值伤害
        if (skill.HPT != 0)
        {
            damageInstance.BaseDamage += dst.hp * skill.HPT * 0.001f;
        }

        damageInstance.BaseDamage += skill.BaseDamage;

        // 防御力带来的伤害减免
        damageInstance.DefReduction = 1000;
        if (skill.DType != TableItem.DamageType.真伤)
        {
            damageInstance.DefReduction = 1000 - (int)(1000 * Math.Tanh(
                Math.Max(damageInstance.防御 - damageInstance.减防, 0) *
                (1000 - damageInstance.穿透) * 0.001f * 0.002f));
        }

        // 暴击计算
        damageInstance.IsCrit = UnityEngine.Random.Range(0, 1000) <= damageInstance.暴击 ? 1 : 0;
        damageInstance.BaseCrit = 1000 + damageInstance.IsCrit * damageInstance.暴击伤害;

        // 增伤/减伤
        damageInstance.BaseIncreased = Math.Max(1000 + damageInstance.增伤 - damageInstance.减伤, 0);

        // 斩杀伤
        damageInstance.KillDamage = 0;
        if (skill.KT != 0)
        {
            damageInstance.KillDamage += (dst.maxHp - dst.hp) * skill.KT * 0.001f;
        }

        // 最终伤害计算
        damageInstance.FinalDamage = (int)(damageInstance.BaseDamage *
            (damageInstance.DefReduction * 0.001f) *
            (damageInstance.BaseCrit * 0.001f) *
            (damageInstance.BaseIncreased * 0.001f) +
            damageInstance.KillDamage);

        return damageInstance;
    }
}
