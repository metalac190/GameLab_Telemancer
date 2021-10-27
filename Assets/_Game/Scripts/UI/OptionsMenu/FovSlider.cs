using UnityEngine;

public class FovSlider : OptionSlider
{
    protected override void SetValue(int n)
    {
        string str = n + "";
        if (n >= 110) 
            str = "QUAKE PRO";
        
        SetText(str);

        float convertedFOV = Camera.HorizontalToVerticalFieldOfView(n, Camera.main.aspect);
        
        SaveValue(convertedFOV);
    }

    protected override void LoadValue()
    {
        float val = PlayerPrefs.GetFloat("Fov");
        val = Camera.VerticalToHorizontalFieldOfView(val, Camera.main.aspect);
        SetText(val + "");
        SetSlider((int)val);
    }
}