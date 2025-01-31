using System;
using System.Collections.Generic;

[Serializable]
public class DoorDataClass
{
    public int currentDoorId;
    public List<string> doorDescription;
    public List<int> eventIds;
}