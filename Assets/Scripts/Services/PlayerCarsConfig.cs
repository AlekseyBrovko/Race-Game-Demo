using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCarsConfig", menuName = "Configs/PlayerCarsConfig", order = 2)]
public class PlayerCarsConfig : ScriptableObject
{
    public PlayerCarSo DefaultCar;
    public PlayerCarSo TutorialCar;

    public PlayerCarSo[] Cars;

    public GameObject GetCarById(string id)
    {
        GameObject result = null;
        PlayerCarSo carInConfig = Cars.FirstOrDefault(x => x.Id == id);
        result = carInConfig?.CarPrefab;
        return result;
    }

    public PlayerCarSo GetCarSoById(string id) =>
        Cars.FirstOrDefault(x => x.Id == id);
}