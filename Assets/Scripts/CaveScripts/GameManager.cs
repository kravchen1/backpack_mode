using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<Level> levels;
    private const int totalLevels = 5;
    private const int minDoorsPerLevel = 2;
    private const int maxDoorsPerLevel = 4;
    private DrawLine drawLine;
    private List<int> possibleValues = new List<int>();

    void Awake()
    {
        drawLine = GetComponent<DrawLine>();
        levels = new List<Level>();
        GenerateLevels();
    }

    void GenerateLevels()
    {
        for (int i = 0; i < totalLevels; i++)
        {
            Level level = new Level(i);
            int numberOfDoors = Random.Range(minDoorsPerLevel, maxDoorsPerLevel + 1);

            // Создание дверей для текущего уровня
            for (int j = 0; j < numberOfDoors; j++)
            {
                Door door = new Door($"Дверь {j} на уровне {i}", transform);
                level.AddDoor(door);
            }

            //// Если это не последний уровень, связываем двери с дверями следующего уровня
            //if (i < totalLevels - 1)
            //{


            //    //// Создание дверей для следующего уровня
            //    //for (int j = 0; j < nextLevelDoorCount; j++)
            //    //{
            //    //    Door nextDoor = new Door($"Дверь {j + 1} на уровне {i + 2}");
            //    //    nextLevel.AddDoor(nextDoor);
            //    //}

            //    // Добавляем следующий уровень в список уровней
            //    levels.Add(nextLevel);
            //}

            // Добавляем текущий уровень в список уровней
            levels.Add(level);
        }
        for (int i = 0; i < levels.Count; i++)
        {
            if (i < totalLevels - 1)
            {
                Level level = levels[i];
                Level nextLevel = levels[i + 1];
                int nextLevelDoorCount = Random.Range(minDoorsPerLevel, maxDoorsPerLevel + 1);
                //Связываем двери текущего уровня с дверями следующего уровня
                foreach (var door in level.doors)
                {
                    BuildPossibleValues(nextLevel.doors.Count);
                    int randomNextDoorsCount = Random.Range(1, nextLevel.doors.Count + 1);
                    for(int j = 0;j < randomNextDoorsCount; j++)
                    {
                        int randomNextDoorIndex = Random.Range(0, possibleValues.Count);
                        door.nextDoors.Add(nextLevel.doors[possibleValues[randomNextDoorIndex]]);
                        if(j != randomNextDoorsCount - 1)
                            GetRandomValueExcluding(randomNextDoorIndex);
                    }
                    possibleValues.Clear();
                }
            }
        }
        // Выводим информацию о сгенерированных уровнях и дверях
        PrintLevels();
    }

    void BuildPossibleValues(int maxValues)
    {
        possibleValues.Clear();
        for (int i = 0; i < maxValues; i++)
        {
            possibleValues.Add(i);
        }
    }

    int GetRandomValueExcluding(int exclude)
    {
        // Заполняем список возможными значениями
        for(int i = 0; i < possibleValues.Count; i++) 
        {
            if (i == exclude)
            {
                possibleValues.Remove(i);
            }
        }
        // Выбираем случайное значение из оставшихся
        int randomIndex = Random.Range(0, possibleValues.Count);
        return possibleValues[randomIndex];
    }
    void PrintLevels()
    {
        foreach (var level in levels)
        {
            Debug.Log($"Уровень {level.levelNumber}:");
            foreach (var door in level.doors)
            {
                Debug.Log($" - {door.name} (Ведет к: {door.nextDoors.Count} дверям)");
            }
        }
    }
}