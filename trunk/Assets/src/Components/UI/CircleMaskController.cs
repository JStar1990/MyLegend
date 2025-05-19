using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircleMaskController : MonoBehaviour
{
    private Material materialInstance;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        // 为每个控件创建独立的材质实例
        if (image.material != null)
        {
            materialInstance = new Material(image.material);
            image.material = materialInstance;
        }
    }

    public void SetFillAmount(float amount)
    {
        if (materialInstance != null)
        {
            materialInstance.SetFloat("_FillAmount", amount);
        }
    }

    public void SetStartAngle(float angle)
    {
        if (materialInstance != null)
        {
            materialInstance.SetFloat("_StartAngle", angle);
        }
    }

    public void SetAlpha(float alpha)
    {
        if (materialInstance != null)
        {
            materialInstance.SetFloat("_Alpha", alpha);
        }
    }

    private void OnDestroy()
    {
        // 清理材质实例
        if (materialInstance != null)
        {
            if (Application.isPlaying)
            {
                Destroy(materialInstance);
            }
            else
            {
                DestroyImmediate(materialInstance);
            }
        }
    }
}