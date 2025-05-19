using System;
using TableItem;

public class SkillSlot
{
    public int slot;
    public int[] skillIds; // 技能ID
    public int level; // 技能等级

    public TableItem.Skill[] skills;

    public int currentSkillIndex = -1; // 当前选中的技能索引
    public SkillSlot(int slot)
    {
        this.slot = slot;
    }

    public void SkillLearn(int[] skillId, int level)
    {
        this.skillIds = skillId;
        skills = new TableItem.Skill[skillId.Length];
        for (int i = 0; i < skillId.Length; i++)
        {
            skills[i] = Table.skill.Get(skillId[i]);
        }
        this.level = level;
        Console.WriteLine($"学习技能: ID={skillId}, 等级={level}");
    }

    public void SkillUpgrade(int level)
    {
        this.level = level;
        Console.WriteLine($"升级技能: ID={skillId}, 新等级={level}");
    }

    public int skillId { get { return skillIds[currentSkillIndex==-1?0:currentSkillIndex]; } }

    public TableItem.Skill skill { get { return skills[currentSkillIndex==-1?0:currentSkillIndex]; } }

    public float cd
    {
        get
        {// 假设技能冷却时间与技能等级有关
            return skill.CD - (level - 1) * 0.1f; // 示例公式
        }
    }

    public bool IsNormalSkill
    {
        get { return slot == 0; } // 假设技能ID小于1000为普通技能
    }

    public CDParams time = new CDParams();
    public bool isCD
    {
        get { return time.isCD; }
    }

    public void pushCD(float time)
    {
        this.time.set(time > 0 ? time : 0, 1);
    }

    private float currentSkillIndexModifyTime;
    public void SetSkillIndex()
    {
        if (IsNormalSkill)
        {
            if (skillIds.Length > 1)
            {
                currentSkillIndex++;
            }
            if (currentSkillIndex >= skillIds.Length)
            {
                currentSkillIndex = 0;
            }
            currentSkillIndexModifyTime = 2;
        }
    }

    public void Update(float deltaTime)
    {
        if (currentSkillIndexModifyTime > 0)
        {
            currentSkillIndexModifyTime -= deltaTime;
            if (currentSkillIndexModifyTime < 0)
            {
                currentSkillIndexModifyTime = 0;
                currentSkillIndex = -1;
            }
        }
    }
}
