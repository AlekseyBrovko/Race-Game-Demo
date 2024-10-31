using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPanelConfig", menuName = "Configs/UIPanelConfig", order = 2)]
public class UIPanelConfig : ScriptableObject
{
    public List<PanelInStorage> Panels = new List<PanelInStorage>();
}