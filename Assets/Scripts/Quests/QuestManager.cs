using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();
    //public TextMeshProUGUI questText; // Убедитесь, что вы привязали этот элемент из UI в Inspector
    public GameObject prefabQuestText;
    [HideInInspector] public Transform questsContainer;

    public QuestData questData;
    void Start()
    {
        questData = new QuestData();
        questData.questData = new QDataList();
        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json")))
        {
            questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
            quests.AddRange(questData.questData.quests);
            UpdateQuestUI();
        }
    }
    public void AddQuest(Quest quest)
    {
        questData.questData.quests.Add(quest);
        questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));  
        quests.Add(quest);
        UpdateQuestUI();
    }
    public void CompleteQuest(int questID)
    {
        var quest = questData.questData.quests.Where(e => e.id == questID).ToList();
        quest[0].isCompleted = true;
        questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
        UpdateQuestUI();
    }
    private void UpdateQuestUI()
    {
        // Обновите ваш интерфейс здесь
        // Например, пройдитесь по всем квестам и обновите текст элементов UI
        //удаляем старые квесты
        foreach (Transform child in questsContainer)
        {
            Destroy(child.gameObject);
        }
        // Создаем новые
        foreach (Quest quest in quests)
        {
            if (!quest.isCompleted)
            {
                GameObject questGO = Instantiate(prefabQuestText, questsContainer);
                var textDescr = "";
                if (quest.necessaryProgress == -1)
                {
                    textDescr = quest.description;// + " (" + quest.currentProgress.ToString() + "/" + quest.necessaryProgress.ToString() + ")";
                }
                else
                {
                    textDescr = quest.description + " (" + quest.currentProgress.ToString() + "/" + quest.necessaryProgress.ToString() + ")";
                }
                //questGO.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = quest.questName;
                questGO.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = textDescr;
            }
        }
    }

    public void ToogleActive()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(!gameObject.transform.GetChild(0).gameObject.activeSelf);
        gameObject.transform.GetChild(1).gameObject.SetActive(!gameObject.transform.GetChild(1).gameObject.activeSelf);

        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.needGPSTracker = !player.needGPSTracker;


    }
}