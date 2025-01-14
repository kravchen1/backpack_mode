using UnityEngine;

public class MainMusicManager : MonoBehaviour
{
    private static MainMusicManager instance;

    void Awake()
    {
        // Проверяем, существует ли уже экземпляр MusicManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожаем этот объект при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликаты
        }
    }
}
