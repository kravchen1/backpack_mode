using Assets.Scripts.ItemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OtherItemMusicEffects : MonoBehaviour
{
    public AudioClip pickUpSound;
    public AudioClip dropSound;
    [HideInInspector] public AudioSource audioSource; // Компонент AudioSource

    void Start()
    {
        // Получаем компонент AudioSource на этом объекте
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void OnItemUp()
    {
        audioSource.pitch = 1f;
        audioSource.clip = pickUpSound;
        audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        audioSource.Play();
    }
    public void OnItemDown()
    {
        audioSource.clip = dropSound;
        audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        audioSource.Play();
    }
    //void OnMouseExit()
    //{
    //    // Останавливаем звук, если мышь покидает объект (по желанию)
    //    audioSource.Stop();
    //}
}
