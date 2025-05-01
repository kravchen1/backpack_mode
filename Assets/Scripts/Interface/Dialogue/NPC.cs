using Steamworks;
using TMPro;
using UnityEngine;

public class NPC : EventParent
{
    public Dialogue dialogue;
    public Dialogue alternativeDialogue1;
    public Dialogue alternativeDialogue2;
    protected bool isPlayerInTrigger = false;
    protected int dialogueNumber = 0;
    //public virtual void Initialize()
    //{
    //}
    public virtual void StartDialogue()
    {
        if (PlayerPrefs.HasKey(gameObject.name))
            dialogueNumber = PlayerPrefs.GetInt(gameObject.name);
        else
            dialogueNumber = 0;

        switch (dialogueNumber)
        {
            case 0:
                FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue, this);
                //Initialize();
                break;
            case 1:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue1, this);
                //Initialize();
                break;
            case 2:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue1, this);
                //Initialize();
                break;
        }

    }

    protected void OnTriggerEnter2D()
    {
        isPlayerInTrigger = true;
        if (isShowPressE)
        {
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    protected void OnTriggerExit2D()
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
        var dialogueManager = FindFirstObjectByType<DialogueManager>();
        if(dialogueManager.isDialogStarted)
            FindFirstObjectByType<DialogueManager>().EndDialogue();
    }

    protected void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            StartDialogue();
        }
    }
}