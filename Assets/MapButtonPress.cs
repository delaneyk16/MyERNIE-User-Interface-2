using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButtonPress : MonoBehaviour
{
    public void GoToMap()
    {
        SceneManager.LoadScene("Map Page");
    }
}
