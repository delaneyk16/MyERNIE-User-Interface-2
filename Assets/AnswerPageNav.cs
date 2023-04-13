using UnityEngine;
using UnityEngine.SceneManagement;

public class AnswerPageNav : MonoBehaviour
{
    public GameObject button;

    public void GoToAnswerPage()
    {
        SceneManager.LoadScene("Answer Page");
    }

    public void ToggleOn()
    {
        button.SetActive(true);
    }

    public void ToggleOff()
    {
        button.SetActive(false);
    }
}
