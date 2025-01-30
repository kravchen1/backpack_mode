using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.CaveScripts
{

    public class ViewDoorDesription:MonoBehaviour
    {
        public GameObject text;
        private TextMeshPro textMeshPro;

        private void Start()
        {
            textMeshPro = text.GetComponent<TextMeshPro>();
        }

        private void OnMouseEnter()
        {
            textMeshPro.enabled = true;
        }


        private void OnMouseExit()
        {
            textMeshPro.enabled = false;
        }
    }
}
