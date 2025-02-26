using System.IO;
using System.Linq;
using UnityEngine;

public class Quest2Tracker : MonoBehaviour
{
    private QuestData questData;

    private QuestManager qm;
    //public void Initialize()
    //{
    //    qm = FindFirstObjectByType<QuestManager>();
    //}


    //public void Start()
    //{
    //    Invoke("Initialize", 1f);
    //}

    private void Update()
    {
        //if (qm != null)
        //{
        //    if (qm.questData.questData.quests.Where(e => e.id == 2 && e.isCompleted == false).Count() > 0)
        //    {
        //        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    qm = FindFirstObjectByType<QuestManager>();
        //}
    }
}