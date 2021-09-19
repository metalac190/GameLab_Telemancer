using UnityEngine;

public class FovSlider : OptionSlider
{
    public override void SaveValue(int n)
    {
        PlayerPrefs.SetFloat("Fov", n);
    }

    public override void SetValue(int n)
    {
        string str = n + "";
        if (n >= 110) 
            str = "QUAKE PRO";
        
        SetText(str);
        SaveValue(n);
    }
}