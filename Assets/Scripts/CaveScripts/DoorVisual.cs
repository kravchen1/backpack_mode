using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorVisual : MonoBehaviour
{
    public GameObject doorPrefab; // Префаб двери
    public GameObject levelPrefab; // Префаб уровня
    private float levelSpacing = 150f; // Расстояние между уровнями
    private float doorSpacing = 150f; // Расстояние между уровнями
    private List<Level> levels;
    private Color color;
    private DrawLine drawLine;

    private List<Door> doorsForLines;

    void Start()
    {
        drawLine = GetComponent<DrawLine>();
        doorsForLines = new List<Door>();
        levels = new List<Level>();
        levels = GetComponent<GameManager>().levels;
        GenerateLevels(); // Генерируем уровни (можно использовать предыдущий код)
        VisualizeLevels();
    }

    void GenerateLevels()
    {
        // Генерация уровней (можно использовать код из предыдущего ответа)
        // Добавьте здесь код генерации уровней
    }

    void VisualizeLevels()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            Level level = levels[i];
            // Создание объектов дверей
            for (int j = 0; j < level.doors.Count; j++)
            {
                Door door = level.doors[j];
                GameObject doorObject = Instantiate(doorPrefab, new Vector3(gameObject.GetComponent<RectTransform>().anchoredPosition.x + j * doorSpacing - (level.doors.Count - 1), gameObject.GetComponent<RectTransform>().anchoredPosition.y + i * levelSpacing, 0), Quaternion.identity, gameObject.transform);
                doorObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(gameObject.GetComponent<RectTransform>().anchoredPosition.x + j * doorSpacing - (level.doors.Count - 1), gameObject.GetComponent<RectTransform>().anchoredPosition.y + i * levelSpacing, 0);
                doorObject.name = door.name;
                door.transform = doorObject.transform;
            }
        }
        for (int i = 0; i < levels.Count; i++)
        {
            Level level = levels[i];
            for (int j = 0; j < level.doors.Count; j++)
            {
                Door door = level.doors[j];
                switch (j)
                {
                    case 0:
                        color = Color.white;
                        break;
                    case 1:
                        color = Color.red;
                        break;
                    case 2:
                        color = Color.green;
                        break;
                    case 3:
                        color = Color.blue;
                        break;
                    case 4:
                        color = Color.yellow;
                        break;
                }
                     
                for (int k = 0; k < door.nextDoors.Count; k++)
                {
                    Door nextDoor = door.nextDoors[k];
                    drawLine.DrawLineBetweenDoors(door.transform, nextDoor.transform, color);
                }
            }
        }
    }
}
