using Client;

public interface IRangeNpc
{
    public float RangeOfAttack { get; }
    public float RangeAttackCoolDown { get; }
    public float DistanceOfMeleeAttack { get; }
    public bool CanMeleeAttack { get; set; }
    public bool CanRun { get; set; }
    public float DefaultRunSpeed { get; }
    public void TakeObjectForRangeAttack();

    public void OnStartAiming();
    public void OnRangeAttack();
}

public interface IRangeThrowingNpc : IRangeNpc
{
    public string ThrowObjectId { get; }
    GameObjectsPool PoolOfThrowObjects { get; set; }
    public IThrowObject ThrowObjectInHands { get; set; }
    public bool CanThrowObject { get; }
}