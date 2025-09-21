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

    //[Header("События")]
    [HideInInspector] public SelectedEvent onSelected;
    [HideInInspector] public SelectedEvent onDeselected;
    [HideInInspector] public SelectedEvent onHover; // Срабатывает только если нет выбранной кнопки

    [Header("GameObjects")]
    public TextMeshProUGUI textButtonLeft;
    public TextMeshProUGUI textButtonRight;

    [Header("Настройки")]
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
        // Автоматически отслеживаем потерю выделения через EventSystem
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

        // Если курсор все еще на кнопке и она потеряла выделение,
        // вызываем hover событие (если разрешено)
        if (isHovered && !IsAnyButtonSelected() && enableHoverOnlyWhenNoSelection)
        {
            onHover?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        // Вызываем hover событие только если:
        // 1. Разрешена проверка И нет выбранных кнопок
        // 2. ИЛИ проверка отключена
        if ((enableHoverOnlyWhenNoSelection && !IsAnyButtonSelected()) || !enableHoverOnlyWhenNoSelection)
        {
            onHover?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    // Проверяет, есть ли какая-либо кнопка в данный момент выбрана
    private bool IsAnyButtonSelected()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return false;

        // Проверяем, является ли выбранный объект кнопкой
        var selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        return selectedButton != null;
    }

    // Публичный метод для принудительной проверки
    public bool CheckIfAnyButtonSelected()
    {
        return IsAnyButtonSelected();
    }

    // Метод для ручного вызова выделения
    public void SelectButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    // Метод для ручного снятия выделения
    public void DeselectButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Метод для проверки, выбрана ли эта конкретная кнопка
    public bool IsThisButtonSelected()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject == gameObject;
    }
}