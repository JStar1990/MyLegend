using System.Collections;
using System.Collections.Generic;
using TableItem;
using UnityEngine;

public class EntityVisual : MonoBehaviour
{
    public int TableId = 0; // 表格ID
    [System.Serializable]
    public class SkillDetectorConfig
    {
        public string type;                    // 技能类型标识
        public BaseSkillDetector detectorPrefab; // 对应的预制体
    }
    public SkillDetectorConfig[] skillDetectors; // 在Inspector中设置多个技能检测器
    private Dictionary<string, BaseSkillDetector> skillDetectorInstances = new Dictionary<string, BaseSkillDetector>();
    private Rigidbody rb;
    private AnimatorController animController; // 动画控制器引用
    public Entity entity; // 实体引用
    // 这里可以添加其他需要的变量和引用
    protected IEntityConfig config; // 可以指向多个数据类型
    [Header("血条设置")]
    [SerializeField] private GameObject healthBarPrefab; // 血条预制体
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 0, 0); // 血条位置偏移
    protected HealthBar healthBar;
    public void Start()
    {
        entity = new Entity(config);

        // moveSpeed = entity.moveSpeed; // 获取实体的移动速度
        // rotationSpeed = entity.rotationSpeed; // 获取实体的旋转速度
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody 未附加到对象上！");
        }

        // 获取 AnimatorController 组件（包括子对象）
        animController = AddComponent<AnimatorController>();
        if (animController == null)
        {
            Debug.LogError("AnimatorController 未附加到子对象上！");
        }
        // rb.MovePosition(entity.position); // 设置初始位置

        // 修改获取 World 组件的方式
        DisplayWorld world = GameObject.Find("Wrold").GetComponent<DisplayWorld>();
        if (world != null)
        {
            world.AddEntitVisual(this);
        }
        else
        {
            Debug.LogError("World 组件未找到！请确认场景中存在名为 'Wrold' 的游戏对象");
        }
        InitializeHealthBar();
        InitializeSkillDetectors();
    }

    protected void InitializeHealthBar()
    {
        GameObject hpbarPrefab = Resources.Load<GameObject>("Prefabs/UI/HeadBar");
        if (hpbarPrefab == null)
        {
            Debug.LogError($"找不到预制体: Prefabs/UI/HeadBar");
            return;
        }
        GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/UI/pfHealthBarUI");
        if (healthBarPrefab == null)
        {
            Debug.LogError($"找不到预制体: Prefabs/UI/HeadBar");
            return;
        }
        GameObject hpbarObj = Instantiate(hpbarPrefab, transform);
        GameObject healthBarObj = Instantiate(healthBarPrefab, hpbarObj.transform);
        healthBarObj.transform.localPosition = healthBarOffset;
        healthBar = healthBarObj.GetComponent<HealthBar>();

        // 初始化血量显示
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(entity.Attrib.hp, entity.Attrib.maxHp);
        }
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    protected T AddComponent<T>() where T : Component
    {
        T component;
        if (GetComponent<T>() == null)
        {
            component = gameObject.AddComponent<T>();
            Debug.Log($"已添加组件: {typeof(T).Name}");
        }
        else
        {
            component = GetComponent<T>();
            Debug.Log($"组件 {typeof(T).Name} 已存在");
        }
        return component;
    }

    private void InitializeSkillDetectors()
    {
        if (skillDetectors != null)
        {
            foreach (var detectorConfig in skillDetectors)
            {
                if (string.IsNullOrEmpty(detectorConfig.type) || detectorConfig.detectorPrefab == null)
                {
                    Debug.LogWarning($"技能检测器配置无效: {detectorConfig.type}");
                    continue;
                }

                InitializeSkillDetector(detectorConfig.type, detectorConfig.detectorPrefab);
                Debug.Log($"初始化技能检测器: {detectorConfig.type}");
            }
        }
    }

    public void Update()
    {
        if (entity != null)
        {
            entity.Update(Time.deltaTime); // 更新实体状态
        }
        else
        {
            Debug.LogError($"Entity[{TableId}] 未初始化！");
        }
    }

    public void SetMoving()
    {
        animController.SetMoving();
    }

    public void StopMoving()
    {
        animController.StopMoving();
    }

    public void PlaySkillAnimation(string name)
    {
        animController.PlaySkillAnimation(name);
    }

    public void onSkillHit(string hitEffect)
    {
        animController.PlaySkillAnimation("Hit");
        EffectController.Instance.PlayEffect(hitEffect, transform);
    }

    public void InitializeSkillDetector(string type, BaseSkillDetector detectorPrefab)
    {
        if (!skillDetectorInstances.ContainsKey(type))
        {
            var detector = Instantiate(detectorPrefab, transform);
            detector.Initialize(this);
            detector.gameObject.SetActive(false);
            skillDetectorInstances.Add(type, detector);
        }
    }

    public BaseSkillDetector GetSkillDetector(string type)
    {
        return skillDetectorInstances.TryGetValue(type, out var detector) ? detector : null;
    }

    public void TryCastSkill(int slot)
    {
        Debug.Log($"释放技能{slot}");
        bool result = entity.TryCastSkill(slot);
        if (result)
        {
            SkillSlot skillSlot = entity.GetSkillSlot(slot);
            PlaySkillAnimation(skillSlot.skill.CastAnimation);

            // 播放特效并自动清理
            if (skillSlot.skill.CastEffect != "")
            {
                EffectController.Instance.PlayEffect(skillSlot.skill.CastEffect, transform);
            }

            // 使用实体自己的检测器
            if (skillSlot.skill.SType == SkillType.扇形)
            {
                if (skillDetectorInstances.TryGetValue("sector", out var detector))
                {
                    var sectorDetector = detector as SectorSkillDetector;
                    if (sectorDetector != null)
                    {
                        sectorDetector.radius = skillSlot.skill.Radius;
                        sectorDetector.angle = skillSlot.skill.Angle;
                        sectorDetector.skillId = skillSlot.skillId;
                        sectorDetector.gameObject.SetActive(true);

                        StartCoroutine(DelayDetect(sectorDetector, skillSlot.skill.CastTime));
                    }
                }
            }
            else if (skillSlot.skill.SType == SkillType.圆形)
            {
                if (skillDetectorInstances.TryGetValue("circle", out var detector))
                {
                    var circleDetector = detector as CircleSkillDetector;
                    if (circleDetector != null)
                    {
                        circleDetector.radius = skillSlot.skill.Radius;
                        circleDetector.skillId = skillSlot.skillId;
                        circleDetector.gameObject.SetActive(true);

                        StartCoroutine(DelayDetect(circleDetector, skillSlot.skill.CastTime));
                    }
                }
            }
            // else if(skillSlot.skill.SType == TableItem.SkillType.矩形)
            // {
            //     if (skillDetectorInstances.TryGetValue("rectangle", out var detector))
            //     {
            //         var rectangleDetector = detector as RectangleSkillDetector;
            //         if (rectangleDetector != null)
            //         {
            //             rectangleDetector.width = skillSlot.skill.Range;
            //             rectangleDetector.height = skillSlot.skill.Radius;
            //             rectangleDetector.gameObject.SetActive(true);

            //             StartCoroutine(DelayDetect(rectangleDetector, skillSlot.skill.CastTime));
            //         }
            //     }
            // }
        }
    }

    private IEnumerator DelayDetect(BaseSkillDetector detector, float delay)
    {
        yield return new WaitForSeconds(delay);
        detector.DetectTargets();
        detector.gameObject.SetActive(false);
    }

    public bool IsDeath
    {
        get
        {
            return entity != null && entity.Attrib.hp <= 0;
        }
    }

    public void onDie()
    {
        entity.Attrib.hp = 0; // 设置为死亡状态
        animController.PlaySkillAnimation("Die");
        Destroy(gameObject, 2f); // 2秒后销毁，可以根据需要调整时间
    }
}