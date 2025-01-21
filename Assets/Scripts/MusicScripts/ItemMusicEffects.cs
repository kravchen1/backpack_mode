using UnityEngine;

public class ItemMusicEffects : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip pickUpSound;
    public AudioClip dropSound;
    [HideInInspector] public AudioSource audioSource; // Компонент AudioSource

    void Start()
    {
        // Получаем компонент AudioSource на этом объекте
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseEnter()
    {
        audioSource.pitch = Random.Range(1f, 2f);
        //audioSource.volume = 0.8f;
        audioSource.clip = hoverSound;
        audioSource.Play();
    }

    public void OnItemUp()
    {
        audioSource.pitch = 1f;
        audioSource.clip = pickUpSound;
        audioSource.Play();
    }
    public void OnItemDown()
    {
        audioSource.clip = dropSound;
        audioSource.Play();
    }
    //void OnMouseExit()
    //{
    //    // Останавливаем звук, если мышь покидает объект (по желанию)
    //    audioSource.Stop();
    //}
}
