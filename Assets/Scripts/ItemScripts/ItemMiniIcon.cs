using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemMiniIcon : MonoBehaviour
{
    public GameObject Description;

    private GameObject placeForDescription;
    private bool Exit = false;
    private GameObject CanvasDescription;
    private void Start()
    {
        placeForDescription = GameObject.FindWithTag("DescriptionPlace");
    }

    private void OnMouseEnter()
    {
        Exit = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        StartCoroutine(ShowDescription());
    }


    public void DeleteAllDescriptions()
    {
        for (int i = 0; i < placeForDescription.transform.childCount; i++)
            Destroy(placeForDescription.transform.GetChild(i).gameObject);
    }
    public IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            DeleteAllDescriptions();
            CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
        }
    }

    private void OnMouseExit()
    {
        Exit = true;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        if (CanvasDescription != null)
            Destroy(CanvasDescription.gameObject);
    }
}
