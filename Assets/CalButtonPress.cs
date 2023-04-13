using UnityEngine;
using UnityEngine.SceneManagement;

public class CalButtonPress : MonoBehaviour
{
    public void GoToCalendar()
    {
        SceneManager.LoadScene("Calendar Page");
    }
}
