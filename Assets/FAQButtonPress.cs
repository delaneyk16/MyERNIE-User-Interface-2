using UnityEngine;
using UnityEngine.SceneManagement;

public class FAQButtonPress : MonoBehaviour
{
    public void GoToFAQ()
    {
        SceneManager.LoadScene("FAQ Page");
    }
}
