using UnityEngine;
using UnityEngine.SceneManagement;

public class EscController : MonoBehaviour
{
    public GameObject ToogleCanvas;

    private bool isPaused = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        // Останавливаем или возобновляем время
        if(SceneManager.GetActiveScene().name != "BackPackBattle")
            Time.timeScale = isPaused ? 0 : 1;

        // Включаем или выключаем меню паузы
        if (ToogleCanvas != null)
        {
            ToogleCanvas.SetActive(isPaused);
        }
    }
}

