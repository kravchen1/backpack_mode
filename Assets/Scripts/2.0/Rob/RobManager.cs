using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobManager : MonoBehaviour
{
    public static RobManager Instance;
    public NPCBaseTrigger trigger;
    public GameObject robBlock;
    public Transform robAreaTransform;

    public GameObject RobPlayerInventory;
    public GameObject RobShopInventory;
    public GameObject RobBlocksCanvas;
    private CellsData shopInventory;
    private CellsData playerInventory;

    public Vector2 localPositionMinRobBlock = new Vector2(-110f, -510f);
    public Vector2 localPositionMaxRobBlock = new Vector2(120f, 510f);

    public int countRobBlocks = 5;
    public float speedRobBlocks = 1.0f;
    public bool isRobActive = false;

    private List<GameObject> activeRobBlocks = new List<GameObject>();
    private List<Vector2> targetPositions = new List<Vector2>();
    private Coroutine robCoroutine;



    private void Awake()
    {
        if (Instance == null)
        {
            shopInventory = RobShopInventory.transform.GetChild(0).gameObject.GetComponent<CellsData>();
            playerInventory = RobPlayerInventory.transform.GetChild(0).gameObject.GetComponent<CellsData>();
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRob(string settingsKeyShop)
    {
        if (isRobActive) return;

        isRobActive = true;
        RobPlayerInventory.SetActive(true);
        RobShopInventory.SetActive(true);
        RobBlocksCanvas.SetActive(true);

        shopInventory.settingsKey = settingsKeyShop;
        shopInventory.LoadData();

        // Создаем блоки
        for (int i = 0; i < countRobBlocks; i++)
        {
            CreateRobBlock(i);
        }

        // Запускаем корутину для движения блоков
        robCoroutine = StartCoroutine(MoveRobBlocks());
    }

    public void EndRob()
    {
        if (!isRobActive) return;
        RobPlayerInventory.SetActive(false);
        RobShopInventory.SetActive(false);
        RobBlocksCanvas.SetActive(false);

        // Останавливаем корутину
        if (robCoroutine != null)
        {
            StopCoroutine(robCoroutine);
            robCoroutine = null;
        }
        ClearExistingBlocks();
        shopInventory.SaveData();
        playerInventory.SaveData();
        isRobActive = false;
    }

    public void EndRobWithCatch()
    {
        if (!isRobActive) return;
        RobPlayerInventory.SetActive(false);
        RobShopInventory.SetActive(false);
        RobBlocksCanvas.SetActive(false);

        // Останавливаем корутину
        if (robCoroutine != null)
        {
            StopCoroutine(robCoroutine);
            robCoroutine = null;
        }
        ClearExistingBlocks();
        isRobActive = false;
    }

    private void CreateRobBlock(int index)
    {
        if (robBlock == null)
        {
            Debug.LogError("RobBlock prefab is not assigned!");
            return;
        }

        GameObject block = Instantiate(robBlock, robAreaTransform);
        block.name = $"RobBlock_{index}";

        // Устанавливаем случайную начальную позицию
        Vector2 randomPosition = GetRandomPosition();
        ((RectTransform)block.transform).localPosition = randomPosition;

        // Сохраняем ссылку и целевую позицию
        activeRobBlocks.Add(block);
        targetPositions.Add(GetRandomPosition());
    }

    private IEnumerator MoveRobBlocks()
    {
        while (isRobActive && activeRobBlocks.Count > 0)
        {
            for (int i = 0; i < activeRobBlocks.Count; i++)
            {
                if (activeRobBlocks[i] == null) continue;

                MoveBlockToTarget(i);
            }
            yield return null;
        }
    }

    private void MoveBlockToTarget(int index)
    {
        Transform blockTransform = activeRobBlocks[index].transform;
        Vector2 currentPos = ((RectTransform)blockTransform).localPosition;
        Vector2 targetPos = targetPositions[index];

        // Двигаемся к целевой позиции
        ((RectTransform)blockTransform).localPosition = Vector2.MoveTowards(
            currentPos, targetPos, speedRobBlocks * Time.deltaTime);

        // Если достигли цели, выбираем новую целевую позицию
        if (Vector2.Distance(currentPos, targetPos) < 0.1f)
        {
            targetPositions[index] = GetRandomPosition();
        }
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(localPositionMinRobBlock.x, localPositionMaxRobBlock.x),
            Random.Range(localPositionMinRobBlock.y, localPositionMaxRobBlock.y)
        );
    }

    private void ClearExistingBlocks()
    {
        // Останавливаем корутину
        if (robCoroutine != null)
        {
            StopCoroutine(robCoroutine);
            robCoroutine = null;
        }

        // Удаляем все блоки
        foreach (GameObject block in activeRobBlocks)
        {
            if (block != null)
                Destroy(block);
        }

        activeRobBlocks.Clear();
        targetPositions.Clear();
    }

    public void Catch()
    {
        if (!isRobActive) return;

        EndRobWithCatch();

        if (trigger != null)
        {
            trigger.Attack();
        }
        else
        {
            Debug.LogWarning("No trigger assigned to RobManager");
        }
    }

    private void OnDestroy()
    {
        ClearExistingBlocks();
    }
}