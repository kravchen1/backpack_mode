using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();
    //public TextMeshProUGUI questText; // Убедитесь, что вы привязали этот элемент из UI в Inspector
    public GameObject questText;
    public Transform questsContainer;
    void Start()
    {
        Quest quest1 = new Quest("Поиск кольца", "Найти золотое кольцо в лесу.");
        AddQuest(quest1);
    }
    public void AddQuest(Quest quest)
    {
        quests.Add(quest);
        UpdateQuestUI();
    }
    public void CompleteQuest(Quest quest)
    {
        quest.isCompleted = true;
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
            GameObject questGO = Instantiate(questText, questsContainer);
            var textDescr = quest.description + " (" + quest.currentProgress.ToString() + "/" + quest.necessaryProgress.ToString() + ")";
            questGO.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = quest.questName;
            questGO.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = textDescr;
            // Настраиваем действие кнопки
            //button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
        }
    }
}