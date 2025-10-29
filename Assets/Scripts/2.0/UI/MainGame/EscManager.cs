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
    private bool isPlayerDead = false; // Флаг смерти игрока

    void Start()
    {
        // Инициализация кнопок
        InitializeButtons();

        // Скрываем канвас при старте
        if (escCanvas != null)
            escCanvas.enabled = false;

        if (escPanel != null)
            escPanel.SetActive(false);

        // Подписка на событие смерти игрока
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.OnDeath += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        Debug.Log("Player died - ESC menu locked in paused state");

        // Автоматически включаем паузу и меню при смерти
        ForceOpenMenu();
    }

    // Принудительное открытие меню (используется при смерти)
    private void ForceOpenMenu()
    {
        isPaused = true;
        saveButton.gameObject.SetActive(false);
        if (escCanvas != null)
            escCanvas.enabled = true;

        if (escPanel != null)
            escPanel.SetActive(true);

        Time.timeScale = 0f;

        // Дополнительно можно заблокировать курсор
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
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
    }

    public void ToggleEscMenu()
    {
        if (isPlayerDead)
        {
            // Можно добавить звуковой эффект или визуальную обратную связь
            Debug.Log("Cannot close menu while player is dead");
            return;
        }

        isPaused = !isPaused;

        if (escCanvas != null)
            escCanvas.enabled = isPaused;

        if (escPanel != null)
            escPanel.SetActive(isPaused);

        // Пауза игры
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
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
        //CloseMenu();
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
}