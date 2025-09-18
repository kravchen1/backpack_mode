using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemNew : MonoBehaviour
{
    public ItemType itemType;
    public ItemRarity rarity;
    public int WidthCell;
    public int HeightCell;

    protected RaycastHit2D[] hits1;
    protected RaycastHit2D[] hits2;
    protected RaycastHit2D[] hitCells;

    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 offset;
    [HideInInspector] public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    private BoxCollider2D[] collidersArray;
    private int colliderCount;

    private List<Collider2D> previousHitColliders = new List<Collider2D>();
    private Dictionary<Collider2D, Color> originalColors = new Dictionary<Collider2D, Color>();
    private List<Cell> currentGreenCells = new List<Cell>();
    private List<Cell> currentRedCells = new List<Cell>();
    private Vector3 originalPosition;
    private bool canBePlaced = true;
    private Transform originalParent;
    private GameObject playerInventory;
    private bool needToRotate = true;

    void Awake()
    {
        Initialization();
    }

    public void Initialization()
    {
        mainCamera = Camera.main;
        playerInventory = GameObject.Find("Inventory") ?? new GameObject("Inventory");
        initializationItemColliders();
        originalPosition = transform.position;
        originalParent = transform.parent;
    }

    private void initializationItemColliders()
    {
        itemColliders.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            BoxCollider2D[] childColliders = child.GetComponentsInChildren<BoxCollider2D>();
            itemColliders.AddRange(childColliders);
        }

        colliderCount = itemColliders.Count;
        hits1 = new RaycastHit2D[colliderCount];
        hits2 = new RaycastHit2D[colliderCount];
        hitCells = new RaycastHit2D[colliderCount];
    }

    public virtual void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        SaveOriginalColors();
        ClearCurrentCells();
        canBePlaced = true;
        ClearOldCellReferences();
    }

    public virtual void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // Сбрасываем цвета ВСЕХ ячеек сразу
        ResetAllColorsToDefault();

        if (canBePlaced && currentGreenCells.Count > 0 && currentRedCells.Count == 0)
        {
            FillCellNestedObjects();
            CorrectPosition();
            MoveToInventory();
        }
        else
        {
            ClearOldCellReferences();
            ReturnToOriginalPosition();
        }

        previousHitColliders.Clear();
        ClearCurrentCells();
    }

    public virtual void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
            RaycastEvent();
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem(90);
        }
        else if (scrollData > 0.01f) // Прокрутка вверх
        {
            RotateItem(90);
        }
        else if (scrollData < -0.01f) // Прокрутка вниз
        {
            RotateItem(-90);
        }
    }

    private void RotateItem(float angle)
    {
        // Сбрасываем цвета перед поворотом
        ResetAllColorsToDefault();

        transform.Rotate(0, 0, angle);
        Physics2D.SyncTransforms();

        // Обновляем рейкасты после поворота
        UpdateRaycastImmediately();
    }

    private void UpdateRaycastImmediately()
    {
        LayerMask mask = 1 << 8;
        hits1 = CreateCareRaycast(mask);

        CheckPlacementValidity(hits1);
        UpdateColorsWithOccupationCheck(hits1);
        UpdatePreviousColliders(hits1);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    public virtual void RaycastEvent()
    {
        LayerMask mask = 1 << 8;
        hits1 = CreateCareRaycast(mask);

        CheckPlacementValidity(hits1);
        ResetColorsForMissedColliders(hits1);
        UpdateColorsWithOccupationCheck(hits1);
        UpdatePreviousColliders(hits1);
    }

    protected virtual RaycastHit2D[] CreateCareRaycast(int mask)
    {
        RaycastHit2D[] hitsTemp = new RaycastHit2D[colliderCount];
        for (int i = 0; i < colliderCount; i++)
        {
            hitsTemp[i] = Physics2D.Raycast(itemColliders[i].bounds.center, Vector2.zero, 0, mask);
        }
        return hitsTemp;
    }

    private void CheckPlacementValidity(RaycastHit2D[] currentHits)
    {
        canBePlaced = true;

        foreach (var hit in currentHits)
        {
            if (hit.collider == null)
            {
                canBePlaced = false;
                return;
            }

            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell != null && cell.nestedObject != null && cell.nestedObject != gameObject)
            {
                canBePlaced = false;
                return;
            }
        }
    }

    private void UpdateColorsWithOccupationCheck(RaycastHit2D[] currentHits)
    {
        ClearCurrentCells();

        for (int i = 0; i < colliderCount; i++)
        {
            if (currentHits[i].collider == null) continue;

            Cell cell = currentHits[i].collider.GetComponent<Cell>();
            if (cell == null) continue;

            SpriteRenderer sr = currentHits[i].collider.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            if (cell.nestedObject != null && cell.nestedObject != gameObject)
            {
                sr.color = Color.red;
                currentRedCells.Add(cell);
            }
            else
            {
                sr.color = Color.green;
                currentGreenCells.Add(cell);
            }
        }
    }

    private void FillCellNestedObjects()
    {
        foreach (Cell cell in currentGreenCells)
        {
            if (cell != null)
            {
                cell.nestedObject = gameObject;
            }
        }
    }

    private void ClearOldCellReferences()
    {
        Cell[] allCells = FindObjectsOfType<Cell>();
        foreach (Cell cell in allCells)
        {
            if (cell.nestedObject == gameObject)
            {
                cell.nestedObject = null;
            }
        }
    }

    private void ClearCurrentCells()
    {
        currentGreenCells.Clear();
        currentRedCells.Clear();
    }

    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.SetParent(originalParent);
        transform.rotation = Quaternion.identity;
    }

    private void MoveToInventory()
    {
        transform.SetParent(playerInventory.transform);
    }

    //public virtual Vector3 calculateOffset(List<BoxCollider2D> itemColliders)
    //{
    //    if (itemColliders == null || itemColliders.Count == 0)
    //        return Vector3.zero;

    //    Bounds totalBounds = new Bounds(itemColliders[0].bounds.center, Vector3.zero);
    //    foreach (var collider in itemColliders)
    //    {
    //        totalBounds.Encapsulate(collider.bounds);
    //    }

    //    return -totalBounds.center + transform.position;
    //}

    public virtual void CorrectPosition()
    {
        if (currentGreenCells.Count == 0) return;

        Bounds cellsBounds = new Bounds(currentGreenCells[0].transform.position, Vector3.zero);
        foreach (Cell cell in currentGreenCells)
        {
            cellsBounds.Encapsulate(cell.transform.position);
        }

        Bounds itemBounds = new Bounds(itemColliders[0].bounds.center, Vector3.zero);
        foreach (var collider in itemColliders)
        {
            itemBounds.Encapsulate(collider.bounds);
        }

        Vector3 centerOffset = cellsBounds.center - itemBounds.center;
        transform.position += centerOffset;
    }

    private void ResetColorsForMissedColliders(RaycastHit2D[] currentHits)
    {
        List<Collider2D> currentColliders = new List<Collider2D>();
        foreach (var hit in currentHits)
        {
            if (hit.collider != null)
            {
                currentColliders.Add(hit.collider);
            }
        }

        // Сбрасываем цвета у всех предыдущих коллайдеров, которых нет в текущих
        foreach (var previousCollider in previousHitColliders.ToList())
        {
            if (previousCollider != null && !currentColliders.Contains(previousCollider))
            {
                ResetColliderColorToDefault(previousCollider);
                previousHitColliders.Remove(previousCollider);
            }
        }
    }

    private void UpdatePreviousColliders(RaycastHit2D[] currentHits)
    {
        // Очищаем только те, которые больше не актуальны
        foreach (var previousCollider in previousHitColliders.ToList())
        {
            bool stillExists = false;
            foreach (var hit in currentHits)
            {
                if (hit.collider == previousCollider)
                {
                    stillExists = true;
                    break;
                }
            }

            if (!stillExists && previousCollider != null)
            {
                ResetColliderColorToDefault(previousCollider);
                previousHitColliders.Remove(previousCollider);
            }
        }

        // Добавляем новые коллайдеры
        foreach (var hit in currentHits)
        {
            if (hit.collider != null && !previousHitColliders.Contains(hit.collider))
            {
                previousHitColliders.Add(hit.collider);
            }
        }
    }

    private void ResetColliderColorToDefault(Collider2D collider)
    {
        if (collider == null) return;

        Cell cell = collider.GetComponent<Cell>();
        if (cell == null) return;

        SpriteRenderer sr = collider.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        if (originalColors.TryGetValue(collider, out Color originalColor))
        {
            sr.color = originalColor;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    private void ResetAllColorsToDefault()
    {
        // Сбрасываем цвета ВСЕХ ячеек, которые были изменены
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                SpriteRenderer sr = kvp.Key.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = kvp.Value;
                }
            }
        }

        // Также сбрасываем цвета у текущих коллайдеров на всякий случай
        foreach (var collider in previousHitColliders.ToList())
        {
            if (collider != null)
            {
                ResetColliderColorToDefault(collider);
            }
        }

        previousHitColliders.Clear();
        originalColors.Clear();
    }

    private void SaveOriginalColors()
    {
        originalColors.Clear();
        LayerMask mask = 1 << 8;
        Collider2D[] allColliders = Physics2D.OverlapAreaAll(new Vector2(-100, -100), new Vector2(100, 100), mask);

        foreach (var collider in allColliders)
        {
            Cell cell = collider.GetComponent<Cell>();
            if (cell == null) continue;

            SpriteRenderer sr = collider.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            originalColors[collider] = sr.color;
        }
    }

    void OnDestroy()
    {
        ClearOldCellReferences();
        ResetAllColorsToDefault();
    }
}