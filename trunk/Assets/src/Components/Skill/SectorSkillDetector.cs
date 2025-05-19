using UnityEngine;
using System.Collections.Generic;

public class SectorSkillDetector : BaseSkillDetector
{
    public float radius = 5f;           // 扇形半径
    public float angle = 120f;          // 扇形角度
    public float heightTolerance = 2f;  // 高度容差，可以在 Inspector 中调整

    void Start()
    {
        // 确保设置了正确的目标层级
        if (targetLayer == 0)
        {
            targetLayer = LayerMask.GetMask("Enemy"); // 根据实际目标层级名称修改
            Debug.Log($"设置目标层级: {targetLayer}");
        }
    }

    public override void DetectTargets()
    {
        // 将检测器位置更新为拥有者的位置
        transform.position = owner.transform.position;
        // 将检测器的朝向设置为拥有者的朝向
        transform.forward = owner.transform.forward;

        Debug.Log($"检测器位置: {transform.position}, 前方方向: {transform.forward}");
        Debug.Log($"目标层级: {LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(targetLayer, 2)))}");
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        Debug.Log($"检测到 {hitColliders.Length} 个碰撞体");
        
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"检测到物体: {hitCollider.gameObject.name}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.gameObject == owner.gameObject) continue;

            // 检查高度差异
            float heightDifference = Mathf.Abs(hitCollider.transform.position.y - transform.position.y);
            if (heightDifference > heightTolerance)
            {
                Debug.Log($"目标 {hitCollider.name} 高度差异过大: {heightDifference}");
                continue;
            }

            Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
            directionToTarget.y = 0;
            
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            Debug.Log($"目标 {hitCollider.name} 角度: {angleToTarget}, 高度差: {heightDifference}");
            
            if (angleToTarget <= angle * 0.5f)
            {
                Debug.Log($"成功检测到目标: {hitCollider.name}");
                OnDetectTarget(hitCollider.gameObject);
            }
        }
    }

    // 修改 OnDrawGizmos 来更好地显示检测范围
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // 编辑器模式下使用自身位置
            DrawGizmosAt(transform.position, transform.forward);
        }
        else if (owner != null)
        {
            // 运行时使用拥有者位置
            DrawGizmosAt(owner.transform.position, owner.transform.forward);
        }
    }

    // 抽取绘制逻辑为单独的方法
    private void DrawGizmosAt(Vector3 position, Vector3 forward)
    {
        Gizmos.color = Color.red;
        
        int segments = 20;
        float halfAngle = angle * 0.5f;
        
        // 绘制扇形范围
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angle * i / segments);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            Gizmos.DrawLine(position, position + direction * radius);
        }
        
        // 绘制弧线
        for (int i = 0; i < segments; i++)
        {
            float currentAngle = -halfAngle + (angle * i / segments);
            float nextAngle = -halfAngle + (angle * (i + 1) / segments);
            
            Vector3 current = position + Quaternion.Euler(0, currentAngle, 0) * forward * radius;
            Vector3 next = position + Quaternion.Euler(0, nextAngle, 0) * forward * radius;
            
            Gizmos.DrawLine(current, next);
        }

        // 绘制高度容差范围
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Vector3 upOffset = Vector3.up * heightTolerance;
        Vector3 downOffset = Vector3.down * heightTolerance;
        Gizmos.DrawLine(position + upOffset, position + downOffset);
    }

    // 添加检测范围可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制检测球体范围
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // 绘制前向方向
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * radius);
    }
}