using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CaveChest : EventParent
{
    private GameObject player;
    private Player classPlayer;
    private CharacterStats characterStats;
    public Sprite emptyChest;
    private bool isPlayerInTrigger = false;

    private bool isClosed;

    private GameObject cave;
    //public GameObject infoText;
    //public bool isFull = true;

    public List<GameObject> dropItems;
    public List<float> probabilityDropItems;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("isChestClosed"))
        {
            isClosed = true;
        }
        else
        {
            isClosed = false;
            ChangeSprite();
        }
        cave = GameObject.FindGameObjectWithTag("Cave");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        isPlayerInTrigger = true;
        player = collider.gameObject;
        characterStats = player.GetComponent<CharacterStats>();
        if(isShowPressE && isClosed)
        {
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void DropAnimation()
    {
        if (dropItems.Count > 0 && dropItems.Count == probabilityDropItems.Count )
        {
            for (int i = 0; i < dropItems.Count; i++)
            {
                float r = UnityEngine.Random.Range(1.0f, 100.0f);

                if (r <= probabilityDropItems[i])
                {
                    Debug.Log(dropItems[i].name + "  loot " + r);
                    Instantiate(dropItems[i], gameObject.transform.position + new Vector3(-200, 0,0), Quaternion.identity, cave.GetComponent<RectTransform>().transform);
                }
            }
            isClosed = false;
            PlayerPrefs.SetInt("isChestClosed", 0);
            transform.parent.GetComponent<AudioSource>().Play();
        }
        //Destroy(gameObject.transform.parent.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetActivePressE(false);
    }

    void ChangeSprite()
    {
        transform.parent.GetComponent<SpriteRenderer>().sprite = emptyChest;
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE && isClosed)
        {
            DropAnimation();
            ChangeSprite();
            SetActivePressE(false);
        }
    }
}

