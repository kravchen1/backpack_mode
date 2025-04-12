using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Assets.Scripts.ItemScripts;
using JetBrains.Annotations;

public class Bag : Item
{
    private GameObject backpack;
    private List<RaycastStructure> careHitsNow = new List<RaycastStructure>();
    private List<ObjectInCells> objectsInCells = new List<ObjectInCells>();
    public void StayParentForChild()
    {
        var cellList = gameObject.GetComponentsInChildren<Cell>().ToList();
        foreach (var cell in cellList.Where(e => e.nestedObject != null))
        {
            cell.nestedObject.transform.SetParent(gameObject.transform);
            if (!objectsInCells.Any(e => e.gameObject.name == cell.nestedObject.name))
            {
                objectsInCells.Add(new ObjectInCells(cell.nestedObject.GetComponent<Item>()));
            }
        }
        foreach (var objectInCell in objectsInCells)
        {
            objectInCell.gameObject.DeleteNestedObject(gameObject.transform.parent.tag);
        }
    }
    private Vector3 shopItemStartPosition;

    public new void TapRotate()
    {
        needToRotate = true;
        if (needToDynamic)
        {
            needToRotateToStartRotation = true;
        }
        needToDynamic = false;
    }

    public void TapShowBackPack()
    {
        if (backpack != null)
        {
            for (int i = 0; i < backpack.transform.childCount; i++)
            {
                if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
                {
                    backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }


    public void SetOrderLayerPriority(string layerNameBag, string layerNameNestedObject, int weaponOrder)
    {
        image.sortingLayerName = layerNameBag;
        var cellsTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        cellsTransforms.Remove(gameObject.transform);
        cellsTransforms = cellsTransforms.Where(e => e.GetComponent<Cell>() != null).ToList();//удаляем не ячейки

        foreach (var cellSprite in cellsTransforms)
        {
            cellSprite.GetComponent<SpriteRenderer>().sortingLayerName = layerNameBag;
        }

        foreach (var cellSprite in cellsTransforms.Where(e => e.GetComponent<Cell>().nestedObject != null))
        {
            var nestedObjectSprite = cellSprite.GetComponent<Cell>().nestedObject.GetComponent<SpriteRenderer>();
            nestedObjectSprite.sortingLayerName = layerNameNestedObject;
            nestedObjectSprite.sortingOrder = weaponOrder;
        }
    }

    public override void OnMouseDown()
    {
        if (returnToOriginalPosition != null)
        {
            StopCoroutine(returnToOriginalPosition);
        }
        lastParentWasStorage = transform.parent.CompareTag("Storage");
        //rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        shopItem = GetComponent<ShopItem>();
        if(backpack == null)
            backpack = GameObject.Find("backpack");
        if (SceneManager.GetActiveScene().name != "BackPackBattle" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            if (shopItem != null)
            {
                if (shopItem.CanBuy(GetComponent<Item>()))
                {
                    DragManager.isDragging = true;
                    itemMusicEffects.OnItemUp();
                    if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null) animator.Play("ItemClick");
                    IgnoreCollisionObject(true);
                    SetOrderLayerPriority("DraggingObject", "DraggingObject", 100);
                    StayParentForChild();
                    lastItemPosition = gameObject.transform.position;
                    //TapFirst();
                    TapRotate();
                    TapShowBackPack();
                    
                    canShowDescription = false;
                    //originalLayer = gameObject.layer;
                    //gameObject.layer = LayerMask.NameToLayer("DraggingObject");
                    // Начинаем перетаскивание
                    isDragging = true;
                    // Вычисляем смещение между курсором и объектом
                    offset = transform.position - GetMouseWorldPosition();
                }
                else
                {

                }
            }
            else
            {
                DragManager.isDragging = true;
                itemMusicEffects.OnItemUp();
                if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null) animator.Play("ItemClick");
                IgnoreCollisionObject(true);
                SetOrderLayerPriority("DraggingObject", "DraggingObject", 100);
                StayParentForChild();
                if (!DragManager.isReturnToOrgignalPos)
                    lastItemPosition = gameObject.transform.position;
                //TapFirst();
                TapRotate();
                TapShowBackPack();
                
                canShowDescription = false;
                //originalLayer = gameObject.layer;
               //gameObject.layer = LayerMask.NameToLayer("DraggingObject");
                // Начинаем перетаскивание
                isDragging = true;
                // Вычисляем смещение между курсором и объектом
                offset = transform.position - GetMouseWorldPosition();
            }
        }
    }


    //public void CreateRaycast(int layerMask = 128)
    //{
    //    foreach (var collider in itemColliders)
    //    {
    //        hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, layerMask));
    //    }
    //}
    public override void CreateCareRaycast()
    {
        foreach (var hit in hits)
        {
            if (hit.hits[0].collider != null)
            {
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.hits[0].collider.name).Count() == 0)
                {
                    careHits.Add(new RaycastStructure(hit.hits[0]));//объекты
                    bagTransform = hit.hits[0].transform.parent.transform;
                }
            }
        }
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                if (careHitsForBackpack.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    careHitsForBackpack.Add(new RaycastStructure(hit));//объекты
                }
            }
        }
    }
    public override void RaycastEvent()
    {
        hits.Clear();
        hitsForBackpack.Clear();
        hitSellChest.Clear();
        foreach (var objectInCell in objectsInCells)
        {
            objectInCell.gameObject.hits = objectInCell.gameObject.CreateRaycast(256);
            objectInCell.gameObject.hitsForBackpack = objectInCell.gameObject.CreateRaycastForSellChest(128);
            objectInCell.gameObject.ClearCareRaycast(true);
            objectInCell.gameObject.CreateCareRaycast();
        }

        hits = CreateRaycast(128);
        hitSellChest = CreateRaycastForSellChest(32768);

        hitsForNotShopZone.Clear();
        hitsForNotShopZone = CreateRaycastForSellChest(LayerMask.GetMask("NotShopZoneCollider"));

        ClearCareRaycast(false);
        CreateCareRaycast();
    }
    public void ChangeColorMyCells()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null && e.raycastHit.collider.GetComponent<Cell>().nestedObject != this.gameObject).Count() == 0)
        {
            foreach (var collider in itemColliders)
            {
                collider.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        else
            foreach (var collider in itemColliders)
            {
                collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
    }

    public void BagDefauldUpdate()
    {
        if(SceneManager.GetActiveScene().name != "BackPackBattle")
        {
            if (isDragging)
            {

                transform.position = GetMouseWorldPosition() + offset;
                RaycastEvent();
                ChangeColorMyCells();
                SellChest();
                DeleteAllDescriptions();

            }
            Rotate();
            SwitchDynamicStatic();
            //OnImpulse();
            RotationToStartRotation();
        }
        else
        {
            CoolDownStart();
            StartActivation();
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
        }
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
        {
            BagDefauldUpdate();
        }
    }




    public override bool CorrectEndPoint()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            if (hits.Where(e => e.hits[0].collider == null).Count() == 0)
            {
                var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
                Vector3 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;
                //Vector2 colliderPos2 = careHits[0].raycastHit.collider.transform.position;

                for (int i = 1; i < careHits.Count; i++)
                {
                    if (careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                    {
                        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                    }
                }
                var newListCareHits = careHits.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY).ToList();
                var minX = newListCareHits[0].raycastHit.collider.transform.localPosition.x;
                foreach (var careHit in newListCareHits)
                {
                    if (careHit.raycastHit.collider.transform.localPosition.y == maxY)
                    {
                        if (careHit.raycastHit.collider.transform.localPosition.x <= minX)
                        {
                            minX = careHit.raycastHit.collider.transform.localPosition.x;
                            colliderPos = careHit.raycastHit.collider.transform.localPosition;
                        }
                    }
                }
                rectTransform.SetParent(bagTransform);
                gameObject.transform.SetAsFirstSibling();
                Vector3 offset;
                //offset = calculateOffset(itemColliders);
                if (itemColliders.Count == 4)
                    offset = new Vector2(itemColliders[0].size.x / 2, -itemColliders[0].size.y / 2);
                else if (itemColliders.Count == 9)
                    offset = new Vector2(itemColliders[0].size.x, -itemColliders[0].size.y);
                else if (itemColliders.Count == 2)
                {
                    if (rectTransform.eulerAngles.z == 90f || rectTransform.eulerAngles.z == 270f)
                    {
                        offset = new Vector2(0, -itemColliders[0].size.y / 2);
                    }
                    else
                    {
                        offset = new Vector2(itemColliders[0].size.x / 2, 0);
                    }
                }
                else
                {
                    offset = new Vector2(0, 0);
                }

                rectTransform.localPosition = offset + colliderPos + new Vector3(0f, 0f, -1f);
                needToDynamic = false;
                //Debug.Log("offset: " + offset.ToString());
                //Debug.Log("colliderPos: " + colliderPos.ToString());
                //Debug.Log("offset + colliderPos: " + rectTransform.localPosition.ToString());
                foreach (var careHit in careHits)
                {
                    careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            return true;
        }
        else
        {
            needToDynamic = true;
            return false;
        }
    }

    private void swapBags()
    {
        foreach (var Carehit in careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null))
        {
            var nestedObjectItem = Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject.GetComponent<Bag>();
            //nestedObjectItem.MoveObjectOnEndDrag();
            nestedObjectItem.EffectPlaceNoCorrect();
            //nestedObjectItem.ClearParentForChild();
            nestedObjectItem.EndDragForChildObjects(false);
            nestedObjectItem.DeleteNestedObject(nestedObjectItem.transform.parent.tag);
            nestedObjectItem.needToDynamic = true;
            timerStatic_locked_out = true;
            timerStatic = timer_cooldownStatic;
            //nestedObjectItem.Impulse = true;
            //nestedObjectItem.rb.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);
            nestedObjectItem.rb.excludeLayers = 0;
            nestedObjectItem.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Storage").transform);
            if (characterStats == null)
            {
                characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
            }
            nestedObjectItem.lastParentWasStorage = true;
            decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)nestedObjectItem.weight;
            characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
            //Debug.Log("case2");
        }
        //gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("backpack").transform);
        gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
        CorrectPosition();
        SetNestedObject();
        rb.excludeLayers = (1 << 9) | (1 << 10);
        EffectPlaceCorrect();
        if (lastParentWasStorage)
        {
            decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)weight;
            characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
            lastParentWasStorage = false;
        }
    }
    public override int ExtendedCorrectEndPoint()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            CorrectPosition();
            return 1; //no swap item
        }
        else if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() != 0)
        {
            //CorrectPosition();
            needToDynamic = true;
            return 2; //swap item
        }
        else
        {
            needToDynamic = true;
            return 3; //don`t correct
        }
    }
    public new void ChangeColorToDefault()
    {
        foreach (var collider in itemColliders)
        {
            collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
    public void DisableBackpackCells()
    {
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    public void ClearParentForChild()
    {
        var cellList = gameObject.GetComponentsInChildren<Cell>();
        foreach (var cell in cellList)
        {
            if (cell.nestedObject != null)
            {
                cell.nestedObject.transform.SetParent(backpack.transform);
            }
        }
    }
    public void EndDragForChildObjects(bool canEndDragParent)
    {
        
        foreach (var objectInCell in objectsInCells)
        {
            if (canEndDragParent)
            {
                switch (objectInCell.gameObject.ExtendedCorrectEndPoint())
                {
                    case 1:
                        objectInCell.gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
                        if (objectInCell.gameObject.lastParentWasStorage)
                        {
                            if (characterStats == null)
                            {
                                characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                            }
                            decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)objectInCell.gameObject.weight;
                            characterStats.storageWeight -= (float)Math.Round(preciseWeight, 2);
                            objectInCell.gameObject.lastParentWasStorage = false;
                        }
                        Debug.Log("case1:");
                        objectInCell.gameObject.rectTransform.localPosition += new Vector3(0f, 0f, -1f);
                        objectInCell.gameObject.SetNestedObject();
                        objectInCell.gameObject.rb.excludeLayers = (1 << 9) | (1 << 10);
                        break;
                    case 2:
                        Debug.Log("case2:");
                        foreach (var Carehit in objectInCell.gameObject.careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null))
                        {
                            var nestedObjectItem = Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject.GetComponent<Item>();
                            //nestedObjectItem.MoveObjectOnEndDrag();
                            nestedObjectItem.DeleteNestedObject(nestedObjectItem.transform.parent.tag);
                            nestedObjectItem.needToDynamic = true;
                            //nestedObjectItem.Impulse = true;
                            nestedObjectItem.rb.excludeLayers = 0;
                            nestedObjectItem.gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                            if (nestedObjectItem.lastParentWasStorage)
                            {
                                if (characterStats == null)
                                {
                                    characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                                }
                                decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)nestedObjectItem.weight;
                                characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                                nestedObjectItem.lastParentWasStorage = false;
                            }
                        }
                        //objectInCell.gameObject.gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                        objectInCell.gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
                        if (objectInCell.gameObject.lastParentWasStorage)
                        {
                            if (characterStats == null)
                            {
                                characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                            }
                            decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)objectInCell.gameObject.weight;
                            characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                            objectInCell.gameObject.lastParentWasStorage = false;
                        }

                        objectInCell.gameObject.rectTransform.localPosition += new Vector3(0f, 0f, -1f);
                        objectInCell.gameObject.SetNestedObject();
                        objectInCell.gameObject.rb.excludeLayers = (1 << 9) | (1 << 10);
                        break;
                    case 3:
                        Debug.Log("case3:");
                        objectInCell.gameObject.gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                        if (objectInCell.gameObject.lastParentWasStorage)
                        {
                            if (characterStats == null)
                            {
                                characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                            }
                            decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)objectInCell.gameObject.weight;
                            characterStats.storageWeight = (float)Math.Round(objectInCell.gameObject.weight, 2);
                            objectInCell.gameObject.lastParentWasStorage = false;
                        }
                        objectInCell.gameObject.needToDynamic = true;
                        //objectInCell.gameObject.Impulse = true;
                        //objectInCell.gameObject.MoveObjectOnEndDrag();
                        objectInCell.gameObject.IgnoreCollisionObject(false);
                        objectInCell.gameObject.rb.excludeLayers = (1 << 10) | (1 << 11);// (1 << 9);
                        break;
                }
            }
            else
            {
                objectInCell.gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                var nestedObjectSprite = objectInCell.gameObject.GetComponent<SpriteRenderer>();
                nestedObjectSprite.sortingLayerName = "Weapon";
                nestedObjectSprite.sortingOrder = 1;
                if (!objectInCell.gameObject.lastParentWasStorage)
                {
                    if (characterStats == null)
                    {
                        characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                    }
                    decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)objectInCell.gameObject.weight;
                    characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                    objectInCell.gameObject.lastParentWasStorage = true;
                }
                objectInCell.gameObject.needToDynamic = true;
                objectInCell.gameObject.rb.excludeLayers = (1 << 10) | (1 << 11);
            }
            objectInCell.gameObject.ChangeColorToDefault();
            //objectInCell.gameObject.gameObject.layer = objectInCell.gameObject.originalLayer;
        }
        objectsInCells.Clear();
    }
    public new void SetNestedObject()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject = gameObject;
        }
    }

    public override void CorrectPosition()
    {
        if (hits.Where(e => e.hits[0].collider == null).Count() == 0)
        {
            var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
            Vector3 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;
            //Vector2 colliderPos2 = careHits[0].raycastHit.collider.transform.position;

            for (int i = 1; i < careHits.Count; i++)
            {
                if (careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                {
                    maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                }
            }
            var newListCareHits = careHits.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY).ToList();
            var minX = newListCareHits[0].raycastHit.collider.transform.localPosition.x;
            foreach (var careHit in newListCareHits)
            {
                if (careHit.raycastHit.collider.transform.localPosition.y == maxY)
                {
                    if (careHit.raycastHit.collider.transform.localPosition.x <= minX)
                    {
                        minX = careHit.raycastHit.collider.transform.localPosition.x;
                        colliderPos = careHit.raycastHit.collider.transform.localPosition;
                    }
                }
            }
            rectTransform.SetParent(bagTransform);
            gameObject.transform.SetAsFirstSibling();
            Vector3 offset;
            //offset = calculateOffset(itemColliders);
            if (itemColliders.Count == 4)
                offset = new Vector2(itemColliders[0].size.x / 2, -itemColliders[0].size.y / 2);
            else if (itemColliders.Count == 9)
                offset = new Vector2(itemColliders[0].size.x, -itemColliders[0].size.y);
            else if (itemColliders.Count == 2)
            {
                if (rectTransform.eulerAngles.z == 90f || rectTransform.eulerAngles.z == 270f)
                {
                    offset = new Vector2(0, -itemColliders[0].size.y / 2);
                }
                else
                {
                    offset = new Vector2(itemColliders[0].size.x / 2, 0);
                }
            }
            else
            {
                offset = new Vector2(0, 0);
            }

            rectTransform.localPosition = offset + colliderPos + new Vector3(0f, 0f, -1f);
            needToDynamic = false;
            //Debug.Log("offset: " + offset.ToString());
            //Debug.Log("colliderPos: " + colliderPos.ToString());
            //Debug.Log("offset + colliderPos: " + rectTransform.localPosition.ToString());
            foreach (var careHit in careHits)
            {
                careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
    public override void ExtendedCorrectPosition()
    {
        switch (ExtendedCorrectEndPoint())
        {
            case 1:
                //Debug.Log("1");
                //rb.bodyType = RigidbodyType2D.Static;
                SetNestedObject();
                EndDragForChildObjects(true);
                rb.excludeLayers = (1 << 9) | (1 << 10);
                break;
            case 2:
                //Debug.Log("3");
                gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                EndDragForChildObjects(false);
                Impulse = true;
                //MoveObjectOnEndDrag();
                rb.excludeLayers = 0;
                break;
            case 3:
                //Debug.Log("3");
                gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                EndDragForChildObjects(false);
                Impulse = true;
                //MoveObjectOnEndDrag();
                rb.excludeLayers = 0;
                break;
        }
    }

    private void EndDrag()
    {
        ChangeColorToDefault();
        ExtendedCorrectPosition();
        DisableBackpackCells();
        //ClearParentForChild();
        SetOrderLayerPriority("Bag", "Weapon", 1);
        Debug.Log(3);
        careHits.Clear();
        canShowDescription = true;
        
    }

    //var armor1 = -30;
    //Debug.Log(armor1)
    //-30 * 0.06 / 1 - (-30 * 0.06) = 



    public override void OnMouseUp()
    {
        if (isDragging)
        {
            DragManager.isDragging = false;
            needToRotate = false;
            if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null) animator.Play("ItemClickOff");
            if (GetComponent<AnimationStart>() != null)
            {
                GetComponent<AnimationStart>().Play();
            }
            IgnoreCollisionObject(false);
        }
        //if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        if(SceneManager.GetActiveScene().name != "BackPackBattle")
        {
            //List<GameObject> gameObjects = new List<GameObject>();
            //ItemInGameObject("backpack", gameObjects);
            if (shopItem != null)
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {
                    shopItem.BuyItem(gameObject.GetComponent<Item>());
                    DeleteNestedObject(gameObject.transform.parent.tag);
                    EndDrag();
                    placeForDescription = GameObject.FindWithTag("DescriptionPlace");
                }
                else
                {
                    if (shopItem.defaultPosition != transform.position)
                    {
                        returnToOriginalPosition = StartCoroutine(ReturnToOriginalPosition(shopItem.defaultPosition));
                        SetOrderLayerPriority("Bag", "Weapon", 1);
                        Debug.Log(1);
                    }
                }
            }
            else
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {
                    DeleteNestedObject(gameObject.transform.parent.tag);
                    EndDrag();
                }
                else
                {
                    returnToOriginalPosition = StartCoroutine(ReturnToOriginalPosition(lastItemPosition));
                    SetOrderLayerPriority("Bag", "Weapon", 1);
                    Debug.Log(2);
                }
            }

            if (isSellChest)
            {
                SellItem();
            }


            canShowDescription = true;
            ChangeColorToDefault();
            // Заканчиваем перетаскивание
            isDragging = false;
            //gameObject.layer = originalLayer;
            StartCoroutine(ShowDescription());
        }

       


    }

    public virtual void CoolDownStart()
    {

    }
}