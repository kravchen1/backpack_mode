﻿using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class AssetItem :ScriptableObject, IItem
    {
        public string Name => throw new NotImplementedException();
        public Sprite UIIcon => throw new NotImplementedException();

        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;

    }
}
