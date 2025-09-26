using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class AdvancedButtonEventStats : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class SelectedEvent : UnityEvent { }

    //[Header("�������")]
    [HideInInspector] public SelectedEvent onSelected;
    [HideInInspector] public SelectedEvent onDeselected;
    [HideInInspector] public SelectedEvent onHover; // ����������� ������ ���� ��� ��������� ������

    [Header("GameObjects")]
    public TextMeshProUGUI textNameStat;
    public TextMeshProUGUI textCountStat;
    public TextMeshProUGUI textAddStat;
    public TextMeshProUGUI textTotalStat;
    public TextMeshProUGUI textDescriptionsCharacterStats;

    [Header("���������")]
    public bool enableHoverOnlyWhenNoSelection = true;
    public int countStat = 0;
    public int addStat = 0;
    [TextArea(3, 10)]
    public string DescriptionKey;

    private bool isSelected;
    private bool isHovered;


    void Start()
    {
        isSelected = false;
        isHovered = false;


        Initialized();


    }

    public void Initialized()
    {
        onSelected.AddListener(() => textDescriptionsCharacterStats.text = DescriptionKey);
        onHover.AddListener(() => textDescriptionsCharacterStats.text = DescriptionKey);
        textCountStat.text = $"{countStat}";
        textAddStat.text = $"{addStat}";
        textTotalStat.text = $"{countStat + addStat}";
    }
    /*
        1)Type
        2)Rarity
        3)Quality:
        4)Description:
        5)Weight:
        6)Durability:
        7)AvgDamageMelee: MinDamageMelee: - MaxDamageMelee:
        8)AvgDamageRange: MinDamageRange: - MaxDamageRange:
        9)CoolDownMelee:
        10)CoolDownRange:
        11)CritChanceMelee: 
        12)CritChanceRange: 
        13)CritDamageMelee:
        14)CritDamageRange: 
        15)BaseStaminaMelee:
        16)BaseStaminaRange:
        17)AccuracyMelee:
        18)AccuracyRaange:
        19)Price:
        20)CountHeal:
        21)ActivationConditions:
        22)Modifiers:
        23)Requirements:
        
    */



    void Update()
    {
        // ������������� ����������� ������ ��������� ����� EventSystem
        if (isSelected && EventSystem.current.currentSelectedGameObject != gameObject)
        {
            OnDeselect(null);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        onSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        onDeselected?.Invoke();

        // ���� ������ ��� ��� �� ������ � ��� �������� ���������,
        // �������� hover ������� (���� ���������)
        if (isHovered && !IsAnyButtonSelected() && enableHoverOnlyWhenNoSelection)
        {
            onHover?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        // �������� hover ������� ������ ����:
        // 1. ��������� �������� � ��� ��������� ������
        // 2. ��� �������� ���������
        if ((enableHoverOnlyWhenNoSelection && !IsAnyButtonSelected()) || !enableHoverOnlyWhenNoSelection)
        {
            onHover?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    // ���������, ���� �� �����-���� ������ � ������ ������ �������
    private bool IsAnyButtonSelected()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return false;

        // ���������, �������� �� ��������� ������ �������
        var selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        return selectedButton != null;
    }

    // ��������� ����� ��� �������������� ��������
    public bool CheckIfAnyButtonSelected()
    {
        return IsAnyButtonSelected();
    }

    // ����� ��� ������� ������ ���������
    public void SelectButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    // ����� ��� ������� ������ ���������
    public void DeselectButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // ����� ��� ��������, ������� �� ��� ���������� ������
    public bool IsThisButtonSelected()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject == gameObject;
    }
}