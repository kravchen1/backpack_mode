using UnityEngine;
using System.Collections;

public class ItemActionFlashLightController : ItemActionInfluenceWorldController
{
    private FlashLightStats flashLightStats;
    protected override void Awake()
    {
        base.Awake();
        flashLightStats = GetComponent<FlashLightStats>();
    }


    public override void InfluenceOnThePlayer()
    {
        if (PlayerDataManager.Instance != null)
        {
            //if (PlayerDataManager.Instance.flashLightRadius < flashLightStats._flashLightRadius)
            //{
            //    PlayerDataManager.Instance.flashLightRadius = flashLightStats._flashLightRadius;
            //}
            if (!isActive)
            {
                if (flashLightStats._flashLightRadius > 0)
                {
                    PlayerDataManager.Instance.flashLightRadius += flashLightStats._flashLightRadius;
                }
                if (flashLightStats._flashLightIntensity > 0)
                {
                    PlayerDataManager.Instance.flashLightIntensity += flashLightStats._flashLightIntensity;
                }
                isActive = true;
            }
        }
    }

    public override void ReverseInfluenceOnThePlayer()
    {
        if (PlayerDataManager.Instance != null)
        {
            //if (PlayerDataManager.Instance.flashLightRadius == flashLightStats._flashLightRadius)
            //{
            //    PlayerDataManager.Instance.flashLightRadius = 0;
            //}
            if (isActive)
            {
                if (flashLightStats._flashLightRadius > 0)
                {
                    PlayerDataManager.Instance.flashLightRadius -= flashLightStats._flashLightRadius;
                }
                if (flashLightStats._flashLightIntensity > 0)
                {
                    PlayerDataManager.Instance.flashLightIntensity -= flashLightStats._flashLightIntensity;
                }
                isActive = false;
            }
        }
    }

}