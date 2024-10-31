using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class KillQuestChapterSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<KillQuestChapterComp>> _filter = default;

        //TODO переделать, работать не будет
        //private EcsFilterInject<Inc<DeathEvent, EnemyComp>> _deathFilter = default;
        private EcsFilterInject<Inc<NpcStartDeadSystemsEvent, EnemyComp>> _deathFilter = default;

        private EcsPoolInject<KillQuestChapterComp> _killChapterPool = default;
        private EcsPoolInject<QuestCounterPanelComp> _counterPanelPool = default;
        private EcsPoolInject<StopKillChapterEvent> _stopKillChapterPool = default;
        private EcsPoolInject<EnemyComp> _enemyPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                foreach (var enemyEntity in _deathFilter.Value)
                {
                    ref var killComp = ref _killChapterPool.Value.Get(entity);

                    ref var enemyComp = ref _enemyPool.Value.Get(enemyEntity);
                    string enemyType = enemyComp.EnemyMb.EnemyId;

                    if (enemyType == killComp.NameOfMonster)
                    {
                        killComp.Counter++;

                        ref var counterPanelComp = ref _counterPanelPool.Value.Get(_state.Value.InterfaceEntity);
                        counterPanelComp.MissionCounterPanel.ShowCounter(killComp.Counter);

                        if (killComp.Counter >= killComp.Amount)
                        {
                            counterPanelComp.MissionCounterPanel.ShowCounter(killComp.Amount);

                            ref var stopKillChapterPool = ref _stopKillChapterPool.Value.Add(_world.Value.NewEntity());
                            stopKillChapterPool.CurrentQuest = killComp.CurrentQuest;
                            stopKillChapterPool.KillChapter = killComp.KillChapter;
                        }
                    }
                }
            }
        }
    }
}