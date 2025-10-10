//using UnityEngine;
//using UnityEngine.UI;

//public class ImageIsUseFight : MonoBehaviour
//{
//    public Sprite isUseFight;
//    public Sprite isNotUseFight;

//    private Image image;
//    public bool isUse = true;

//    private void Awake()
//    {
//        image = GetComponent<Image>();
//    }
//    public void SetImageIsUseFight()
//    {
//        image.sprite = isUseFight;
//        isUse = false;
//    }

//    public void SetImageIsNotUseFight()
//    {
//        image.sprite = isNotUseFight;
//        isUse = true;
//    }

//    public void ToogleImage()
//    {
//        if(isUse)
//        {
//            SetImageIsNotUseFight();
//        }
//        else
//        {
//            SetImageIsNotUseFight();
//        }
//    }

//    public void ToogleImage(bool isUse)
//    {
//        if (isUse)
//        {
//            SetImageIsNotUseFight();
//        }
//        else
//        {
//            SetImageIsNotUseFight();
//        }
//    }
//}