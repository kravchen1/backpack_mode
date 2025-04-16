using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.ItemScripts;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ItemShowCase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Item item;
    bool Exit = false;
    private GameObject CanvasDescription;
    private GameObject placeForDescription;
    private List<GameObject> stars;
    private AimItemMusicEffects aimItemMusicEffects;
    private Animator animator;

    void FindPlaceForDescription()
    {
        if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name)
            item.placeForDescription = GameObject.FindWithTag("DescriptionPlace");
        else
            item.placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
    }
    private void Awake()
    {
        FindPlaceForDescription();
        stars = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "StarActivation")
            {
                stars.Add(child);
            }
        }
        aimItemMusicEffects = GetComponent<AimItemMusicEffects>();
        animator = GetComponent<Animator>();
        item.timer_cooldown = item.baseTimerCooldown;
        if (PlayerPrefs.GetInt("Found" + item.originalName) != 1)
        {
            Color black = new Color(0f,0f,0f,0f);
            //animator.Play("black", 0, 0f);
            //transform.GetChild(0).GetComponent<Image>().color = black;
            transform.GetChild(1).GetComponent<Image>().color = black;
            //animator.Play("black", 0, 0f);
            //Debug.Log("Alo");
        }
    }


    // ����������� ��� ��������� �������
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerPrefs.GetInt("Found" + item.originalName) == 1)
        {
            CanvasDescription = Instantiate(item.Description, placeForDescription.GetComponent<RectTransform>().transform);
            animator.Play("aim", 0, 0f);
            item.Exit = false;
            StartCoroutine(item.ShowDescription());

            ChangeShowStars(true);
        }
        aimItemMusicEffects.PlayAimSound();
    }

    // ����������� ��� ������ �������
    public void OnPointerExit(PointerEventData eventData)
    {
        if (PlayerPrefs.GetInt("Found" + item.originalName) == 1)
        {
            animator.Play("aimRevert", 0, 0f);
            MouseExit();
        }
    }



    void MouseExit()
    {
        ChangeShowStars(false);
        item.Exit = true;
        Destroy(item.CanvasDescription.gameObject);
    }

    public void ChangeShowStars(bool enabled)
    {
        foreach (var star in stars)
        {
            star.gameObject.GetComponent<Image>().enabled = enabled;//SetActive(enabled);
        }
    }

}