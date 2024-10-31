using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabsConfig", menuName = "Configs/EnemyPrefabsConfig", order = 2)]
public class EnemyPrefabsConfig : ScriptableObject
{
    [SerializeField] public GameObject NpcDebugCanvas;

    public PrefabInConfig[] Prefabs;
    public PrefabInConfig[] RagdollPrefabs;
    public PrefabInConfig[] ThrowingObjectsPrefabs;

    public GameObject GetPrefabById(string id)
    {
        GameObject result = null;
        PrefabInConfig storeObj = Prefabs.FirstOrDefault(x => x.Id == id);
        if (storeObj != null)
            result = storeObj.Prefab;
        return result;
    }
}