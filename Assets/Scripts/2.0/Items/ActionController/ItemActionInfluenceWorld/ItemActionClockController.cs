using UnityEngine;
using System.Collections;

public class ItemActionClockController : ItemActionInfluenceWorldController
{
    private ClockStats clockStats;
    private TimeAndDate timeAndDate;
    protected override void Awake()
    {
        base.Awake();
        clockStats = GetComponent<ClockStats>();
        timeAndDate = GameObject.FindGameObjectWithTag("TimeAndDate").GetComponent<TimeAndDate>();
    }


    public override void InfluenceOnThePlayer()
    {
        if (clockStats.isShowTime)
        {
            timeAndDate.TimeOn();
        }
        if (clockStats.isShowDate)
        {
            timeAndDate.dateOn();
        }
    }

    public override void ReverseInfluenceOnThePlayer()
    {
        timeAndDate.TimeOff();
        timeAndDate.dateOff();
    }

}