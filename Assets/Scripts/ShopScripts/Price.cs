using UnityEngine;

public class Price : MonoBehaviour
{

    [SerializeField] private GameObject lockForItem;

    void OnMouseUpAsButton()
    {
        LockItem();
    }


    void LockItem()
    {
        lockForItem.SetActive(!lockForItem.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
