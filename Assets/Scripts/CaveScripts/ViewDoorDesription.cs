using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public GameObject doorEventDistributor;

        private DoorEventDistributor ded;
        private void Start()
        {
            ded = doorEventDistributor.GetComponent<DoorEventDistributor>();
            textMeshPro = text.GetComponent<TextMeshPro>();
        }

        private void OnMouseEnter()
        {
            if(GetComponent<Door>().caveLevel <= ded.doorData.DoorDataClass.currentCaveLevel + 1)
                textMeshPro.enabled = true;
        }


        private void OnMouseExit()
        {
            if (GetComponent<Door>().caveLevel <= ded.doorData.DoorDataClass.currentCaveLevel + 1)
                textMeshPro.enabled = false;
        }
    }
}
