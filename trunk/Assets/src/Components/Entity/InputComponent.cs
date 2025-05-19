using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    [SerializeField] private float rotationSmoothTime = 0.1f; // 添加转身平滑参数
    private bool isMoving = false; // 跟踪移动状态
    private Vector3 joystickDirection = Vector3.zero;
    private Rigidbody rb;
    private EntityVisual entityVisual;
    [SerializeField] private bool useVelocity = true; // 使用速度控制移动
    [SerializeField] private Camera mainCamera; // 添加对主摄像机的引用

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (entityVisual == null) entityVisual = GetComponent<EntityVisual>();
        if (mainCamera == null) mainCamera = Camera.main; // 如果没有手动指定，则使用主摄像机
    }

    void OnEnable()
    {
                // 注册摇杆事件
        GameEvents.Instance.onJoystickMove += OnJoystickMove;
        GameEvents.Instance.onClickJoySkill += OnClickJoySkill;
    }

    private void OnJoystickMove(Vector3 direction)
    {
        joystickDirection = direction;
    }

    private void OnClickJoySkill(int slot)
    {
        entityVisual.TryCastSkill(slot); // 尝试施法   
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标左键按下时触发攻击动画
        // if (Input.GetMouseButtonDown(0)) // 0 表示鼠标左键
        // {
        //     entityVisual.TryCastSkill(0); // 尝试施法   
        // }
        // // 按下键盘数字1时释放技能1
        // else if (Input.GetKeyDown(KeyCode.Alpha1)) // Alpha1 表示键盘数字键1
        // {
        //     entityVisual.TryCastSkill(1); // 释放技能槽位1的技能
        // }
    }

    private Entity entity
    {
        get { return entityVisual.entity; }
    }

    private float moveSpeed
    {
        get { return entity.moveSpeed; }
    }   
    private float rotationSpeed
    {
        get { return entity.rotationSpeed; }
    }

    void FixedUpdate()
    {
        if (entity.IsCasting) return;

        Vector3 moveDirection = Vector3.zero;

        // 处理键盘输入
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 keyboardDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 如果有摇杆输入，优先使用摇杆
        if (joystickDirection != Vector3.zero)
        {
            moveDirection = joystickDirection;
        }
        // 否则使用键盘输入
        else if (keyboardDirection != Vector3.zero)
        {
            moveDirection = keyboardDirection;
        }

        // 处理移动和转身
        if (moveDirection != Vector3.zero)
        {
            // 将移动方向转换为相对于摄像机的方向
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            
            // 将摄像机的前后方向和左右方向在水平面上投影
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 基于摄像机方向计算实际的移动方向
            Vector3 movement = (cameraForward * moveDirection.z + cameraRight * moveDirection.x).normalized;
            
            // 使用速度控制移动
            if (useVelocity)
            {
                // 计算目标速度
                Vector3 targetVelocity = movement * moveSpeed;
                // 保持当前的Y轴速度（处理重力等垂直运动）
                targetVelocity.y = rb.velocity.y;
                // 平滑设置速度
                rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * 10f);
            }
            else
            {
                // 保持原有的位置移动方式
                rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            }

            // 角色朝向移动方向
            if (movement != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
            }

            // // 平滑旋转
            // if (movement != Vector3.zero)
            // {
            //     Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            //     transform.rotation = Quaternion.Slerp(
            //         transform.rotation,
            //         targetRotation,
            //         Time.fixedDeltaTime * rotationSpeed
            //     );
            // }

            // 只在状态改变时触发动画
            if (!isMoving)
            {
                isMoving = true;
                entityVisual.SetMoving();
            }
        }
        else
        {
            if (useVelocity)
            {
                // 逐渐停止水平移动
                Vector3 velocity = rb.velocity;
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.fixedDeltaTime * 10f);
                velocity.z = Mathf.Lerp(velocity.z, 0, Time.fixedDeltaTime * 10f);
                rb.velocity = velocity;
            }

            // 只在状态改变时停止动画
            if (isMoving)
            {
                isMoving = false;
                entityVisual.StopMoving();
            }
        }
    }

    void OnDisable()
    {
        GameEvents.Instance.onJoystickMove -= OnJoystickMove;
        GameEvents.Instance.onClickJoySkill -= OnClickJoySkill;
    }
}
