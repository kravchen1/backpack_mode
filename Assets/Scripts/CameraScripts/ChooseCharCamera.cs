using System.Collections;
using UnityEngine;

public class ChooseCharCamera : MonoBehaviour
{
    private Camera _camera;
    private float time;
    private Coroutine currentCameraCoroutine;
    private GameObject lastCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
    }


    public void CharacterSelection(GameObject character)
    {
        if (currentCameraCoroutine != null)
        {
            StopCoroutine(currentCameraCoroutine);
        }
        if(lastCharacter != null)
        {
            lastCharacter.transform.GetChild(0).gameObject.SetActive(false);
        }
        lastCharacter = character;
        character.transform.GetChild(0).gameObject.SetActive(true);
        currentCameraCoroutine = StartCoroutine(ZoomCamera(character.GetComponent<Collider2D>().bounds.center));

    }


    public IEnumerator ZoomCamera(Vector3 endPosition, float endSize = 3f, float duration = 0.5f)
    {
        int count = 0;
        float elapsedTime = 0f;
        float startSize = _camera.orthographicSize;
        while (elapsedTime < duration)
        {
            _camera.orthographicSize = Mathf.Lerp(startSize, endSize, elapsedTime / duration);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, new Vector3(endPosition.x, endPosition.y, _camera.transform.position.z), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // ∆дем следующего кадра
            count++;
        }

        _camera.orthographicSize = endSize; // ”бедимс€, что размер установлен в конечное значение
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (currentCameraCoroutine != null)
            {
                StopCoroutine(currentCameraCoroutine);
            }
            currentCameraCoroutine = StartCoroutine(ZoomCamera(new Vector3(0,0, _camera.transform.position.z), 5f));
        }
    }
}
