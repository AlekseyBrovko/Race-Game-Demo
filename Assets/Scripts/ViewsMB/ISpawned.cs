public interface ISpawned
{
    public int Entity { get; }
    public string ZoneName { get; set; }
    public int SpawnerEntity { get; set; }
    public void OnSpawn();
    public void ReturnObjectInPool();
}