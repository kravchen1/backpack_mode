using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TextMeshPro dialogueText;
    public GameObject responseButtonPrefab;
    public Transform responsesContainer;
    private Dialogue currentDialogue;
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
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
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnResponseSelected(response));
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