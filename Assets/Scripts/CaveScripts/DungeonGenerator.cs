using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    [HideInInspector] public Room[,] rooms;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        rooms = new Room[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Room newRoom = new Room(new Vector3(x, 0, y));
                rooms[x, y] = newRoom;

                // Пример добавления дверей
                if (x > 0) // Соседняя комната слева
                {
                    newRoom.AddDoor("left", rooms[x - 1, y]);
                }
                if (y > 0) // Соседняя комната снизу
                {
                    newRoom.AddDoor("down", rooms[x, y - 1]);
                }
                // Добавьте другие направления по необходимости
            }
        }
    }
}
