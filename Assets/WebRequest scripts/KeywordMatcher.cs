using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets;
using Newtonsoft.Json;
using Porter2Stemmer;
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
        Debug.Log($"User input: {_previousUserInput}");

        // Debug output of stemmed user input
        string[] stemmedInputWords = RemovePunctuation(_previousUserInput.ToLower())
            .Split(' ')
            .Where(word => !stopWords.Contains(word))
            .Select(word => StemWord(word))
            .ToArray();
        Debug.Log($"Stemmed user input: {string.Join(" ", stemmedInputWords)}");

        MatchKeywords(_previousUserInput);

        if (AnswerTextbox != null)
        {
            AnswerTextbox.text = _matchedAnswer;
        }
    }
    }

    public void MatchKeywords(string userInput)
{
    float highestSimilarityScore = 0f;
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

    float highestWeightedMatchCount = 0f;

    foreach (WebDataFetcher.QuestionAnswer qa in questionAnswers)
    {
        string primaryKeywords = string.IsNullOrEmpty(qa.Keywords) ? qa.Question : qa.Keywords;
        string secondaryKeywords = qa.SecondaryKeywords;

        // Apply stemming on the primary and secondary keywords
        string stemmedPrimaryKeywords = string.Join(" ", RemovePunctuation(primaryKeywords.ToLower())
            .Split(' ')
            .Where(word => !stopWords.Contains(word))
            .Select(word => StemWord(word))
        );

        string stemmedSecondaryKeywords = string.Join(" ", RemovePunctuation(secondaryKeywords.ToLower())
            .Split(' ')
            .Where(word => !stopWords.Contains(word))
            .Select(word => StemWord(word))
        );

        float primarySimilarityScore = CountMatchingKeywords(userInput, stemmedPrimaryKeywords);
        float secondarySimilarityScore = CountMatchingKeywords(userInput, stemmedSecondaryKeywords);

        // Calculate the weighted similarity score
        float weightedSimilarityScore = primarySimilarityScore + (0.5f * secondarySimilarityScore);

        if (weightedSimilarityScore > highestSimilarityScore)
        {
            highestSimilarityScore = weightedSimilarityScore;
            _matchedAnswer = qa.Answer;
        }
    }

    // Check if highestSimilarityScore is less than a certain threshold, then set _matchedAnswer to the error message
    if (highestSimilarityScore < 0.1f) // Adjust the threshold according to your requirements
    {
        _matchedAnswer = "Sorry, couldn't not recognize the request, please try again.";
    }
}



    public float CountMatchingKeywords(string input, string keywords)
{
    HashSet<string> keywordSet = new HashSet<string>(
        RemovePunctuation(keywords.ToLower())
            .Split(' ')
            .Where(word => !stopWords.Contains(word))
            .Select(word => StemWord(word))
    );

    string[] inputWords = RemovePunctuation(input.ToLower())
        .Split(' ')
        .Where(word => !stopWords.Contains(word))
        .Select(word => StemWord(word))
        .ToArray();

    // Generate 2-grams for both the input words and the keyword set
    HashSet<string> inputNGrams = GetNGrams(string.Join(" ", inputWords), 2);
    HashSet<string> keywordNGrams = GetNGrams(string.Join(" ", keywordSet), 2);

    // Calculate the Jaccard similarity coefficient
    return JaccardSimilarityCoefficient(inputNGrams, keywordNGrams);
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

    private string StemWord(string word)
    {
    var stemmer = new EnglishPorter2Stemmer();
    return stemmer.Stem(word).Value;
    }

    private HashSet<string> GetNGrams(string input, int n)
{
    HashSet<string> nGrams = new HashSet<string>();

    for (int i = 0; i < input.Length - n + 1; i++)
    {
        nGrams.Add(input.Substring(i, n));
    }

    return nGrams;
}

private float JaccardSimilarityCoefficient(HashSet<string> setA, HashSet<string> setB)
{
    int intersectionSize = setA.Intersect(setB).Count();
    int unionSize = setA.Union(setB).Count();

    return unionSize == 0 ? 0 : (float)intersectionSize / unionSize;
}

}
