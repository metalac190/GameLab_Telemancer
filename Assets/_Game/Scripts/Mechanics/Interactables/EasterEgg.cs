using UnityEngine;

public class EasterEgg : MonoBehaviour, IPlayerInteractable
{
    private bool _used;
    
    public void OnInteract()
    {
        if (_used) return;
        
        EasterEggManager.current.OnEasterEggFound.Invoke();
        _used = true;
    }
}