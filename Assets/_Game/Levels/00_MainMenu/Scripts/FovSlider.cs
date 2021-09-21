public class FovSlider : OptionSlider
{
    protected override void SetValue(int n)
    {
        string str = n + "";
        if (n >= 110) 
            str = "QUAKE PRO";
        
        SetText(str);
        SaveValue(n);
    }
}