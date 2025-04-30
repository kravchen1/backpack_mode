using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class LinkController : MonoBehaviour
{
    public string url = ""; // Замените на нужную ссылку

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}

