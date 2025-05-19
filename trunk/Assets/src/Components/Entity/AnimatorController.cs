using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    // private float combatIdleTimer = 0f; // 战斗待机计时器
    // private bool isInCombatIdle = false; // 是否处于战斗待机状态
    // public float combatIdleDuration = 5f; // 战斗待机持续时间

    // private float comboAttackTimmer = 0f;

    // 动画状态哈希值与名字的映射
    // private Dictionary<int, string> animationNameMap = new Dictionary<int, string>
    // {
    //     { Animator.StringToHash("Idle_Battle"), "Idle_Battle" },
    //     { Animator.StringToHash("Attack01"), "Attack01" },
    //     { Animator.StringToHash("Attack02"), "Attack02" },
    //     { Animator.StringToHash("Attack03"), "Attack03" },
    //     { Animator.StringToHash("Attack04"), "Attack04" },
    // };

    void Start()
    {
        // 获取 Animator 组件
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator 组件未附加到对象上！");
        }
    }

    void Update()
    {
        // 获取 Animator 的当前状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 表示默认的动画层

        // 获取当前动画的哈希值
        int currentAnimationHash = stateInfo.shortNameHash;

        // 从字典中查找动画名字
        // if (animationNameMap.TryGetValue(currentAnimationHash, out string currentAnimationName))
        // {
        //     // Debug.Log($"当前动画名字: {currentAnimationName}");
        // }
        // else
        // {
        //     // Debug.Log("当前动画名字未找到！");
        // }

        if (Input.GetKeyDown(KeyCode.Space)) // 按下空格键
        {
            // animator.SetTrigger("Jump"); // 触发 Jump 动画
        }

        float attackWait = animator.GetFloat("AttackWait"); // 获取当前攻击编号
        attackWait += Time.deltaTime;
        // 鼠标左键按下时触发攻击动画
        // if (Input.GetMouseButtonDown(0)) // 0 表示鼠标左键
        // {
        //     if(currentAnimationName == "Attack01")
        //     {
        //         animator.SetInteger("AttackNum", 2); // 设置攻击编号为下一个值
        //     }
        //     else if(currentAnimationName == "Attack02")
        //     {
        //         animator.SetInteger("AttackNum", 3); // 设置攻击编号为下一个值
        //     }
        //     else if(currentAnimationName == "Attack03")
        //     {
        //         animator.SetInteger("AttackNum", 4); // 设置攻击编号为下一个值
        //     }
        //     else if(currentAnimationName == "Attack04")
        //     {
        //         animator.SetInteger("AttackNum", 1); // 设置攻击编号为下一个值
        //     }
        //     else if(currentAnimationName == "Idle_Battle")
        //     {
        //         int num = animator.GetInteger("AttackNum"); // 设置攻击编号为下一个值
        //         if(num >= 4)num = 1;
        //         else num++;
        //         animator.SetInteger("AttackNum", num); // 设置攻击编号为下一个值
        //     }
        //     else
        //     {
        //         animator.SetInteger("AttackNum", 1); // 设置攻击编号为下一个值
        //     }
         
        //     attackWait = 0;
        //     animator.SetTrigger("Attack"); // 触发 Attack 动画
        //     // 进入战斗待机状态
        //     // EnterCombatIdle();
        // } else {
        //     attackWait += Time.deltaTime;
        // }
        animator.SetFloat("AttackWait", attackWait);
        // 检查战斗待机状态
        // if (isInCombatIdle)
        // {
        //     combatIdleTimer += Time.deltaTime; // 增加计时器
        //     if (combatIdleTimer >= combatIdleDuration)
        //     {
        //         ExitCombatIdle(); // 切换到普通待机状态
        //     }
        // }
    }

    public void SetMoving () {
        animator.SetBool("Moving", true);
    }

    public void StopMoving () {
        animator.SetBool("Moving", false);
    }

    public void PlaySkillAnimation (string name) {
        animator.Play(name, 0, 0f);
        animator.SetFloat("AttackWait", 0);
    }

    // private void EnterCombatIdle()
    // {
    //     isInCombatIdle = true;
    //     combatIdleTimer = 0f; // 重置计时器
    //     animator.SetTrigger("CombatIdle"); // 触发战斗待机动画
    // }

    // private void ExitCombatIdle()
    // {
    //     isInCombatIdle = false;
    //     combatIdleTimer = 0f; // 重置计时器
    //     animator.SetTrigger("Idle"); // 切换到普通待机动画
    // }
}
