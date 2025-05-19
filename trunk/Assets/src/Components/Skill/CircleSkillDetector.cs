using UnityEngine;

public class CircleSkillDetector : BaseSkillDetector
{
    public float radius = 5f;           // 圆形半径
    public float heightTolerance = 2f;  // 高度容差

    void Start()
    {
        if (targetLayer == 0)
        {
            targetLayer = LayerMask.GetMask("Enemy");
            Debug.Log($"设置目标层级: {targetLayer}");
        }
    }

    public override void DetectTargets()
    {
        // 更新检测器位置为释放者位置
        transform.position = owner.transform.position;

        Debug.Log($"检测器位置: {transform.position}, 前方方向: {transform.forward}");
        Debug.Log($"目标层级: {LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(targetLayer, 2)))}");
        
        // 检测范围内的所有碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        Debug.Log($"圆形范围内检测到 {hitColliders.Length} 个碰撞体");

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == owner.gameObject) continue;

            // 检查高度差异
            float heightDifference = Mathf.Abs(hitCollider.transform.position.y - transform.position.y);
            if (heightDifference > heightTolerance)
            {
                Debug.Log($"目标 {hitCollider.name} 高度差异过大: {heightDifference}");
                continue;
            }

            Debug.Log($"圆形技能命中目标: {hitCollider.name}");
            OnDetectTarget(hitCollider.gameObject);
        }
    }

    // 在编辑器中可视化检测范围
    private void OnDrawGizmos()
    {
        Vector3 position = Application.isPlaying ? owner?.transform.position ?? transform.position : transform.position;

        // 绘制圆形范围
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        DrawCircle(position, radius);

        // 绘制高度容差范围
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Vector3 upOffset = Vector3.up * heightTolerance;
        Vector3 downOffset = Vector3.down * heightTolerance;
        Gizmos.DrawLine(position + upOffset, position + downOffset);
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        Vector3 oldPoint = center;
        for (int i = 0; i < segments + 1; i++)
        {
            Vector3 newPoint = center + new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            Gizmos.DrawLine(oldPoint, newPoint);
            oldPoint = newPoint;
            theta += deltaTheta;
        }
    }
}