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
        foreach(var arrow in door[0].GetComponentsInChildren<SpriteRenderer>())
        {
            arrow.color = Color.white;
        }
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
            // ¬ыводим результаты в консоль
            for (int i = 1; i < doorsList.Count; i++)
            {
                GameObject door = doorsList[i];
                door.GetComponent<Door>().eventId = (int)doorEvents[i];
                doorData.DoorDataClass.eventIds.Add((int)doorEvents[i]);
                var textMeshPro = door.GetComponentInChildren<TextMeshPro>();
                if (doorEvents[i] == DoorEvent.Battle)
                {
                    string opponent = GetRandomOpponent();
                    string battleMessage = $"Door {i}: figth with {opponent}!";
                    textMeshPro.enabled = false;
                    string buffOrDebuff = GetRandomBuffOrDebuff();
                    textMeshPro.text = battleMessage + '\n' + buffOrDebuff;
                    doorData.DoorDataClass.doorDescription.Add(battleMessage + '\n' + buffOrDebuff);
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

    private DoorEvent GetRandomEvent()
    {
        float randNum = Random.value; // √енерируем случайное число от 0 до 1

        // ќпредел€ем событи€ и их веро€тности
        if (randNum < 0.7f) // 70% веро€тность бо€
        {
            return DoorEvent.Battle;
        }
        else if (randNum < 0.8f) // 10% веро€тность сундука (70% + 10%)
        {
            return DoorEvent.Chest;
        }
        else if (randNum < 0.9f) // 10% веро€тность фонтана (70% + 10% + 10%)
        {
            return DoorEvent.Fountain;
        }
        else
        {
            return DoorEvent.Store;
        }
    }

    private string GetRandomOpponent()
    {
        string[] opponents = { "Goblin", "Skeleton", "Ogre", "Dragon", "Vampire" };
        int randomIndex = Random.Range(0, opponents.Length);
        return opponents[randomIndex];
    }

    private string GetRandomBuffOrDebuff()
    {
        string[] effects = { "Increased Strength", "Reduced defense", "Increased attack speed", "Reduced health", "Increased magic damage", "Reduced mana" };
        int randomIndex = Random.Range(0, effects.Length);
        return effects[randomIndex];
    }
}