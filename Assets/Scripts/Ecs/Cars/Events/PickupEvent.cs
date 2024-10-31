using UnityEngine;

namespace Client
{
    public struct PickupEvent 
    { 
        public Enums.PickupType Type;
        public Vector3 PositionOfPickup;
    }

    struct RestoreCarHpEvent
    {

    }

    struct RestoreCarFuelEvent
    {

    }
}