using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsShowItems : MonoBehaviour
{
    public GameObject pageBags;
    public GameObject pageFood;
    public GameObject pageWeapons;
    public GameObject pageClothing;
    public GameObject pageCrystals;
    public GameObject pageKeystones;
    public GameObject pageJunk;
    public GameObject pageMushrooms;
    public GameObject pageStuff;
    public GameObject pageDragon;
    public GameObject pagePets;
    public GameObject pageWitchcraft;
    public GameObject pageManaSet;
    public GameObject pageFireSet;
    public GameObject pageVampireSet;



    public GameObject buttonBags, buttonFood, buttonWeapons, buttonClothing, buttonCrystals
                    , buttonKeystones, buttonJunk, buttonMushrooms, buttonStuff, buttonDragon
                    , buttonPets, buttonWitchcraft, buttonManaSet, buttonFireSet, buttonVampireSet;

    //void ReloadColliders(GameObject warehouse)
    //{
    //    for (int i = 0; i < warehouse.transform.childCount; i++)
    //    {
    //        warehouse.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
    //        warehouse.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
    //    }
    //}

    public void ToggleWarehouse(int numberWarehouse)
    {
        switch (numberWarehouse)
        {
            case 1:
                pageBags.SetActive(true);
                buttonBags.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 2:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(true);
                buttonFood.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 3:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(true);
                buttonWeapons.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 4:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(true);
                buttonClothing.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 5:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(true);
                buttonCrystals.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 6:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(true);
                buttonKeystones.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 7:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(true);
                buttonJunk.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 8:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(true);
                buttonMushrooms.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 9:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(true);
                buttonStuff.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 10:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(true);
                buttonDragon.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 11:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(true);
                buttonPets.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 12:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(true);
                buttonWitchcraft.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 13:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(true);
                buttonManaSet.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 14:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(true);
                buttonFireSet.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);

                pageVampireSet.SetActive(false);
                buttonVampireSet.GetComponent<Image>().color = Color.white;
                break;
            case 15:
                pageBags.SetActive(false);
                buttonBags.GetComponent<Image>().color = Color.white;

                pageFood.SetActive(false);
                buttonFood.GetComponent<Image>().color = Color.white;

                pageWeapons.SetActive(false);
                buttonWeapons.GetComponent<Image>().color = Color.white;

                pageClothing.SetActive(false);
                buttonClothing.GetComponent<Image>().color = Color.white;

                pageCrystals.SetActive(false);
                buttonCrystals.GetComponent<Image>().color = Color.white;

                pageKeystones.SetActive(false);
                buttonKeystones.GetComponent<Image>().color = Color.white;

                pageJunk.SetActive(false);
                buttonJunk.GetComponent<Image>().color = Color.white;

                pageMushrooms.SetActive(false);
                buttonMushrooms.GetComponent<Image>().color = Color.white;

                pageStuff.SetActive(false);
                buttonStuff.GetComponent<Image>().color = Color.white;

                pageDragon.SetActive(false);
                buttonDragon.GetComponent<Image>().color = Color.white;

                pagePets.SetActive(false);
                buttonPets.GetComponent<Image>().color = Color.white;

                pageWitchcraft.SetActive(false);
                buttonWitchcraft.GetComponent<Image>().color = Color.white;

                pageManaSet.SetActive(false);
                buttonManaSet.GetComponent<Image>().color = Color.white;

                pageFireSet.SetActive(false);
                buttonFireSet.GetComponent<Image>().color = Color.white;

                pageVampireSet.SetActive(true);
                buttonVampireSet.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                break;
        }
        
    }

    public void ExitScene()
    {
        SceneLoader.Instance.LoadScene("Main");
    }

}
