using UnityEngine;
using UnityEngine.UI;

public class EscManager : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private Canvas escCanvas;
    [SerializeField] private GameObject escPanel;

    [Header("UI Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        // Инициализация кнопок
        InitializeButtons();

        // Скрываем канвас при старте
        if (escCanvas != null)
            escCanvas.enabled = false;

        if (escPanel != null)
            escPanel.SetActive(false);
    }

    void Update()
    {
        // Обработка нажатия ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscMenu();
        }
    }

    private void InitializeButtons()
    {
        // Назначаем методы на кнопки
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveGame);

        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitGame);
    }

    public void ToggleEscMenu()
    {
        isPaused = !isPaused;

        if (escCanvas != null)
            escCanvas.enabled = isPaused;

        if (escPanel != null)
            escPanel.SetActive(isPaused);

        // Пауза игры
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // === МЕТОДЫ ДЛЯ КНОПОК ===

    public void OnSaveGame()
    {
        Debug.Log("Сохранение игры...");
        // Реализация сохранения
        // SaveSystem.SaveGame();
    }

    public void OnLoadGame()
    {
        Debug.Log("Загрузка игры...");
        // Реализация загрузки
        // SaveSystem.LoadGame();

        // После загрузки выходим из меню
        CloseMenu();
    }

    public void OnSettings()
    {
        Debug.Log("Открытие настроек...");
        // Можно открыть отдельную панель настроек
        // settingsPanel.SetActive(true);
    }

    public void OnMainMenu()
    {
        Debug.Log("Выход в главное меню...");

        // Снимаем паузу перед загрузкой меню
        Time.timeScale = 1f;

        // Загрузка главного меню
        // SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitGame()
    {
        Debug.Log("Выход из игры...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Вспомогательный метод для закрытия меню
    private void CloseMenu()
    {
        isPaused = false;
        if (escCanvas != null)
            escCanvas.enabled = false;
        if (escPanel != null)
            escPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Для очистки событий при уничтожении объекта
    private void OnDestroy()
    {
        if (saveButton != null)
            saveButton.onClick.RemoveAllListeners();
        if (loadButton != null)
            loadButton.onClick.RemoveAllListeners();
        if (settingsButton != null)
            settingsButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveAllListeners();
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }
}