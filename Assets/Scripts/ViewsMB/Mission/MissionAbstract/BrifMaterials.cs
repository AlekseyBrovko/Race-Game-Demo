using System;
using UnityEngine;

[Serializable]
public class BrifMaterials : IOpenPanelData
{
    [field: SerializeField] public bool ShowBrif { get; private set; } = true;
    [field: SerializeField] public string LocalizationTag { get; private set; }
    [field: SerializeField] public Sprite AvatarSprite { get; private set; }
    [field: SerializeField] public Sprite[] BriefSprites { get; private set; }
}