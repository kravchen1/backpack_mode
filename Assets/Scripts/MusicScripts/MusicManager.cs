using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // ���������, ���������� �� ��� ��������� MusicManager
        if (instance == null)
        {
            instance = this;
            Debug.Log("if");
            DontDestroyOnLoad(gameObject); // �� ���������� ���� ������ ��� �������� ����� �����
        }
        else
        {
            Debug.Log("Else");
            Destroy(gameObject); // ���������� ���������
        }
    }
}
