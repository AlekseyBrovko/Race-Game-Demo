namespace Saver
{
    public interface ISaver
    {
        public void Save(SavedData data);
        public SavedData Load(bool renew);
    }

    public interface INewSaver
    {
        public void SaveMoneyScore();
        public void SavePlayerPosition();
        public void SaveSettings();
        public void SaveCarsData();
        public void SaveMissionData();
        public void SaveTutorial();

        public void LoadMoneyScore();
        public void LoadPlayerPosition();
        public void LoadSettings();
        public void LoadCarsData();
        public void LoadMissionData();
        public void LoadTutorial();
    }
}