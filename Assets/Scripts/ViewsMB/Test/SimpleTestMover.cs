using UnityEngine;

public class SimpleTestMover : MonoBehaviour
{
    public float Speed = 3f;


    private void Update()
    {
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }
}
