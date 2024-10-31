using Client;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using static Enums;

public class ZoneSeparatorMb : MonoBehaviour
{
    [SerializeField] private Enums.IslandName _islandName;

    private EcsWorld _world;
    private EcsPool<CrossZoneEvent> _crossZonePool = default;

    public void Init(GameState state)
    {
        _world = state.EcsWorld;
        _crossZonePool = _world.GetPool<CrossZoneEvent>();
    }

    private bool _hasInvoke = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!_hasInvoke)
        {
            if (other.TryGetComponent(out PlayerCar playerCar))
            {
                ref var crossZoneComp = ref _crossZonePool.Add(_world.NewEntity());
                crossZoneComp.IslandName = _islandName;

                _hasInvoke = true;
                StartCoroutine(UpdateCor());
            }
        }
    }

    private IEnumerator UpdateCor()
    {
        yield return null;
        _hasInvoke = false;
    }
}