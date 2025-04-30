using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBackpack : MonoBehaviour
{
    [SerializeField] protected GameObject buttonClick;
    [SerializeField] protected float jumpHeight = 50f;    // ������ ������ � ��������
    [SerializeField] protected float jumpDuration = 0.5f; // ������������ � ��������
    private float staticY;
    private Vector3 staticScale;
    public float glowFadeDuration = 0.3f; // ������������ �������
    private string settingLanguage = "en";

    public void Start()
    {
        staticY = transform.localPosition.y;
        staticScale = transform.localScale;
    }
    public void OnMouseDown()
    {
        buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        buttonClick.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnMouseUp()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        Jump();
    }

    public void OnMouseEnter()
    {
        transform.DOScale(staticScale + new Vector3(0.5f, 0.5f, 0.5f), glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(staticScale, glowFadeDuration).SetEase(Ease.InOutSine);
    }

    void Jump()
    {

        // �������� ���������� ��������, ����� �� ���� ���������
        transform.DOKill();

        // ������ ����� � ������� � "���������"
        transform.DOLocalMoveY(jumpHeight, jumpDuration / 2)
            .SetEase(Ease.OutQuad) // ������� ����
            .OnComplete(() =>
            {
                // ������� ���� � �������� "�����������"
                transform.DOLocalMoveY(staticY, jumpDuration / 2)
                    .SetEase(Ease.InOutBack).OnComplete(() =>
                    {
                        ButtonShowAllItems();
                    }); // ˸���� "��������"
            });
    }
    public void ButtonShowAllItems()
    {
        SceneLoader.Instance.LoadScene("SceneShowItems");
    }
}
