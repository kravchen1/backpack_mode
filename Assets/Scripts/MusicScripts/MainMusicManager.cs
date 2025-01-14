using UnityEngine;

public class MainMusicManager : MonoBehaviour
{
    private static MainMusicManager instance;

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
}
