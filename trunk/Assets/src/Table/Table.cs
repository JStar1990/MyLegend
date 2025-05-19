using TableItem;

// public class TableSkill : TableBase<TableItem.Skill>
// {
//     // 这里可以添加特定于技能表的逻辑
//     // 例如，加载技能特有的数据或方法
// }

// public class TableMonster : TableBase<TableItem.Monster>
// {
//     // 这里可以添加特定于怪物表的逻辑
//     // 例如，加载怪物特有的数据或方法
// }

// public class TableHero : TableBase<TableItem.Hero>
// {
//     // 这里可以添加特定于英雄表的逻辑
//     // 例如，加载英雄特有的数据或方法
// }
// public class TableEffect : TableBase<TableItem.Effect>
// {

// }
// public class TablePrefab : TableBase<TableItem.Prefab>
// {
//     // 这里可以添加特定于预制体表的逻辑
//     // 例如，加载预制体特有的数据或方法
// }
public class Table
{
    public static TableBase<TableItem.Skill> skill = new TableBase<TableItem.Skill>();
    public static TableBase<Monster> monster = new TableBase<Monster>();
    public static TableBase<Hero> hero = new TableBase<Hero>();
    public static TableBase<Effect> effect = new TableBase<Effect>();
    public static TableBase<Prefab> prefab = new TableBase<Prefab>();

    public static void LoadAll()
    {
        skill.LoadAll(
            new object[][]
            {
                new object[] { "ID", "Name", "Icon", "CD", "CastTime", "CastAnimation", "CastEffect", "HitEffect", "SType", "Radius", "Angle", "AT" },
                new object[] { 100010001, "skill_100010001", "Icon/Skill/UI_Skill_Icon_Buff", 0.8f, 0.5f, "Attack01", "", "", 2, 1.8f, 90f, 1000 },
                new object[] { 100010002, "skill_100010002", "Icon/Skill/UI_Skill_Icon_Buff", 0.8f, 0.5f, "Attack02", "", "", 2, 1.8f, 90f, 1500 },
                new object[] { 100010003, "skill_100010003", "Icon/Skill/UI_Skill_Icon_Buff", 0.8f, 0.5f, "Attack03", "", "", 2, 1.8f, 90f, 2000 },
                new object[] { 100010004, "skill_100010004", "Icon/Skill/UI_Skill_Icon_Slash", 5f, 0.5f, "Attack04", "Effect_XFZ", "Effect_XFZ_Hit", 3, 2.2f, 360f, 4000 },
                new object[] { 200010001, "skill_200010001", "Icon/Skill/UI_Skill_Icon_Buff", 0.8f, 0.5f, "Attack01", "", "", 1, 1.5f, 360f, 1000 },
            }
        );

        hero.LoadAll(
            new object[][]{
                new object[] { "ID", "Name", "Skill", "Prefab"},
                new object[] { 100000001, "H10000001", new SlotSkillItem[]{
                    new SlotSkillItem { Slot = 0, SkillIds = new int[] { 100010001, 100010002, 100010003 } },
                    new SlotSkillItem { Slot = 1, SkillIds = new int[] { 100010004 } },
                }, "Role_100000001" },
            }
        );

        monster.LoadAll(
            new object[][]{
                new object[] { "ID", "Name", "Skill", "Prefab" },
                new object[] { 200000001, "M20000001", new SlotSkillItem[]{
                    new SlotSkillItem { Slot = 0, SkillIds = new int[] { 200010001 } },
                }, "Monster_200000001" },
                new object[] { 200000002, "M20000002", new SlotSkillItem[]{
                    new SlotSkillItem { Slot = 0, SkillIds = new int[] { 200010001 } },
                }, "Monster_200000002" },
            }
        );

        effect.LoadAll(
            new object[][]{
                new object[] { "Name", "Prefab", "Time", "Loop", "Node", "Offset", "Scale" },
                new object[] { "Effect_XFZ", "Effect_XFZ", 0f, false, "", new float[]{0,0.5f,0}, null },
                new object[] { "Effect_XFZ_Hit", "Effect_XFZ_Hit", 0f, false, "Hit", null, null },
                new object[] { "Area_star_ellow", "Area_star_ellow", 0f, true, "", new float[]{0,0.1f,0}, new float[]{0.3f,0.3f,0.3f} },
            }
        );

        prefab.LoadAll(
            new object[][]{
                new object[] { "Name", "Path" },
                new object[] { "Monster_200000001", "Prefabs/Monster/Monster_200000001" },
                new object[] { "Monster_200000002", "Prefabs/Monster/Monster_200000002" },
                new object[] { "Role_100000001", "Prefabs/Role/Role_100000001" },
                new object[] { "Effect_XFZ", "Effect_XFZ" },
                new object[] { "Effect_XFZ_Hit", "Effect_XFZ_Hit" },
                new object[] { "Area_star_ellow", "Area_star_ellow" },
            }
        );
    }
}