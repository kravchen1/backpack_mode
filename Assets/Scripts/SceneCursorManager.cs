using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneCursorManager : MonoBehaviour
{
    public Texture2D cursorTextureSceneGlobalIdle;
    public Texture2D cursorTextureSceneGlobalClick;
    public Texture2D cursorTextureSceneShopIdle;
    public Texture2D cursorTextureSceneShopClick;
    public Vector2 hotspot = Vector2.zero;

    private bool shop = true;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Меняем курсор в зависимости от сцены
        if(scene.name.Contains("BackPack") && !scene.name.Contains("Battle"))
        {
            Cursor.SetCursor(cursorTextureSceneShopIdle, hotspot, CursorMode.Auto);
            shop = true;
        }
        else
        {
            Cursor.SetCursor(cursorTextureSceneGlobalIdle, hotspot, CursorMode.Auto);
            shop = false;
        }
    }


    void Update()
    {
        // Проверяем, нажата ли левая кнопка мыши
        if (Input.GetMouseButtonDown(0)) // 0 — это левая кнопка мыши
        {
            // Меняем курсор при клике
            if (shop)
            {
                Cursor.SetCursor(cursorTextureSceneShopClick, hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(cursorTextureSceneGlobalClick, hotspot, CursorMode.Auto);
            }

        }

        // Проверяем, отпущена ли левая кнопка мыши
        if (Input.GetMouseButtonUp(0))
        {
            // Возвращаем обычный курсор при отпускании кнопки мыши
            if (shop)
            {
                Cursor.SetCursor(cursorTextureSceneShopIdle, hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(cursorTextureSceneGlobalIdle, hotspot, CursorMode.Auto);
            }
        }
    }
}