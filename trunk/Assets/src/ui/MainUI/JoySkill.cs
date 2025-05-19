using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoySkill : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int Slot;  // 摇杆区域
    [SerializeField] private Image Icon;  // 摇杆区域
    [SerializeField] private Image Mask;  // 摇杆区域
    [SerializeField] private Image CDMask;  // 摇杆区域
    [SerializeField] private TextMeshProUGUI CDText;  // 可在Unity编辑器中直接拖拽Text组件

    private SkillSlot skillSlot;
    private CircleMaskController maskController;
    // Start is called before the first frame update
    void Start()
    {
        Icon.gameObject.SetActive(false);
        Mask.gameObject.SetActive(false);
        CDMask.gameObject.SetActive(false);
        CDText.gameObject.SetActive(false);

        maskController = CDMask.GetComponent<CircleMaskController>();
    }

    void OnEnable()
    {
        GameEvents.Instance.onSkillSlotChanged += onSkillSlotChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (skillSlot != null)
        {
            if(skillSlot.isCD)
            {
                maskController.SetFillAmount(1 - skillSlot.time.percent);
                CDText.text = skillSlot.time.remain.ToString("0.0");
                CDMask.gameObject.SetActive(true);
                CDText.gameObject.SetActive(true);
            }
            else
            {
                CDMask.gameObject.SetActive(false);
                CDText.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        GameEvents.Instance.onSkillSlotChanged -= onSkillSlotChanged;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameEvents.Instance.EmitClickJoySkill(Slot);
    }


    void onSkillSlotChanged(int slot, SkillSlot skillSlot)
    {
        if (slot == 0) return;
        if (slot == Slot)
        {
            if (skillSlot == null)
            {
                gameObject.SetActive(false);
                return;
            }
            this.skillSlot = skillSlot;
            gameObject.SetActive(true);
            if (Icon != null)
            {
                Sprite sprite = Resources.Load<Sprite>(skillSlot.skill.Icon);
                if (sprite != null)
                {
                    Icon.sprite = sprite;
                    Icon.gameObject.SetActive(true);
                }
            }
        }
    }
}
