using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class DropItem : EventParent
{
    public GameObject Light;

    private GameObject player;
    private Player classPlayer;
    private CharacterStats characterStats;
    private bool isPlayerInTrigger = false;

    private GameObject descriptionPlace;
    public GameObject item;
    public GameObject description;
    private GameObject instDescription;



    private float jumpHeight = 20f; // Высота прыжка
    private float jumpDistance = 30f; // Расстояние по X
    private float jumpDuration = 1f; // Длительность прыжка
    private float rotationSpeed = 360f; // Скорость вращения (градусы в секунду)

    private Vector3 startPosition; // Стартовая позиция
    private Vector3 targetPosition; // Конечная позиция
    private float elapsedTime = 0f; // Прошедшее время

    private bool isJumping = false; // Флаг, указывающий, что объект в прыжке
    private GameObject itemSprite;
    private void Awake()
    {
        // Запоминаем стартовую позицию
        itemSprite = transform.GetChild(0).gameObject;
        startPosition = itemSprite.transform.position;

        jumpDistance = UnityEngine.Random.Range(20, 45);
        jumpHeight = UnityEngine.Random.Range(20, 30);
        int rX = UnityEngine.Random.Range(1, 3);
        int rZ = UnityEngine.Random.Range(1, 3);

        if (rX == 1)
            jumpDistance *= -1;
        if (rZ == 1)
            rotationSpeed *= -1;


        targetPosition = startPosition + new Vector3(jumpDistance, 0, 0);



        isJumping = true;


    }


    private void OnTriggerEnter2D()
    {
        isPlayerInTrigger = true;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            characterStats = player.GetComponent<CharacterStats>();
            descriptionPlace = GameObject.FindGameObjectWithTag("DescriptionPlace");
        }
        instDescription = Instantiate(description, descriptionPlace.GetComponent<RectTransform>().transform);
        SetActivePressE(isShowPressE);
    }

    public void Activate()
    {
        giveItem(item.name);
        //player.animator.play("giveItem");
        Destroy(gameObject);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
        Destroy(instDescription);
    }


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            Activate();
        }

        if (isJumping)
        {
            // Увеличиваем прошедшее время
            elapsedTime += Time.deltaTime;

            // Нормализованное время (от 0 до 1)
            float t = elapsedTime / jumpDuration;

            // Если прыжок завершен
            if (t >= 1f)
            {
                isJumping = false;
                itemSprite.transform.position = targetPosition; // Убедимся, что объект точно в конечной позиции
                gameObject.transform.position = itemSprite.transform.position;
                itemSprite.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                gameObject.GetComponent<CircleCollider2D>().enabled = true;
                Light.SetActive(true);
                return;
            }

            // Рассчитываем позицию по X (линейная интерполяция)
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);

            // Рассчитываем позицию по Y (параболическая траектория)
            float y = startPosition.y + jumpHeight * Mathf.Sin(t * Mathf.PI);

            // Применяем новую позицию
            itemSprite.transform.position = new Vector3(x, y, startPosition.z);
            // Вращаем объект
            float rotationAmount = rotationSpeed * Time.deltaTime; // Угол поворота за кадр
            itemSprite.transform.Rotate(0, 0, rotationAmount); // Вращаем вокруг оси Z
        }

    }

    private void giveItem(string itemName)
    {
        BackPackAndStorageData backPackAndStorageData = new BackPackAndStorageData();
        backPackAndStorageData.storageData = new BackpackData();
        backPackAndStorageData.storageData.itemData = new ItemData();
        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json")))
        {
            backPackAndStorageData.storageData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
        }

        backPackAndStorageData.storageData.itemData.items.Add(new Data(itemName, new Vector2(0, 0)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
    }
}

