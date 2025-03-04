using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameCharacterText;
    public GameObject responseButtonPrefab;
    public Transform responsesContainer;
    public RectTransform Content;
    private Dialogue currentDialogue;
    private NPC currentNPC;
    public AudioSource dialogueSound;
    public AudioSource dialogueResponseSound;


    private BackPackAndStorageData backPackAndStorageData;
    public void StartDialogue(Dialogue dialogue, NPC npc)
    {
        currentDialogue = dialogue;
        currentNPC = npc.GetComponent<NPC>();
        DisplayDialogue();
    }
    private void DisplayDialogue()
    {
        dialogueSound.Play();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        nameCharacterText.text = currentNPC.name;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
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

            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(dialogueResponseSound.Play);
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

        if (response.questComplete)
        {
            FindFirstObjectByType<QuestManager>().CompleteQuest(response.idQuestComplete);
        }

        if (response.quest)
        {
            Quest quest = new Quest(response.questName, response.questDescription, response.necessaryProgress, response.questID);
            FindFirstObjectByType<QuestManager>().AddQuest(quest);
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

    //private void OnResponseSelectedQuest(Response response, Quest quest)
    //{
    //    if (response.giveItem)
    //    {
    //        foreach (var item in response.giveItemPrefab)
    //        {
    //            giveItem(item);
    //        }
    //    }

        

    //    if (response.switchDialogID >= 0)
    //    {
    //        PlayerPrefs.SetInt(currentNPC.gameObject.name, response.switchDialogID);
    //    }

    //    if (response.nextDialogue != null)
    //    {
    //        currentDialogue = response.nextDialogue;
    //        DisplayDialogue();
    //    }
    //    else
    //    {
    //        EndDialogue();
    //    }
    //}

    //private void OnResponseSelectedQuestCompleted(Response response, int questID)
    //{
    //    if (response.giveItem)
    //    {
    //        foreach (var item in response.giveItemPrefab)
    //        {
    //            giveItem(item);
    //        }
    //    }

        

    //    if (response.switchDialogID >= 0)
    //    {
    //        PlayerPrefs.SetInt(currentNPC.gameObject.name, response.switchDialogID);
    //    }

    //    if (response.nextDialogue != null)
    //    {
    //        currentDialogue = response.nextDialogue;
    //        DisplayDialogue();
    //    }
    //    else
    //    {
    //        EndDialogue();
    //    }
    //}
    public void EndDialogue()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.GetComponent<Image>().enabled = false;
        dialogueText.text = "";
        foreach (Transform child in responsesContainer)
        {
            Destroy(child.gameObject);
        }
    }


}