using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image delayImage;
    [SerializeField] private float updateSpeed = 10f;
    private float targetFillAmount;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("无法找到RectTransform组件！");
            return;
        }

        // 根据图片设置调整RectTransform参数
        rectTransform.sizeDelta = new Vector2(200f, 30f); // 设置宽度为200，高度为50
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // 设置Canvas缩放，使血条在世界空间中大小合适
        transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        // 设置血条填充方向
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Right; // 从左向右填充
        }
        if (delayImage != null)
        {
            delayImage.type = Image.Type.Filled;
            delayImage.fillMethod = Image.FillMethod.Horizontal;
            delayImage.fillOrigin = (int)Image.OriginHorizontal.Right; // 从左向右填充
        }

        // Debug.Log($"Canvas: {canvas.name}, 血条尺寸: {rectTransform.sizeDelta}, 缩放: {transform.localScale}");
    }

    private void LateUpdate()
    {
        if (delayImage.fillAmount != targetFillAmount)
        {
            delayImage.fillAmount = Mathf.Lerp(delayImage.fillAmount, targetFillAmount, Time.deltaTime * updateSpeed);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        targetFillAmount = currentHealth / maxHealth;
        fillImage.fillAmount = targetFillAmount;
        gameObject.SetActive(targetFillAmount < 1f);
    }

    // 设置血条颜色
    public void SetColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}