using System.IO;
using System.Linq;
using UnityEngine;

public class NPC_King : NPC
{
    private QuestData questData;

    private QuestManager qm;
    public void Initialize()
    {
        qm = FindFirstObjectByType<QuestManager>();
    }


    public void Start()
    {
        Invoke("Initialize", 0.5f);
        
    }

    private void Update()
    {
        if (qm != null)
        {
            if (qm.questData.questData.quests.Where(e => e.id == 1 && e.isCompleted == false).Count() > 0)
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}