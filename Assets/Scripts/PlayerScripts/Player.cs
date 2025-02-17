using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public GameObject goMap;
    public Animator animator;
    public GameObject textInfo;
    //private generateMapScript map;

    private DoorEventDistributor distributor;

    [HideInInspector] public Map map;
    private RectTransform rectTransform;
    private Collider2D previusTree = null;
    // [SerializeField] private Canvas backpackCanvas;
    //[SerializeField] private Canvas mapCanvas;

    private CharacterStats characterStats;

    [HideInInspector] public Rigidbody2D rb;
    public float speed = 100f;
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


    private bool textInfoE = false;
    private void Awake()
    {
        Time.timeScale = 1f;
        if(SceneManager.GetActiveScene().name == "Cave")
            distributor = GameObject.FindGameObjectWithTag("DoorEventDistributor").GetComponent<DoorEventDistributor>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        //goMap = GameObject.FindGameObjectWithTag("GoMap");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        countdown = GameObject.FindGameObjectWithTag("Countdown").GetComponent<TextMeshPro>();
        //scriptMainCamera = mainCamera.GetComponent<MainCamera>();
    }
    void LoadCharacterStats()
    {
        characterStats = GetComponent<CharacterStats>();
        characterStats.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json"));
        characterStats.InitializeCharacterStats();
    }


    void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        LoadCharacterStats();

        startMove = true;
    }


    private bool isCollidingArea = false;
    private bool speakNow = false;
    private Coroutine countdownCoroutine;
    private Animator activatePointAnimator;


    public void OutFortress1()
    {
        if (activePoint.name == "entranceOutFortress1")
        {
            PlayerPrefs.SetFloat("PostionMapX", 988f);
            PlayerPrefs.SetFloat("PostionMapY", 427f);
            SceneManager.LoadScene("GenerateMap");
        }
        if (activePoint.name == "entranceOutFortress1 1")
        {
            PlayerPrefs.SetFloat("PostionMapX", 1137f);
            PlayerPrefs.SetFloat("PostionMapY", 636f);
            SceneManager.LoadScene("GenerateMap");
        }
        if (activePoint.name == "entranceOutFortress1 2")
        {
            PlayerPrefs.SetFloat("PostionMapX", 830f);
            PlayerPrefs.SetFloat("PostionMapY", 587f);
            SceneManager.LoadScene("GenerateMap");
        }
    }
    public void InFortress1()
    {
        if (activePoint.name == "entranceInFortress1 0" || activePoint.name == "entranceInFortress1 2" || activePoint.name == "entranceInFortress1 1")
        {
            PlayerPrefs.DeleteKey("PostionMapX");
            PlayerPrefs.DeleteKey("PostionMapY");
            SceneManager.LoadScene("GenerateMapFortress1");
        }
        
    }

    public void OutInternumFortress1()
    {
        if (activePoint.name == "entranceOutInternumFortress1 left" || activePoint.name == "entranceOutInternumFortress1 right")
        {
            PlayerPrefs.SetFloat("PostionMapX", 948f);
            PlayerPrefs.SetFloat("PostionMapY", 614f);
            SceneManager.LoadScene("GenerateMapFortress1");
        }
    }
    public void InBackpackShopItem()
    {
        if (activePoint.name == "entranceBackpackShopItems")
        {
            FindFirstObjectByType<QuestManager>().CompleteQuest(2);
            SceneManager.LoadScene("BackPackShop");
        }
    }

    public void InInternumFortress1()
    {
        if (activePoint.name == "entranceIntenumFortress")
        {
            PlayerPrefs.DeleteKey("PostionMapX");
            PlayerPrefs.DeleteKey("PostionMapY");
            SceneManager.LoadScene("GenerateMapInternumFortress1");
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
            activePoint = hit.collider.gameObject.GameObject();
            if (hit.collider.tag == "AreaEventEnemy")
            {
                //Debug.Log(activePoint);
                //isCollidingArea = true;
                //// Запускаем корутину обратного отсчета, если она не запущена
                //if (countdownCoroutine == null)
                //{
                //    activatePointAnimator = activePoint.GetComponent<Animator>();
                //    activatePointAnimator.Play("ActivateArea", 0, 0f);
                //    countdownCoroutine = StartCoroutine(Countdown());
                //}
            }

            if (hit.collider.tag == "AreaEventNPC" && !speakNow)
            {
                activePoint.GetComponentInParent<NPC>().StartDialogue();
                speakNow = true;
            }
            if (hit.collider.tag == "AreaEventEntrance")
            {
                Debug.Log(activePoint.name);
                if (!textInfoE)
                {
                    textInfo.SetActive(true);
                    textInfoE = true;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (SceneManager.GetActiveScene().name == "GenerateMapFortress1")
                    {
                        OutFortress1();
                        InInternumFortress1();
                        InBackpackShopItem();
                    }
                    if (SceneManager.GetActiveScene().name == "GenerateMap")
                        InFortress1();
                    if (SceneManager.GetActiveScene().name == "GenerateMapInternumFortress1")
                        OutInternumFortress1();
                }

                if (activePoint.name == "entranceInCave1")
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SceneManager.LoadScene("BackPackCave1");
                    }
                }   
            }
            if (hit.collider.tag == "AreaCaveDoor")
            {
                if (!textInfoE)
                {
                    textInfo.SetActive(true);
                    textInfoE = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    distributor.doorData.DoorDataClass.currentDoorId = activePoint.gameObject.transform.parent.GetComponent<Door>().doorId;
                    distributor.doorData.DoorDataClass.currentCaveLevel = activePoint.gameObject.transform.parent.GetComponent<Door>().caveLevel;
                    distributor.doorData.SaveData();
                    SceneManager.LoadScene("Cave");
                }
            }
            //if (hit.collider.tag == "AreaFountain")
            //{
            //    if (!textInfoE)
            //    {
            //        textInfo.SetActive(true);
            //        textInfoE = true;
            //    }

            //    if (Input.GetKeyDown(KeyCode.E))
            //    {
            //        hit.collider.transform.parent.gameObject.GetComponent<CaveFountain>().ActivateFountain();
            //    }
            //}
        }
        else
        {
            if (textInfoE)
            {
                textInfo.SetActive(false);
                textInfoE = false;
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

        
    }

    //private IEnumerator Countdown()
    //{
    //    countdown.enabled = true;
    //    // Обратный отсчет
    //    for (int i = 3; i > 0; i--)
    //    {
    //        // Если соединение все еще существует, продолжаем отсчет
    //        if (isCollidingArea)
    //        {
    //            //Debug.Log(i); // Вывод каждого числа в консоль
    //            countdown.text = i.ToString() + "...";
    //            yield return new WaitForSeconds(1f);
    //        }
    //        else
    //        {
    //            // Если объекты больше не пересекаются, выходим из корутины
    //            countdown.enabled = false;
    //            countdownCoroutine = null;
    //            //activePoint.GetComponent<Animator>().Play("ActivateArea",0,0);
    //            //activePoint.GetComponent<Animator>().enabled = false;

    //            // Остановка всей анимации
    //            activatePointAnimator.Play("DeActivateArea");

    //            yield break;
    //        }
    //    }
    //    if (isCollidingArea)
    //    {
    //        // Здесь выполняем событие после окончания обратного отсчета
    //        TriggerEvent();
    //    }
    //    else
    //    {
    //        activatePointAnimator.Play("DeActivateArea");
    //    }
    //    countdown.enabled = false;
    //    countdownCoroutine = null;
    //}
    //private void TriggerEvent()
    //{
    //    characterStats.SaveData();
    //    activePoint.GetComponentInParent<Enemy>().StartBattle();
    //    Debug.Log("Событие произошло!");
    //    // Здесь ваше событие
    //}

    void pressI()
    {
       
    }



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

        Vector3 theScale = transform.GetChild(0).localScale;
        theScale.x *= -1;

        transform.GetChild(0).localScale = theScale;
    }

    private float stepInterval = 0.3f; // Интервал между шагами
    private float nextStepTime = 0f; // Время, когда можно воспроизвести следующий звук
    private bool isMoving = false;


    void StepSound()
    {
        // Проверяем, движется ли персонаж
        if (rb.velocity.magnitude > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Воспроизводим звук шагов
        if (isMoving && Time.time >= nextStepTime)
        {
            GetComponent<AudioSource>().Play();
            nextStepTime = Time.time + stepInterval; // Устанавливаем время для следующего шага
        }
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
            StepSound();
            //moveVector.x = Input.GetAxis("Horizontal");
            //moveVector.y = Input.GetAxis("Vertical");
            //rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
            animator.SetFloat("Move", Math.Abs(Input.GetAxis("Horizontal")) + Math.Abs(Input.GetAxis("Vertical")));

            RaycastEvent();
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            animator.SetFloat("Move", 0);
        }
    }
}
