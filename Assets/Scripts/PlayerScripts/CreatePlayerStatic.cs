using System;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerStatic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantinateCharacterModel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isDemon()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<CharacterStats>().playerTime >= 9)
            return true;
        else
            return false;
    }

    void InstantinateCharacterModel()
    {
        switch (gameObject.name)
        {
            case "Character":
                Instantiate(Resources.Load<GameObject>(PlayerPrefs.GetString("characterClass") + "Static"), gameObject.transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
                break;
            case "CharacterEnemy":
                var enemy = Instantiate(Resources.Load<GameObject>(PlayerPrefs.GetString("enemyName")), gameObject.transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
                enemy.transform.localScale = new Vector3(-5, 5, 5);
                var ap = enemy.GetComponent<RectTransform>().anchoredPosition;
                ap = new Vector2(ap.x + enemy.GetComponent<RectTransform>().rect.width * Math.Abs(enemy.transform.localScale.x) / 2
                    , ap.y - enemy.GetComponent<RectTransform>().rect.height * enemy.transform.localScale.y / 2);
                enemy.GetComponent<Image>().enabled = false;

                if(isDemon())
                {
                    for(int i = 0; i < enemy.transform.childCount; i++)
                    {
                        if(enemy.transform.GetChild(i).name == "pointInterestBattleDemon")
                        {
                            enemy.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                        }
                        if (enemy.transform.GetChild(i).name == "pointInterestBattle")
                        {
                            enemy.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                }

                break;
        }
    }
}
