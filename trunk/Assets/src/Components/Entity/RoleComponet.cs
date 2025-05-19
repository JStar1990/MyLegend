using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RoleComponent : EntityVisual
{

    public new void Start()
    {
        config = (IEntityConfig)Table.hero.Get(TableId);
        base.Start();

        // 获取场景中的摄像机控制器并设置目标
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.SetTarget(this.transform);
        }
        AddComponent<InputComponent>();
        for (int i = 0; i < 4; ++i)
        {
            GameEvents.Instance.EmitSkillSlotChanged(i, entity.GetSkillSlot(i));
        }

        // InitializeHealthBar();
        healthBar.SetColor(Color.green);
    }

    public new void OnEnable()
    {
        base.OnEnable();
        // GameEvents.Instance.onEffectLoaded += onEffectLoaded;
        EffectController.Instance.PlayEffect("Area_star_ellow", this.transform);
    }

    public new void Update()
    {
        base.Update();
    }

    public new void OnDisable()
    {
        base.OnDisable();
        // GameEvents.Instance.onEffectLoaded -= onEffectLoaded;
    }

    // void onEffectLoaded()
    // {
    //     EffectController.Instance.PlayEffect("Area_star_ellow", this.transform);
    // }
}