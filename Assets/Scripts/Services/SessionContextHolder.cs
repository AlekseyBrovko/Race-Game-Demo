using UnityEngine;

public class SessionContextHolder : MonoBehaviour
{
    public static SessionContextHolder Instance { get; private set; }

    public SessionAdContext SessionAdContext { get; private set; }
    public Enums.GameModType GameModeType { get; set; }

    public void Init()
    {
        if (SessionContextHolder.Instance == null)
        {
            SessionContextHolder.Instance = this;
            SessionAdContext = new();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}