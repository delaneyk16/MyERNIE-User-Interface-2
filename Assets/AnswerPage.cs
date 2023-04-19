using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;

public class AnswerPage : MonoBehaviour
{
    public TextMeshProUGUI displaySpeechText;
private string FormatUserInput(string userInput)
{
    if (string.IsNullOrEmpty(userInput))
    {
        return userInput;
    }

    // Capitalize the first word
    string[] words = userInput.Split(' ');
    if (words.Length > 0)
    {
        words[0] = char.ToUpper(words[0][0]) + words[0].Substring(1);
    }

    // Ensure the input ends with a question mark
    if (!userInput.EndsWith("?"))
    {
        userInput = string.Join(" ", words) + "?";
    }
    else
    {
        userInput = string.Join(" ", words);
    }

    return userInput;
}
    //Will put speech recognition text on Answer Page
    public void Awake()
{
    displaySpeechText.text = FormatUserInput(GCSR_Example.speechText);
}
}


