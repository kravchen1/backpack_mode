using UnityEngine;

[SelectionBase]
public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject _nestedObject;

    public GameObject NestedObject
    {
        get => _nestedObject;
        set => _nestedObject = value;
    }
}