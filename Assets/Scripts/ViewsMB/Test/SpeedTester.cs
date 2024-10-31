using UnityEngine;

public class SpeedTester : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private bool _running = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            _running = !_running;

        if (_running)
            transform.Translate(0, 0, _speed * Time.deltaTime);
    }
}