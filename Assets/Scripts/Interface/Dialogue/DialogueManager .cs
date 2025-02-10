using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TextMeshPro dialogueText;
    public GameObject responseButtonPrefab;
    public Transform responsesContainer;
    private Dialogue currentDialogue;
    private NPC currentNPC;
    public void StartDialogue(Dialogue dialogue, NPC npc)
    {
        currentDialogue = dialogue;
        currentNPC = npc.GetComponent<NPC>();
        DisplayDialogue();
    }
    private void DisplayDialogue()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        dialogueText.text = currentDialogue.dialogueText;

        // Удаляем старые кнопки
        foreach (Transform child in responsesContainer)
        {
            Destroy(child.gameObject);
        }
        // Создаем кнопки для ответов
        foreach (var response in currentDialogue.responses)
        {
            GameObject button = Instantiate(responseButtonPrefab, responsesContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
            // Настраиваем действие кнопки
            if (response.quest)
            {
                Quest quest = new Quest(response.questName, response.questDescription, response.necessaryProgress);
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelectedQuest(response, quest));
            }
            else if(response.questComplete)
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelectedQuestCompleted(response, response.idQuestComplete));
            }
            else
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
            }
        }
    }
    private void OnResponseSelected(Response response)
    {
        if (response.nextDialogue != null)
        {
            currentDialogue = response.nextDialogue;
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void OnResponseSelectedQuest(Response response, Quest quest)
    {
        FindFirstObjectByType<QuestManager>().AddQuest(quest);
        PlayerPrefs.SetInt(currentNPC.gameObject.name, 1);
        if (response.nextDialogue != null)
        {
            currentDialogue = response.nextDialogue;
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void OnResponseSelectedQuestCompleted(Response response, int questID)
    {
        FindFirstObjectByType<QuestManager>().CompleteQuest(questID);
        PlayerPrefs.SetInt(currentNPC.gameObject.name, 1);
        if (response.nextDialogue != null)
        {
            currentDialogue = response.nextDialogue;
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }
    public void EndDialogue()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        dialogueText.text = "";
        foreach (Transform child in responsesContainer)
        {
            Destroy(child.gameObject);
        }
    }
}