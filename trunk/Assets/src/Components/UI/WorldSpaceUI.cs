using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // 要跟随的3D物体
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); // 血条位置偏移
    [SerializeField] private Image fillImage; // 血条填充图片

    private Camera mainCamera;
    private Canvas canvas;
    private RectTransform rectTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        
        if (!targetTransform)
        {
            targetTransform = transform.parent;
        }
    }

    private void LateUpdate()
    {
        if (!targetTransform) return;

        // 将3D世界坐标转换为屏幕坐标
        Vector3 targetPosition = targetTransform.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPosition);

        // 如果目标在相机后面，不显示血条
        if (screenPos.z < 0)
        {
            rectTransform.gameObject.SetActive(false);
            return;
        }

        rectTransform.gameObject.SetActive(true);
        
        // 设置血条位置
        rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
    }

    // 更新血量显示
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}