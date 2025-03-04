using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MapFilling : MonoBehaviour
{
    public GameObject doorEventDistributor;

    public Sprite battleSprite;
    public Sprite storeSprite;
    public Sprite chestSprite;
    public Sprite fountainSprite;
    private void Start()
    {
        var ded = doorEventDistributor.GetComponent<DoorEventDistributor>();
        foreach (var door in ded.doorsList.Where(e => e.GetComponent<Door>().caveLevel <= ded.doorData.DoorDataClass.currentCaveLevel + 1))
        {
            switch(door.GetComponent<Door>().eventId)
            {
                case 0:
                    door.GetComponent<SpriteRenderer>().sprite = battleSprite;
                    break;
                case 1:
                    door.GetComponent<SpriteRenderer>().sprite = chestSprite;
                    break;
                case 2:
                    door.GetComponent<SpriteRenderer>().sprite = fountainSprite;
                    break;
                case 3:
                    door.GetComponent<SpriteRenderer>().sprite = storeSprite;
                    break;
            }
        }
    }
}

