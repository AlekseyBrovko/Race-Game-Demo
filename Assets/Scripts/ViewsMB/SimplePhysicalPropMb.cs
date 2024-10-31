using UnityEngine;

public class SimplePhysicalPropMb : PhysicalObject
{
    public override Enums.PhysicalInteractableType PhysicalInteractableType 
    { 
        get => _physicalInteractableType; 
        protected set => _physicalInteractableType = value; 
    }

    [SerializeField] private Enums.PhysicalInteractableType _physicalInteractableType;
}