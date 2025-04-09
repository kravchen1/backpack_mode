using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DoorEventDistributor : MonoBehaviour
{
    public List<GameObject> doorsList = new List<GameObject>();


    public GameObject glowCircle;

    [HideInInspector] public DoorData doorData;
    private enum DoorEvent
    {
        Battle,
        Chest,
        Fountain,
        Store,
        CaveBoss
    }

    private void Awake()
    {
        doorData = gameObject.GetComponent<DoorData>();
        DistributeEvents();
        LightArrows();
    }


    private void LightArrows()
    {
        var currentDoor = doorData.DoorDataClass.currentDoorId;
        var door = doorsList.Where(e => e.GetComponent<Door>().doorId == currentDoor).ToList();
        glowCircle.transform.SetParent(door[0].transform);
        glowCircle.transform.localPosition = Vector3.zero;
        glowCircle.GetComponent<ParticleSystem>().Play();
        //foreach(var arrow in door[0].GetComponentsInChildren<SpriteRenderer>().Where(e => e != door[0].GetComponent<SpriteRenderer>()))
        //{
        //    arrow.color = Color.red;
        //}
    }

    private void DistributeEvents()
    {
        doorData.LoadData();
        if (doorData.DoorDataClass.doorDescription.Count > 0)
        {
            for (int i = 1; i < doorsList.Count; i++)
            {
                GameObject door = doorsList[i];
                var textMeshPro = door.GetComponentInChildren<TextMeshPro>();
                textMeshPro.enabled = false;
                textMeshPro.text = doorData.DoorDataClass.doorDescription[i-1];
                door.GetComponent<Door>().eventId = doorData.DoorDataClass.eventIds[i-1];
            }
        }
        else
        {
            DoorEvent[] doorEvents = new DoorEvent[doorsList.Count];

            for (int i = 1; i < doorsList.Count - 1; i++)
            {
                doorEvents[i] = GetRandomEvent();
            }
            doorEvents[doorsList.Count-1] = DoorEvent.CaveBoss;
            // Выводим результаты в консоль
            for (int i = 1; i < doorsList.Count; i++)
            {
                GameObject door = doorsList[i];
                door.GetComponent<Door>().eventId = (int)doorEvents[i];
                doorData.DoorDataClass.eventIds.Add((int)doorEvents[i]);
                var textMeshPro = door.GetComponentInChildren<TextMeshPro>();
                if (doorEvents[i] == DoorEvent.Battle)
                {
                    //string opponent = GetRandomOpponent();
                    string battleMessage = $"Door {i}: figth!";
                    textMeshPro.enabled = false;
                    //string buffOrDebuff = GetRandomBuffOrDebuff();
                    textMeshPro.text = battleMessage;
                    doorData.DoorDataClass.doorDescription.Add(battleMessage);
                }
                else
                {
                    textMeshPro.enabled = false;
                    textMeshPro.text = $"Door {i}: {doorEvents[i]}";
                    doorData.DoorDataClass.doorDescription.Add($"Door {i}: {doorEvents[i]}");
                }
            }
            doorData.DoorDataClass.currentDoorId = 0;
            doorData.SaveData();
        }
    }

    private List<DoorEvent> _recentEvents = new List<DoorEvent>();
    private int _memorySize = 3; // Запоминаем последние 3 события

    private DoorEvent GetRandomEvent()
    {
        var baseProbabilities = new Dictionary<DoorEvent, float>()
    {
        { DoorEvent.Battle, 0.7f }, // Базовый шанс
        { DoorEvent.Chest, 0.1f },
        { DoorEvent.Fountain, 0.1f },
        { DoorEvent.Store, 0.1f }
    };

        // Уменьшаем шанс недавних событий
        foreach (var recentEvent in _recentEvents)
        {
            baseProbabilities[recentEvent] *= 0.5f;
        }

        // Выбираем событие
        DoorEvent selectedEvent = ChooseEvent(baseProbabilities);

        // Обновляем историю
        if(selectedEvent != DoorEvent.Battle)
            _recentEvents.Add(selectedEvent);
        if (_recentEvents.Count > _memorySize)
        {
            _recentEvents.RemoveAt(0);
        }

        return selectedEvent;
    }

    private DoorEvent ChooseEvent(Dictionary<DoorEvent, float> probabilities)
    {
        float total = probabilities.Values.Sum();
        float randNum = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var pair in probabilities)
        {
            cumulative += pair.Value;
            if (randNum < cumulative)
            {
                return pair.Key;
            }
        }

        return DoorEvent.Battle;
    }

    //private DoorEvent GetRandomEvent()
    //{
    //    float randNum = Random.value; // Генерируем случайное число от 0 до 1

    //    // Определяем события и их вероятности
    //    if (randNum < 0.7f) // 70% вероятность боя
    //    {
    //        return DoorEvent.Battle;
    //    }
    //    else if (randNum < 0.8f) // 10% вероятность сундука (70% + 10%)
    //    {
    //        return DoorEvent.Chest;
    //    }
    //    else if (randNum < 0.9f) // 10% вероятность фонтана (70% + 10% + 10%)
    //    {
    //        return DoorEvent.Fountain;
    //    }
    //    else
    //    {
    //        return DoorEvent.Store;
    //    }
    //}

    //private string GetRandomOpponent()
    //{
    //    string[] opponents = { "Goblin", "Skeleton", "Ogre", "Dragon", "Vampire" };
    //    int randomIndex = Random.Range(0, opponents.Length);
    //    return opponents[randomIndex];
    //}

    //private string GetRandomBuffOrDebuff()
    //{
    //    string[] effects = { "Increased Strength", "Reduced defense", "Increased attack speed", "Reduced health", "Increased magic damage", "Reduced mana" };
    //    int randomIndex = Random.Range(0, effects.Length);
    //    return effects[randomIndex];
    //}
}