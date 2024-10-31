using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class AnimationHandlerMb : MonoBehaviour
{
    public GameState GameState => _state;
    public int Entity => _entity;

    protected NpcMeleeAttackBodyPoints _attackBodyPoints;
    protected Animator _animator;
    protected GameState _state;
    protected EcsWorld _world;
    protected int _entity;

    //эти пулы отвечают за старт и конец удара в анимации
    protected EcsPool<StartMeleeAttackMonitoringEvent> _startAttackPool;
    protected EcsPool<StopMeleeAttackMonitoringEvent> _stopAttackPool;

    public virtual void Init(GameState state, int entity, Animator animator)
    {
        _state = state;
        _world = state.EcsWorld;
        _entity = entity;
        _animator = animator;

        _attackBodyPoints = GetComponent<NpcMeleeAttackBodyPoints>();

        _startAttackPool = _world.GetPool<StartMeleeAttackMonitoringEvent>();
        _stopAttackPool = _world.GetPool<StopMeleeAttackMonitoringEvent>();
    }

    public virtual void StartMonitoringMeleeAttack(Enums.BodyPartType bodyPart)
    {
        ref var startAttackComp = ref _startAttackPool.Add(_entity);
        switch(bodyPart)
        {
            case Enums.BodyPartType.Head:
                startAttackComp.AttackPoint = _attackBodyPoints.HeadTransform;
                break;

            case Enums.BodyPartType.LeftHand:
                startAttackComp.AttackPoint = _attackBodyPoints.LeftHandTransform;
                break;

            case Enums.BodyPartType.RightHand:
                startAttackComp.AttackPoint = _attackBodyPoints.RightHandTransform;
                break;

            case Enums.BodyPartType.LeftLeg:
                startAttackComp.AttackPoint = _attackBodyPoints.LeftLegTransform;
                break;

            case Enums.BodyPartType.RightLeg:
                startAttackComp.AttackPoint = _attackBodyPoints.RightLegTransform;
                break;
        }
    }
        
    public virtual void StopMonitoringMeleeAttack() =>
        _stopAttackPool.Add(_entity);

    public virtual void Attack()
    {
        
    }
}