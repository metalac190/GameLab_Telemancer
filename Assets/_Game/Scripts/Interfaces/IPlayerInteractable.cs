/// <summary>
/// Implement this interface for objects that require the player to use the "Interact" button
/// </summary>

public interface IPlayerInteractable
{
    void OnInteract();
}

public interface IHoverInteractable : IPlayerInteractable
{
    void OnBeginHover();
    void OnEndHover();
}
