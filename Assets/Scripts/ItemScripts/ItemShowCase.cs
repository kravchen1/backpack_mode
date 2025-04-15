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


public abstract class ItemShowCase : MonoBehaviour
{

    public GameObject Description;
    bool Exit = false;
    private GameObject CanvasDescription;
    public string originalName;
    public GameObject placeForDescription;
    private List<GameObject> stars;
    
    void FindPlaceForDescription()
    {
        if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name)
            placeForDescription = GameObject.FindWithTag("DescriptionPlace");
        else
            placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
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
    }


    private void OnMouseEnter()
    {

        if (!DragManager.isDragging)
        {
            Exit = false;
            if (PlayerPrefs.GetInt("Found" + originalName) == 1)
            {
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
            }
        }
    }

    private void OnMouseExit()
    {
        Exit = true;
        MouseExit();
    }

    void MouseExit()
    {
        ChangeShowStars(false);
        Destroy(CanvasDescription.gameObject);
    }

    public void ChangeShowStars(bool enabled)
    {
        foreach (var star in stars)
        {
            star.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;//SetActive(enabled);
        }
    }

}