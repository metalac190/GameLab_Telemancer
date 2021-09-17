using UnityEngine;

// Universal Scriptable Object to Manage Pausing and Resuming of game
// Use the Observer Pattern to allow things to be activated or disabled on Pausing
public class Temp_PauseManager : ScriptableObject
{
    private bool _isPaused;

    private void Awake()
    {
        // Start the game un-paused
        PauseGame(false);
    }

    // Called by scripts that reference this scriptable object. Currently only Player Input
    public void Pause()
    {
        PauseGame(!_isPaused);
    }

    private void PauseGame(bool pause = true)
    {
        _isPaused = pause;
        Time.timeScale = pause ? 0 : 1;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
    }
}