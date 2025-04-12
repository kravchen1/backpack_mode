using UnityEngine;
using UnityEngine.SceneManagement;

public class Flask : Item
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            FillnestedObjectStarsStars(256);
            animator.speed = 1f / 0.5f;
            //animator.Play(originalName + "Activation");
        }
    }

    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;

                StartActivation();
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            Activation();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
        {
            defaultItemUpdate();
        }
    }
}
