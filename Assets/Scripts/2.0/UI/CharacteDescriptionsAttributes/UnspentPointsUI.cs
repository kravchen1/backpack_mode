using UnityEngine;
using TMPro;

public class UnspentPointsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private string _prefix = "Points: ";
    [SerializeField] private string _suffix = "";
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hasPointsColor = Color.yellow;
    [SerializeField] private bool _animateOnChange = true;

    private PlayerDataManager _dataManager;

    private void Start()
    {
        InitializePointsUI();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializePointsUI()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        SubscribeToEvents();
        UpdatePointsText();
    }

    private void SubscribeToEvents()
    {
        _dataManager.Stats.OnSkillPointsChanged += OnSkillPointsChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Stats != null)
        {
            _dataManager.Stats.OnSkillPointsChanged -= OnSkillPointsChanged;
        }
    }

    private void OnSkillPointsChanged(int unspentPoints)
    {
        UpdatePointsText();

        if (_animateOnChange)
        {
            PlayPointsChangeEffect(unspentPoints);
        }
    }

    private void UpdatePointsText()
    {
        if (_pointsText != null && _dataManager != null)
        {
            int points = _dataManager.Stats.UnspentSkillPoints;
            _pointsText.text = $"{_prefix}{points}{_suffix}";

            // Меняем цвет если есть очки для распределения
            _pointsText.color = points > 0 ? _hasPointsColor : _normalColor;
        }
    }

    private void PlayPointsChangeEffect(int newPoints)
    {
        StartCoroutine(PointsChangeAnimation(newPoints));
    }

    private System.Collections.IEnumerator PointsChangeAnimation(int newPoints)
    {
        if (_pointsText == null) yield break;

        RectTransform rect = _pointsText.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        Color originalColor = _pointsText.color;

        // Анимация для добавления очков
        if (newPoints > 0)
        {
            // Пульсация и мигание
            for (int i = 0; i < 2; i++)
            {
                rect.localScale = originalScale * 1.01f;
                //_pointsText.color = Color.green;
                yield return new WaitForSeconds(0.1f);

                rect.localScale = originalScale;
                //_pointsText.color = _hasPointsColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            // Анимация для траты очков
            rect.localScale = originalScale * 0.99f;
            //_pointsText.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            rect.localScale = originalScale;
            _pointsText.color = _normalColor;
        }
    }

    public void RefreshPoints()
    {
        UpdatePointsText();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_pointsText == null)
            _pointsText = GetComponentInChildren<TextMeshProUGUI>();
    }

    [ContextMenu("Test Add Points")]
    private void TestAddPoints()
    {
        if (_dataManager != null)
        {
            _dataManager.Stats.AddSkillPoints(3);
        }
    }

    [ContextMenu("Test Spend Points")]
    private void TestSpendPoints()
    {
        if (_dataManager != null && _dataManager.Stats.UnspentSkillPoints > 0)
        {
            _dataManager.Stats.UnspentSkillPoints--;
        }
    }
#endif
}