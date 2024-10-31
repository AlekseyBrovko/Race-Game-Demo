using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Configs/LocalizationConfig", order = 2)]
public class LocalizationConfig : ScriptableObject
{
    [field: SerializeField] public LocalizedStringTable CarShopTable { get; private set; }
    [field: SerializeField] public LocalizedStringTable MissionBrifsTable { get; private set; }
    [field: SerializeField] public LocalizedStringTable MissionNamesTable { get; private set; }
    [field: SerializeField] public LocalizedStringTable MissionPartsTable { get; private set; }
    [field: SerializeField] public LocalizedStringTable TutorialTable { get; private set; }
    [field: SerializeField] public LocalizedStringTable UiTable { get; private set; }
}
