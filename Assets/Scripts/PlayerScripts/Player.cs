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

    public Animator animator;
    //public GameObject textInfo;

    private DoorEventDistributor distributor;

    [HideInInspector] public Map map;
    private RectTransform rectTransform;
    private Collider2D previusTree = null;

    public CharacterStats characterStats;
    [SerializeField] protected AudioClip grassStep;
    [SerializeField] protected AudioClip asphaltStep;
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



    //GPS индикатор
    private List<Transform> targets = new List<Transform>(); // Целевые объекты, на которые указывает стрелка
    private Transform target; // Целевой объект, на который указывает стрелка (ближайший)
    private RectTransform arrowRectTransform; // RectTransform стрелки
    private QuestManager questManager;
    public bool needGPSTracker = true;


    private void Awake()
    {
        Time.timeScale = 1f;

        if(SceneManager.GetActiveScene().name == "Cave")
            distributor = GameObject.FindGameObjectWithTag("DoorEventDistributor").GetComponent<DoorEventDistributor>();

        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        arrowRectTransform = transform.GetChild(2).GetComponent<RectTransform>();
        
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

        questManager = FindFirstObjectByType<QuestManager>();

        InitializedGPSTracker();

        startMove = true;
    }


    private bool isCollidingArea = false;
    private bool speakNow = false;
    private Coroutine countdownCoroutine;
    private Animator activatePointAnimator;


    //public void OutFortress1()
    //{
    //    if (activePoint.name == "entranceOutFortress1")
    //    {
    //        //PlayerPrefs.SetFloat("PostionMapX", 988f);
    //        //PlayerPrefs.SetFloat("PostionMapY", 427f);
    //        PlayerPrefs.SetFloat("PostionMapX", 343f);
    //        PlayerPrefs.SetFloat("PostionMapY", -200f);
    //        SceneManager.LoadScene("GenerateMap");
    //    }
    //    if (activePoint.name == "entranceOutFortress1 1")
    //    {
    //        PlayerPrefs.SetFloat("PostionMapX", 246f);
    //        //PlayerPrefs.SetFloat("PostionMapX", 1137f);
    //        PlayerPrefs.SetFloat("PostionMapY", 172f);
    //        //PlayerPrefs.SetFloat("PostionMapY", 636f);
    //        SceneManager.LoadScene("GenerateMap");
    //    }
    //    if (activePoint.name == "entranceOutFortress1 2")
    //    {
    //        //PlayerPrefs.SetFloat("PostionMapX", 830f);
    //        //PlayerPrefs.SetFloat("PostionMapY", 587f);
    //        PlayerPrefs.SetFloat("PostionMapX", -93f);
    //        PlayerPrefs.SetFloat("PostionMapY", 91f);
    //        SceneManager.LoadScene("GenerateMap");
    //    }
    //}


    public void OutInternumFortress1()
    {
        if (activePoint.name == "entranceOutInternumFortress1 left" || activePoint.name == "entranceOutInternumFortress1 right")
        {
            PlayerPrefs.SetFloat("PostionMapX", 34f);
            PlayerPrefs.SetFloat("PostionMapY", 144f);
            SceneManager.LoadScene("GenerateMapFortress1");
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
                if (Input.GetKeyDown(KeyCode.E))
                {
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

    private float stepInterval = 0.5f; // Интервал между шагами
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
            var audioSource = GetComponent<AudioSource>();
            if (SceneManager.GetActiveScene().name == "GenerateMap")
                audioSource.clip = grassStep;
            else
                audioSource.clip = asphaltStep;
            audioSource.Play();
            nextStepTime = Time.time + stepInterval; // Устанавливаем время для следующего шага
        }
    }


    void InitializedGPSTracker()
    {
        if (questManager != null)
        {
            if (questManager.questData.questData.quests.Where(e => e.id == 2 && e.isCompleted == false).Count() > 0)
            {
                if (SceneManager.GetActiveScene().name == "GenerateMapFortress1")
                {
                    var QuestTrackers = GameObject.FindObjectsByType<QuestTracker>(FindObjectsSortMode.None).Where(e => e.idQuests.Where(e2 => e2 == 2).Count() > 0);

                    foreach (var Tracker in QuestTrackers)
                    {
                        if (Tracker.gameObject.name == "backpackShopItems")
                        {
                            if (!PlayerPrefs.HasKey("id2ShopItem"))
                            {
                                targets.Add(Tracker.gameObject.transform);
                            }
                        }
                        if (Tracker.gameObject.name == "backpackShopEat")
                        {
                            if (!PlayerPrefs.HasKey("id2ShopEat"))
                            {
                                targets.Add(Tracker.gameObject.transform);
                            }
                        }
                    }
                }
            }
            if (questManager.questData.questData.quests.Where(e => e.id == 4 && e.isCompleted == false).Count() > 0)
            {
                if (SceneManager.GetActiveScene().name == "GenerateMapFortress1")
                {
                    var QuestTrackers = GameObject.FindObjectsByType<QuestTracker>(FindObjectsSortMode.None).Where(e => e.idQuests.Where(e2 => e2 == 4).Count() > 0);
                    foreach (var Tracker in QuestTrackers)
                    {
                        targets.Add(Tracker.gameObject.transform);
                    }
                }
            }
        }
    }
    void GPSTracker()
    {
        if (needGPSTracker && targets.Count > 0)
        {
            arrowRectTransform.gameObject.SetActive(true);
            float minDistance = 99999f;
            int indexMinDestance = 0;
            float Distance = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                Distance = Vector2.Distance(targets[i].position, transform.position);
                if (Distance < minDistance)
                {
                    minDistance = Distance;
                    indexMinDestance = i;
                }
            }

            target = targets[indexMinDestance];

            // Вычисляем направление к объекту
            Vector3 direction = (target.position - transform.position).normalized;

            // Вычисляем угол поворота стрелки
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            arrowRectTransform.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        GPSTracker();
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
