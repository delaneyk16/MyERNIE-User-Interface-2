using UnityEngine;
using UnityEngine.UI;

public class QuestionListButton : MonoBehaviour
{
    public Button button;
    public string websiteURL = "https://pluto.pr.erau.edu/~wildlfctr/MyERNIE/QuestionList.html";

    void Start()
    {
        button.onClick.AddListener(OpenWebsite);
    }

    void OpenWebsite()
    {
        Application.OpenURL(websiteURL);
    }
}
