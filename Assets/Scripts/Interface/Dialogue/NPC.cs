using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dialogue dialogue;
    public void StartDialogue()
    {
        FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }
}