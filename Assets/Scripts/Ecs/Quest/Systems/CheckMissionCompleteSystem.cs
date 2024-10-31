using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CheckMissionCompleteSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<MissionPartCompleteEvent>> _filter = default;
        private EcsPoolInject<MissionPartCompleteEvent> _missionPartCompletePool = default;
        private EcsPoolInject<MissionCompleteEvent> _missionCompletePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                _state.Value.SaveMissionData();

                if (MissionComplete())
                    _missionCompletePool.Value.Add(_world.Value.NewEntity());

                Debug.Log("CheckMissionCompleteSystem");
                _missionPartCompletePool.Value.Del(entity);
            }
        }

        private bool MissionComplete()
        {
            foreach (var part in _state.Value.MissionSaveData.AllMissionsParts)
            {
                if (!part.Complete)
                    return false;
            }
            return true;
        }
    }
}