using System.IO;
using System.Linq;
using UnityEngine;

public class NPC_King : NPC
{
    private QuestData questData;

    private QuestManager qm;

    public Dialogue alternativeDialogue3;
    public Dialogue alternativeDialogue4;
    public Dialogue alternativeDialogue5;
    public Dialogue alternativeDialogue6;
    public Dialogue alternativeDialogue7;
    public Dialogue alternativeDialogue8;
    public Dialogue alternativeDialogue9;
    public Dialogue alternativeDialogue10;
    public Dialogue alternativeDialogue11;

    public void Initialize()
    {
        qm = FindFirstObjectByType<QuestManager>();
    }


    public void Start()
    {
        //Invoke("Initialize", 1f);
        
    }


    //private int dialogueNumber = 0;

    //public virtual void Initialize()
    //{
    //}
    public override void StartDialogue()
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
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue2, this);
                //Initialize();
                break;
            case 3:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue3, this);
                //Initialize();
                break;
            case 4:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue4, this);
                //Initialize();
                break;
            case 5:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue5, this);
                //Initialize();
                break;
            case 6:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue6, this);
                //Initialize();
                break;
            case 7:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue7, this);
                //Initialize();
                break;
            case 8:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue8, this);
                //Initialize();
                break;
            case 9:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue9, this);
                //Initialize();
                break;
            case 10:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue10, this);
                //Initialize();
                break;
            case 11:
                FindFirstObjectByType<DialogueManager>().StartDialogue(alternativeDialogue11, this);
                //Initialize();
                break;
        }

    }
}