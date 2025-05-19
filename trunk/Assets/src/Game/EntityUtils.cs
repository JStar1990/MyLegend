using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityUtils
{
    private static EntityUtils instance;
    private static readonly object lockObject = new object();
    private Dictionary<string, EntityVisual.SkillDetectorConfig> skillDetectors = new Dictionary<string, EntityVisual.SkillDetectorConfig>();
    // 私有构造函数，防止外部实例化
    private EntityUtils()
    {
        skillDetectors["circle"] = new EntityVisual.SkillDetectorConfig
        {
            type = "circle",
            detectorPrefab = Resources.Load<BaseSkillDetector>("Prefabs/SkillDetector/CircleSkillDetector")
        };
        skillDetectors["sector"] = new EntityVisual.SkillDetectorConfig
        {
            type = "sector",
            detectorPrefab = Resources.Load<BaseSkillDetector>("Prefabs/SkillDetector/SectorSkillDetector")
        };
    }

    public static EntityUtils Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new EntityUtils();
                    }
                }
            }
            return instance;
        }
    }

    public void onSkillHit(EntityVisual owner, EntityVisual target, int skillId)
    {
        TableItem.Skill skill = Table.skill.Get(skillId);

        target.onSkillHit(skill.HitEffect);

        DamageInstance damage = Formula.ApplyDamage(skill, owner.entity.Attrib, target.entity.Attrib);

        // 计算伤害
        target.entity.Attrib.hp -= damage.FinalDamage;
        target.UpdateHealthBar();
        if (target.entity.Attrib.hp <= 0)
        {
            target.entity.Attrib.hp = 0;
            target.onDie();
        }
    }

    public void AddSkillDetector(EntityVisual owner)
    {
        var detectorList = new List<EntityVisual.SkillDetectorConfig>();
        foreach (var key in skillDetectors.Keys)
        {
            detectorList.Add(skillDetectors[key]);
        }
        owner.skillDetectors = detectorList.ToArray();
    }
}
