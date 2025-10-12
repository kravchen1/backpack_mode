using UnityEngine;
using System.Collections;

public class ItemActionClockController : ItemActionInfluenceWorldController
{
    private ClockStats clockStats;
    private GameObject timeAndDate;
    protected override void Awake()
    {
        base.Awake();
        clockStats = GetComponent<ClockStats>();
        timeAndDate = GameObject.FindGameObjectWithTag("TimeAndDate");
    }


    public override void InfluenceOnThePlayer()
    {
        if (clockStats.isShowTime)
        {
            timeAndDate.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (clockStats.isShowDate)
        {
            timeAndDate.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public override void ReverseInfluenceOnThePlayer()
    {
        timeAndDate.transform.GetChild(0).gameObject.SetActive(false);
        timeAndDate.transform.GetChild(1).gameObject.SetActive(false);
    }

}