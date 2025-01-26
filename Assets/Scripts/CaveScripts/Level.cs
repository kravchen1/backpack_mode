using System.Collections.Generic;

public class Level
{
    public int levelNumber;
    public List<Door> doors;

    public Level(int levelNumber)
    {
        this.levelNumber = levelNumber;
        doors = new List<Door>();
    }

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }
}
