using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private Canvas newGameCanvas;
    [SerializeField] private Canvas loadGameCanvas;
    [SerializeField] private Canvas educationCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Canvas creatorsCanvas;

    [Header("Button References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button educationButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creatorsButton;
    [SerializeField] private Button exitGameButton;

    private void Awake()
    {
        // Инициализация кнопок
        InitializeButtons();

        // Скрываем все дополнительные канвасы при старте
        HideAllCanvases();

        // Показываем только главное меню
        ShowMainMenu();
    }

    private void InitializeButtons()
    {
        // Подписываемся на события кнопок
        newGameButton.onClick.AddListener(OnNewGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        educationButton.onClick.AddListener(OnEducationClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creatorsButton.onClick.AddListener(OnCreatorsClicked);
        exitGameButton.onClick.AddListener(OnExitGameClicked);
    }

    private void OnNewGameClicked()
    {
        HideAllCanvases();
        ShowCanvas(newGameCanvas);
        Debug.Log("New Game button clicked");

        // Здесь можно добавить дополнительную логику для новой игры
    }

    private void OnLoadGameClicked()
    {
        HideAllCanvases();
        ShowCanvas(loadGameCanvas);
        Debug.Log("Load Game button clicked");

        // Здесь можно добавить логику загрузки игры
    }

    private void OnEducationClicked()
    {
        HideAllCanvases();
        ShowCanvas(educationCanvas);
        Debug.Log("Education button clicked");
    }

    private void OnSettingsClicked()
    {
        HideAllCanvases();
        ShowCanvas(settingsCanvas);
        Debug.Log("Settings button clicked");
    }

    private void OnCreatorsClicked()
    {
        HideAllCanvases();
        ShowCanvas(creatorsCanvas);
        Debug.Log("Creators button clicked");
    }

    private void OnExitGameClicked()
    {
        Debug.Log("Exit Game button clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Вспомогательные методы для управления канвасами
    private void HideAllCanvases()
    {
        Canvas[] allCanvases = { newGameCanvas, loadGameCanvas, educationCanvas, settingsCanvas, creatorsCanvas };

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }
    }

    private void ShowCanvas(Canvas canvas)
    {
        if (canvas != null)
            canvas.gameObject.SetActive(true);
    }

    private void ShowMainMenu()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.gameObject.SetActive(true);
    }

    // Метод для возврата в главное меню (можно вызвать из других канвасов)
    public void ReturnToMainMenu()
    {
        HideAllCanvases();
        ShowMainMenu();
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        newGameButton.onClick.RemoveListener(OnNewGameClicked);
        loadGameButton.onClick.RemoveListener(OnLoadGameClicked);
        educationButton.onClick.RemoveListener(OnEducationClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        creatorsButton.onClick.RemoveListener(OnCreatorsClicked);
        exitGameButton.onClick.RemoveListener(OnExitGameClicked);
    }
}