using UnityEngine;
using UnityEngine.UI;

public abstract class OptionSelector : MonoBehaviour
{
    [SerializeField] private Button[] options;

    private void Start()
    {
        // Create listeners for all the option buttons
        for (int x = 0; x < options.Length; x++)
        {
            var x1 = x;
            options[x].onClick.AddListener(delegate { SelectItem(x1); });
        }
    }

    public void SelectItem(int item)
    {
        for (var x = 0; x < options.Length; x++)
        {
            // Make only the selected item non-interactable
            options[x].interactable = x != item; 
        }
        
        // Call method for handling interaction outcome
        OnItemSelected(item);
    }

    public abstract void OnItemSelected(int item);
}