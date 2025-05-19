using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBar : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 3f, 0); // 血条偏移量
    private Camera mainCamera;
    private Canvas canvas;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("无法找到Canvas组件！请检查血条预制体的层级结构。");
            return;
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("无法找到RectTransform组件！");
            return;
        }

        // 设置Canvas
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 更新位置
            transform.position = transform.parent.position + offset;
            // 朝向摄像机
            transform.forward = -mainCamera.transform.forward;
        }
    }
}
