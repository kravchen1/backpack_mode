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
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume",1f);
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void DropAnimation()
    {
        if (dropItems.Count > 0 && dropItems.Count == probabilityDropItems.Count)
        {
            List<GameObject> droppedItems = new List<GameObject>();
            float probabilityReductionFactor = 0.6f; // Коэффициент снижения вероятности

            // Гарантируем выпадение первого предмета
            int guaranteedDropIndex = UnityEngine.Random.Range(0, dropItems.Count);
            GameObject guaranteedItem = Instantiate(
                dropItems[guaranteedDropIndex],
                gameObject.transform.position + new Vector3(-200 * (droppedItems.Count + 1), 0, 0),
                Quaternion.identity,
                cave.GetComponent<RectTransform>().transform
            );
            droppedItems.Add(guaranteedItem);
            Debug.Log("Guaranteed drop: " + dropItems[guaranteedDropIndex].name);
            //PlayerPrefs.SetInt("caveEnemyLvl", 1);
            if (PlayerPrefs.GetInt("caveEnemyLvl") != 1)
            {
                // Пробуем выпасть остальные предметы с уменьшающейся вероятностью
                for (int i = 0; i < dropItems.Count; i++)
                {
                    if (i == guaranteedDropIndex) continue; // Уже выпал

                    float modifiedProbability = probabilityDropItems[i] * Mathf.Pow(probabilityReductionFactor, droppedItems.Count);
                    float r = UnityEngine.Random.Range(0f, 100f);

                    if (r <= modifiedProbability)
                    {
                        Debug.Log(dropItems[i].name + " bonus loot with roll " + r + " (modified prob: " + modifiedProbability + ")");
                        GameObject droppedItem = Instantiate(
                            dropItems[i],
                            gameObject.transform.position + new Vector3(-200 * (droppedItems.Count + 1), 0, 0),
                            Quaternion.identity,
                            cave.GetComponent<RectTransform>().transform
                        );
                        droppedItems.Add(droppedItem);
                    }
                }
            }

            isClosed = false;
            PlayerPrefs.SetInt("isChestClosed", 0);
            transform.parent.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            transform.parent.GetComponent<AudioSource>().Play();
        }
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

