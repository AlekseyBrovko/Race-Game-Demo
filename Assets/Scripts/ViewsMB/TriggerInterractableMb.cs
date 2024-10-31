using UnityEngine;

public class TriggerInterractableMb : PhysicalObject
{
    public override Enums.PhysicalInteractableType PhysicalInteractableType
    {
        get => _physicalInteractableType;
        protected set => _physicalInteractableType = value;
    }
     
    [SerializeField] private Enums.PhysicalInteractableType _physicalInteractableType;
}