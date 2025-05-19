using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 跟随的目标
    public Vector3 offset = new Vector3(0, 5, -10); // 摄像机相对于目标的偏移量
    public float followSpeed = 5f; // 跟随的平滑速度
    
    private bool isFollowing = false; // 是否开始跟随目标

    public float mouseSensitivity = 2f; // 鼠标灵敏度
    public float minVerticalAngle = -30f; // 最小垂直角度
    public float maxVerticalAngle = 60f; // 最大垂直角度
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isDragging = false;

    // 添加一个变量来保存当前的偏移量
    private Vector3 currentOffset;

    void Start()
    {
        // 初始化当前偏移量
        currentOffset = offset;
        // 设置初始位置和朝向
        transform.position = offset; // 使用偏移量作为初始位置
        transform.LookAt(Vector3.zero); // 朝向原点(0,0,0)
    }

    // 设置目标并开始跟随
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFollowing = true;
    }

    void LateUpdate()
    {
        if (target != null && isFollowing)
        {
            // 处理鼠标拖动
            if (Input.GetMouseButtonDown(1)) // 鼠标右键按下
            {
                isDragging = true;
            }
            else if (Input.GetMouseButtonUp(1)) // 鼠标右键抬起
            {
                isDragging = false;
                // 在松开鼠标时保存当前的偏移量
                currentOffset = transform.position - target.position;
            }

            if (isDragging)
            {
                // 获取鼠标移动增量
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                // 更新旋转角度
                rotationY += mouseX;
                rotationX -= mouseY;
                rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

                // 计算新的偏移位置
                Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
                Vector3 newOffset = rotation * new Vector3(0, offset.y, -Mathf.Abs(offset.z));

                // 计算目标位置
                Vector3 targetPosition = target.position + newOffset;

                // 平滑移动摄像机到目标位置
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }
            else
            {
                // 使用保存的偏移量而不是默认偏移量
                Vector3 targetPosition = target.position + currentOffset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }

            // 始终朝向目标
            transform.LookAt(target);
        }
    }
}
