//using UnityEngine;

//public class ButtonForgeOk : MonoBehaviour
//{


//    void DeleteParentItem(string name)
//    {
//        var forgeItems = GameObject.FindGameObjectsWithTag("ForgeItem");
//        var nameItem = "Forge" + name.Replace("DescriptionForge", "");
//        foreach (var forgeItem in forgeItems)
//        {
//            if(forgeItem.name == nameItem)
//            {
//                Destroy(forgeItem.gameObject);
//                var forgeItemsSave = GameObject.FindGameObjectWithTag("ForgeItems").GetComponent<GenerateForgeItems>().listForgeData;
//                for(int i = 0;i < forgeItemsSave.Count; i++)
//                {
//                    //if forgeItemSave.
//                    if (forgeItemsSave[i].prefabName == nameItem.Replace("(Clone)", ""))
//                    {
//                        forgeItemsSave.Remove(forgeItemsSave[i]);
//                    }
//                }
//               // .Remove(forgeItem.gameObject)
//            }
//        }
//    }
//    public void OnMouseUpAsButton()
//    {
//        var descriptionForge = GameObject.FindGameObjectWithTag("ForgeDescription").GetComponent<DescriptionForge>();
//        if(descriptionForge.haveCountItem1 >= descriptionForge.needCountItem1)
//        {
//            descriptionForge.CreateAndDeleteItemsFromBackPackAndStorage();
//            Destroy(descriptionForge.gameObject);
//            DeleteParentItem(descriptionForge.name);

//        }
//    }
//}
