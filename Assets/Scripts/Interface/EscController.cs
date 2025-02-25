using UnityEngine;

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

        // ������������� ��� ������������ �����
        Time.timeScale = isPaused ? 0 : 1;

        // �������� ��� ��������� ���� �����
        if (ToogleCanvas != null)
        {
            ToogleCanvas.SetActive(isPaused);
        }
    }
}

