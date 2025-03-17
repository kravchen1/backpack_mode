using System;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerStatic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool timer_locked_out = true;
    private float timer = 3f;
    private float timer_cooldown = 3f;
    void Start()
    {
        InstantinateCharacterModel();
    }

    private void CoolDown()
    {
        if (timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
            }
        }
    }

    private void ActivateAnimationAttack()
    {
        if (!timer_locked_out)
        {
            timer_locked_out = true;
            //GetComponentInChildren<Animator>().Play("Attack1");
        }
    }


    // Update is called once per frame
    void Update()
    {
        CoolDown();
        ActivateAnimationAttack();
    }


    void InstantinateCharacterModel()
    {
        switch (gameObject.name)
        {
            case "Character":
                //Assets/Resources/BattleChars/Enemys1/Crystall Guard ForBattle.prefab
                Instantiate(Resources.Load<GameObject>("BattleChars/Player/" + PlayerPrefs.GetString("characterClass") + "ForBattle"), gameObject.transform);
                break;
            case "CharacterEnemy":
                //Debug.Log(PlayerPrefs.GetString("enemyName"));
                if (Resources.Load<GameObject>("BattleChars/Enemys1/" + PlayerPrefs.GetString("enemyName").Replace("(Clone)","") + " ForBattle") != null)
                {
                    //Debug.Log(PlayerPrefs.GetString("enemyName"));
                    //var enemy = 
                    Instantiate(Resources.Load<GameObject>("BattleChars/Enemys1/" + PlayerPrefs.GetString("enemyName").Replace("(Clone)", "") + " ForBattle"), gameObject.transform);
                    //var ap = enemy.GetComponent<RectTransform>().anchoredPosition;
                    //ap = new Vector2(ap.x + enemy.GetComponent<RectTransform>().rect.width * Math.Abs(enemy.transform.localScale.x) / 2
                    //    , ap.y - enemy.GetComponent<RectTransform>().rect.height * enemy.transform.localScale.y / 2);
                    //enemy.GetComponent<Image>().enabled = false;
                }
                break;
        }
    }
}
