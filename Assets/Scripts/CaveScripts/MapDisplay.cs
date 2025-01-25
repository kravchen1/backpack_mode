using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    void OnDrawGizmos()
    {
        if (dungeonGenerator == null) return;

        foreach (var room in dungeonGenerator.rooms)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(room.Position + new Vector3(100, 100, 0), Vector3.one + new Vector3(100, 100, 0)); // Отображение комнаты

            foreach (var door in room.Doors)
            {
                // Отображение двери (можно изменить на более сложную логику)
                Gizmos.color = Color.red;
                Gizmos.DrawLine(room.Position + new Vector3(100,100,0), door.Value.Position + new Vector3(100, 100, 0));
            }
        }
    }
}
