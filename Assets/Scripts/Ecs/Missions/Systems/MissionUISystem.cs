using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Linq;

namespace Client
{
    sealed class MissionUISystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<BuildAtStartMissionPanelEvent>> _buildAtStartFilter = default;
        private EcsPoolInject<BuildAtStartMissionPanelEvent> _buildAtStartPool = default;

        private EcsFilterInject<Inc<MissionPanelComp>> _missionPanelFilter = default;
        private EcsPoolInject<MissionPanelComp> _missionPanelPool = default;

        private EcsFilterInject<Inc<MissionPartProgressUiEvent>> _progressFilter = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _progressUiPool = default;

        private EcsPoolInject<LocalizationComp> _localizationPool = default;

        //TODO для панели миссий лучше сюда воткнуть текс
        //сделать как раз обновление в компоненте, и от туда тягать описание

        public void Run(EcsSystems systems)
        {
            HandleStart();
            HandleMissionPartsProgress();
        }

        private void HandleStart()
        {
            foreach (var entity in _buildAtStartFilter.Value)
            {
                foreach (var panelEntity in _missionPanelFilter.Value)
                {
                    ref var panelComp = ref _missionPanelPool.Value.Get(panelEntity);
                    panelComp.MissionPanelMb.BuildPanel(_state.Value.CurrentMission);

                    foreach (MissionPartSaveDataBase basePart in _state.Value.MissionSaveData.AllMissionsParts)
                    {
                        string progressText = GetTextForMissionPartProgress(basePart.PartId, basePart);
                        panelComp.MissionPanelMb.ShowMissionPartProgress(basePart.PartId, progressText);
                        if (basePart.Complete)
                            panelComp.MissionPanelMb.ShowMissionPartComplete(basePart.PartId);
                    }
                    _buildAtStartPool.Value.Del(entity);
                }
            }
        }

        private void HandleMissionPartsProgress()
        {
            foreach (var entity in _progressFilter.Value)
            {
                foreach (var panelEntity in _missionPanelFilter.Value)
                {
                    ref var uiEventComp = ref _progressUiPool.Value.Get(entity);
                    ref var panelComp = ref _missionPanelPool.Value.Get(_state.Value.InterfaceEntity);
                    int partId = uiEventComp.MissionPartId;
                    MissionPartSaveDataBase partSaveData = _state.Value.MissionSaveData.AllMissionsParts.FirstOrDefault(x => x.PartId == partId);

                    string progressText = GetTextForMissionPartProgress(partId, partSaveData);
                    panelComp.MissionPanelMb.ShowMissionPartProgress(partId, progressText);

                    if (partSaveData.Complete)
                        panelComp.MissionPanelMb.ShowMissionPartComplete(partId);
                    else if (partSaveData.Failed)
                        panelComp.MissionPanelMb.ShowMissionPartFailed(partId);
                }
                _progressUiPool.Value.Del(entity);
            }
        }

        private string GetTextForMissionPartProgress(int missionPartId, MissionPartSaveDataBase partSaveData)
        {
            ref var panelComp = ref _missionPanelPool.Value.Get(_state.Value.InterfaceEntity);
            MissionPartBase missionPart =
                _state.Value.CurrentMission.MissionParts.FirstOrDefault(x => x.Id == missionPartId);

            string progressText = string.Empty;

            //TODO возможно нужно обрабатывать эксепшены
            progressText = GetMissionPartLocalizedDescription(missionPart.LocalizationDescriptionTag);

            switch (missionPart.MissionType)
            {
                case Enums.MissionType.Kill:
                    //progressText = missionPart.MissionDescription;
                    progressText += " " + GetCounterTextForKillMission(missionPart as KillByNameMissionPart);
                    break;

                case Enums.MissionType.KillOnZone:
                    //progressText = missionPart.MissionDescription;
                    progressText += " " + GetCounterTextForKillMission(missionPart as KillByNameMissionPart);
                    break;
            }

            if (missionPart.MissionOnTime && !partSaveData.Failed && !partSaveData.Complete)
                progressText += " " + GetProgressTextForMissionOnTime(missionPart);

            return progressText;
        }

        private string GetMissionPartLocalizedDescription(string localizationTag)
        {
            string result = string.Empty;

            ref var localizationComp = ref _localizationPool.Value.Get(_state.Value.InterfaceEntity);
            result = localizationComp.MissionPartsCurrentTable[localizationTag].Value;

            return result;
        }

        private string GetCounterTextForKillMission(KillByNameMissionPart missionPart)
        {
            string amount = missionPart.EnemyAmount.ToString();
            string current = _state.Value.MissionSaveData.GetKillPartProgress(missionPart.Id).ToString();
            return $"{current}/{amount}";
        }

        private string GetProgressTextForMissionOnTime(MissionPartBase missionPart)
        {
            MissionPartSaveDataBase saveData = _state.Value.MissionSaveData.GetMissionPartById(missionPart.Id);
            int timer = (int)saveData.Timer;
            return TimeSpan.FromSeconds(timer).ToString("mm\\:ss");
        }
    }
}