using NUnit;
using UnityEngine;

public abstract class BaseSkillDetector : MonoBehaviour
{
    protected EntityVisual owner;  // 技能释放者
    public LayerMask targetLayer; // 目标层级

    public int skillId;

    public virtual void Initialize(EntityVisual owner)
    {
        this.owner = owner;
    }

    public abstract void DetectTargets();

    protected virtual void OnDetectTarget(GameObject target)
    {
        var entityVisual = target.GetComponent<EntityVisual>();
        if (entityVisual != null)
        {
            if (entityVisual.IsDeath)
            {
                Debug.Log($"{gameObject.name} 的技能击中了 {target}");
                return;
            }

            // 通知释放者处理伤害逻辑
            EntityUtils.Instance.onSkillHit(owner, entityVisual, skillId);
            Debug.Log($"{gameObject.name} 的技能击中了 {target}");
        }
    }
}