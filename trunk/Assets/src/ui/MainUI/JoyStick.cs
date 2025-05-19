using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SearchService;
using Unity.VisualScripting;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image area;  // 摇杆区域
    [SerializeField] private Image joystickBg;  // 摇杆区域
    [SerializeField] private Image joystick;  // 摇杆握柄

    private RectTransform joystickRect;  // 摇杆区域的RectTransform
    private bool isDragging = false;     // 是否正在拖拽
    private float radiusLimit;           // 摇杆移动半径限制
    private Vector2 InitialPos;   // 记录摇杆初始位置

    private Vector2 moveDirection;

    private void Awake()
    {
        joystickRect = GetComponent<RectTransform>();
        radiusLimit = joystickBg.rectTransform.sizeDelta.x * joystickBg.rectTransform.localScale.x * 0.5f;
        joystickRect.sizeDelta = new Vector2(Screen.width * 0.5f, Screen.height * 0.7f);
        // 记录初始位置
        InitialPos = new Vector2(-joystickRect.sizeDelta.x / 2 + joystickRect.sizeDelta.x * 0.3f, -joystickRect.sizeDelta.y / 2 + joystickRect.sizeDelta.y * 0.25f);
        OnPointerUp(null);
    }

    // 按下摇杆
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // 获取Canvas和其缩放
        Canvas canvas = GetComponentInParent<Canvas>();
        float canvasScale = canvas.scaleFactor;

        // 将屏幕坐标转换为带缩放的本地坐标
        Vector2 screenPoint = eventData.position;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area.rectTransform,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out localPoint);

        // 计算area的边界
        float areaWidthHalf = area.rectTransform.rect.width * 0.5f;
        float areaHeightHalf = area.rectTransform.rect.height * 0.5f;
        float joystickBgWidthHalf = joystickBg.rectTransform.rect.width * joystickBg.rectTransform.localScale.x * 0.5f;
        float joystickBgHeightHalf = joystickBg.rectTransform.rect.height * joystickBg.rectTransform.localScale.y * 0.5f;

        // 限制joystickBg不超出area范围，考虑Canvas缩放
        localPoint.x = Mathf.Clamp(localPoint.x,
            -areaWidthHalf / canvasScale,
            (areaWidthHalf - joystickBgWidthHalf) / canvasScale);
        localPoint.y = Mathf.Clamp(localPoint.y,
            -areaHeightHalf / canvasScale,
            (areaHeightHalf - joystickBgHeightHalf) / canvasScale);

        // 记录摇杆背景位置用于后续拖拽计算
        Vector2 anchoredPos = new Vector2(
            localPoint.x * canvasScale,
            localPoint.y * canvasScale
        );

        // 更新摇杆背景和摇杆的位置
        joystickBg.rectTransform.anchoredPosition = anchoredPos;
        joystick.rectTransform.anchoredPosition = anchoredPos;

        Debug.Log($"Screen Point: {screenPoint}, Local Point: {localPoint}, Anchored Pos: {anchoredPos}");

        OnDrag(eventData);
    }

    // 拖动摇杆
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        Vector2 dragPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area.rectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out dragPos);

        Vector2 joystickBgPos = joystickBg.rectTransform.anchoredPosition;

        // 修改方向计算方式
        Vector2 offset = dragPos - (joystickBgPos / canvas.scaleFactor);
        float distance = offset.magnitude;

        // 计算标准化方向
        moveDirection = offset.normalized;

        // 限制摇杆移动范围
        if (distance > radiusLimit)
        {
            offset = moveDirection * radiusLimit;
        }

        // 更新摇杆位置
        joystick.rectTransform.anchoredPosition = joystickBgPos + (offset * canvas.scaleFactor);

        // 发送移动事件
        Vector3 worldDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
        GameEvents.Instance.EmitJoystickMove(worldDirection);
    }

    // 移除Update中的事件发送，避免重复发送
    public void Update()
    {
        // 可以添加调试信息
        if (isDragging && moveDirection != Vector2.zero)
        {
            Debug.DrawRay(transform.position, new Vector3(moveDirection.x, 0, moveDirection.y) * 5f, Color.red);
        }
    }

    // 释放摇杆
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        moveDirection = Vector2.zero;
        // 发送停止移动事件
        GameEvents.Instance.EmitJoystickMove(Vector3.zero);
        // 返回到初始位置
        joystickBg.rectTransform.anchoredPosition = InitialPos;
        joystick.rectTransform.anchoredPosition = InitialPos;
    }

    private void OnDisable()
    {
        isDragging = false;
        // 返回到初始位置
        joystickBg.rectTransform.anchoredPosition = InitialPos;
        joystick.rectTransform.anchoredPosition = InitialPos;
    }
}