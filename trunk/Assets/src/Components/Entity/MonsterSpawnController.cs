using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TableItem;

public class MonsterSpawnController : MonoBehaviour
{
    [Header("怪物配置")]
    [SerializeField] private int monsterId;               // 怪物ID
    
    [Header("生成设置")]
    [SerializeField] private float checkRadius = 10f;     // 检测范围
    [SerializeField] private float spawnRadius = 2f;      // 生成范围半径
    [SerializeField] private int maxMonsterCount = 5;     // 最大怪物数量
    [SerializeField] private float spawnDelay = 3f;       // 重生延迟时间

    private float spawnTimer;                             // 生成计时器
    private bool isSpawnTimerActive;                      // 计时器激活状态
    private List<GameObject> activeMonsters;              // 当前活跃的怪物列表
    private Monster monsterConfig;                        // 怪物配置
    private string prefabPath;                            // 预制体路径

    private void Start()
    {
        activeMonsters = new List<GameObject>();
        
        // 获取怪物配置
        monsterConfig = Table.monster.Get(monsterId);
        if (monsterConfig == null)
        {
            Debug.LogError($"找不到ID为{monsterId}的怪物配置！");
            return;
        }

        // 获取预制体路径
        var prefabConfig = Table.prefab.Get(monsterConfig.Prefab);
        if (prefabConfig == null)
        {
            Debug.LogError($"找不到预制体配置：{monsterConfig.Prefab}");
            return;
        }
        prefabPath = prefabConfig.Path;

        // 初始生成怪物
        SpawnInitialMonsters();
    }

    private void Update()
    {
        // 检查当前范围内的怪物数量
        CheckMonsterCount();

        // 处理生成计时器
        if (isSpawnTimerActive)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnMonsters();
                isSpawnTimerActive = false;
            }
        }
    }

    private void CheckMonsterCount()
    {
        // 清理已销毁的怪物
        activeMonsters.RemoveAll(monster => monster == null);

        // 如果怪物数量少于最大值，启动生成计时器
        if (activeMonsters.Count < maxMonsterCount && !isSpawnTimerActive)
        {
            StartSpawnTimer();
        }
    }

    private void StartSpawnTimer()
    {
        spawnTimer = spawnDelay;
        isSpawnTimerActive = true;
    }

    private void SpawnMonsters()
    {
        int monstersToSpawn = maxMonsterCount - activeMonsters.Count;
        
        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnSingleMonster();
        }
    }

    private void SpawnInitialMonsters()
    {
        for (int i = 0; i < maxMonsterCount; i++)
        {
            SpawnSingleMonster();
        }
    }

    private void SpawnSingleMonster()
    {
        if (string.IsNullOrEmpty(prefabPath)) return;

        // 加载预制体
        GameObject monsterPrefab = Resources.Load<GameObject>(prefabPath);
        if (monsterPrefab == null)
        {
            Debug.LogError($"无法加载怪物预制体：{prefabPath}");
            return;
        }

        // 在检测范围内生成随机点
        Vector2 randomPoint = Random.insideUnitCircle.normalized * Random.Range(0, checkRadius);
        Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

        // 生成随机朝向
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // 生成怪物（使用随机朝向）
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, randomRotation);
        MonsterComponent monsterComponent = monster.AddComponent<MonsterComponent>();
        monsterComponent.TableId = monsterId;
        EntityUtils.Instance.AddSkillDetector(monsterComponent);
        // 获取怪物的生命值组件并添加死亡监听
        // if (monster.TryGetComponent<EntityVisual>(out var health))
        // {
        //     health.OnDeath += () => 
        //     {
        //         // 从活跃列表中移除
        //         activeMonsters.Remove(monster);
        //         // 延迟销毁物体
        //         Destroy(monster, 2f); // 2秒后销毁，可以根据需要调整时间
        //     };
        // }

        // 设置怪物的父对象为当前触发器的父对象
        if (transform.parent != null)
        {
            monster.transform.SetParent(transform.parent.parent);
        }
        
        activeMonsters.Add(monster);
    }

    // 在Unity编辑器中显示生成范围和检测范围
    private void OnDrawGizmos()
    {
        // 显示检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // 显示生成范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}