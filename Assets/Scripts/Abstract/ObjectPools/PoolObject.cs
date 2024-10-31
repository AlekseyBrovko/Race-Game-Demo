using UnityEngine;
namespace Client
{
    public class PoolObject
    {
        public GameState State { get; private set; }
        public Transform Parent { get; private set; }
        public GameObject GameObject { get; private set; }
        public GameObjectsPool ObjectPool { get; private set; }
        public bool IsReadyToWork { get; set; }
        private IPoolObjectMb _objectMb { get; set; }

        public PoolObject(GameObject gameObject, Transform parent, GameObjectsPool poolOfObject, GameState state)
        {
            GameObject = gameObject;
            IsReadyToWork = true;
            GameObject.SetActive(false);
            Parent = parent;
            ObjectPool = poolOfObject;
            State = state;

            IPoolObjectMb poolObjectMb = gameObject.GetComponent<IPoolObjectMb>();
            poolObjectMb.InitObjectOfPool(this, State);
            _objectMb = poolObjectMb;
        }
    }

    public interface IPoolObjectMb
    {   
        //TODO прокинуть сюда id
        public PoolObject PoolObject { get; }
        public GameObject GameObject { get; }

        public void InitObjectOfPool(PoolObject poolObject, GameState state);
        public void OnSpawn();
        public void ReturnObjectInPool();
    }
}