using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadMapPoint : MonoBehaviour
{
    [SerializeField] GameObject pointDescription;
    [SerializeField] protected GameObject buttonClick;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] protected List<GameObject> otherDescription = new List<GameObject>();
    public float glowFadeDuration = 0.3f;
    private void OnMouseUp()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f);
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
        foreach (var description in otherDescription.Where(p => p.gameObject.activeSelf == true))
        {
            description.gameObject.SetActive(false);
        }
        pointDescription.SetActive(true);
    }
    public void OnMouseDown()
    {
        buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        buttonClick.GetComponent<AudioSource>().Play();
        spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f);
    }
    public void OnMouseEnter()
    {
        if (!pointDescription.activeSelf)
            transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
    }

    public void CloseDescription()
    {
        pointDescription.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pointDescription.activeSelf)
            {
                CloseDescription();
            }
        }
    }

}

