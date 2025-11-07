using UnityEngine;
using System.Collections.Generic;

public class TreeManager : MonoBehaviour
{
    #region Singleton Pattern
    private static TreeManager _instance;
    public static TreeManager Instance => _instance;
    #endregion

    #region Private Variables
    private List<Tree> _allTrees = new List<Tree>();
    private const string TREES_SAVED_KEY = "TreesDataSaved";
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        FindAllTrees();
    }

    //private void OnApplicationQuit()
    //{
    //    SaveAllTreesState();
    //}

    //private void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus)
    //    {
    //        SaveAllTreesState();
    //    }
    //}
    #endregion

    #region Public Methods
    /// <summary>
    /// Finds all trees in the scene and registers them
    /// </summary>
    public void FindAllTrees()
    {
        _allTrees.Clear();
        Tree[] treesInScene = FindObjectsByType<Tree>(FindObjectsSortMode.None);
        _allTrees.AddRange(treesInScene);

        //Debug.Log($"Found {_allTrees.Count} trees in scene");
    }

    /// <summary>
    /// Saves the state of all trees to PlayerPrefs
    /// </summary>
    public void SaveAllTreesState()
    {
        foreach (Tree tree in _allTrees)
        {
            if (tree != null)
            {
                tree.SaveTreeState();
            }
        }

        Debug.Log($"Saved state for {_allTrees.Count} trees");
    }

    /// <summary>
    /// Loads the state for all trees from PlayerPrefs
    /// </summary>
    public void LoadAllTreesState()
    {
        foreach (Tree tree in _allTrees)
        {
            if (tree != null)
            {
                tree.LoadTreeState();
            }
        }

        Debug.Log($"Loaded state for {_allTrees.Count} trees");
    }

    /// <summary>
    /// Registers a new tree with the manager
    /// </summary>
    public void RegisterTree(Tree tree)
    {
        if (!_allTrees.Contains(tree))
        {
            _allTrees.Add(tree);
        }
    }

    /// <summary>
    /// Unregisters a tree from the manager
    /// </summary>
    public void UnregisterTree(Tree tree)
    {
        _allTrees.Remove(tree);
    }

    /// <summary>
    /// Clears all saved tree data from PlayerPrefs
    /// </summary>

    /// <summary>
    /// Gets the total number of registered trees
    /// </summary>
    public int GetTreeCount()
    {
        return _allTrees.Count;
    }
    #endregion

    #region Private Methods
    private void InitializeSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}