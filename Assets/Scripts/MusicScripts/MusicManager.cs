using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Проверяем, существует ли уже экземпляр MusicManager
        if (instance == null)
        {
            instance = this;
            Debug.Log("if");
            DontDestroyOnLoad(gameObject); // Не уничтожаем этот объект при загрузке новой сцены
        }
        else
        {
            Debug.Log("Else");
            Destroy(gameObject); // Уничтожаем дубликаты
        }
    }
}
