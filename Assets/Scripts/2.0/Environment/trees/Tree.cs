using UnityEngine;

public class Tree : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject treeSprout;
    [SerializeField] private GameObject treeMiddle;
    [SerializeField] private GameObject treeAdult;
    [SerializeField] private int _currentState = DEFAULT_STATE;
    #endregion

    #region Private Variables
    private DayManager _dayManager;
    private string _treeId;
    #endregion

    #region Constants
    private const int SPROUT_STATE = 0;
    private const int MIDDLE_STATE = 1;
    private const int ADULT_STATE = 2;
    private const int CUT_DOWN_STATE = 3;
    private const string TREE_SAVE_PREFIX = "TreeState_";
    private const int DEFAULT_STATE = -1;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        GenerateTreeId();
        LoadTreeState();
        InitializeDayManager();

        if (_currentState == DEFAULT_STATE)
        {
            _currentState = Random.Range(0, 3);
        }

        UpdateTreeVisuals();
    }

    private void OnDestroy()
    {
        UnsubscribeFromDayManager();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Cuts down the tree and resets its growth cycle
    /// </summary>
    public void CutDownTree()
    {
        _currentState = CUT_DOWN_STATE;
        UpdateTreeVisuals();
        SaveTreeState();
    }

    /// <summary>
    /// Saves the current tree state to PlayerPrefs
    /// </summary>
    public void SaveTreeState()
    {
        PlayerPrefs.SetInt(GetSaveKey(), _currentState);
        PlayerPrefsMigrationManager.Instance.RegisterIntPref(GetSaveKey());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the tree state from PlayerPrefs
    /// </summary>
    public void LoadTreeState()
    {
        _currentState = PlayerPrefs.GetInt(GetSaveKey(), DEFAULT_STATE);
    }

    /// <summary>
    /// Gets the current state of the tree
    /// </summary>
    public int GetCurrentState()
    {
        return _currentState;
    }

    /// <summary>
    /// Sets the tree state (for loading from save)
    /// </summary>
    public void SetTreeState(int state)
    {
        _currentState = Mathf.Clamp(state, DEFAULT_STATE, CUT_DOWN_STATE);
        UpdateTreeVisuals();
    }
    #endregion

    #region Private Methods
    private void InitializeDayManager()
    {
        _dayManager = FindFirstObjectByType<DayManager>();

        if (_dayManager != null)
        {
            _dayManager.OnDayChanged += OnDayChanged;
        }
        else
        {
            Debug.LogWarning("DayManager not found in scene");
        }
    }

    private void UnsubscribeFromDayManager()
    {
        if (_dayManager != null)
        {
            _dayManager.OnDayChanged -= OnDayChanged;
        }
    }

    private void OnDayChanged()
    {
        UpdateTreeState();
        UpdateTreeVisuals();
        SaveTreeState();
    }

    private void UpdateTreeState()
    {
        if (_currentState < ADULT_STATE)
        {
            _currentState++;
        }
        else if (_currentState == CUT_DOWN_STATE)
        {
            _currentState = SPROUT_STATE;
        }
    }

    private void UpdateTreeVisuals()
    {
        SetAllTreesInactive();

        switch (_currentState)
        {
            case SPROUT_STATE:
                treeSprout.SetActive(true);
                break;
            case MIDDLE_STATE:
                treeMiddle.SetActive(true);
                break;
            case ADULT_STATE:
                treeAdult.SetActive(true);
                break;
            case CUT_DOWN_STATE:
                // All trees remain inactive
                break;
            default:
                Debug.LogWarning($"Unknown tree state: {_currentState}");
                break;
        }
    }

    private void SetAllTreesInactive()
    {
        treeSprout.SetActive(false);
        treeMiddle.SetActive(false);
        treeAdult.SetActive(false);
    }

    private void GenerateTreeId()
    {
        // Generate unique ID based on tree's position and scene
        Vector3 position = transform.position;
        _treeId = $"{position.x:F2}_{position.y:F2}_{position.z:F2}";
    }

    private string GetSaveKey()
    {
        return $"{TREE_SAVE_PREFIX}{_treeId}";
    }
    #endregion
}