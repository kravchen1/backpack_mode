using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Unity.Burst.CompilerServices;
using UnityEditor.VersionControl;

public class DescriptionItemCooldown : DescriptionItem
{
    public float cooldown = 1.1f;



    private void Start()
    {
        SetTextBody();
    }
}
