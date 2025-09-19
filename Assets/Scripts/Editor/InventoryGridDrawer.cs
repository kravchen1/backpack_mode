#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemStructure))]
public class ItemStructureEditor : Editor
{
    private const float CELL_SIZE = 25f;
    private const float SPACING = 2f;

    public override void OnInspectorGUI()
    {
        // Отрисовываем стандартные поля
        DrawDefaultInspector();

        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Forma Predmeta", EditorStyles.boldLabel);

        ItemStructure item = (ItemStructure)target;

        // Рисуем сетку
        DrawGrid(item);

        // Кнопки управления
        EditorGUILayout.Space(10f);
        if (GUILayout.Button("Ochistit' vse"))
        {
            ClearGrid(item);
        }

        if (GUILayout.Button("Zapolnit' vse"))
        {
            FillGrid(item);
        }
    }

    private void DrawGrid(ItemStructure item)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        // Подписи осей
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(20f));
        for (int x = 0; x < item.Size.x; x++)
        {
            EditorGUILayout.LabelField($"X{x + 1}", GUILayout.Width(CELL_SIZE));
        }
        EditorGUILayout.EndHorizontal();

        // Сетка
        for (int y = 0; y < item.Size.y; y++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Y{y + 1}", GUILayout.Width(20f));

            for (int x = 0; x < item.Size.x; x++)
            {
                DrawCell(item, x, y);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawCell(ItemStructure item, int x, int y)
    {
        bool currentValue = item.GetCell(x, y);
        int index = y * item.Size.x + x;

        // Создаем текстуру для фона
        Texture2D bgTexture = new Texture2D(1, 1);
        bgTexture.SetPixel(0, 0, currentValue ? new Color(0.3f, 0.8f, 0.3f, 1f) : new Color(0.8f, 0.8f, 0.8f, 0.3f));
        bgTexture.Apply();

        GUIStyle cellStyle = new GUIStyle(GUI.skin.button);
        cellStyle.normal.background = bgTexture;
        cellStyle.fixedWidth = CELL_SIZE;
        cellStyle.fixedHeight = CELL_SIZE;

        if (GUILayout.Button($"{x + 1}{y + 1}", cellStyle))
        {
            Undo.RecordObject(item, "Toggle Inventory Cell");
            item.SetCell(x, y, !currentValue);
            EditorUtility.SetDirty(item);
        }
    }

    private void ClearGrid(ItemStructure item)
    {
        Undo.RecordObject(item, "Clear Grid");
        for (int i = 0; i < item.Cells.Length; i++)
        {
            item.SetCell(i % item.Size.x, i / item.Size.x, false);
        }
        EditorUtility.SetDirty(item);
    }

    private void FillGrid(ItemStructure item)
    {
        Undo.RecordObject(item, "Fill Grid");
        for (int i = 0; i < item.Cells.Length; i++)
        {
            item.SetCell(i % item.Size.x, i / item.Size.x, true);
        }
        EditorUtility.SetDirty(item);
    }
}
#endif