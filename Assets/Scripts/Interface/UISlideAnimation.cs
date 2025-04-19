using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SlideDirection
{
    Left,
    Right,
    Top,
    Bottom
}

public class UISlideAnimation : MonoBehaviour
{

    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] private RectTransform target;
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Left;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease easeType = Ease.OutQuint;
    [SerializeField] private int hiddenX = 100;
    private List<Collider2D> childColliders;
    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;

    private void Awake()
    {
        _shownPosition = target.anchoredPosition;
        //_shownPosition = Camera.main.GetComponent<RectTransform>().anchoredPosition;
        var directionVector = GetDirectionVector(slideDirection);
        var childColliders = GetComponentsInChildren<Collider2D>();
        _hiddenPosition = _shownPosition - directionVector * (hiddenX);
    }

    private Vector2 GetDirectionVector(SlideDirection direction)
    {
        return direction switch
        {
            SlideDirection.Left => Vector2.left,
            SlideDirection.Right => Vector2.right,
            SlideDirection.Top => Vector2.up,
            SlideDirection.Bottom => Vector2.down,
            _ => Vector2.left
        };
    }

    public void Show()
    {
        // Отменяем предыдущие анимации
        target.DOKill();
        target.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "Main")
        {
            if (childColliders == null)
                childColliders = mainCanvas.GetComponentsInChildren<Collider2D>().Where(e => !e.CompareTag("LeafUI")).ToList();
            foreach (var collider in childColliders)
            {
                collider.enabled = false;
            }
        }
        else
        {
            mainCanvas.GetComponent<CanvasGroup>().interactable = false;
        }
        target.anchoredPosition = _hiddenPosition;
        target.DOAnchorPos(_shownPosition, duration)
            .SetEase(easeType);
    }

    public void Hide()
    {
        // Отменяем предыдущие анимации
        target.DOKill();
        target.DOAnchorPos(_hiddenPosition, duration)
            .SetEase(easeType).OnComplete(() =>
            {
                if (SceneManager.GetActiveScene().name == "Main")
                {
                    if(childColliders == null)
                        childColliders = mainCanvas.GetComponentsInChildren<Collider2D>().Where(e => !e.CompareTag("LeafUI")).ToList();
                    Debug.Log(childColliders.Count());
                    foreach (var collider in childColliders)
                    {
                        collider.enabled = true;
                    }
                }
                else
                {
                    mainCanvas.GetComponent<CanvasGroup>().interactable = true;
                }
                target.gameObject.SetActive(false);
            });
    }

    public void Toggle()
    {
        if (target.anchoredPosition == _shownPosition)
            Hide();
        else
            Show();
    }
}