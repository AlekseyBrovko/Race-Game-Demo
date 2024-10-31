using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public class GameObjectsPool
    {
        public string PoolName { get; private set; }
        public GameState GameState { get; private set; }
        
        private GameObject _parentGo = new GameObject();
        private List<PoolObject> _poolObjects = new List<PoolObject>();
        private GameObject _prefab;

        private int _amount;
        private int _counter = 1;

        public GameObjectsPool(GameState state, string poolName, string parentGoName, int amount, GameObject prefab)
        {
            PoolName = poolName;
            _parentGo.name = parentGoName;
            _parentGo.transform.position = Vector3.zero;
            GameState = state;
            _amount = amount;
            _prefab = prefab;

            FillThePool();
        }

        public GameObject GetFromPool()
        {
            PoolObject poolObject = _poolObjects.FirstOrDefault(x => x.IsReadyToWork);
            if (poolObject == null)
            {
                poolObject = AddMissingObject();
                _poolObjects.Add(poolObject);
            }

            poolObject.IsReadyToWork = false;
            var gameObject = poolObject.GameObject;
            gameObject.transform.parent = null;
            return gameObject;
        }

        public void FillThePool()
        {
            for (int i = 0; i < _amount; i++)
            {
                var poolObject = AddNewObjectInPool(_prefab);
                _poolObjects.Add(poolObject);
            }
        }

        public PoolObject AddMissingObject() =>
            AddNewObjectInPool(_prefab);

        public PoolObject AddNewObjectInPool(GameObject prefab)
        {
            var gameObject = GameObject.Instantiate(prefab, _parentGo.transform);
            gameObject.transform.localPosition = Vector3.zero;
            PoolObject poolObject = new PoolObject(gameObject, _parentGo.transform, this, GameState);
            gameObject.name = gameObject.name + _counter.ToString();
            
            _counter++;
            return poolObject;
        }
    }
}