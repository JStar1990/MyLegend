using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class Main : MonoBehaviour
{
    private bool isInitialized = false;

    private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Table.LoadAll();
        // 注册事件监听
        // GameEvents.Instance.onEffectLoaded += OnEffectLoaded;

        // 开始初始化流程
        StartCoroutine(InitializeWorld());
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private IEnumerator InitializeWorld()
    {
        // // 等待表格数据加载完成
        // while (!Table.IsLoaded)
        // {
        //     yield return null;
        // }
        
        // 等待特效资源加载完成
        while (EffectController.Instance?.IsInitialized == false)
        {
            yield return null;
        }

        // 创建子对象
        CreateWorldObjects();
    }

    private void OnEffectLoaded()
    {
        isInitialized = true;
    }

    private void CreateWorldObjects()
    {
        CreateMap();
        // 创建角色
        CreatePlayer();

        // CreateMonster();
        // 创建其他世界对象
        // CreateEnvironment();
    }

    private GameObject LoadPrefab(string prefabPath)
    {
        // 检查缓存
        if (prefabCache.ContainsKey(prefabPath))
        {
            return prefabCache[prefabPath];
        }

        // 加载预制体
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"找不到预制体: {prefabPath}");
            return null;
        }

        // 添加到缓存
        prefabCache[prefabPath] = prefab;
        return prefab;
    }

    private void CreateMap()
    {
        GameObject mapPrefab = LoadPrefab("Prefabs/Map/Map_1001");
        if (mapPrefab != null)
        {
            GameObject map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
            map.transform.SetParent(transform);
        }
    }

    private void CreateMonster()
    {
        GameObject monsterPrefab = LoadPrefab("Prefabs/Monster/Monster_2000000001");
        if (monsterPrefab != null)
        {
            GameObject monster = Instantiate(
                monsterPrefab,
                new Vector3(0, 0, 0),
                Quaternion.identity
            );
            monster.transform.SetParent(transform);
        }
    }

    private void CreatePlayer()
    {
        GameObject playerPrefab = LoadPrefab("Prefabs/Role/Role_100000001");
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(
                playerPrefab,
                new Vector3(0, 5, 0),
                Quaternion.identity
            );
            RoleComponent roleComponent = player.AddComponent<RoleComponent>();
            roleComponent.TableId = 100000001;
            EntityUtils.Instance.AddSkillDetector(roleComponent);
            player.transform.SetParent(transform);
        }
    }

    private void CreateEnvironment()
    {
        // 按需加载环境预制体
        string[] envPaths = {
            "Prefabs/Environment/Tree01",
            "Prefabs/Environment/Rock01"
        };

        foreach (string path in envPaths)
        {
            GameObject envPrefab = LoadPrefab(path);
            if (envPrefab != null)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(-10f, 10f),
                    0,
                    Random.Range(-10f, 10f)
                );

                GameObject env = Instantiate(
                    envPrefab,
                    randomPos,
                    Quaternion.Euler(0, Random.Range(0, 360), 0)
                );
                env.transform.SetParent(transform);
            }
        }
    }

    private void OnDestroy()
    {
        // 取消事件注册
        if (GameEvents.Instance != null)
        {
            // GameEvents.Instance.onEffectLoaded -= OnEffectLoaded;
        }
    
        // 清理预制体缓存
        prefabCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}