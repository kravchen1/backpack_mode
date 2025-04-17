// LeafSpawner.cs
using UnityEngine;
using System.Collections;

public class LeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;
    public float spawnRate = 0.5f;
    public float spawnHeightAboveCamera = 1f; // Высота спавна над камерой
    private Coroutine spawnCoroutine;
    private Camera mainCamera;
    private float spawnYPosition;
    public Sprite[] leafSprites;

    void Start()
    {
        mainCamera = Camera.main;
        CalculateSpawnYPosition();
        //StartCoroutine(SpawnLeaves());
    }


    void CalculateSpawnYPosition()
    {
        // Вычисляем верхнюю границу камеры
        float cameraTop = mainCamera.transform.position.y + mainCamera.orthographicSize;
        spawnYPosition = cameraTop + spawnHeightAboveCamera;
    }

    void OnDisable()
    {
        // Останавливаем корутину при выключении
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            // Случайная позиция по X в пределах видимой области камеры
            float randomX = Random.Range(-cameraWidth / 2, cameraWidth / 2);

            // Позиция спавна (фиксированная Y, случайная X)
            Vector2 spawnPos = new Vector2(randomX, spawnYPosition);

            GameObject leaf = Instantiate(leafPrefab, spawnPos, Quaternion.identity, transform);
            leaf.GetComponentInChildren<SpriteRenderer>().sprite = leafSprites[Random.Range(0, leafSprites.Length)];
            // Добавляем небольшой случайный поворот
            float rotation = Random.Range(0, 360f);
            leaf.transform.rotation = Quaternion.Euler(0, 0, rotation);

            yield return new WaitForSeconds(spawnRate);
        }
    }
    void Update()
    {
        // Очищаем ссылку на корутину, если она завершилась
        if (spawnCoroutine == null && this.isActiveAndEnabled)
        {
            spawnCoroutine = StartCoroutine(SpawnLeaves());
        }
    }
}