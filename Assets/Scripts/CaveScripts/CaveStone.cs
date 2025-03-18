using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveStone : MonoBehaviour
{
    public List<GameObject> caveStoneCells;
    private List<Cell> scriptCaveStoneCells;
    public GameObject buttonStartAdventure;
    //private Animator animator;

    private int caveEnemyLvl;
    private GameObject activateStone;

    private void Awake()
    {
        scriptCaveStoneCells = new List<Cell>();
        foreach (var cell in caveStoneCells)
        {
            scriptCaveStoneCells.Add(cell.GetComponent<Cell>());
        }
        //animator = GetComponent<Animator>();
    }
    private bool haveCaveLvl = false;//, canAnimation = false;
    //private AnimatorStateInfo currentStateInfo;
    //private float animationLength;

    private void SearchFirstStone()
    {

    }

    private void Update()
    {

        if (scriptCaveStoneCells.Where(e => e.nestedObject != null && e.nestedObject.CompareTag("ItemKeyStone")).Count() > 0)
        {
            var stone = scriptCaveStoneCells.Where(e => e.nestedObject != null && e.nestedObject.CompareTag("ItemKeyStone")).ToList();
            caveEnemyLvl = stone[0].nestedObject.GetComponent<CaveStonesKeys>().stoneLevel;
            activateStone = stone[0].nestedObject;
            haveCaveLvl = true;

            //canAnimation = true;
        }
        else
        {
            haveCaveLvl = false;
        }
       
        if (haveCaveLvl)
        {
            buttonStartAdventure.SetActive(true);
        }
        else
        {
            buttonStartAdventure.SetActive(false);
        }
    }
    //void StartAnimation(string animationName)
    //{
    //    canAnimation = false;
    //    animator.Play(animationName);
    //    StartCoroutine(PlayAnimation());
    //}
    //private System.Collections.IEnumerator PlayAnimation()
    //{
    //    // Ждем окончания реверсированной анимации
    //    yield return new WaitForSeconds(2);
    //    canAnimation = true;
    //}

    public void StartAdventure()
    {
        if (haveCaveLvl)
        { 
            var caveStoneBP = GameObject.FindWithTag("CaveStone").GetComponent<BackpackData>();

            for (int i = 0; i < caveStoneBP.itemData.items.Count; i++)
            {
                if (caveStoneBP.itemData.items[i].name == activateStone.name)
                {
                    caveStoneBP.itemData.items.Remove(caveStoneBP.itemData.items[i]);
                }
            }
            caveStoneBP.SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "caveStoneData.json"));
            Destroy(activateStone);

            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();

            
            PlayerPrefs.SetInt("caveEnemyLvl", caveEnemyLvl);
            SceneLoader.Instance.LoadScene("Cave");
        }
    }

}