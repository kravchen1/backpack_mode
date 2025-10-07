// Класс TimedDestroyer остается тем же, но без GetRemainingTime
using System.Collections;
using UnityEngine;

public class TimedDestroyer : MonoBehaviour
{
    private Coroutine destroyCoroutine;
    private float destroyTimer = 1000f;

    public void StartDestroyCountdown()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }
        destroyCoroutine = StartCoroutine(DestroyCountdown());
    }

    public void StartDestroyCountdown(float customTime)
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }
        destroyTimer = customTime;
        destroyCoroutine = StartCoroutine(DestroyCountdown());
    }

    public void CancelDestroy()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    public void RestartCountdown()
    {
        CancelDestroy();
        StartDestroyCountdown();
    }

    private IEnumerator DestroyCountdown()
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(gameObject);
    }
}