using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // ���������, ���������� �� ��� ��������� MusicManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ���������� ���� ������ ��� �������� ����� �����
        }
        else
        {
            Destroy(gameObject); // ���������� ���������
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���������, ���� ������� ����� ��������� � �������� � ������ �� ���� �����������
        if (scene.name == "BackPackBattle" || scene.name == "Main" || scene.name == "Cave")
        {
            GetComponent<AudioSource>().Stop();
            Destroy(gameObject); // ���������� ������ � �������
        }
    }
}
