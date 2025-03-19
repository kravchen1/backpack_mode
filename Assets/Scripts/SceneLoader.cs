using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private List<Sprite> randomImages;

    public Texture2D cursorTextureSceneGlobalIdle;
    public Texture2D cursorTextureSceneGlobalClick;
    public Texture2D cursorTextureSceneShopIdle;
    public Texture2D cursorTextureSceneShopIdleClick;

    private Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        int r = Random.Range(0, randomImages.Count);
        loadingScreen.GetComponent<Image>().sprite = randomImages[r];
        StartCoroutine(LoadSceneAsync(sceneName));

        // ћен€ем курсор в зависимости от сцены
        switch (sceneName)
        {
            case "Scene1":
                Cursor.SetCursor(cursorTextureSceneGlobalIdle, hotspot, CursorMode.Auto);
                break;
            case "Scene2":
                Cursor.SetCursor(cursorTextureSceneShopIdleClick, hotspot, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // —брос курсора
                break;
        }
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        else
            Debug.LogError("Loading Screen не назначен в инспекторе!");

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;
            else
                Debug.LogWarning("Progress Bar не назначен в инспекторе!");

            if (progressText != null)
                progressText.text = $"{progress * 100}%";
            else
                Debug.LogWarning("Progress Text не назначен в инспекторе!");

            if (progress >= 1f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
}