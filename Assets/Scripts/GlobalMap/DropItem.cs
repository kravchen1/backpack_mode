using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Item;


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

    public int stoneLevel;

    private float jumpHeight = 20f; // Высота прыжка
    private float jumpDistanceX = 30f; // Расстояние по X
    private float jumpDistanceY = 30f; // Расстояние по Y
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

        jumpDistanceX = UnityEngine.Random.Range(-45, 45);
        jumpDistanceY = UnityEngine.Random.Range(0, 30);
        rotationSpeed = UnityEngine.Random.Range(180, 360);
        jumpHeight = UnityEngine.Random.Range(10, 60);
        ///int rX = UnityEngine.Random.Range(1, 3);
        int rZ = UnityEngine.Random.Range(1, 3);

        //if (rX == 1)
        //    jumpDistance *= -1;
        if (rZ == 1)
            rotationSpeed *= -1;

        targetPosition = startPosition + new Vector3(jumpDistanceX, jumpDistanceY, 0);

        isJumping = true;
        Item itemClass = item.GetComponent<Item>();
        Color newColorForCircle;
        switch (itemClass.rarity)
        {
            case ItemRarity.Rare:
                newColorForCircle = new Color(0.5707547f, 0.6549992f, 1f, 0.4f);
                break;
            case ItemRarity.Epic:
                newColorForCircle = new Color(0.6559475f, 0.4571022f, 0.9056604f, 0.4f);
                break;
            case ItemRarity.Legendary:
                newColorForCircle = new Color(0.8584906f, 0.590014f, 0.4170969f, 0.6705883f);
                break;
            default:
                newColorForCircle = new Color(1f, 1f, 1f, 0.4f);
                break;
        }
        Light.GetComponent<SpriteRenderer>().color = newColorForCircle;
    }


    private void OnTriggerEnter2D()
    {
        isPlayerInTrigger = true;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            characterStats = player.GetComponent<CharacterStats>();
            //descriptionPlace = GameObject.FindGameObjectWithTag("DescriptionPlace");
        }
        //instDescription = Instantiate(description, descriptionPlace.GetComponent<RectTransform>().transform);
        SetActivePressE(isShowPressE);
    }

    public void Activate()
    {
        giveItem(item.name);
        SetStorageWeigth(item.GetComponent<Item>().weight);
        //player.animator.play("giveItem");
        Destroy(gameObject);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
        //Destroy(instDescription);
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

            // Рассчитываем позицию по X и Y (линейная интерполяция)
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float y = Mathf.Lerp(startPosition.y, targetPosition.y, t);

            // Добавляем параболическую составляющую по Y
            float parabola = Mathf.Sin(t * Mathf.PI); // Парабола от 0 до 1 и обратно
            y += jumpHeight * parabola; // Применяем высоту прыжка

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

        //Debug.Log(backPackAndStorageData.storageData.itemData.items.Count / 8);
        //Debug.Log(backPackAndStorageData.storageData.itemData.items.Count % 8);
        int y = backPackAndStorageData.storageData.itemData.items.Count / 8;
        int x = backPackAndStorageData.storageData.itemData.items.Count % 8;
        backPackAndStorageData.storageData.itemData.items.Add(new Data(itemName, new Vector3(-370 + (82 * x), -120 + (45 * y), -2)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
    }

    private void SetStorageWeigth(float weight)
    {
        decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)weight;
        characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
        characterStats.SaveData();
    }
}

