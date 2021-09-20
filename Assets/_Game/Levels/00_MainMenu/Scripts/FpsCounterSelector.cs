using UnityEngine;

public class FpsCounterSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("FPS Counter: Hidden");
                break;
            case 1: 
                // Visible
                Debug.Log("FPS Counter: Visible");
                break;
        }
    }
}