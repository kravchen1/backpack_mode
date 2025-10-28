using UnityEngine;
using UnityEngine.UI;

public class EscMenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private Canvas saveGameCanvas;
    [SerializeField] private Canvas loadGameCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Canvas sureExitGameCanvas;
    [SerializeField] private Canvas sureExitInMainMenuCanvas;

    [Header("Button References")]
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitInMainMenuButton;
    [SerializeField] private Button exitGameButton;

    private void Awake()
    {
        // Скрываем все дополнительные канвасы при старте
        HideAllCanvases();
    }
    public void OnSaveGameClicked()
    {
        HideAllCanvases();
        ShowCanvas(saveGameCanvas);
        saveGameCanvas.GetComponent<SaveFileManager>().RefreshSaveFilesList();
    }
    public void OnLoadGameClicked()
    {
        HideAllCanvases();
        ShowCanvas(loadGameCanvas);
        loadGameCanvas.GetComponent<LoadFileManager>().RefreshSaveFilesList();
    }
    public void OnSettingsClicked()
    {
        HideAllCanvases();
        ShowCanvas(settingsCanvas);
    }
    public void OnSureExitInMainMenuCanvasClicked()
    {
        HideAllCanvases();
        ShowCanvas(settingsCanvas);
    }
    public void OnSureExitGameCanvasClicked()
    {
        HideAllCanvases();
        ShowCanvas(loadGameCanvas);
        loadGameCanvas.GetComponent<LoadFileManager>().RefreshSaveFilesList();
    }


    public void OnExitGame()
    {
        Debug.Log("Exit Game button clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Вспомогательные методы для управления канвасами
    public void HideAllCanvases()
    {
        Canvas[] allCanvases = { saveGameCanvas, loadGameCanvas, sureExitGameCanvas, settingsCanvas, sureExitInMainMenuCanvas };

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }
    }

    public void ShowCanvas(Canvas canvas)
    {
        if (canvas != null)
            canvas.gameObject.SetActive(true);
    }
}