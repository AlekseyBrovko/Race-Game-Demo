using System.Collections.Generic;

namespace Client
{
    struct ObjectsPoolComp
    {
        public Dictionary<string, GameObjectsPool> EnemyPoolsDictionary;
        public Dictionary<string, GameObjectsPool> EnemyRagdollsDictionary;
        public Dictionary<string, GameObjectsPool> ThrowingObjectsDictionary;
    }
}