using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameEvents : MonoBehaviour
{
    private static GameEvents instance;
    public static GameEvents Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public event System.Action<Vector3> onJoystickMove;
    public void EmitJoystickMove(Vector3 direction)
    {
        onJoystickMove?.Invoke(direction);
    }

    public event System.Action<float> onHealthChanged;
    public void EmitHealthChanged(float health)
    {
        onHealthChanged?.Invoke(health);
    }

    public event System.Action<int> onClickJoySkill;
    public void EmitClickJoySkill(int slot)
    {
        onClickJoySkill?.Invoke(slot);
    }

    public event System.Action<int, SkillSlot> onSkillSlotChanged;
    public void EmitSkillSlotChanged(int slot, SkillSlot skillSlots)
    {
        onSkillSlotChanged?.Invoke(slot, skillSlots);
    }

    public event System.Action onEffectLoaded;
    public void EmitEffectLoaded()
    {
        onEffectLoaded?.Invoke();
    }
}