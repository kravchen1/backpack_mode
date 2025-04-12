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

    private CharacterStats characterStats;
    private GameObject player;
    public void StartDialogue(Dialogue dialogue, NPC npc)
    {
        currentDialogue = dialogue;
        currentNPC = npc.GetComponent<NPC>();
        DisplayDialogue();
    }
    private void DisplayDialogue()
    {
        dialogueSound.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        dialogueSound.Play();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        nameCharacterText.text = currentNPC.name;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.GetComponent<Image>().enabled = true;


        //currentDialogue.HeightText;
        dialogueText.text = currentDialogue.dialogueLocalizationText.GetText();

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
            button.GetComponentInChildren<TextMeshProUGUI>().text = response.responseLocalizationText.GetText();
            // Настраиваем действие кнопки

            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(dialogueResponseSound.GetComponent<SoundVolume>().SetSoundVolume);
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(dialogueResponseSound.Play);
        }
    }

    private void SetStorageWeigth(float weight)
    {
        decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)weight;
        characterStats.storageWeight = (float)System.Math.Round(preciseWeight, 2);
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
        
        backPackAndStorageData.storageData.itemData.items.Add(new Data(item.name, new Vector3(0, 0, -2)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
    }

    private void OnResponseSelected(Response response)
    {
        if (response.giveItem)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                characterStats = player.GetComponent<CharacterStats>();
            }
            foreach (var item in response.giveItemPrefab)
            {
                SetStorageWeigth(item.GetComponent<Item>().weight);
                giveItem(item);
            }
            characterStats.SaveData();
        }

        if (response.questComplete)
        {
            FindFirstObjectByType<QuestManager>().CompleteQuest(response.idQuestComplete, true);
            FindFirstObjectByType<Player>().InitializedGPSTracker();
        }

        if (response.quest)
        {
            string settingLanguage = "en";
            settingLanguage = PlayerPrefs.GetString("LanguageSettings");

            string questName = QuestManagerJSON.Instance.GetNameQuest(settingLanguage, response.questID);
            string questText = QuestManagerJSON.Instance.GetTextQuest(settingLanguage, response.questID);
            int questProgress = QuestManagerJSON.Instance.GetProgressQuest(settingLanguage, response.questID);

            Quest quest = new Quest(questName, questText, questProgress, response.questID);
            FindFirstObjectByType<QuestManager>().AddQuest(quest);
            FindFirstObjectByType<Player>().InitializedGPSTracker();
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