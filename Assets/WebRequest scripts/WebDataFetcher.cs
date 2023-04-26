using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebDataFetcher : MonoBehaviour
{
    public List<QuestionAnswer> QuestionAnswers { get; private set; }

    private const string Url = "https://pluto.pr.erau.edu/~wildlfctr/MyERNIE/faq.html";

    private bool isDataFetched = false;

    public bool IsDataFetched
    {
        get { return isDataFetched; }
    }



    [System.Serializable]
    public class QuestionAnswer
    {
        public string Question = "";
        public string Answer = "";
        public string Keywords = "";
        public string SecondaryKeywords = "";
    }

    private void Awake()
    {
        if (!isDataFetched)
        {
            StartCoroutine(FetchWebData());
        }
    }

    private IEnumerator FetchWebData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(Url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Web request error: {webRequest.error}");
            }
            else
            {
                // Extract the HTML body from the response and store it in a variable.
                string htmlBody = ExtractHtmlBody(webRequest.downloadHandler.text);

                // Deserialize the JSON data from the HTML body.
                ProcessJsonData(htmlBody);
                Debug.Log($"Raw JSON data: {htmlBody}");
            }
        }
        isDataFetched = true;
    }

    private string ExtractHtmlBody(string htmlContent)
    {
        Match match = Regex.Match(htmlContent, @"<body[^>]*>([\s\S]*?)<\/body>", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            Debug.LogWarning("Unable to find the <body> tag in the HTML content.");
            return string.Empty;
        }
    }

    private void ProcessJsonData(string jsonData)
    {
        // Log the original JSON data before cleaning it
        Debug.Log($"Original JSON data: {jsonData}");
        // Remove newline characters and extra spaces from the JSON data.
        string cleanedJsonData = Regex.Replace(jsonData, @"\s+", " ").Trim();

        // Remove all extra commas from the JSON data.
        cleanedJsonData = Regex.Replace(cleanedJsonData, @",\s*(?=[}\]])", "");

        // Replace non-standard characters with standard double quotes.
        cleanedJsonData = cleanedJsonData.Replace("â€œ", "\"").Replace("â€", "\"");

        try
        {
            QuestionAnswers = JsonConvert.DeserializeObject<List<QuestionAnswer>>(cleanedJsonData);
            if (QuestionAnswers == null)
            {
                Debug.LogWarning("Deserialized QuestionAnswers list is null.");
            }
            else
            {
                Debug.Log($"Deserialized {QuestionAnswers.Count} QuestionAnswer objects.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing JSON data: {e.Message}");
        }
    }

    public List<QuestionAnswer> GetQuestionAnswers()
    {
        return QuestionAnswers;
    }
}
