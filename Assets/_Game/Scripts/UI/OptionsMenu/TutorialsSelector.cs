using UnityEngine;

public class TutorialsSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("Tutorials: Hidden");
                UIEvents.current.HideTutorials();
                break;
            case 1: 
                // Visible
                Debug.Log("Tutorials: Visible");
                UIEvents.current.ShowTutorials();
                break;
            default:
                return;
        }

        PlayerPrefs.SetFloat("Tutorials", item);
    }
}