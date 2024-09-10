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
                enemy.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x + enemy.GetComponent<RectTransform>().rect.width * Math.Abs(enemy.transform.localScale.x) / 2
                    , gameObject.GetComponent<RectTransform>().anchoredPosition.y - enemy.GetComponent<RectTransform>().rect.height * enemy.transform.localScale.y / 2);
                enemy.GetComponent<Image>().enabled = false;
                break;
        }
    }
}
