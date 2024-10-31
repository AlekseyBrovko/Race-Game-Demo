public class CarCharacteristics
{
    public CarCharacteristics(int maxSpeed, int horcePower, int hpLevel, int fuelLevel)
    {
        MaxSpeed = maxSpeed;
        HorcePower = horcePower;
        HpLevel = hpLevel;
        FuelLevel = fuelLevel;
    }

    public int MaxSpeed { get; private set; }
    public int HorcePower { get; private set; }
    public int HpLevel { get; private set; }
    public int FuelLevel { get; private set; }
}