using UnityEngine;
using UnityEngine.UI;

// Temporary UI Script that allows the Player HUD to change color based on input from the Bolt Controller
// This is based on the idea from the UI team to have the hud turn blue/cyan when looking at an interactable for the warp bolt
// SHOULD be replaced later on
namespace Mechanics.WarpBolt
{
    [RequireComponent(typeof(Text))]
    public class UIColorChanger : MonoBehaviour
    {
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void SetColor(Color color)
        {
            _text.color = color;
        }
    }
}