using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    public Dialogue alternativeDialogue1;
    public Dialogue alternativeDialogue2;

    private int dialogueNumber = 0;
    public void StartDialogue()
    {
        if (PlayerPrefs.HasKey(gameObject.name))
            dialogueNumber = PlayerPrefs.GetInt(gameObject.name);
        else
            dialogueNumber = 0;

        switch (dialogueNumber)
        {
            case 0:
                FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue, this);
                break;
            case 1:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue1, this);
                break;
            case 2:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue1, this);
                break;
        }
        
    }
}