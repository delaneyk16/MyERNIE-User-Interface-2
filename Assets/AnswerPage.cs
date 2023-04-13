using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;

public class AnswerPage : MonoBehaviour
{
    public TextMeshProUGUI displaySpeechText;

    //Will put speech recognition text on Answer Page
    public void Awake()
    {
        displaySpeechText.text = GCSR_Example.speechText;
    }
}
