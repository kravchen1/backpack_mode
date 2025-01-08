using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTalant : Button
{
    private Color buttonColorNoActive = new Color(0.5f,0.5f,0.5f), buttonColorActive = new Color(1f, 1f, 1f);
    //private bool activated = false;
    public List<GameObject> connectedLines;
    public bool startTalant = false;

    private TalantTreeStats talantTreeStats;


    private bool CheckoutActive()
    {
        if (startTalant)
        {
            return true;
        }
        else
        {
            //if(careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
            if(connectedLines.Where(e => e.GetComponent<TalantLine>().activated == true).Count() > 0)//== necessaryConnectedLines.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Activate()
    {
        if (CheckoutActive())
        {
            GetComponent<SpriteRenderer>().color = buttonColorActive;
            //activated = true;

            foreach (var line in connectedLines)
            {
                line.GetComponent<TalantLine>().Activate(true);
            }
            talantTreeStats.talantTreeStatsData.activatedTalantNames.Add(gameObject.name);
        }
        else
        {
            //запустить мигание красным незаполненных линий например
        }
    }

    private void IgnoreActivate()
    {
        GetComponent<SpriteRenderer>().color = buttonColorActive;
        //activated = true;
        foreach (var line in connectedLines)
        {
            line.GetComponent<TalantLine>().Activate(true);
        }
    }

    public Canvas Description;
    private Canvas CanvasDescription;
    private bool showCanvasBefore = false;

    override public void OnMouseUpAsButton()
    {
        Activate();
    }

    private void Awake()
    {
        talantTreeStats = GameObject.FindGameObjectWithTag("TalantTreeStats").GetComponent<TalantTreeStats>();
        if(talantTreeStats.talantTreeStatsData.activatedTalantNames.Where(e => e == gameObject.name).Count() > 0)
        {
            IgnoreActivate();
        }
    }


    void ShowDescription()
    {
        if (!showCanvasBefore)
        {
            showCanvasBefore = true;
            CanvasDescription = Instantiate(Description, GameObject.FindGameObjectWithTag("TalantTreeCanvas").GetComponent<RectTransform>().transform);
            //showCanvas.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        }
        else
        {
            CanvasDescription.enabled = true;
        }
    }



    public void OnMouseEnter()
    {   
            ShowDescription();
    }

    public void OnMouseExit()
    {
        if (CanvasDescription != null)
        {
            CanvasDescription.enabled = false;
            var starsDesctiprion = CanvasDescription.GetComponentInChildren<SpriteRenderer>();
            if (starsDesctiprion != null)
            {
                starsDesctiprion.enabled = false;
            }
        }
    }

}
