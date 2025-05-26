using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonLink : MonoBehaviour
{
    public float glowFadeDuration = 0.3f;
    public void OnMouseEnter()
    {
        transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
    }
}
