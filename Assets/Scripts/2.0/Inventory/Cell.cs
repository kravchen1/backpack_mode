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

    public bool IsOccupied => _nestedObject != null;
    public bool IsOccupiedBy(GameObject obj) => _nestedObject == obj;
}