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
    private List<Cell> originallyOccupiedCells = new List<Cell>();

    void Awake()
    {
        Initialization();
    }

    public void Initialization()
    {
        mainCamera = Camera.main;
        playerInventory = GameObject.Find("Inventory") ?? new GameObject("Inventory");
        initializationItemColliders();
        SaveOriginalPosition();
        SaveOriginallyOccupiedCells();
    }

    void SaveOriginalPosition()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
    }

    void SaveOriginallyOccupiedCells()
    {
        originallyOccupiedCells.Clear();
        Cell[] allCells = FindObjectsOfType<Cell>();
        foreach (Cell cell in allCells)
        {
            if (cell.nestedObject == gameObject)
            {
                originallyOccupiedCells.Add(cell);
            }
        }
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
        SaveOriginalPosition();
        SaveOriginallyOccupiedCells();

        // Всегда очищаем ВСЕ ячейки при начале перетаскивания
        ClearAllCellReferences();
    }

    public virtual void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        ResetAllColorsToDefault();

        if (canBePlaced && currentGreenCells.Count > 0 && currentRedCells.Count == 0)
        {
            // Успешное размещение - занимаем новые ячейки
            FillCellNestedObjects();
            CorrectPosition();
            MoveToInventory();
            SaveOriginallyOccupiedCells();
        }
        else
        {
            // Неудачное размещение - восстанавливаем оригинальные ячейки
            RestoreOriginallyOccupiedCells();
            ReturnToOriginalPosition();
        }

        previousHitColliders.Clear();
        ClearCurrentCells();
    }

    private void RestoreOriginallyOccupiedCells()
    {
        // Очищаем все текущие ссылки
        ClearAllCellReferences();

        // Восстанавливаем только оригинальные занятые ячейки
        foreach (Cell cell in originallyOccupiedCells)
        {
            if (cell != null)
            {
                cell.nestedObject = gameObject;
            }
        }
    }

    private void ClearAllCellReferences()
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
        else if (scrollData > 0.01f)
        {
            RotateItem(90);
        }
        else if (scrollData < -0.01f)
        {
            RotateItem(-90);
        }
    }

    private void RotateItem(float angle)
    {
        ResetAllColorsToDefault();
        transform.Rotate(0, 0, angle);
        Physics2D.SyncTransforms();
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
        // Занимаем новые ячейки
        foreach (Cell cell in currentGreenCells)
        {
            if (cell != null)
            {
                cell.nestedObject = gameObject;
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
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }

    private void MoveToInventory()
    {
        transform.SetParent(playerInventory.transform);
    }

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
        ClearAllCellReferences();
        ResetAllColorsToDefault();
    }
}