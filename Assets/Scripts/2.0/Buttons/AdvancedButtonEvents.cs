using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class AdvancedButtonEvents : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class SelectedEvent : UnityEvent { }

    //[Header("�������")]
    [HideInInspector] public SelectedEvent onSelected;
    [HideInInspector] public SelectedEvent onDeselected;
    [HideInInspector] public SelectedEvent onHover; // ����������� ������ ���� ��� ��������� ������

    [Header("GameObjects")]
    public TextMeshProUGUI textButtonLeft;
    public TextMeshProUGUI textButtonRight;

    [Header("���������")]
    public bool enableHoverOnlyWhenNoSelection = true;
    public string ButtonKey;
    public string ButtonAnswerKey;
    public string DescriptionKey;

    private bool isSelected;
    private bool isHovered;

    private TextMeshProUGUI DescriptionsStats;

    void Start()
    {
        isSelected = false;
        isHovered = false;
        DescriptionsStats = GameObject.Find("DescriptionsStats").GetComponent<TextMeshProUGUI>();

        Initialized();


    }

    public void Initialized()
    {
        onSelected.AddListener(() => DescriptionsStats.text = DescriptionKey);
        onHover.AddListener(() => DescriptionsStats.text = DescriptionKey);
        textButtonLeft.text = ButtonKey;
        textButtonRight.text = ButtonAnswerKey;
    }
    /*
        1)Quality:
        2)Description:
        3)Weight:
        4)Durability:
        5)MinDamageMelee: 
        6)MaxDamageMelee:
        7)MinDamageRange: 
        8)MaxDamageRange:
        9)CritChanceMelee: 
        10)CritChanceRange: 
        11)CritDamageMelee:
        12)CritDamageRange: 
        13)Stamina:
        14)Accuracy:
        15)CoolDownMelee:
        16)CoolDownRange:
        17)Price:
        18)Armor:
        19)CountHeal:
        20)ActivationConditions:
        21)Modifiers:
        22)Type
        23)Rarity
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