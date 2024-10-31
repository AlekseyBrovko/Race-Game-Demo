using System.Collections.Generic;
using UnityEngine.Localization.Tables;

namespace Client
{
    struct LocalizationComp
    {
        public Dictionary<string, int> LocalesDictionary;
        public List<string> LocalesCodes;

        public StringTable UiCurrentTable;
        public StringTable CarShopCurrentTable;
        public StringTable BrifCurrentTable;
        public StringTable MissionNamesCurrentTable;
        public StringTable MissionPartsCurrentTable;
        public StringTable TutorialCurrentTable;
    }

    struct LocalizationChangeEvent { }
}