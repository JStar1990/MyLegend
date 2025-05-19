using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-1)]
public class EffectController : MonoBehaviour
{
    private static EffectController instance;
    public static EffectController Instance => instance;
    private Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();

    private bool isInitialized = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        // 预加载特效资源
        LoadEffectPrefabs();
    } 

    private void LoadEffectPrefabs()
    {
        // 从Resources文件夹加载特效预制体
        Object[] effects = Resources.LoadAll("Effects", typeof(GameObject));
        foreach (GameObject effect in effects)
        {
            effectPrefabs[effect.name] = effect;
        }
        isInitialized = true;
        GameEvents.Instance.EmitEffectLoaded();
    }

    public bool IsInitialized => isInitialized;

    /// <summary>
    /// 特效类型
    /// </summary>
    public enum EffectType
    {
        Once,       // 单次播放
        Loop        // 循环播放
    }

    /// <summary>
    /// 特效数据类
    /// </summary>
    private class EffectData
    {
        public string effectId;              // 特效唯一标识
        public string effectName;            // 特效名称
        public GameObject effectInstance;     // 特效实例
        public ParticleSystem particleSystem; // 粒子系统
        public EffectType effectType;         // 特效类型
        public Transform target;              // 绑定目标
        public Vector3 offset;                // 相对偏移
        public Vector3 scale;                 // 特效缩放
    }

    private Dictionary<string, EffectData> activeEffectDatas = new Dictionary<string, EffectData>();

    /// <summary>
    /// 播放特效（非绑定版本）
    /// </summary>
    public void PlayEffect(string effectName, Vector3 position, Quaternion rotation, Vector3? customScale = null)
    {
        var effectConfig = Table.effect.Get(effectName);
        if (effectConfig == null)
        {
            Debug.LogError($"找不到特效配置: {effectName}");
            return;
        }

        Vector3 offset = effectConfig.Offset != null && effectConfig.Offset.Length >= 3 
            ? new Vector3(effectConfig.Offset[0], effectConfig.Offset[1], effectConfig.Offset[2]) 
            : Vector3.zero;

        // 使用辅助方法获取缩放
        Vector3 scale = GetEffectScale(effectConfig, customScale);

        PlayEffectInternal(
            effectConfig.Prefab,
            position,
            rotation,
            null,
            offset,
            scale,
            effectConfig.Loop ? EffectType.Loop : EffectType.Once
        );
    }

    /// <summary>
    /// 播放特效（绑定版本）
    /// </summary>
    public void PlayEffect(string effectName, Transform target, Vector3? customOffset = null, Vector3? customScale = null)
    {
        var effectConfig = Table.effect.Get(effectName);
        if (effectConfig == null)
        {
            Debug.LogError($"找不到特效配置: {effectName}");
            return;
        }

        Vector3 offset;
        if (customOffset.HasValue)
        {
            offset = customOffset.Value;
        }
        else if (effectConfig.Offset != null && effectConfig.Offset.Length >= 3)
        {
            offset = new Vector3(
                effectConfig.Offset[0],
                effectConfig.Offset[1],
                effectConfig.Offset[2]
            );
        }
        else
        {
            offset = Vector3.zero;
        }

        // 获取绑定节点
        Transform bindNode = string.IsNullOrEmpty(effectConfig.Node) ?
            target :
            target.Find(effectConfig.Node);

        if (bindNode == null)
        {
            Debug.LogWarning($"找不到绑定节点: {effectConfig.Node}，将使用目标根节点");
            bindNode = target;
        }

        // 使用辅助方法获取缩放
        Vector3 scale = GetEffectScale(effectConfig, customScale);

        PlayEffectInternal(
            effectConfig.Prefab,
            bindNode.position + offset,
            bindNode.rotation,
            bindNode,
            offset,
            scale,
            effectConfig.Loop ? EffectType.Loop : EffectType.Once
        );
    }

    private void PlayEffectInternal(string effectName, Vector3 position, Quaternion rotation,
        Transform target, Vector3 offset, Vector3 scale, EffectType type)
    {
        if (!effectPrefabs.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect {effectName} not found!");
            return;
        }

        // 创建特效实例
        GameObject effectInstance = Instantiate(effectPrefabs[effectName], position, rotation);
        effectInstance.transform.localScale = scale;  // 设置缩放
        ParticleSystem particleSystem = effectInstance.GetComponent<ParticleSystem>();

        // 如果有绑定目标，设置父物体关系
        if (target != null)
        {
            effectInstance.transform.SetParent(target);
            effectInstance.transform.localPosition = offset;
            effectInstance.transform.localRotation = Quaternion.identity;
        }

        // 生成唯一标识
        string effectId = GenerateEffectId(effectName, target ?? transform);

        // 创建特效数据
        EffectData effectData = new EffectData
        {
            effectId = effectId,
            effectName = effectName,
            effectInstance = effectInstance,
            particleSystem = particleSystem,
            effectType = type,
            target = target,
            offset = offset,
            scale = scale
        };

        // 添加到活动特效列表
        activeEffectDatas[effectId] = effectData;

        // 根据特效类型设置粒子系统
        if (particleSystem != null)
        {
            var effectConfig = Table.effect.Get(effectName);
            var main = particleSystem.main;
            main.loop = effectConfig.Loop;

            if (!effectConfig.Loop)
            {
                float duration = effectConfig.Time > 0 ?
                    effectConfig.Time :
                    particleSystem.main.duration;
                StartCoroutine(AutoDestroyEffect(effectId, duration));
            }
        }
    }

    /// <summary>
    /// 生成唯一特效ID
    /// </summary>
    private string GenerateEffectId(string effectName, Transform target)
    {
        return $"{effectName}_{target.GetInstanceID()}_{System.Guid.NewGuid()}";
    }

    private IEnumerator AutoDestroyEffect(string effectKey, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (activeEffectDatas.TryGetValue(effectKey, out var effectData))
        {
            if (effectData.effectInstance != null)
            {
                Destroy(effectData.effectInstance);
            }
            activeEffectDatas.Remove(effectKey);
        }
    }

    private void Update()
    {
        // 检查并清理无效的特效
        var invalidEffects = new List<string>();
        foreach (var kvp in activeEffectDatas)
        {
            if (kvp.Value.effectInstance == null || kvp.Value.target == null)
            {
                invalidEffects.Add(kvp.Key);
            }
        }

        foreach (var key in invalidEffects)
        {
            activeEffectDatas.Remove(key);
        }
    }

    /// <summary>
    /// 根据目标停止特效
    /// </summary>
    public void StopEffectsByTarget(Transform target)
    {
        var effectsToRemove = activeEffectDatas.Values
            .Where(data => data.target == target)
            .ToList();

        foreach (var effectData in effectsToRemove)
        {
            if (effectData.effectInstance != null)
            {
                Destroy(effectData.effectInstance);
            }
            activeEffectDatas.Remove(effectData.effectId);
        }
    }

    /// <summary>
    /// 根据特效名称和目标停止特效
    /// </summary>
    public void StopEffect(string effectName, Transform target)
    {
        var effectsToRemove = activeEffectDatas.Values
            .Where(data => data.effectName == effectName && data.target == target)
            .ToList();

        foreach (var effectData in effectsToRemove)
        {
            if (effectData.effectInstance != null)
            {
                Destroy(effectData.effectInstance);
            }
            activeEffectDatas.Remove(effectData.effectId);
        }
    }

    /// <summary>
    /// 停止所有特效
    /// </summary>
    public void StopAllEffects()
    {
        foreach (var effectData in activeEffectDatas.Values)
        {
            if (effectData.effectInstance != null)
            {
                Destroy(effectData.effectInstance);
            }
        }
        activeEffectDatas.Clear();
    }

    private void OnDestroy()
    {
        StopAllEffects();
    }

    private Vector3 GetEffectScale(TableItem.Effect effectConfig, Vector3? customScale)
    {
        // 如果有自定义缩放，优先使用自定义缩放
        if (customScale.HasValue)
        {
            return customScale.Value;
        }

        // 使用配置表中的缩放
        if (effectConfig?.Scale != null && effectConfig.Scale.Length >= 3)
        {
            return new Vector3(
                effectConfig.Scale[0],
                effectConfig.Scale[1],
                effectConfig.Scale[2]
            );
        }

        // 默认返回 Vector3.one (1,1,1)
        return Vector3.one;
    }
}
