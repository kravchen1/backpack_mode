using System.Collections.Generic;
using UnityEngine;

public class NPCBackpackManager : MonoBehaviour
{
    public List<NPCDataManager> _NPCDataManagers;


    public void Start()
    {
        //нужно чтобы сработало один раз в начале игры и потом даже после загрузки не работало
        if (!PlayerPrefs.HasKey("NPCBackpacksGenerate"))
        {
            GenerateNPCBackpack();
            PlayerPrefs.SetInt("NPCBackpacksGenerate", 1);
            PlayerPrefs.Save();
        }
    }

    public void GenerateNPCBackpack()
    {
        for(int i = 0 ; i < _NPCDataManagers.Count ; i++)
        {
            switch (_NPCDataManagers[i].backpackKey)
            {
                case "NPCTest":
                    PlayerPrefs.SetString(_NPCDataManagers[i].backpackKey,
                                                                        @"{
                                                    ""inventoryDataJsonList"": [
                                                        {
                                                            ""cellName"": ""cell3 (2)"",
                                                            ""cellNestedObjectName"": ""GunTest"",
                                                            ""rotationZ"": 0.0,
                                                            ""occupiedCells"": [""cell3 (2)"", ""cell4 (2)"", ""cell5 (2)""],
                                                            ""qualityKey"": 1,
                                                            ""durability"": 85.0,
                                                            ""countStack"": 0
                                                        }
                                                    ]
                                                }");
                    break;
                default:
                    //могут быть и другие ключи
                    break;
            }
        }
    }
}