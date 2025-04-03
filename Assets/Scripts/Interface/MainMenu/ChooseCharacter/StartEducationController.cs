using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEducationController : MonoBehaviour
{
    public GameObject page1, page1_2, page2, page3, page3_1, page3_2, page4, page5, page5_0, page5_1, page5_2, page5_3, page5_4, page6, page7, page8;

    public GameObject startEducation;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("StartEducation"));
        if(PlayerPrefs.GetInt("StartEducation") == 0)
        {
            OpenEducation();
            //PlayerPrefs.SetInt("StartEducation", 1);
        }
    }

    public void CloseEducation()
    {
        startEducation.SetActive(false);
    }

    public void OpenEducation()
    {
        startEducation.SetActive(true);
    }


    public void Page1Forward()
    {
        page1.SetActive(false);
        page1_2.SetActive(true);
    }



    public void Page1_2Forward()
    {
        page1_2.SetActive(false);
        page2.SetActive(true);
    }

    public void Page1_2Back()
    {
        page1_2.SetActive(false);
        page1.SetActive(true);
    }



    public void Page2Forward()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }
    public void Page2Back()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }


    public void Page3Forward()
    {
        page3.SetActive(false);
        page3_1.SetActive(true);
    }
    public void Page3Back()
    {
        page3.SetActive(false);
        page2.SetActive(true);
    }



    public void Page3_1Forward()
    {
        page3_1.SetActive(false);
        page3_2.SetActive(true);
    }
    public void Page3_1Back()
    {
        page3_1.SetActive(false);
        page3.SetActive(true);
    }



    public void Page3_2Forward()
    {
        page3_2.SetActive(false);
        page4.SetActive(true);
    }
    public void Page3_2Back()
    {
        page3_2.SetActive(false);
        page3_1.SetActive(true);
    }



    public void Page4Forward()
    {
        page4.SetActive(false);
        page5.SetActive(true);
    }
    public void Page4Back()
    {
        page4.SetActive(false);
        page3.SetActive(true);
    }



    public void Page5Forward()
    {
        page5.SetActive(false);
        page5_0.SetActive(true);
    }
    public void Page5Back()
    {
        page5.SetActive(false);
        page4.SetActive(true);
    }



    public void Page5_0Forward()
    {
        page5_0.SetActive(false);
        page5_1.SetActive(true);
    }
    public void Page5_0Back()
    {
        page5_0.SetActive(false);
        page5.SetActive(true);
    }



    public void Page5_1Forward()
    {
        page5_1.SetActive(false);
        page5_2.SetActive(true);
    }
    public void Page5_1Back()
    {
        page5_1.SetActive(false);
        page5_0.SetActive(true);
    }



    public void Page5_2Forward()
    {
        page5_2.SetActive(false);
        page5_3.SetActive(true);
    }
    public void Page5_2Back()
    {
        page5_2.SetActive(false);
        page5_1.SetActive(true);
    }



    public void Page5_3Forward()
    {
        page5_3.SetActive(false);
        page5_4.SetActive(true);
    }
    public void Page5_3Back()
    {
        page5_3.SetActive(false);
        page5_2.SetActive(true);
    }



    public void Page5_4Forward()
    {
        page5_4.SetActive(false);
        page6.SetActive(true);
    }
    public void Page5_4Back()
    {
        page5_4.SetActive(false);
        page5_3.SetActive(true);
    }



    public void Page6Forward()
    {
        page6.SetActive(false);
        page7.SetActive(true);
    }
    public void Page6Back()
    {
        page6.SetActive(false);
        page5_4.SetActive(true);
    }



    public void Page7Forward()
    {
        page7.SetActive(false);
        page8.SetActive(true);
    }
    public void Page7Back()
    {
        page7.SetActive(false);
        page6.SetActive(true);
    }




    public void Page8Back()
    {
        page8.SetActive(false);
        page7.SetActive(true);
    }

    public void Page8End()
    {
        CloseEducation();
    }
}