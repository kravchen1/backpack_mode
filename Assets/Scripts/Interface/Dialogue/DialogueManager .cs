using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject responseButtonPrefab;
    public Transform responsesContainer;
    public RectTransform Content;
    private Dialogue currentDialogue;
    private NPC currentNPC;


    private BackPackAndStorageData backPackAndStorageData;
    public void StartDialogue(Dialogue dialogue, NPC npc)
    {
        currentDialogue = dialogue;
        currentNPC = npc.GetComponent<NPC>();
        DisplayDialogue();
    }
    private void DisplayDialogue()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.GetComponent<Image>().enabled = true;


        //currentDialogue.HeightText;
        dialogueText.text = currentDialogue.dialogueText;

        dialogueText.ForceMeshUpdate();
        
        Content.sizeDelta = new Vector2(
                                Content.sizeDelta.x,
                                dialogueText.preferredHeight - 200 // Используем preferredHeight для автоматического расчета высоты
        );

        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);

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
                Quest quest = new Quest(response.questName, response.questDescription, response.necessaryProgress, response.questID);
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

    private void giveItem(GameObject item)
    {
        backPackAndStorageData = new BackPackAndStorageData();
        backPackAndStorageData.storageData = new BackpackData();
        backPackAndStorageData.storageData.itemData = new ItemData();
        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json")))
        {
            backPackAndStorageData.storageData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
        }
        
        backPackAndStorageData.storageData.itemData.items.Add(new Data(item.name, new Vector2(0, 0)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));


        //BackpackData storageData = new BackpackData();
        //storageData.itemData = new ItemData();
        //Data data = null;


        //data = new Data(item.name, new Vector3(0f,0f,0f));

        //storageData.itemData.items.Add(data);
        //storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));


    }

    private void OnResponseSelected(Response response)
    {
        if(response.giveItem)
        {
            foreach(var item in response.giveItemPrefab)
            {
                giveItem(item);
            }
        }

        if (response.switchDialogID >= 0)
        {
            PlayerPrefs.SetInt(currentNPC.gameObject.name, response.switchDialogID);
        }
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
        if (response.giveItem)
        {
            foreach (var item in response.giveItemPrefab)
            {
                giveItem(item);
            }
        }

        FindFirstObjectByType<QuestManager>().AddQuest(quest);

        if (response.switchDialogID >= 0)
        {
            PlayerPrefs.SetInt(currentNPC.gameObject.name, response.switchDialogID);
        }

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
        if (response.giveItem)
        {
            foreach (var item in response.giveItemPrefab)
            {
                giveItem(item);
            }
        }

        FindFirstObjectByType<QuestManager>().CompleteQuest(questID);

        if (response.switchDialogID >= 0)
        {
            PlayerPrefs.SetInt(currentNPC.gameObject.name, response.switchDialogID);
        }

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
        gameObject.GetComponent<Image>().enabled = false;
        dialogueText.text = "";
        foreach (Transform child in responsesContainer)
        {
            Destroy(child.gameObject);
        }
    }


}