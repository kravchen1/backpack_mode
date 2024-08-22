using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject goMap;

    //private generateMapScript map;

    private Map map;
    private RectTransform rectTransform;


    private Rigidbody2D rb;
    private float speed = 5f;
    private Vector2 moveVector;

    private RaycastHit2D hit;

    private Collider2D collider;

    private GameObject activePoint;
    private Color trueActivePointColor;



    private bool startMove = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
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

        SetStartPosition();



        startMove = true;

        
    }


    public void RaycastEvent()
    {
        if (rectTransform != null)
        {
            hit = Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f,0.0f));
            //Debug.Log(hit.);
        }


        if (hit.collider != null)
        {
            activePoint = hit.collider.gameObject.GameObject();
            activePoint.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            //Debug.Log(activePoint.name);
        }

        if(activePoint != null && (hit.collider == null || activePoint != hit.collider.gameObject.GameObject()))
        {
            activePoint.GetComponent<UnityEngine.UI.Image>().color = new Color(1,1,1);
        }
    }


    void pressF()
    {
        if(Input.GetButton("Jump"))
        {
            if (activePoint != null && activePoint.name.Contains("Shop"))
            {

                Time.timeScale = 0f;
                map.startPlayerPosition = rectTransform.anchoredPosition;
                map.SaveData();
                //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
                SceneManager.LoadScene("BackPackShop");
                Time.timeScale = 1f;
            }
        }
            
    }

    void Start()
    {
        Invoke("Initialize",0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    private void Update()
    {
        if (startMove)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.y = Input.GetAxis("Vertical");
            //rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
            RaycastEvent();


            pressF();
        }
    }
}
