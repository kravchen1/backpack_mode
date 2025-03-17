using Assets.Scripts.ItemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AimItemMusicEffects : MonoBehaviour
{
    public AudioClip hoverSound;
    [HideInInspector] public AudioSource audioSource; // Компонент AudioSource

    void Start()
    {
        // Получаем компонент AudioSource на этом объекте
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseEnter()
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            if (!DragManager.isDragging)
            {
                if (!gameObject.tag.ToUpper().Contains("BAG"))
                {
                    audioSource.pitch = Random.Range(1f, 2f);
                    //audioSource.volume = 0.8f;
                    audioSource.clip = hoverSound;
                    audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
                    audioSource.Play();
                }
                else
                {
                    if (gameObject.GetComponent<ShopItem>() != null)
                    {
                        audioSource.pitch = Random.Range(1f, 2f);
                        //audioSource.volume = 0.8f;
                        audioSource.clip = hoverSound;
                        audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
