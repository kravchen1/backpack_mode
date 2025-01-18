using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BagOld : ItemOld
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
            objectInCell.gameObject.DeleteNestedObject();
        }
    }
    public new void TapFirst()
    {
        if (firstTap)
        {
            firstTap = false;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            backpack = GameObject.Find("backpack");
        }
    }
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
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }


    public void SetOrderLayerPriority(string layerNameBag, string layerNameNestedObject, int weaponOrder)
    {
        image.sortingLayerName = layerNameBag;
        var cellsTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        cellsTransforms.Remove(gameObject.transform);
        cellsTransforms = cellsTransforms.Where(e => e.GetComponent<Cell>() != null).ToList();//удал€ем не €чейки

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

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            if (GetComponent<ShopItem>() != null)
            {
                shopItem = GetComponent<ShopItem>();
                if (shopItem.CanBuy(GetComponent<Item>()))
                {
                    SetOrderLayerPriority("DraggingObject", "DraggingObject", 100);
                    StayParentForChild();

                    TapFirst();
                    TapRotate();
                    TapShowBackPack();
                    DeleteNestedObject();
                    OnPointerExit(eventData);
                    canShowDescription = false;
                }
                else
                {
                    eventData.pointerDrag = null;
                }
            }
            else
            {
                SetOrderLayerPriority("DraggingObject", "DraggingObject", 100);
                StayParentForChild();

                TapFirst();
                TapRotate();
                TapShowBackPack();
                DeleteNestedObject();
                OnPointerExit(eventData);
                canShowDescription = false;
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

        ClearCareRaycast();
        CreateCareRaycast();
    }
    public void ChangeColorMyCells()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
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
    public override void OnDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            RaycastEvent();
            ChangeColorMyCells();
            SellChest();
        }
    }
    public override bool CorrectEndPoint()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            if (hits.Where(e => e.hits[0].collider == null).Count() == 0)
            {
                var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
                Vector2 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;
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
                var offset = new Vector2(itemColliders[0].size.x, -itemColliders[0].size.y);
                rectTransform.localPosition = offset + colliderPos;
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
            if (objectInCell.gameObject.CorrectEndPoint() && canEndDragParent)
            {
                //objectInCell.gameObject.ExtendedCorrectPosition();
                objectInCell.gameObject.SetNestedObject();
            }
            else
            {
                objectInCell.gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                objectInCell.gameObject.needToDynamic = true;
                objectInCell.gameObject.MoveObjectOnEndDrag();
            }

            objectInCell.gameObject.ChangeColorToDefault();
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

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            //List<GameObject> gameObjects = new List<GameObject>();
            //ItemInGameObject("backpack", gameObjects);
            if (shopItem != null)
            {
                shopItem.BuyItem(gameObject.GetComponent<Item>());
            }

            if (isSellChest)
            {
                SellItem();
                //gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                //EndDragForChildObjects(false);
                //Impulse = true;
                //MoveObjectOnEndDrag();
            }
            ChangeColorToDefault();
            needToRotate = false;
            if (CorrectEndPoint())
            {
                SetNestedObject();
                EndDragForChildObjects(true);
            }
            else
            {
                gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                EndDragForChildObjects(false);
                Impulse = true;
                MoveObjectOnEndDrag();
            }
            DisableBackpackCells();
            ClearParentForChild();
            SetOrderLayerPriority("Bag", "Weapon", 1);
            careHits.Clear();
            canShowDescription = true;
            OnPointerEnter(eventData);

        }




    }
}