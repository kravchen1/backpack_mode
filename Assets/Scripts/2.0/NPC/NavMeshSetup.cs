//using NavMeshPlus.Extensions;
//using UnityEngine;
//using UnityEngine.AI;

//public class NavMeshSetup : MonoBehaviour
//{
//    [SerializeField] private CollectSources2d navMeshSurface;

//    void Start()
//    {
//        BakeNavMesh();
//    }

//    public void BakeNavMesh()
//    {
//        if (navMeshSurface != null)
//        {
//            navMeshSurface.BuildNavMesh();
//            navMeshSurface.Na
//        }
//    }

//    // ��������� ���� ����� ��� ��������� ������ � runtime
//    public void UpdateNavMesh()
//    {
//        if (navMeshSurface != null)
//        {
//            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
//        }
//    }
//}