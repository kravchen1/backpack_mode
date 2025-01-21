using UnityEngine;

public class ItemMusicEffects : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip pickUpSound;
    public AudioClip dropSound;
    [HideInInspector] public AudioSource audioSource; // ��������� AudioSource

    void Start()
    {
        // �������� ��������� AudioSource �� ���� �������
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
    //    // ������������� ����, ���� ���� �������� ������ (�� �������)
    //    audioSource.Stop();
    //}
}
