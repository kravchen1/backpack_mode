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
    //private generateMapScript map;

    [HideInInspector] public Map map;
    private RectTransform rectTransform;
    private Collider2D previusTree = null;
    // [SerializeField] private Canvas backpackCanvas;
    //[SerializeField] private Canvas mapCanvas;

    private CharacterStats characterStats;

    [HideInInspector] public Rigidbody2D rb;
    private float speed = 1f;
    private Vector2 moveVector;

    [HideInInspector] public RaycastHit2D hit;

    private new Collider2D collider;

    [HideInInspector] public GameObject activePoint;
    private Color trueActivePointColor;

    private List<SpriteRenderer> sprites;
    [HideInInspector] public bool startMove = false;
    bool B_FacingRight = true;

    bool createdDialogCanvas = false;

    [HideInInspector] public Collider2D lastCrossCollider;
    [HideInInspector] public GameObject mainCamera;

    private GameObject dialogCanvas;

    private TextMeshPro countdown;
    private void Awake()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        goMap = GameObject.FindGameObjectWithTag("GoMap");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        countdown = GameObject.FindGameObjectWithTag("Countdown").GetComponent<TextMeshPro>();
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
        Debug.Log(map.startPlayerPosition);
        Debug.Log(rectTransform.anchoredPosition);
    }


    void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        map = goMap.GetComponent<generateMapScript>();

        //SetStartPosition();
        LoadCharacterStats();
        if(characterStats.playerTime >= 9f)
        {
            map.ChangeMapRedTimeZone();
        }

        startMove = true;
    }


    //public void InstantinateDialog()
    //{
    //    if (hit.collider.gameObject.name.Contains("Battle") || hit.collider.gameObject.name.Contains("Portal") || hit.collider.gameObject.name.Contains("Fountain") || hit.collider.gameObject.name.Contains("ChestOfFortune") || hit.collider.gameObject.name.Contains("Forge"))
    //    {
    //        activePoint = hit.collider.gameObject.GameObject();
    //        activePoint.GetComponent<UnityEngine.UI.Image>().color = Color.red;
    //        if (!createdDialogCanvas)
    //        {
    //            startMove = false;
    //            //Time.timeScale = 0f;
    //            if (hit.collider.gameObject.name.Contains("Battle") || hit.collider.gameObject.name.Contains("Portal"))
    //                dialogCanvas = Resources.Load<GameObject>("DialogBattleCanvas");
    //            else if (hit.collider.gameObject.name.Contains("Fountain"))
    //                dialogCanvas = Resources.Load<GameObject>("DialogFountainCanvas");
    //            else if (hit.collider.gameObject.name.Contains("ChestOfFortune"))
    //            {
    //                dialogCanvas = Resources.Load<GameObject>("DialogChestOfFortuneCanvas");
    //                dialogCanvas.GetComponent<DialogCanvas>().GenerateEvent();
    //            }
    //            else if (hit.collider.gameObject.name.Contains("Forge"))
    //            {
    //                dialogCanvas = Resources.Load<GameObject>("DialogForgeCanvas");
    //                //dialogCanvas.GetComponent<DialogCanvas>().GenerateEvent();
    //            }

    //            var canvas = Instantiate(dialogCanvas, GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<RectTransform>().transform);
    //            canvas.gameObject.SetActive(true);
    //            //canvas.transform.GetChild(0).GetComponent<PointInterestButtonYesNO>().pointInterestCollision = hit.collider;
    //            //canvas.transform.GetChild(1).GetComponent<PointInterestButtonYesNO>().pointInterestCollision = hit.collider;
    //            createdDialogCanvas = true;
    //        }
    //    }
    //    else
    //    {
    //        lastCrossCollider = hit.collider;
    //    }
    //}

    private bool isCollidingArea = false;
    private bool speakNow = false;
    private Coroutine countdownCoroutine;
    private Animator activatePointAnimator;

    public void RaycastEvent()
    {
        if (rectTransform != null)
        {
            hit = Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f));
            //Debug.Log(hit.);
        }


        if (hit.collider != null)
        {
            activePoint = hit.collider.gameObject.GameObject();
            if (hit.collider.tag == "AreaEventEnemy")
            {
                Debug.Log(activePoint);
                isCollidingArea = true;
                // Запускаем корутину обратного отсчета, если она не запущена
                if (countdownCoroutine == null)
                {
                    activatePointAnimator = activePoint.GetComponent<Animator>();
                    activatePointAnimator.Play("ActivateArea", 0, 0f);
                    countdownCoroutine = StartCoroutine(Countdown());
                }
            }

            if (hit.collider.tag == "AreaEventNPC" && !speakNow)
            {
                activePoint.GetComponentInParent<NPC>().StartDialogue();
                speakNow = true;
            }
        }

        if (activePoint != null && (hit.collider == null || activePoint != hit.collider.gameObject.GameObject()))
        {
            //activePoint.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1);
            isCollidingArea = false;
            if (speakNow)
            {
                FindFirstObjectByType<DialogueManager>().EndDialogue();
            }
            speakNow = false;

        }
    }

    private IEnumerator Countdown()
    {
        countdown.enabled = true;
        // Обратный отсчет
        for (int i = 3; i > 0; i--)
        {
            // Если соединение все еще существует, продолжаем отсчет
            if (isCollidingArea)
            {
                //Debug.Log(i); // Вывод каждого числа в консоль
                countdown.text = i.ToString() + "...";
                yield return new WaitForSeconds(1f);
            }
            else
            {
                // Если объекты больше не пересекаются, выходим из корутины
                countdown.enabled = false;
                countdownCoroutine = null;
                //activePoint.GetComponent<Animator>().Play("ActivateArea",0,0);
                //activePoint.GetComponent<Animator>().enabled = false;

                // Остановка всей анимации
                activatePointAnimator.Play("DeActivateArea");

                yield break;
            }
        }
        if (isCollidingArea)
        {
            // Здесь выполняем событие после окончания обратного отсчета
            TriggerEvent();
        }
        else
        {
            activatePointAnimator.Play("DeActivateArea");
        }
        countdown.enabled = false;
        countdownCoroutine = null;
    }
    private void TriggerEvent()
    {
        characterStats.SaveData();
        activePoint.GetComponentInParent<Enemy>().StartBattle();
        Debug.Log("Событие произошло!");
        // Здесь ваше событие
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
        Invoke("Initialize",0.1f);
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

            if (Input.GetAxis("Horizontal") > 0 && B_FacingRight)
            {
                Filp();
            }
            else if (Input.GetAxis("Horizontal") < 0 && !B_FacingRight)
            {
                Filp();
            }
            //moveVector.x = Input.GetAxis("Horizontal");
            //moveVector.y = Input.GetAxis("Vertical");
            //rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
            animator.SetFloat("Move", Math.Abs(Input.GetAxis("Horizontal")) + Math.Abs(Input.GetAxis("Vertical")));

            RaycastEvent();


            //pressF();
            //pressI();
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            animator.SetFloat("Move", 0);
        }
    }
}
