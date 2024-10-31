using UnityEngine;

public class DumpMb : MonoBehaviour, IDump
{
    public int Entity { get; set; }
}

public interface IDump
{
    public int Entity { get; set; }
}