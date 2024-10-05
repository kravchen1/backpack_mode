using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Chest : MonoBehaviour
{
    public Animator animator;
    public Animator itemChestAnimation;
    public Spline spline;
    private BackpackData storageData;
    public List<GameObject> chestItems;
    private GameObject[] prefabs;

    private bool move;
    private float timer_cooldown = 0.2f;
    private float timer = 0f;
    private bool timer_locked_out = false;
    private int index = 0;
    public bool foolItemChestList = false;

    private int winnerIndex = -1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        storageData = new BackpackData();
        chestItems = new List<GameObject>();
    }


    public void Roulete(int index)
    {
        if (timer_locked_out == false && index < chestItems.Count())
        {
            timer_locked_out = true;
            var generationObjectChest = Instantiate(chestItems[index], new Vector3(0, 0, 0), Quaternion.identity, GetComponent<RectTransform>());
            itemChestAnimation = generationObjectChest.GetComponent<Animator>();
            var r = Random.Range(1, 5);
            itemChestAnimation.Play("ChestItemAnimation" + r.ToString());
        }
    }


    public void SetWinner()
    {
        winnerIndex = Random.Range(0, chestItems.Count());
    }

    void GetWinnerItem()
    {
        var generationObjectChest = Instantiate(chestItems[winnerIndex], new Vector3(0, 0, 0), Quaternion.identity, GetComponent<RectTransform>());
        itemChestAnimation = generationObjectChest.GetComponent<Animator>();
        itemChestAnimation.Play("ChestItemWinnerAnimation");
        foolItemChestList = false;
        isEnding = false;
    }

    public void CoolDown()
    {
        if (timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
                index++;
            }
        }
    }

    
    public void LoadChestItems(string tagName)
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("");
        }
        chestItems.AddRange(prefabs.Where(e => e.tag == tagName).ToList());

    }

    public void AddNewItemInStorage()
    {
        storageData.LoadData("Assets/Saves/storageData.json");
        storageData.itemData.items.Add(new Data(chestItems[winnerIndex].name, new Vector2(0,0)));
        storageData.SaveDataFromChest("Assets/Saves/storageData.json");
    }

    private bool isEnding = false;
    private void Update()
    {
        if ((chestItems.Count() != 0 || chestItems != null) && foolItemChestList && index < chestItems.Count())
        {

            CoolDown();
            if (winnerIndex != index)
            {
                Roulete(index);
            }
            else
            {
                timer_locked_out = true;
            }
            if(index == chestItems.Count() - 1)
            {
                isEnding = true;
            }
        }
        if (isEnding)
            GetWinnerItem();
    }
}

