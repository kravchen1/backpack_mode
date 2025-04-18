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
    private bool isPointerEntered = false;
    private Image image;

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
        item.stars = new List<GameObject>();
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

        image = transform.GetChild(1).GetComponent<Image>();
        if (PlayerPrefs.GetInt("Found" + item.originalName) != 1)
        {
            Color black = new Color(0f,0f,0f,0f);
            image.color = black;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerEntered || PlayerPrefs.GetInt("Found" + item.originalName) != 1)
            return;
        //Debug.Log(FindObjectsOfType<EventSystem>().Length);
        //Debug.Log($"OnPointerEnter: {Time.frameCount}");
        isPointerEntered = true;
        animator.Rebind();
        animator.Play("aim", 0, 0f);
        item.Exit = false;
        item.ShowDescription();
        ChangeShowStars(true);
        aimItemMusicEffects.PlayAimSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPointerEntered || PlayerPrefs.GetInt("Found" + item.originalName) != 1)
            return;

        //Debug.Log($"OnPointerExit: {Time.frameCount}");
        isPointerEntered = false;
        animator.Play("aimRevert", 0, 0f);
        image.raycastTarget = true; // Включаем обратно
        MouseExit();
    }



    void MouseExit()
    {
        ChangeShowStars(false);
        item.Exit = true;
        if(item.CanvasDescription.gameObject != null)
            Destroy(item.CanvasDescription.gameObject);
    }

    public void ChangeShowStars(bool enabled)
    {
        foreach (var star in stars)
        {
            star.gameObject.GetComponent<Image>().enabled = enabled;//SetActive(enabled);
        }
    }





    //private void OnDestroy()
    //{
    //    if (item != null)
    //    {
    //        item.Exit = true;
    //        if (item.CanvasDescription != null)
    //            Destroy(item.CanvasDescription);
    //    }
    //}

}