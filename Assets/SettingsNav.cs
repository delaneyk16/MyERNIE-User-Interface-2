using UnityEngine;

public class SettingsNav : MonoBehaviour
{
    public GameObject button;
    
    public void ToggleOn()
    {
        button.SetActive(true);
    }

    public void ToggleOff()
    {
        button.SetActive(false);
    }
}
