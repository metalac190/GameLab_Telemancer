﻿using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.WarpBolt
{
    // Temporary UI Script that allows the Player HUD to change color based on input from the Bolt Controller
    // This is based on the idea from the UI team to have the hud turn blue/cyan when looking at an interactable for the warp bolt
    // SHOULD be replaced later on
    [RequireComponent(typeof(Text))]
    public class Temp_UIColorChanger : MonoBehaviour
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