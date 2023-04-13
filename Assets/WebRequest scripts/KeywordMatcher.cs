using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets;
using Newtonsoft.Json;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;

public class KeywordMatcher : MonoBehaviour
{
    public WebDataFetcher WebDataFetcherInstance;
    public TextMeshProUGUI AnswerTextbox;
    [SerializeField] private TextAsset stopWordsFile;
    private HashSet<string> stopWords;
    private string _previousUserInput;
    private string _matchedAnswer;

 private void Start()
    {
        stopWords = new HashSet<string>(JsonConvert.DeserializeObject<List<string>>(stopWordsFile.text));
    }

    private void Update()
    {
        if (WebDataFetcherInstance == null || !WebDataFetcherInstance.IsDataFetched)
        {
            Debug.LogWarning("WebDataFetcher instance is not set or data is not fetched yet.");
            return;
        }

        if (string.IsNullOrEmpty(GCSR_Example.speechText))
        {
            // Skip this frame if there is no speech input yet
            return;
        }

        if (GCSR_Example.speechText != _previousUserInput)
        {
            _previousUserInput = GCSR_Example.speechText;
            Debug.Log($"User input: {_previousUserInput}"); // Add this line
            MatchKeywords(_previousUserInput);

            if (AnswerTextbox != null)
            {
                AnswerTextbox.text = _matchedAnswer;
            }
        }
    }

    public void MatchKeywords(string userInput)
    {
        if (WebDataFetcherInstance == null)
        {
            Debug.LogWarning("WebDataFetcher instance is not set.");
            return;
        }

        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogWarning("User input is empty.");
            return;
        }

        List<WebDataFetcher.QuestionAnswer> questionAnswers = WebDataFetcherInstance.GetQuestionAnswers();

        if (questionAnswers == null)
        {
            Debug.LogWarning("QuestionAnswers list is null. Please wait for it to be loaded before calling GetQuestionAnswers().");
            return;
        }

        if (questionAnswers.Count == 0)
        {
            Debug.LogWarning("QuestionAnswers list is empty.");
            return;
        }

        int highestMatchCount = 0;

        foreach (WebDataFetcher.QuestionAnswer qa in questionAnswers)
        {
            string keywords = string.IsNullOrEmpty(qa.Keywords) ? qa.Question : qa.Keywords;
            int matchCount = CountMatchingKeywords(userInput, keywords);

            if (matchCount > highestMatchCount)
            {
                highestMatchCount = matchCount;
                _matchedAnswer = qa.Answer;
            }
        }
    }

    public int CountMatchingKeywords(string input, string keywords)
    {
        HashSet<string> keywordSet = new HashSet<string>(RemovePunctuation(keywords.ToLower()).Split(' ').Where(word => !stopWords.Contains(word)));
        string[] inputWords = RemovePunctuation(input.ToLower()).Split(' ').Where(word => !stopWords.Contains(word)).ToArray();

        int count = 0;
        foreach (string word in inputWords)
        {
            if (keywordSet.Contains(word))
            {
                count++;
            }
        }
        return count;
    }

    private string RemovePunctuation(string text)
    {
        return new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
    }

    [System.Serializable]
    public class StopWords
    {
        public List<string> stopWordsList;
    }
}
