using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ICar carMb))
        {
            Debug.Log("Œ“–¿¡Œ“¿À¿  ŒÀ»«»ﬂ ◊¿—“» “≈À¿");
        }
    }
}
