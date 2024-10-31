using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }

    private void Awake()
    {
        if (ServiceLocator.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitServices()
    {

    }

    private void Update()
    {
        
    }
}