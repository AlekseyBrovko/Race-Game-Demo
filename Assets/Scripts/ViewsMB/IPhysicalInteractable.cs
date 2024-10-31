/// <summary>
/// Для всех объектов, которые взаимодействуют с физикой.
/// </summary>
public interface IPhysicalInteractable 
{ 
    public Enums.PhysicalInteractableType PhysicalInteractableType { get; }
}