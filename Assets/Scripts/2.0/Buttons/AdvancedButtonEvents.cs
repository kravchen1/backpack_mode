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

    private Button button;
    private bool isSelected;
    private bool isHovered;

    private TextMeshProUGUI DescriptionsStats;

    void Start()
    {
        button = GetComponent<Button>();
        isSelected = false;
        isHovered = false;
        DescriptionsStats = GameObject.Find("DescriptionsStats").GetComponent<TextMeshProUGUI>();




        switch (ButtonKey)
        {
            default:
                onSelected.AddListener(() => DescriptionsStats.text = "Play button selected" + gameObject.name);
                onHover.AddListener(() => DescriptionsStats.text = "Hover over play button" + gameObject.name);
                break;
        }

    }

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