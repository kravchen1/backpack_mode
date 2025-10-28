using System.Collections.Generic;
using TMPro;
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
            return; // Важно: прекращаем выполнение если объект уничтожается
        }
    }

    public void LoadScene(string sceneName, string _saveFilePath = "")
    {
        if (!string.IsNullOrEmpty(_saveFilePath))
        {
            if (PlayerPrefsMigrationManager.Instance != null)
            {
                PlayerPrefsMigrationManager.Instance._savePath = _saveFilePath;
                PlayerPrefsMigrationManager.Instance.ImportFromJson();
            }
        }
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadSceneNewGame(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void OpenLoadingCanvas()
    {
        int r = Random.Range(0, randomImages.Count);
        loadingScreen.GetComponent<Image>().sprite = randomImages[r];
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        else
            Debug.LogError("Loading Screen не назначен в инспекторе!");
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
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