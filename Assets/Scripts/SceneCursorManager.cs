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
        // ������ ������ � ����������� �� �����
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
        // ���������, ������ �� ����� ������ ����
        if (Input.GetMouseButtonDown(0)) // 0 � ��� ����� ������ ����
        {
            // ������ ������ ��� �����
            if (shop)
            {
                Cursor.SetCursor(cursorTextureSceneShopClick, hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(cursorTextureSceneGlobalClick, hotspot, CursorMode.Auto);
            }

        }

        // ���������, �������� �� ����� ������ ����
        if (Input.GetMouseButtonUp(0))
        {
            // ���������� ������� ������ ��� ���������� ������ ����
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