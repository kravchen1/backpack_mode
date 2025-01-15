using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();
    //public TextMeshProUGUI questText; // ���������, ��� �� ��������� ���� ������� �� UI � Inspector
    public GameObject questText;
    public Transform questsContainer;
    void Start()
    {
        Quest quest1 = new Quest("����� ������", "����� ������� ������ � ����.");
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
        // �������� ��� ��������� �����
        // ��������, ���������� �� ���� ������� � �������� ����� ��������� UI
        //������� ������ ������
        foreach (Transform child in questsContainer)
        {
            Destroy(child.gameObject);
        }
        // ������� �����
        foreach (Quest quest in quests)
        {
            GameObject questGO = Instantiate(questText, questsContainer);
            var textDescr = quest.description + " (" + quest.currentProgress.ToString() + "/" + quest.necessaryProgress.ToString() + ")";
            questGO.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = quest.questName;
            questGO.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = textDescr;
            // ����������� �������� ������
            //button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
        }
    }
}