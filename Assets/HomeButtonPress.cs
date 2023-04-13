using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonPress : MonoBehaviour
{
    public void GoToHome()
    {
        SceneManager.LoadScene("Home Page");
    }
}
