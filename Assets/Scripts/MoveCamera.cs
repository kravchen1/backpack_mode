using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Player player;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(player.rectTransform.anchoredPosition.x - 885, player.rectTransform.anchoredPosition.y - 400, -10800);
        }
    }
}
