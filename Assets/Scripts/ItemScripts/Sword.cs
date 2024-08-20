using UnityEngine;

public class Sword : Weapon
{
    public int attack;
    public int attackSpeed;

    [SerializeField] private float timer_cooldown = 5f;
    private float timer = 0f;
    private bool timer_locked_out = false;
    private void Start()
    {
        timer = timer_cooldown;
    }

    public override void ShowDiscriptionActivation()
    {
        Debug.Log("Атакует на " + attack + "/n" +
            "каждые " + attackSpeed + "/n" +
            "");
    }
    public override void Activation()
    {
        
        if (timer_locked_out == false)
        {
            timer_locked_out = true;

            // do things
            Debug.Log("иди на хуй!");
        }
    }


    private void Update()
    {
        
        if (timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;

                // a delayed action could be called from here
                // once the lock-out period expires
            }
        }
        Activation();
    }
}
