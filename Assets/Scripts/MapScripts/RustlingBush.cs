using UnityEngine;

public class RustlingBush : MonoBehaviour
{
    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            audioSource.Play();
        }
    }
}
