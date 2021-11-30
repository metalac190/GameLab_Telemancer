using UnityEngine;
using UnityEngine.InputSystem;

namespace _Game.Scripts.UI.HUD
{
    public class RestartCheckpointButton : MonoBehaviour
    {
        public void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                UIEvents.current.PlayerRespawn();
        }
    }
}