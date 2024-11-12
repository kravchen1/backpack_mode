using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject goMap;
    public Animator animator;
    private generateMapScript generateMapScript;

    [HideInInspector] public Map map;
    [HideInInspector] public RectTransform rectTransform;

    // [SerializeField] private Canvas backpackCanvas;
    //[SerializeField] private Canvas mapCanvas;

    private CharacterStats characterStats;

    [HideInInspector] public Rigidbody2D rb;
    //private float speed = 3f;
    private Vector2 moveVector;

    [HideInInspector] public RaycastHit2D hit;

    private new Collider2D collider;
    private float careY;
    [HideInInspector] public GameObject activePoint;
    private Color trueActivePointColor;

    private List<SpriteRenderer> sprites;
    [HideInInspector] public bool startMove = false;
    bool B_FacingRight = false;

    bool createdDialogCanvas = false;

    [HideInInspector] public Collider2D lastCrossCollider;

    private GameObject dialogCanvas;

    public Vector2 targetPosition;

    private bool needToRaycast = true;

    public GameObject mainCamera;

    //public MainCamera scriptMainCamera;
    //private float movingStepCamera = 0.9f;



    private void Awake()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        goMap = GameObject.FindGameObjectWithTag("GoMap");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //scriptMainCamera = mainCamera.GetComponent<MainCamera>();
    }
    void LoadCharacterStats()
    {
        characterStats = GetComponent<CharacterStats>();
        characterStats.LoadData("Assets/Saves/characterStatsData.json");
        characterStats.InitializeCharacterStats();
    }
    void SetStartPosition()
    {
        rectTransform.anchoredPosition = map.startPlayerPosition;
        //startMove = true;
        Debug.Log(map.startPlayerPosition);
        Debug.Log(rectTransform.anchoredPosition);
    }


    void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        map = goMap.GetComponent<generateMapScript>();
        generateMapScript = goMap.GetComponent<generateMapScript>();
        //SetStartPosition();
        LoadCharacterStats();
        if (characterStats.playerTime >= 9f)
        {
            map.ChangeMapRedTimeZone();
        }
        startMove = true;
    }


    public void InstantinateDialog()
    {
        if (hit.collider.gameObject.name.Contains("Battle") || hit.collider.gameObject.name.Contains("Portal") || hit.collider.gameObject.name.Contains("Fountain") || hit.collider.gameObject.name.Contains("ChestOfFortune") || hit.collider.gameObject.name.Contains("Forge"))
        {
            activePoint = hit.collider.gameObject.GameObject();
            activePoint.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            if (!createdDialogCanvas)
            {
                startMove = false;
                //Time.timeScale = 0f;
                if (hit.collider.gameObject.name.Contains("Battle") || hit.collider.gameObject.name.Contains("Portal"))
                    dialogCanvas = Resources.Load<GameObject>("DialogBattleCanvas");
                else if (hit.collider.gameObject.name.Contains("Fountain"))
                    dialogCanvas = Resources.Load<GameObject>("DialogFountainCanvas");
                else if (hit.collider.gameObject.name.Contains("ChestOfFortune"))
                {
                    dialogCanvas = Resources.Load<GameObject>("DialogChestOfFortuneCanvas");
                    dialogCanvas.GetComponent<DialogCanvas>().GenerateEvent();
                }
                else if (hit.collider.gameObject.name.Contains("Forge"))
                {
                    dialogCanvas = Resources.Load<GameObject>("DialogForgeCanvas");
                    //dialogCanvas.GetComponent<DialogCanvas>().GenerateEvent();
                }

                var canvas = Instantiate(dialogCanvas, GameObject.FindGameObjectWithTag("Camera Canvas").GetComponent<RectTransform>().transform);
                canvas.gameObject.SetActive(true);
                //canvas.transform.GetChild(0).GetComponent<PointInterestButtonYesNO>().pointInterestCollision = hit.collider;
                //canvas.transform.GetChild(1).GetComponent<PointInterestButtonYesNO>().pointInterestCollision = hit.collider;
                createdDialogCanvas = true;
            }
        }
        else
        {
            lastCrossCollider = hit.collider;
        }
    }

    public void RaycastEvent()
    {
        if (rectTransform != null)
        {
            hit = Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f));
            //Debug.Log(hit.);
        }


        if (hit.collider != null)
        {
            InstantinateDialog();
            //Debug.Log(activePoint.name);
        }

        if (activePoint != null && (hit.collider == null || activePoint != hit.collider.gameObject.GameObject()))
        {
            activePoint.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1);
            createdDialogCanvas = false;
        }
    }
    //void pressF()
    //{
    //    if(Input.GetButton("Jump"))
    //    {
    //        if (activePoint != null && activePoint.name.Contains("Shop"))
    //        {

    //            Time.timeScale = 0f;
    //            map.startPlayerPosition = rectTransform.anchoredPosition;
    //            characterStats.playerTime += 1f;
    //            map.SaveData("Assets/Saves/mapData.json");
    //            characterStats.SaveData();
    //            //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
    //            SceneManager.LoadScene("BackPackShop");
    //        }
    //        if (activePoint != null && activePoint.name.Contains("Battle"))
    //        {

    //            Time.timeScale = 0f;
    //            map.startPlayerPosition = rectTransform.anchoredPosition;
    //            characterStats.playerTime += 2f;
    //            map.SaveData("Assets/Saves/mapData.json");
    //            characterStats.SaveData();
    //            //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
    //            PlayerPrefs.SetString("enemyName", activePoint.gameObject.name.Replace("(Clone)", ""));
    //            SceneManager.LoadScene("BackPackBattle");
    //        }
    //        if (activePoint != null && activePoint.name.Contains("Portal"))
    //        {

    //            Time.timeScale = 0f;
    //            characterStats.playerTime = 0f;
    //            characterStats.SaveData();
    //            map.DeleteData("Assets/Saves/mapData.json");
    //            //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
    //            SceneManager.LoadScene("GenerateMap");
    //        }
    //    }

    //}
    void pressI()
    {
        //if (!backpackCanvas.gameObject.activeInHierarchy)
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        mapCanvas.gameObject.SetActive(false);
        //        backpackCanvas.gameObject.SetActive(true);
        //    }
        //}
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        mapCanvas.gameObject.SetActive(true);
        //        backpackCanvas.gameObject.SetActive(false);
        //    }
        //}
    }

    //void pressEsc()
    //{
    //    if (backpackCanvas.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        backpackCanvas.gameObject.SetActive(false);
    //    }
    //}

    void Start()
    {
        Invoke("Initialize", 0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    void Filp()
    {
        B_FacingRight = !B_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        transform.localScale = theScale;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

    }


    private void Update()
    {
        if (startMove)
        {
            if (needToRaycast)
            {
                if (Input.GetAxis("Horizontal") > 0 && B_FacingRight)
                {
                    Filp();
                }
                else if (Input.GetAxis("Horizontal") < 0 && !B_FacingRight)
                {
                    Filp();
                }
                // Получаем ввод от пользователя
                if (Input.GetKeyDown(KeyCode.W)) // Вверх
                {
                    targetPosition = rectTransform.anchoredPosition + new Vector2(0, 100);
                    if (!map.tiles.Any(e => e.tilePosition == targetPosition && e.tileName.ToUpper().Contains("TREE")) && targetPosition.y < generateMapScript.height)
                    {
                        needToRaycast = false;
                        StartCoroutine(MoveAlongParabola(rectTransform.anchoredPosition, targetPosition));
                        mainCamera.GetComponent<MoveCamera>().MoveCameraMethod(targetPosition);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S)) // Вниз
                {
                    targetPosition = rectTransform.anchoredPosition + new Vector2(0, -100);
                    if (!map.tiles.Any(e => e.tilePosition == targetPosition && e.tileName.ToUpper().Contains("TREE")) && targetPosition.y >= 0)
                    {
                        needToRaycast = false;
                        StartCoroutine(MoveAlongParabola(rectTransform.anchoredPosition, targetPosition));
                        mainCamera.GetComponent<MoveCamera>().MoveCameraMethod(targetPosition);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.A)) // Влево
                {
                    targetPosition = rectTransform.anchoredPosition + new Vector2(-100, 0);
                    if (!map.tiles.Any(e => e.tilePosition == targetPosition && e.tileName.ToUpper().Contains("TREE")) && targetPosition.x >= 0)
                    {
                        needToRaycast = false;
                        StartCoroutine(MoveAlongParabola(rectTransform.anchoredPosition, targetPosition));
                        mainCamera.GetComponent<MoveCamera>().MoveCameraMethod(targetPosition);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D)) // Вправо
                {
                    targetPosition = rectTransform.anchoredPosition + new Vector2(100, 0);
                    if (!map.tiles.Any(e => e.tilePosition == targetPosition && e.tileName.ToUpper().Contains("TREE")) && targetPosition.x < generateMapScript.width)
                    {
                        needToRaycast = false;
                        StartCoroutine(MoveAlongParabola(rectTransform.anchoredPosition, targetPosition));
                        mainCamera.GetComponent<MoveCamera>().MoveCameraMethod(targetPosition);
                    }
                }

                if (needToRaycast)
                {
                    RaycastEvent();
                }
                    
            }
            else
            {
                
            }
            //pressF();
            //pressI();
        }
    }
    public float duration = 2f; // Время, за которое объект переместится

    //private void Start()
    //{
    //    // Запускаем корутину для перемещения объекта
    //    StartCoroutine(MoveAlongParabola());
    //}

    private IEnumerator MoveAlongParabola(Vector2 startPoint, Vector2 endPoint)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Вычисляем процент завершения движения
            float t = elapsedTime / duration;

            Debug.Log(t);
            // Вычисляем положение по параболе
            Vector2 newPosition = CalculateParabola(t, startPoint, endPoint, 100f); // 100f - высота параболы
            rectTransform.anchoredPosition = newPosition;

            elapsedTime += Time.deltaTime * 10;
            yield return null; // Ждем следующий кадр
        }

        // Убедимся, что объект точно на конечной позиции
        rectTransform.anchoredPosition = endPoint;
        needToRaycast = true;
    }

    private Vector2 CalculateParabola(float t, Vector3 startPoint, Vector3 endPoint, float height)
    {
        // Вычисляем положение по параболе
        float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
        float y = Mathf.Lerp(startPoint.y, endPoint.y, t) + height * Mathf.Sin(t * Mathf.PI); // Высота параболы

        return new Vector2(x, y);
    }
}
