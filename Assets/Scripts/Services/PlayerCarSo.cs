using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCarSo", menuName = "Configs/PlayerCarSo", order = 2)]
public class PlayerCarSo : ScriptableObject, ICarSo
{
    public string Id;
    public string Name;
    public int Price;
    public GameObject CarPrefab;

    public ShopValue[] MaxSpeedLevels;
    public ShopValue[] HorcePowerLevels;
    public ShopValue[] HpLevels;
    public ShopValue[] FuelLevels;

    GameObject ICarSo.CarPrefab => CarPrefab;
}