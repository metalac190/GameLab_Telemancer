using UnityEngine;

public class VolumeSlider : OptionSlider
{
     
     
     protected override void SetText(string s)
     {
          base.SetText(s + "%");
     }
}